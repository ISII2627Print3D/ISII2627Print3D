using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ReservaDTOs;

namespace AppForSEII.UT.ReservasController_test;

public class CreateReserva_test : AppForSEII4SqliteUT
{
    [Fact]
    public async Task SinMetodoPago_DevuelveBadRequestYNoGuardaReserva()
    {
        // Arrange
        var linea = new LineaReservaCreateDTO(
            1,
            TiempoReserva.DosHoras);

        var lineas = new List<LineaReservaCreateDTO>();
        lineas.Add(linea);

        var solicitud = new ReservaCreateDTO(
            "cliente-test",
            "Ana",
            "García",
            "Calle de prueba 1 Albacete",
            null,
            lineas);

        using var contexto = CreateContext();

        var logger = new Mock<ILogger<ReservasController>>();
        var controller = new ReservasController(
            contexto,
            logger.Object);

        // Act
        var respuesta = await controller.CreateReserva(solicitud);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
        Assert.Equal(400, badRequest.StatusCode);
        Assert.Equal(
            "Debes seleccionar un método de pago.",
            badRequest.Value);

        using var contextoComprobacion = CreateContext();

        Assert.False(
            await contextoComprobacion.ReservasImpresoras.AnyAsync());

        Assert.False(
            await contextoComprobacion.LineasReserva.AnyAsync());
    }


    [Theory]
[InlineData(99)]
[InlineData(-1)]
public async Task ConMetodoPagoInvalido_DevuelveBadRequestYNoGuarda(
    int metodoPago)
{
    // Arrange
    var linea = new LineaReservaCreateDTO(
        1,
        TiempoReserva.DosHoras);

    var lineas = new List<LineaReservaCreateDTO>();
    lineas.Add(linea);

    var solicitud = new ReservaCreateDTO(
        "cliente-test",
        "Ana",
        "García",
        "Calle de prueba 1",
        (MetodoPago)metodoPago,
        lineas);

    using var contexto = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(
        contexto,
        logger.Object);

    // Act
    var respuesta = await controller.CreateReserva(solicitud);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);
    Assert.Equal(
        "El método de pago no es válido.",
        badRequest.Value);

    using var contextoComprobacion = CreateContext();

    Assert.False(
        await contextoComprobacion.ReservasImpresoras.AnyAsync());

    Assert.False(
        await contextoComprobacion.LineasReserva.AnyAsync());
}
[Theory]
[InlineData(false)]
[InlineData(true)]
public async Task SinImpresoras_DevuelveBadRequestYNoGuarda(bool listaNula)
{
    // Arrange
    var lineas = new List<LineaReservaCreateDTO>();

    if (listaNula)
        lineas = null!;

    var solicitud = new ReservaCreateDTO(
        "cliente-test",
        "Ana",
        "García",
        "Calle de prueba 1 Albacete",
        MetodoPago.TarjetaCredito,
        lineas);

    using var contexto = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(
        contexto,
        logger.Object);

    // Act
    var respuesta = await controller.CreateReserva(solicitud);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);
    Assert.Equal(
        "Debes seleccionar al menos una impresora.",
        badRequest.Value);

    using var contextoComprobacion = CreateContext();

    Assert.False(
        await contextoComprobacion.ReservasImpresoras.AnyAsync());

    Assert.False(
        await contextoComprobacion.LineasReserva.AnyAsync());
}

[Fact]
public async Task ConModelStateInvalido_DevuelveBadRequestYNoGuarda()
{
    // Arrange
    var linea = new LineaReservaCreateDTO(
        1,
        TiempoReserva.DosHoras);

    var lineas = new List<LineaReservaCreateDTO>();
    lineas.Add(linea);

    var solicitud = new ReservaCreateDTO(
        "cliente-test",
        "",
        "García",
        "Calle de prueba 1 Albacete",
        MetodoPago.TarjetaCredito,
        lineas);

    using var contexto = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(
        contexto,
        logger.Object);

    controller.ModelState.AddModelError(
        "NombreCliente",
        "El nombre es obligatorio.");

    // Act
    var respuesta = await controller.CreateReserva(solicitud);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);

    var errores = Assert.IsType<SerializableError>(badRequest.Value);
    Assert.True(errores.ContainsKey("NombreCliente"));

    var mensajes = Assert.IsType<string[]>(errores["NombreCliente"]);
    Assert.Contains("El nombre es obligatorio.", mensajes);

    using var contextoComprobacion = CreateContext();

    Assert.False(
        await contextoComprobacion.ReservasImpresoras.AnyAsync());

    Assert.False(
        await contextoComprobacion.LineasReserva.AnyAsync());
}

