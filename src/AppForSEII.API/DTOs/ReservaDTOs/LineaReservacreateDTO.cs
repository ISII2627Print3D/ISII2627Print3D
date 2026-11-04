using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.ReservaDTOs;

public class LineaReservaCreateDTO
{
    public LineaReservaCreateDTO(
        int impresora3DId,
        TiempoReserva tiempoReserva = TiempoReserva.UnaHora)
    {
        Impresora3DId = impresora3DId;
        TiempoReserva = tiempoReserva;
    }

    [Range(1, int.MaxValue,
        ErrorMessage = "El identificador de la impresora debe ser positivo.")]
    public int Impresora3DId { get; set; }

    [EnumDataType(typeof(TiempoReserva),
        ErrorMessage = "El tiempo de reserva no es válido.")]
    public TiempoReserva TiempoReserva { get; set; }
}