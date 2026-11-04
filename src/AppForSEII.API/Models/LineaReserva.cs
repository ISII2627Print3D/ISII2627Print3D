using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII.API.Models;

public class LineaReserva
{
    [Key]
    public int Id { get; set; }

    [EnumDataType(typeof(TiempoReserva))]
    public TiempoReserva TiempoReserva { get; set; }
        = global::AppForSEII.API.Models.TiempoReserva.UnaHora;

    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0", "99999999.99",
        ParseLimitsInInvariantCulture = true)]
    public decimal PrecioSubtotal { get; set; }

    public int ReservaImpresoraId { get; set; }

    [ForeignKey(nameof(ReservaImpresoraId))]
    public ReservaImpresora ReservaImpresora { get; set; } = null!;

    public int Impresora3DId { get; set; }

    [ForeignKey(nameof(Impresora3DId))]
    public Impresora3D Impresora3D { get; set; } = null!;
public LineaReserva()
{
}

public LineaReserva(
    Impresora3D impresora,
    TiempoReserva tiempoReserva,
    decimal precioSubtotal)
{
    Impresora3D = impresora;
    Impresora3DId = impresora.Id;
    TiempoReserva = tiempoReserva;
    PrecioSubtotal = precioSubtotal;
}
}