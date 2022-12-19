namespace PolyclinicWebAPI.Models.Request
{
    public class LaboratoryTestPostRequest
    {
        public string? ReleaseDate { get; set; } //Дата выполнения.
        public string? AppointmentDate { get; set; } //Дата назначения.
        public string? Name { get; set; } //Наименование исследования.
        public string? Result { get; set; } //Результат исследования.
    }
}
