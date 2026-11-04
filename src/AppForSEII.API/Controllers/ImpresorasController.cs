using AppForSEII.API.DTOs.ImpresoraDTOs;

namespace AppForSEII.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImpresorasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ImpresorasController> _logger;

    public ImpresorasController(ApplicationDbContext context,
        ILogger<ImpresorasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(List<ImpresoraForReservaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetImpresorasForReserva(
        [FromQuery] string? nombre = null, [FromQuery] TipoImpresora? tipo = null)
    {
        if (tipo.HasValue && !Enum.IsDefined(typeof(TipoImpresora), tipo.Value))
        {
            ModelState.AddModelError(nameof(tipo), "El tipo de impresora no es válido.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Filtros no válidos al consultar impresoras para reserva.");
            return BadRequest(ModelState);
        }

        var impresoras = _context.Impresoras3D.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            var filtroNombre = nombre.Trim();
            impresoras = impresoras.Where(i => i.Nombre.Contains(filtroNombre));
        }

        if (tipo.HasValue)
        {
            impresoras = impresoras.Where(i => i.Tipo == tipo.Value);
        }

        var resultado = await impresoras.OrderBy(i => i.Nombre).ThenBy(i => i.Id)
            .Select(i => new ImpresoraForReservaDTO(i.Id, i.Nombre, i.Modelo,
                i.Tipo, i.Descripcion, i.PrecioKilovatioHora, i.PrecioReserva))
            .ToListAsync();

        // Una lista vacía permite al cliente indicar que no hay resultados.
        return Ok(resultado);
    }
}
