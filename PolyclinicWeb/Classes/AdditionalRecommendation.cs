using System.ComponentModel.DataAnnotations;

namespace PolyclinicWeb.Classes
{
    public class AdditionalRecommendation //Дополнительная рекомендация.
    {
        public Guid Id { get; set; } //Код.
        public string? ReleaseDate { get; set; } //Дата выполнения.
        [Required]
        public string? AppointmentDate { get; set; } //Дата назначения.
        [Required]
        public string? Recommendation { get; set; } //Рекомендация.


        public Guid PatientId { get; set; } //Код связи.
        public virtual Patient? Patient { get; set; }
    }
}
