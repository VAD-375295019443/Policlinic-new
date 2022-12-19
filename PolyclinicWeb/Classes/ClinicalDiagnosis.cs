using System.ComponentModel.DataAnnotations;

namespace PolyclinicWeb.Classes
{
    public class ClinicalDiagnosis //Клинический диагноз.
    {
        public Guid Id { get; set; } //Код.
        public string? DeregistrationDate { get; set; } //Дата снятия с учета.
        [Required]
        public string? RegistrationDate { get; set; } //Дата взятия на учет.
        [Required]
        public string? Diagnosis { get; set; } //Диагноз.


        public Guid PatientId { get; set; } //Код связи.
        public virtual Patient? Patient { get; set; }
    }
}