[Theory]
[InlineData(0, 1)]
[InlineData(-1, 1)]
[InlineData(1, 6)]
[InlineData(1, -1)]
public async Task ConLineaInvalida_DevuelveBadRequestYNoGuarda(
    int impresoraId,
    int horas)
{
    // Arrange
    var linea = new LineaReservaCreateDTO(
        impresoraId,
        (TiempoReserva)horas);

    var lineas = new List<LineaReservaCreateDTO>();
    lineas.Add(linea);

    var solicitud = new ReservaCreateDTO(
        "cliente-test",
        "Ana",
        "García",
        "Calle de prueba 1 Albacete",
        MetodoPago.TarjetaCredito,
        lineas);

    using var contexto = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(
        contexto,
        logger.Object);

    // Act
    var respuesta = await controller.CreateReserva(solicitud);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);
    Assert.Equal(
        "Una línea contiene una impresora o un tiempo no válido.",
        badRequest.Value);

    using var contextoComprobacion = CreateContext();

    Assert.False(
        await contextoComprobacion.ReservasImpresoras.AnyAsync());

    Assert.False(
        await contextoComprobacion.LineasReserva.AnyAsync());
}

[Fact]
public async Task ConLineaNula_DevuelveBadRequestYNoGuarda()
{
    // Arrange
    var lineas = new List<LineaReservaCreateDTO>();
    lineas.Add(null!);

    var solicitud = new ReservaCreateDTO(
        "cliente-test",
        "Ana",
        "García",
        "Calle de prueba 1 Albacete",
        MetodoPago.TarjetaCredito,
        lineas);

    using var contexto = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(
        contexto,
        logger.Object);

    // Act
    var respuesta = await controller.CreateReserva(solicitud);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);
    Assert.Equal(
        "Una línea contiene una impresora o un tiempo no válido.",
        badRequest.Value);

    using var contextoComprobacion = CreateContext();

    Assert.False(
        await contextoComprobacion.ReservasImpresoras.AnyAsync());

    Assert.False(
        await contextoComprobacion.LineasReserva.AnyAsync());
}

[Fact]
public async Task ConClienteInexistente_DevuelveBadRequestYNoGuarda()
{
    // Arrange
    var linea = new LineaReservaCreateDTO(
        1,
        TiempoReserva.DosHoras);

    var lineas = new List<LineaReservaCreateDTO>();
    lineas.Add(linea);

    var solicitud = new ReservaCreateDTO(
        "cliente-inexistente",
        "Ana",
        "García",
        "Calle de prueba 1",
        MetodoPago.TarjetaCredito,
        lineas);

    using var contexto = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(
        contexto,
        logger.Object);

    // Act
    var respuesta = await controller.CreateReserva(solicitud);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);
    Assert.Equal(
        "El cliente indicado no existe.",
        badRequest.Value);

    using var contextoComprobacion = CreateContext();

    Assert.False(
        await contextoComprobacion.ReservasImpresoras.AnyAsync());

    Assert.False(
        await contextoComprobacion.LineasReserva.AnyAsync());
}

[Fact]
public async Task ConImpresorasDuplicadas_DevuelveBadRequestYNoGuarda()
{
    // Arrange
    var cliente = new Cliente(
        "cliente-test",
        "Ana",
        "García",
        "ana@example.com",
        "Calle de prueba 1");

    _context.Clientes.Add(cliente);
    await _context.SaveChangesAsync();

    var primeraLinea = new LineaReservaCreateDTO(
        1,
        TiempoReserva.DosHoras);

    var segundaLinea = new LineaReservaCreateDTO(
        1,
        TiempoReserva.UnaHora);

    var lineas = new List<LineaReservaCreateDTO>();
    lineas.Add(primeraLinea);
    lineas.Add(segundaLinea);

    var solicitud = new ReservaCreateDTO(
        cliente.Id,
        "Ana",
        "García",
        "Calle de prueba 1",
        MetodoPago.TarjetaCredito,
        lineas);

    using var contexto = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(
        contexto,
        logger.Object);

    // Act
    var respuesta = await controller.CreateReserva(solicitud);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);
    Assert.Equal(
        "No puedes incluir la misma impresora varias veces.",
        badRequest.Value);

    using var contextoComprobacion = CreateContext();

    Assert.False(
        await contextoComprobacion.ReservasImpresoras.AnyAsync());

    Assert.False(
        await contextoComprobacion.LineasReserva.AnyAsync());
}

