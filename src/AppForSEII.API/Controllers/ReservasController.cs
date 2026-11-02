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
}
