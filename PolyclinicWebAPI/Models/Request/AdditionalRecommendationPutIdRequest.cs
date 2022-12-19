namespace PolyclinicWebAPI.Models.Request
{
    public class AdditionalRecommendationPutIdRequest
    {
        public string? ReleaseDate { get; set; } //Дата выполнения.
        public string? AppointmentDate { get; set; } //Дата назначения.
        public string? Recommendation { get; set; } //Рекомендация.
    }
}