[Fact]
public async Task ConImpresoraInexistente_DevuelveBadRequestYNoGuarda()
{
    // Arrange
    var cliente = new Cliente(
        "cliente-test",
        "Ana",
        "García",
        "ana@example.com",
        "Calle de prueba 1");

    _context.Clientes.Add(cliente);
    await _context.SaveChangesAsync();

    var linea = new LineaReservaCreateDTO(
        99,
        TiempoReserva.DosHoras);

    var lineas = new List<LineaReservaCreateDTO>();
    lineas.Add(linea);

    var solicitud = new ReservaCreateDTO(
        cliente.Id,
        "Ana",
        "García",
        "Calle de prueba 1",
        MetodoPago.TarjetaCredito,
        lineas);

    using var contexto = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(
        contexto,
        logger.Object);

    // Act
    var respuesta = await controller.CreateReserva(solicitud);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);
    Assert.Equal(
        "Alguna de las impresoras seleccionadas ya no existe.",
        badRequest.Value);

    using var contextoComprobacion = CreateContext();

    Assert.False(
        await contextoComprobacion.ReservasImpresoras.AnyAsync());

    Assert.False(
        await contextoComprobacion.LineasReserva.AnyAsync());
}

[Fact]
public async Task ConDatosValidos_CreaReservaConDosImpresoras()
{
    // Arrange
    var cliente = new Cliente(
        "cliente-test", "Ana", "García",
        "ana@example.com", "Calle de prueba Albacete");

    var filamento = new Impresora3D(
        1, "Impresora Filamento", "Modelo F1",
        TipoImpresora.Filamento, "Impresora para prototipos",
        0.15m, 3m);

    var resina = new Impresora3D(
        2, "Impresora Resina", "Modelo R1",
        TipoImpresora.Resina, "Impresora de detalle",
        0.18m, 5m);

    _context.Clientes.Add(cliente);
    _context.Impresoras3D.AddRange(filamento, resina);
    await _context.SaveChangesAsync();

    var lineas = new List<LineaReservaCreateDTO>();
    lineas.Add(new LineaReservaCreateDTO(1, TiempoReserva.DosHoras));
    lineas.Add(new LineaReservaCreateDTO(2, TiempoReserva.UnaHora));

    var solicitud = new ReservaCreateDTO(
        cliente.Id,
        "Ana",
        "García",
        "Calle de prueba Albacete",
        MetodoPago.TarjetaCredito,
        lineas);

    using var contexto = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(contexto, logger.Object);

    var inicio = DateTime.UtcNow;

    // Act
    var respuesta = await controller.CreateReserva(solicitud);

    // Assert
    var fin = DateTime.UtcNow;

    var created = Assert.IsType<CreatedAtActionResult>(respuesta);
    Assert.Equal(201, created.StatusCode);

    var obtenido = Assert.IsType<ReservaDetailDTO>(created.Value);

    Assert.True(obtenido.Id > 0);
    Assert.InRange(obtenido.FechaReserva, inicio, fin);
    Assert.Equal(nameof(ReservasController.GetReserva), created.ActionName);
    Assert.NotNull(created.RouteValues);
    Assert.Equal(obtenido.Id, Assert.IsType<int>(created.RouteValues["id"]));

    var esperadoFilamento = new LineaReservaDetailDTO(
        1, "Impresora Filamento", "Modelo F1",
        TipoImpresora.Filamento, "Impresora para prototipos",
        TiempoReserva.DosHoras, 0.15m, 3m, 3.30m);

    var esperadoResina = new LineaReservaDetailDTO(
        2, "Impresora Resina", "Modelo R1",
        TipoImpresora.Resina, "Impresora de detalle",
        TiempoReserva.UnaHora, 0.18m, 5m, 5.18m);

    var lineasEsperadas = new List<LineaReservaDetailDTO>();
    lineasEsperadas.Add(esperadoFilamento);
    lineasEsperadas.Add(esperadoResina);

    var esperado = new ReservaDetailDTO(
        obtenido.Id,
        obtenido.FechaReserva,
        "Ana",
        "García",
        "Calle de prueba Albacete",
        MetodoPago.TarjetaCredito,
        8.48m,
        lineasEsperadas);

    // Comparamos las líneas por impresora para no depender
    // de los identificadores generados para cada línea.
    obtenido.LineasReserva = obtenido.LineasReserva
        .OrderBy(l => l.Impresora3DId)
        .ToList();

    Assert.True(esperado.Equals(obtenido));

    using var contextoComprobacion = CreateContext();

    var guardada = await contextoComprobacion.ReservasImpresoras
        .Include(r => r.LineasReserva)
        .SingleAsync();

    Assert.Equal(obtenido.Id, guardada.Id);
    Assert.Equal(cliente.Id, guardada.ClienteId);
    Assert.Equal("Ana", guardada.NombreCliente);
    Assert.Equal("García", guardada.ApellidosCliente);
    Assert.Equal("Calle de prueba Albacete", guardada.DireccionFacturacion);
    Assert.Equal(MetodoPago.TarjetaCredito, guardada.MetodoPago);
    Assert.Equal(obtenido.FechaReserva, guardada.FechaReserva);
    Assert.Equal(8.48m, guardada.PrecioTotal);
    Assert.Equal(2, guardada.LineasReserva.Count);

    var lineaFilamento = Assert.Single(
        guardada.LineasReserva.Where(l => l.Impresora3DId == 1));

    Assert.Equal(TiempoReserva.DosHoras, lineaFilamento.TiempoReserva);
    Assert.Equal(3.30m, lineaFilamento.PrecioSubtotal);

    var lineaResina = Assert.Single(
        guardada.LineasReserva.Where(l => l.Impresora3DId == 2));

    Assert.Equal(TiempoReserva.UnaHora, lineaResina.TiempoReserva);
    Assert.Equal(5.18m, lineaResina.PrecioSubtotal);
}

