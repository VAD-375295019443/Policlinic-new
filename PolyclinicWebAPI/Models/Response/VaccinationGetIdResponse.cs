namespace PolyclinicWebAPI.Models.Response
{
    public class VaccinationGetIdResponse
    {
        public Guid Id { get; set; }
        public string? ReleaseDate { get; set; } //Дата выполнения.
        public string? AppointmentDate { get; set; } //Дата назначения.
        public string? Type { get; set; } //Тип вакцинации.
        public string? Dose { get; set; } //Доза вакцины.
        public string? Series { get; set; } //Серия препарата.
        public string? Name { get; set; } //Наименование вакцины.
        public string? Production { get; set; } //Место производства вакцины.
        public string? AdditionalInformation { get; set; } //Дополнительная информация.
    }
}
