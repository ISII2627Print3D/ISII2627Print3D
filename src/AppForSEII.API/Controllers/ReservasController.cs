using AppForSEII.API.DTOs.ReservaDTOs;

namespace AppForSEII.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(ApplicationDbContext context, ILogger<ReservasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("[action]/{id}")]
    [ProducesResponseType(typeof(ReservaDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReserva(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var reserva = await _context.ReservasImpresoras.AsNoTracking()
            .Include(r => r.LineasReserva)
            .ThenInclude(l => l.Impresora3D)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reserva == null)
        {
            _logger.LogWarning("No existe la reserva con identificador {ReservaId}.", id);
            return NotFound();
        }

        var lineas = reserva.LineasReserva.OrderBy(l => l.Id)
            .Select(l => new LineaReservaDetailDTO(l.Impresora3DId,
                l.Impresora3D.Nombre, l.Impresora3D.Modelo, l.Impresora3D.Tipo,
                l.Impresora3D.Descripcion, l.TiempoReserva,
                l.Impresora3D.PrecioKilovatioHora, l.Impresora3D.PrecioReserva,
                l.PrecioSubtotal)).ToList();

        // Se muestran los datos del cliente y los importes guardados en la reserva.
        var resultado = new ReservaDetailDTO(reserva.Id, reserva.FechaReserva,
            reserva.NombreCliente, reserva.ApellidosCliente, reserva.DireccionFacturacion,
            reserva.MetodoPago, reserva.PrecioTotal, lineas);

        return Ok(resultado);
    }

[HttpPost("[action]")]
[ProducesResponseType(typeof(ReservaDetailDTO), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> CreateReserva(
    [FromBody] ReservaCreateDTO reservaDTO)
{
    // 1. Validar los datos recibidos.
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    if (reservaDTO.LineasReserva == null ||
        reservaDTO.LineasReserva.Count == 0)
    {
        return BadRequest("Debes seleccionar al menos una impresora.");
    }

    if (!reservaDTO.MetodoPago.HasValue)
    return BadRequest("Debes seleccionar un método de pago.");

    if (!Enum.IsDefined(typeof(MetodoPago), reservaDTO.MetodoPago.Value))
    return BadRequest("El método de pago no es válido.");

    foreach (var linea in reservaDTO.LineasReserva)
    {
        if (linea == null ||
            linea.Impresora3DId <= 0 ||
            !Enum.IsDefined(typeof(TiempoReserva), linea.TiempoReserva))
        {
            return BadRequest("Una línea contiene una impresora o un tiempo no válido.");
        }
    }

    // 2. Comprobar que el cliente existe.
    var cliente = await _context.Clientes
        .FirstOrDefaultAsync(c => c.Id == reservaDTO.ClienteId);

    if (cliente == null)
        return BadRequest("El cliente indicado no existe.");

    // 3. Comprobar las impresoras seleccionadas.
    var ids = reservaDTO.LineasReserva
        .Select(l => l.Impresora3DId)
        .ToList();

    if (ids.Distinct().Count() != ids.Count)
        return BadRequest("No puedes incluir la misma impresora varias veces.");

    var impresoras = await _context.Impresoras3D
        .Where(i => ids.Contains(i.Id))
        .ToListAsync();

    if (impresoras.Count != ids.Count)
        return BadRequest("Alguna de las impresoras seleccionadas ya no existe.");

    // 4. Crear las líneas usando los precios de la base de datos.
    var lineas = new List<LineaReserva>();

    foreach (var seleccion in reservaDTO.LineasReserva)
    {
        var impresora = impresoras.Single(
            i => i.Id == seleccion.Impresora3DId);

        int horas = (int)seleccion.TiempoReserva;
        decimal subtotal = impresora.PrecioReserva+(horas * impresora.PrecioKilovatioHora);
        subtotal=decimal.Round(subtotal,2);

        if (subtotal < 0 || subtotal > 99999999.99m)
            return BadRequest("El subtotal está fuera del importe permitido.");

        var linea = new LineaReserva(
            impresora,
            seleccion.TiempoReserva,
            subtotal);

        lineas.Add(linea);
    }

    if (lineas.Sum(l => l.PrecioSubtotal) > 99999999.99m)
        return BadRequest("El total supera el importe permitido.");

    // 5. Crear y guardar la reserva con todas sus líneas.
    var reserva = new ReservaImpresora(
        cliente,
        DateTime.UtcNow,
        reservaDTO.NombreCliente,
        reservaDTO.ApellidosCliente,
        reservaDTO.DireccionFacturacion,
        reservaDTO.MetodoPago.Value,
        lineas);

    _context.ReservasImpresoras.Add(reserva);
    await _context.SaveChangesAsync();

    // 6. Preparar la respuesta.
    var lineasDTO = reserva.LineasReserva
        .OrderBy(l => l.Id)
        .Select(l => new LineaReservaDetailDTO(
            l.Impresora3DId,
            l.Impresora3D.Nombre,
            l.Impresora3D.Modelo,
            l.Impresora3D.Tipo,
            l.Impresora3D.Descripcion,
            l.TiempoReserva,
            l.Impresora3D.PrecioKilovatioHora,
            l.Impresora3D.PrecioReserva,
            l.PrecioSubtotal))
        .ToList();

    var resultado = new ReservaDetailDTO(
        reserva.Id,
        reserva.FechaReserva,
        reserva.NombreCliente,
        reserva.ApellidosCliente,
        reserva.DireccionFacturacion,
        reserva.MetodoPago,
        reserva.PrecioTotal,
        lineasDTO);

    return CreatedAtAction(
        nameof(GetReserva),
        new { id = reserva.Id },
        resultado);
}
}
