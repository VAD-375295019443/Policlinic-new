using Microsoft.EntityFrameworkCore;

namespace PolyclinicWeb.Classes
{
    public class PolyclinicContext : DbContext //Не забыть using Microsoft.EntityFrameworkCore;
    {
        //Набор объектов, хранимых в БД.
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Vaccination> Vaccinations { get; set; }
        public DbSet<ClinicalDiagnosis> ClinicalDiagnoses { get; set; }
        public DbSet<DispensaryDiagnosis> DispensaryDiagnoses { get; set; }
        public DbSet<InstrumentalTest> InstrumentalTests { get; set; }
        public DbSet<LaboratoryTest> LaboratoryTests { get; set; }
        public DbSet<MedicalRecommendation> MedicationRecommendations { get; set; }
        public DbSet<AdditionalRecommendation> AdditionalRecommendations { get; set; }



        //Формирование пути подключения к БД.
        private const string Path = "Server=WIN-CF61RM6U860;" + //Наименование сервера.
                                    "Database=Polyclinic;" + //Имя нашей БД.
                                    "Trusted_Connection = True;"; //.

        //Подключение к БД по пути.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Path);
        }
    }
}
