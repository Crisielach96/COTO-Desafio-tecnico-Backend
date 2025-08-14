using System.ComponentModel.DataAnnotations;

namespace SistemaReservasAPI.Models;

public class Reserva
{
    public int Id { get; set; }

    [Required]
    public string Cliente { get; set; } = string.Empty;

    [Required]
    public string SalonId { get; set; } = string.Empty;

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    public string HoraInicio { get; set; } = string.Empty;
    
    [Required]
    public string HoraFin { get; set; } = string.Empty;
}
