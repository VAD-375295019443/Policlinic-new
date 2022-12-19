namespace PolyclinicWebAPI.Models.Response
{
    public class ClinicalDiagnosisGetIdResponse
    {
        public Guid Id { get; set; } //Код.
        public string? DeregistrationDate { get; set; } //Дата снятия с учета.
        public string? RegistrationDate { get; set; } //Дата взятия на учет.
        public string? Diagnosis { get; set; } //Диагноз.
    }
}
