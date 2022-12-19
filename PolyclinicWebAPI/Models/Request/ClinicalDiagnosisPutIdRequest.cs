namespace PolyclinicWebAPI.Models.Request
{
    public class ClinicalDiagnosisPutIdRequest
    {
        public string? DeregistrationDate { get; set; } //Дата снятия с учета.
        public string? RegistrationDate { get; set; } //Дата взятия на учет.
        public string? Diagnosis { get; set; } //Диагноз.
    }
}
