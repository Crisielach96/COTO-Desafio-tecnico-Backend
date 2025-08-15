using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaReservasAPI.Models;

public class Reserva
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Cliente { get; set; } = string.Empty;

    [Required]
    [Column("salon_id")]
    public string SalonId { get; set; } = string.Empty;

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    [Column("hora_inicio")]
    public string HoraInicio { get; set; } = string.Empty;
    
    [Required]
    [Column("hora_fin")]
    public string HoraFin { get; set; } = string.Empty;
}
