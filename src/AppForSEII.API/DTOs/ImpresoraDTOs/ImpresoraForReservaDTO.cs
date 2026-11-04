namespace AppForSEII.API.DTOs.ImpresoraDTOs;

public class ImpresoraForReservaDTO
{
    public ImpresoraForReservaDTO(int id, string nombre, string modelo,
        TipoImpresora tipo, string descripcion, decimal precioKilovatioHora,
        decimal precioReserva)
    {
        Id = id;
        Nombre = nombre;
        Modelo = modelo;
        Tipo = tipo;
        Descripcion = descripcion;
        PrecioKilovatioHora = precioKilovatioHora;
        PrecioReserva = precioReserva;
    }

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Modelo { get; set; }
    public TipoImpresora Tipo { get; set; }
    public string Descripcion { get; set; }
    public decimal PrecioKilovatioHora { get; set; }
    public decimal PrecioReserva { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is ImpresoraForReservaDTO other &&
            Id == other.Id && Nombre == other.Nombre && Modelo == other.Modelo &&
            Tipo == other.Tipo && Descripcion == other.Descripcion &&
            PrecioKilovatioHora == other.PrecioKilovatioHora &&
            PrecioReserva == other.PrecioReserva;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Nombre, Modelo, Tipo, Descripcion,
            PrecioKilovatioHora, PrecioReserva);
    }
}
