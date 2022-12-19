using System.ComponentModel.DataAnnotations;

namespace PolyclinicWeb.Classes
{
    public class DispensaryDiagnosis //Диспансерный диагноз.
    {
        public Guid Id { get; set; } //Код.
        public string? DeregistrationDate { get; set; } //Дата снятия с диспансерного учета.
        [Required]
        public string? RegistrationDate { get; set; } //Дата взятия на диспансерный учет.
        [Required]
        public string? Diagnosis { get; set; } //Диагноз диспансерного учета.


        public Guid PatientId { get; set; } //Код связи.
        public virtual Patient? Patient { get; set; }
    }
}
