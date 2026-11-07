using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ImpresoraDTOs;

namespace AppForSEII.UT.ImpresorasController_test;

public class GetImpresorasForReserva_test : AppForSEII4SqliteUT
{
    [Fact]
    public async Task SinFiltros_DevuelveTodasLasImpresorasOrdenadas() //Guarda dos impesoras
    {
        // Arrange
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

        _context.Impresoras3D.AddRange(resina, filamento);
        await _context.SaveChangesAsync();

        using var contextoConsulta = CreateContext();

        var logger = new Mock<ILogger<ImpresorasController>>();
        var controller = new ImpresorasController(
            contextoConsulta,
            logger.Object);

        var esperadoFilamento = new ImpresoraForReservaDTO(
            1,
            "Impresora Filamento",
            "Modelo F1",
            TipoImpresora.Filamento,
            "Impresora para prototipos",
            0.15m,
            3m);

        var esperadoResina = new ImpresoraForReservaDTO(
            2,
            "Impresora Resina",
            "Modelo R1",
            TipoImpresora.Resina,
            "Impresora de detalle",
            0.18m,
            5m);

        // Act
        var respuesta = await controller.GetImpresorasForReserva();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(respuesta);
        var impresoras = Assert.IsType<List<ImpresoraForReservaDTO>>(ok.Value);

        Assert.Equal(2, impresoras.Count);
        Assert.True(esperadoFilamento.Equals(impresoras[0]));
        Assert.True(esperadoResina.Equals(impresoras[1]));
    }
    [Fact]
public async Task ConFiltroNombre_DevuelveSoloLasCoincidencias() //guarda las dos y solicita una
{
    // Arrange
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

    _context.Impresoras3D.AddRange(filamento, resina);
    await _context.SaveChangesAsync();

    using var contextoConsulta = CreateContext();

    var logger = new Mock<ILogger<ImpresorasController>>();
    var controller = new ImpresorasController(
        contextoConsulta,
        logger.Object);

    var esperado = new ImpresoraForReservaDTO(
        2,
        "Impresora Resina",
        "Modelo R1",
        TipoImpresora.Resina,
        "Impresora de detalle",
        0.18m,
        5m);

    // Act
    var respuesta = await controller.GetImpresorasForReserva(
        nombre: "Resina");

    // Assert
    var ok = Assert.IsType<OkObjectResult>(respuesta);
    var impresoras = Assert.IsType<List<ImpresoraForReservaDTO>>(ok.Value);

    var impresora = Assert.Single(impresoras);
    Assert.True(esperado.Equals(impresora));
}
[Fact] //solicita la de filamento solo
public async Task ConFiltroTipo_DevuelveSoloLasImpresorasDeEseTipo()
{
    // Arrange
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

    _context.Impresoras3D.AddRange(filamento, resina);
    await _context.SaveChangesAsync();

    using var contextoConsulta = CreateContext();

    var logger = new Mock<ILogger<ImpresorasController>>();
    var controller = new ImpresorasController(
        contextoConsulta,
        logger.Object);

    var esperado = new ImpresoraForReservaDTO(
        1,
        "Impresora Filamento",
        "Modelo F1",
        TipoImpresora.Filamento,
        "Impresora para prototipos",
        0.15m,
        3m);

    // Act
    var respuesta = await controller.GetImpresorasForReserva(
        tipo: TipoImpresora.Filamento);

    // Assert
    var ok = Assert.IsType<OkObjectResult>(respuesta);
    var impresoras = Assert.IsType<List<ImpresoraForReservaDTO>>(ok.Value);

    var impresora = Assert.Single(impresoras);
    Assert.True(esperado.Equals(impresora));
}

[Theory]
[InlineData(TipoImpresora.Resina, true)]
[InlineData(TipoImpresora.Filamento, false)]
public async Task ConNombreYTipo_DevuelveSoloCoincidenciasDeAmbos(
    TipoImpresora tipo,
    bool hayCoincidencia)
{
    // Arrange
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

    _context.Impresoras3D.AddRange(filamento, resina);
    await _context.SaveChangesAsync();

    using var contextoConsulta = CreateContext();

    var logger = new Mock<ILogger<ImpresorasController>>();
    var controller = new ImpresorasController(
        contextoConsulta,
        logger.Object);

    var esperado = new ImpresoraForReservaDTO(
        2,
        "Impresora Resina",
        "Modelo R1",
        TipoImpresora.Resina,
        "Impresora de detalle",
        0.18m,
        5m);

    // Act
    var respuesta = await controller.GetImpresorasForReserva(
        nombre: "Resina",
        tipo: tipo);

    // Assert
    var ok = Assert.IsType<OkObjectResult>(respuesta);
    var impresoras = Assert.IsType<List<ImpresoraForReservaDTO>>(ok.Value);

    if (hayCoincidencia)
    {
        var impresora = Assert.Single(impresoras);
        Assert.True(esperado.Equals(impresora));
    }
    else
    {
        Assert.Empty(impresoras);
    }
}

[Theory]
[InlineData(99)]
[InlineData(-1)]
public async Task ConTipoInvalido_DevuelveBadRequest(int tipoInvalido){
    // Arrange
    using var contextoConsulta = CreateContext();

    var logger = new Mock<ILogger<ImpresorasController>>();
    var controller = new ImpresorasController(
        contextoConsulta,
        logger.Object);

    // Act
    var respuesta = await controller.GetImpresorasForReserva(
        tipo: (TipoImpresora)tipoInvalido);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);

    var errores = Assert.IsType<SerializableError>(badRequest.Value);
    Assert.True(errores.ContainsKey("tipo"));

    var mensajes = Assert.IsType<string[]>(errores["tipo"]);
    Assert.Contains("El tipo de impresora no es válido.", mensajes);
}

[Fact]
public async Task ConModelStateInvalido_DevuelveBadRequest()
{
    // Arrange
    using var contextoConsulta = CreateContext();

    var logger = new Mock<ILogger<ImpresorasController>>();
    var controller = new ImpresorasController(
        contextoConsulta,
        logger.Object);

    controller.ModelState.AddModelError(
        "nombre",
        "El filtro de nombre no es válido.");

    // Act
    var respuesta = await controller.GetImpresorasForReserva();

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);

    var errores = Assert.IsType<SerializableError>(badRequest.Value);
    Assert.True(errores.ContainsKey("nombre"));

    var mensajes = Assert.IsType<string[]>(errores["nombre"]);
    Assert.Contains("El filtro de nombre no es válido.", mensajes);
}

}