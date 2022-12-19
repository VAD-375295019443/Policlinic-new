namespace PolyclinicWebAPI.Models.Response
{
    public class AdditionalRecommendationGetIdResponse
    {
        public Guid Id { get; set; } //Код.
        public string? ReleaseDate { get; set; } //Дата выполнения.
        public string? AppointmentDate { get; set; } //Дата назначения.
        public string? Recommendation { get; set; } //Рекомендация.
    }
}
