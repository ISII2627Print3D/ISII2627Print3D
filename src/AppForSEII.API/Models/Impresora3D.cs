using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII.API.Models;

public class Impresora3D
{
    public Impresora3D()
{
}

public Impresora3D(
    int id,
    string nombre,
    string modelo,
    TipoImpresora tipo,
    string descripcion,
    decimal precioKilovatioHora,
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

    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Modelo { get; set; } = string.Empty;

    [EnumDataType(typeof(TipoImpresora))]
    public TipoImpresora Tipo { get; set; }

    [Required]
    [StringLength(1000)]
    public string Descripcion { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,4)")]
    [Range(typeof(decimal), "0", "999999.9999")]
    public decimal PrecioKilovatioHora { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal PrecioReserva { get; set; }

    public ICollection<LineaReserva> LineasReserva { get; set; }
    = new List<LineaReserva>();
}