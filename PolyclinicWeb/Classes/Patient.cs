using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicWeb.Classes
{
    public class Patient //Пациент.
    {
        public Guid Id { get; set; } //Код.
        [Required]
        [EmailAddress]
        public string? Email { get; set; } //Логин.
        [DataType(DataType.Password)]
        public string? Password { get; set; } //Пароль.
        [Required]
        public string? Surname { get; set; } //Фамилия.
        [Required]
        public string? Name { get; set; } //Имя.
        [Required]
        public string? MiddleName { get; set; } //Отчество.
        [Required]
        public string? DateOfBirth { get; set; } //Дата рождения.
        [Required]
        public string? Floor { get; set; } //Пол.
        [Required]
        [Phone]
        public string? Telephone { get; set; } //Телефон.
        [Required]
        public string? Country { get; set; } //Страна.
        [Required]
        public string? Region { get; set; } //Область.
        [Required]
        public string? District { get; set; } //Район.
        [Required]
        public string? LocalityType { get; set; } //Статус населенного пункта.
        [Required]
        public string? Locality { get; set; } //Населенный пункт.
        [Required]
        public string? StreetType { get; set; } //Статус улицы.
        [Required]
        public string? Street { get; set; } //Улица.
        [Required]
        public uint? House { get; set; } //Дом.
        [Required]
        public uint? Flat { get; set; } //Квартира.
        public string? AdditionalInformation { get; set; } //Дополнительная информация.

        public virtual List<Vaccination>? Vaccinations { get; set; }//Лист вакцинаций.
        public virtual List<ClinicalDiagnosis>? ClinicalDiagnoses { get; set; } //Лист клинических диагнозов.
        public virtual List<DispensaryDiagnosis>? DispensaryDiagnoses { get; set; } //Лист диспансерных диагнозов.
        public virtual List<InstrumentalTest>? InstrumentalTests { get; set; } //Лист инструментальных исследований.
        public virtual List<LaboratoryTest>? LaboratoryTests { get; set; } //Лист лабораторных исследований.
        public virtual List<MedicalRecommendation>? MedicalRecommendations { get; set; } //Лист медицинских рекомендаций.
        public virtual List<AdditionalRecommendation>? AdditionalRecommendations { get; set; } //Лист дополнительных рекомендаций.
    }
}
