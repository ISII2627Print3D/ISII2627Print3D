using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models;

public class Cliente : ApplicationUser
{
    public Cliente()
    {
    }

    public Cliente(
        string id,
        string nombre,
        string apellidos,
        string email,
        string direccionFacturacion)
        : base(id, nombre, apellidos, email)
    {
        DireccionFacturacion = direccionFacturacion;
    }

    [Required]
    [StringLength(250)]
    public string DireccionFacturacion { get; set; } = string.Empty;
}