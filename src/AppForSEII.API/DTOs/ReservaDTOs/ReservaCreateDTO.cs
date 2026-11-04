using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.ReservaDTOs;

public class ReservaCreateDTO
{
    public ReservaCreateDTO(
        string clienteId,
        string nombreCliente,
        string apellidosCliente,
        string direccionFacturacion,
        MetodoPago? metodoPago,
        List<LineaReservaCreateDTO> lineasReserva)
    {
        ClienteId = clienteId;
        NombreCliente = nombreCliente;
        ApellidosCliente = apellidosCliente;
        DireccionFacturacion = direccionFacturacion;
        MetodoPago = metodoPago;
        LineasReserva = lineasReserva;
    }

    [Required]
    public string ClienteId { get; set; }

    [Required]
    [StringLength(50)]
    public string NombreCliente { get; set; }

    [Required]
    [StringLength(100)]
    public string ApellidosCliente { get; set; }

    [Required]
    [StringLength(250)]
    public string DireccionFacturacion { get; set; }

    [Required(ErrorMessage = "Debes seleccionar un método de pago.")]
    [EnumDataType(typeof(MetodoPago))]
    public MetodoPago? MetodoPago { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Debes seleccionar al menos una impresora.")]
    public List<LineaReservaCreateDTO> LineasReserva { get; set; }
}