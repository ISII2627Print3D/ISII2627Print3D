using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ReservaDTOs;

namespace AppForSEII.UT.ReservasController_test;

public class GetReserva_test : AppForSEII4SqliteUT
{
    [Theory]
    [InlineData(99)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ReservaInexistente_DevuelveNotFound(int id)
    {
        // Arrange
        using var contextoConsulta = CreateContext();

        var logger = new Mock<ILogger<ReservasController>>();
        var controller = new ReservasController(
            contextoConsulta,
            logger.Object);

        // Act
        var respuesta = await controller.GetReserva(id);

        // Assert
        var notFound = Assert.IsType<NotFoundResult>(respuesta);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Fact]
public async Task ReservaExistente_DevuelveDetalleConSusLineas()
{
    // Arrange
    var cliente = new Cliente(
        "cliente-test",
        "Ana",
        "García",
        "ana@example.com",
        "Calle de prueba 1");

    var filamento = new Impresora3D(
        1,
        "Impresora Filamento",
        "Modelo F1",
        TipoImpresora.Filamento,
        "Impresora para prototipos",
        0.15m,
        3m);

    var resina = new Impresora3D(
        2,
        "Impresora Resina",
        "Modelo R1",
        TipoImpresora.Resina,
        "Impresora de detalle",
        0.18m,
        5m);

    var lineaFilamento = new LineaReserva(
        filamento,
        TiempoReserva.DosHoras,
        3.30m);

    var lineaResina = new LineaReserva(
        resina,
        TiempoReserva.UnaHora,
        5.18m);

    var lineas = new List<LineaReserva>();
    lineas.Add(lineaFilamento);
    lineas.Add(lineaResina);

    var fecha = new DateTime(2026, 11, 1, 10, 0, 0);

    var reserva = new ReservaImpresora(
        cliente,
        fecha,
        "Ana",
        "García",
        "Calle de prueba 1",
        MetodoPago.TarjetaCredito,
        lineas);

    _context.ReservasImpresoras.Add(reserva);
    await _context.SaveChangesAsync();

    using var contextoConsulta = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(
        contextoConsulta,
        logger.Object);

    var esperadoFilamento = new LineaReservaDetailDTO(
        1,
        "Impresora Filamento",
        "Modelo F1",
        TipoImpresora.Filamento,
        "Impresora para prototipos",
        TiempoReserva.DosHoras,
        0.15m,
        3m,
        3.30m);

    var esperadoResina = new LineaReservaDetailDTO(
        2,
        "Impresora Resina",
        "Modelo R1",
        TipoImpresora.Resina,
        "Impresora de detalle",
        TiempoReserva.UnaHora,
        0.18m,
        5m,
        5.18m);

    var lineasEsperadas = new List<LineaReservaDetailDTO>();

    foreach (var linea in lineas.OrderBy(l => l.Id))
    {
        lineasEsperadas.Add(
            linea.Impresora3DId == 1
                ? esperadoFilamento
                : esperadoResina);
    }

    var esperado = new ReservaDetailDTO(
        reserva.Id,
        fecha,
        "Ana",
        "García",
        "Calle de prueba 1",
        MetodoPago.TarjetaCredito,
        8.48m,
        lineasEsperadas);

    // Act
    var respuesta = await controller.GetReserva(reserva.Id);

    // Assert
    var ok = Assert.IsType<OkObjectResult>(respuesta);
    var obtenido = Assert.IsType<ReservaDetailDTO>(ok.Value);

    Assert.Equal(2, obtenido.LineasReserva.Count);
    Assert.True(esperado.Equals(obtenido));
}

[Fact]
public async Task ConModelStateInvalido_DevuelveBadRequest()
{
    // Arrange
    using var contextoConsulta = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(
        contextoConsulta,
        logger.Object);

    controller.ModelState.AddModelError(
        "id",
        "El identificador no es válido.");

    // Act
    var respuesta = await controller.GetReserva(1);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);

    var errores = Assert.IsType<SerializableError>(badRequest.Value);
    Assert.True(errores.ContainsKey("id"));

    var mensajes = Assert.IsType<string[]>(errores["id"]);
    Assert.Contains("El identificador no es válido.", mensajes);
}

}