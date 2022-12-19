using System.ComponentModel.DataAnnotations;

namespace PolyclinicWeb.Classes
{
    public class Vaccination
    {
        public Guid Id { get; set; }
        public string? ReleaseDate { get; set; } //Дата выполнения.
        [Required]
        public string? AppointmentDate { get; set; } //Дата назначения.
        [Required]
        public string? Type { get; set; } //Тип вакцинации.
        public string? Dose { get; set; } //Доза вакцины.
        public string? Series { get; set; } //Серия препарата.
        [Required]
        public string? Name { get; set; } //Наименование вакцины.
        public string? Production { get; set; } //Место производства вакцины.
        public string? AdditionalInformation { get; set; } //Дополнительная информация.


        public Guid PatientId { get; set; }
        public virtual Patient? Patient { get; set; }
    }
}
