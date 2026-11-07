namespace AppForSEII.API.DTOs.ReservaDTOs;

public class ReservaDetailDTO
{
    public ReservaDetailDTO(int id, DateTime fechaReserva, string nombreCliente,
        string apellidosCliente, string direccionFacturacion, MetodoPago metodoPago,
        decimal precioTotal, List<LineaReservaDetailDTO> lineasReserva)
    {
        Id = id;
        FechaReserva = fechaReserva;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionFacturacion = direccionFacturacion;
        MetodoPago = metodoPago;
        PrecioTotal = precioTotal;
        LineasReserva = lineasReserva;
    }

    public int Id { get; set; }
    public DateTime FechaReserva { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidosCliente { get; set; }
    public string DireccionFacturacion { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public decimal PrecioTotal { get; set; }
    public List<LineaReservaDetailDTO> LineasReserva { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is ReservaDetailDTO other &&
            Id == other.Id && FechaReserva == other.FechaReserva &&
            NombreCliente == other.NombreCliente && ApellidosCliente == other.ApellidosCliente &&
            DireccionFacturacion == other.DireccionFacturacion && MetodoPago == other.MetodoPago &&
            PrecioTotal == other.PrecioTotal && LineasReserva.SequenceEqual(other.LineasReserva);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id);
        hash.Add(FechaReserva);
        hash.Add(NombreCliente);
        hash.Add(ApellidosCliente);
        hash.Add(DireccionFacturacion);
        hash.Add(MetodoPago);
        hash.Add(PrecioTotal);
        foreach (var linea in LineasReserva)
            hash.Add(linea);
        return hash.ToHashCode();
    }
}
