using System.ComponentModel.DataAnnotations;

namespace PolyclinicWeb.Classes
{
    public class MedicalRecommendation //Медицинская рекомендация.
    {
        public Guid Id { get; set; } //Код.
        public string? ReleaseDate { get; set; } //Дата выполнения.
        [Required]
        public string? AppointmentDate { get; set; } //Дата назначения.
        [Required]
        public string? Drug { get; set; } //Наименование препарата.
        [Required]
        public string? ReleaseForm { get; set; } //Форма выпуска.
        [Required]
        public string? Dose { get; set; } //Доза.
        [Required]
        public string? DosesNumber { get; set; } //Количество доз.
        [Required]
        public string? Signature { get; set; } //Сигратура.


        public Guid PatientId { get; set; }
        public virtual Patient? Patient { get; set; }
    }
}