[Theory]
[InlineData(-1, 0, TiempoReserva.UnaHora)]
[InlineData(80000000, 999999, TiempoReserva.DosDias)]
public async Task ConSubtotalFueraDeLimites_DevuelveBadRequestYNoGuarda(
    int precioReserva,
    int precioKilovatioHora,
    TiempoReserva tiempo)
{
    // Arrange
    var cliente = new Cliente(
        "cliente-test", "Ana", "García",
        "ana@example.com", "Calle de prueba 1 Albacete");

    var impresora = new Impresora3D(
        1, "Impresora de prueba", "Modelo F1",
        TipoImpresora.Filamento, "Impresora para pruebas",
        precioKilovatioHora, precioReserva);

    _context.Clientes.Add(cliente);
    _context.Impresoras3D.Add(impresora);
    await _context.SaveChangesAsync();

    var lineas = new List<LineaReservaCreateDTO>();
    lineas.Add(new LineaReservaCreateDTO(1, tiempo));

    var solicitud = new ReservaCreateDTO(
        cliente.Id,
        "Ana",
        "García",
        "Calle de prueba 1 Albacete",
        MetodoPago.TarjetaCredito,
        lineas);

    using var contexto = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(contexto, logger.Object);

    // Act
    var respuesta = await controller.CreateReserva(solicitud);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);
    Assert.Equal(
        "El subtotal está fuera del importe permitido.",
        badRequest.Value);

    using var contextoComprobacion = CreateContext();

    Assert.False(
        await contextoComprobacion.ReservasImpresoras.AnyAsync());

    Assert.False(
        await contextoComprobacion.LineasReserva.AnyAsync());
}

///
/// 

[Fact]
public async Task ConDireccionNovalida()
{
    // Arrange
    var cliente = new Cliente(
        "cliente-test", "Ana", "García",
        "ana@example.com", "Calle de prueba ");

    var filamento = new Impresora3D(
        1, "Impresora Filamento", "Modelo F1",
        TipoImpresora.Filamento, "Impresora para prototipos",
        0.15m, 3m);


    _context.Clientes.Add(cliente);
    _context.Impresoras3D.AddRange(filamento);
    await _context.SaveChangesAsync();

    var lineas = new List<LineaReservaCreateDTO>();
    lineas.Add(new LineaReservaCreateDTO(1, TiempoReserva.DosHoras));


    var solicitud = new ReservaCreateDTO(
        cliente.Id,
        "Ana",
        "García",
        "Calle de prueba ",
        MetodoPago.TarjetaCredito,
        lineas);

    using var contexto = CreateContext();

    var logger = new Mock<ILogger<ReservasController>>();
    var controller = new ReservasController(contexto, logger.Object);

    var inicio = DateTime.UtcNow;

    // Act
    var respuesta = await controller.CreateReserva(solicitud);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta);
    Assert.Equal(400, badRequest.StatusCode);
    Assert.Equal(
        "!Error! solo se admiten direciones de facturacion en albacete",
        badRequest.Value);

    using var contextoComprobacion = CreateContext();

    Assert.False(
        await contextoComprobacion.ReservasImpresoras.AnyAsync());

    Assert.False(
        await contextoComprobacion.LineasReserva.AnyAsync());
}

}


