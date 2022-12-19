namespace PolyclinicWebAPI.Models.Response
{
    public class PatientPostResponse
    {
        public Guid Id { get; set; } //Код.
        public string? Email { get; set; } //Логин.
        public string? Surname { get; set; } //Фамилия.
        public string? Name { get; set; } //Имя.
        public string? MiddleName { get; set; } //Отчество.
        public string? DateOfBirth { get; set; } //Дата рождения.
        public string? Floor { get; set; } //Пол.
        public string? Telephone { get; set; } //Телефон.
        public string? Country { get; set; } //Страна.
        public string? Region { get; set; } //Область.
        public string? District { get; set; } //Район.
        public string? LocalityType { get; set; } //Статус населенного пункта.
        public string? Locality { get; set; } //Населенный пункт.
        public string? StreetType { get; set; } //Статус улицы.
        public string? Street { get; set; } //Улица.
        public uint? House { get; set; } //Дом.
        public uint? Flat { get; set; } //Квартира.
        public string? AdditionalInformation { get; set; } //Дополнительная информация.
    }
}
