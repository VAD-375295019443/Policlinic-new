using System.ComponentModel.DataAnnotations;

namespace PolyclinicWeb.Classes
{
    public class InstrumentalTest //Инструментальное исследование.
    {
        public Guid Id { get; set; } //Код.
        public string? ReleaseDate { get; set; } //Дата выполнения.
        [Required]
        public string? AppointmentDate { get; set; } //Дата назначения.
        [Required]
        public string? Name { get; set; } //Наименование исследования.
        public string? Result { get; set; } //Результат исследования.


        public Guid PatientId { get; set; } //Код связи.
        public virtual Patient? Patient { get; set; }
    }
}
