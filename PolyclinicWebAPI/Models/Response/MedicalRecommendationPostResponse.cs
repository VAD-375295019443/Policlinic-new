namespace PolyclinicWebAPI.Models.Response
{
    public class MedicalRecommendationPostResponse
    {
        public Guid Id { get; set; } //Код.
        public string? ReleaseDate { get; set; } //Дата выполнения.
        public string? AppointmentDate { get; set; } //Дата назначения.
        public string? Drug { get; set; } //Наименование препарата.
        public string? ReleaseForm { get; set; } //Форма выпуска.
        public string? Dose { get; set; } //Доза.
        public string? DosesNumber { get; set; } //Количество доз.
        public string? Signature { get; set; } //Сигратура.
    }
}
