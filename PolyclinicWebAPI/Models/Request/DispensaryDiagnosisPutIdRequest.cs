namespace PolyclinicWebAPI.Models.Request
{
    public class DispensaryDiagnosisPutIdRequest
    {
        public string? DeregistrationDate { get; set; } //Дата снятия с диспансерного учета.
        public string? RegistrationDate { get; set; } //Дата взятия на диспансерный учет.
        public string? Diagnosis { get; set; } //Диагноз диспансерного учета.
    }
}
