namespace AppForSEII.API.DTOs.ReservaDTOs;

public class LineaReservaDetailDTO
{
    public LineaReservaDetailDTO(int impresora3DId, string nombre, string modelo,
        TipoImpresora tipo, string descripcion, TiempoReserva tiempoReserva,
        decimal precioKilovatioHora, decimal precioReserva, decimal precioSubtotal)
    {
        Impresora3DId = impresora3DId;
        Nombre = nombre;
        Modelo = modelo;
        Tipo = tipo;
        Descripcion = descripcion;
        TiempoReserva = tiempoReserva;
        PrecioKilovatioHora = precioKilovatioHora;
        PrecioReserva = precioReserva;
        PrecioSubtotal = precioSubtotal;
    }

    public int Impresora3DId { get; set; }
    public string Nombre { get; set; }
    public string Modelo { get; set; }
    public TipoImpresora Tipo { get; set; }
    public string Descripcion { get; set; }
    public TiempoReserva TiempoReserva { get; set; }
    public decimal PrecioKilovatioHora { get; set; }
    public decimal PrecioReserva { get; set; }
    public decimal PrecioSubtotal { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is LineaReservaDetailDTO other &&
            Impresora3DId == other.Impresora3DId && Nombre == other.Nombre &&
            Modelo == other.Modelo && Tipo == other.Tipo && Descripcion == other.Descripcion &&
            TiempoReserva == other.TiempoReserva && PrecioKilovatioHora == other.PrecioKilovatioHora &&
            PrecioReserva == other.PrecioReserva && PrecioSubtotal == other.PrecioSubtotal;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            HashCode.Combine(Impresora3DId, Nombre, Modelo, Tipo, Descripcion),
            TiempoReserva, PrecioKilovatioHora, PrecioReserva, PrecioSubtotal);
    }
}
