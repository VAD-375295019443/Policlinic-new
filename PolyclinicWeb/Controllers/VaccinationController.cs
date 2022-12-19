using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PolyclinicWeb.Classes;
using PolyclinicWeb.Data;
using PolyclinicWeb.Models;
using Serilog;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Principal;
using System.Xml.Linq;

namespace PolyclinicWeb.Controllers
{
    public class VaccinationController : Controller
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Main()
        {
            try
            {
                if (User.Identity == null)
                {
                    throw new Exception("Autentication Error");
                }
                
                var Db = new PolyclinicContext();
                var Patient = await Db.Patients.FirstOrDefaultAsync(z => z.Email == User.Identity.Name);
                if (Patient == null)
                {
                    throw new Exception("Patient Error");
                }
                
                var VaccinationModel = new VaccinationModel();



                VaccinationModel.VaccinationsRelease = await Db.Vaccinations
                    .Where(z => z.PatientId == Patient.Id && z.ReleaseDate != null)
                    .ToListAsync();
                VaccinationModel.VaccinationsRelease = VaccinationModel.VaccinationsRelease.TakeLast(50).ToList();

                VaccinationModel.Vaccinations = await Db.Vaccinations
                    .Where(z => z.PatientId == Patient.Id && z.ReleaseDate == null)
                    .ToListAsync();
                return View(VaccinationModel);
            }
            catch (Exception ex)
            {
                Log.Error($"{Environment.NewLine}" +
                          $"Текст: {ex.Message}.{Environment.NewLine}" +
                          $"Имя объекта: {ex.Source}.{Environment.NewLine}" +
                          $"Трассировка стека: {ex.StackTrace}.{Environment.NewLine}" +
                          $"Метод: {ex.TargetSite}.");

                return StatusCode(500, ex.Message);
            }
        }

        //Ограничение количества записей релиза.

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SaveNewEntry(Vaccination EntryForm)
        {
            try
            {
                bool ResaltReleaseDate = DateOnly.TryParse(EntryForm.ReleaseDate, out DateOnly ReleaseDate);
                if (ResaltReleaseDate == true)
                {
                    EntryForm.ReleaseDate = Convert.ToString(ReleaseDate);
                }
                else
                {
                    EntryForm.ReleaseDate = null;
                }
                bool ResaltAppointmentDate = DateOnly.TryParse(EntryForm.AppointmentDate, out DateOnly AppointmentDate);
                if (ResaltAppointmentDate == true)
                {
                    EntryForm.AppointmentDate = Convert.ToString(AppointmentDate);
                }
                else
                {
                    EntryForm.AppointmentDate = null;
                }
                if (ModelState.IsValid == false || ResaltAppointmentDate == false)
                {
                    return Redirect("https://localhost:7240/Vaccination/Main");
                }


                if (User.Identity == null)
                {
                    throw new Exception("Autentication Error");
                }


                var Db = new PolyclinicContext();
                var Patient = await Db.Patients.FirstOrDefaultAsync(z => z.Email == User.Identity.Name);
                if (Patient == null)
                {
                    throw new Exception("Patient Error");
                }


                var EntryDb = new Vaccination()
                {
                    Id = Guid.NewGuid(),
                    PatientId = Patient.Id,
                    ReleaseDate = EntryForm.ReleaseDate,
                    AppointmentDate = EntryForm.AppointmentDate,
                    Type = EntryForm.Type,
                    Dose = EntryForm.Dose,
                    Series = EntryForm.Series,
                    Name = EntryForm.Name,
                    Production = EntryForm.Production,
                    AdditionalInformation = EntryForm.AdditionalInformation
                };
                Db.Vaccinations.Add(EntryDb);
                Db.SaveChanges();
                return Redirect("https://localhost:7240/Vaccination/Main");
            }
            catch (Exception ex)
            {
                Log.Error($"{Environment.NewLine}" +
                          $"Текст: {ex.Message}.{Environment.NewLine}" +
                          $"Имя объекта: {ex.Source}.{Environment.NewLine}" +
                          $"Трассировка стека: {ex.StackTrace}.{Environment.NewLine}" +
                          $"Метод: {ex.TargetSite}.");

                return StatusCode(500, ex.Message);
            }
        }




        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SaveEntry(List<Vaccination> EntryForm, int NumberEntry)
        {
            try
            {
                bool ResaltReleaseDate = DateOnly.TryParse(EntryForm[NumberEntry].ReleaseDate, out DateOnly ReleaseDate);
                if (ResaltReleaseDate == true)
                {
                    EntryForm[NumberEntry].ReleaseDate = Convert.ToString(ReleaseDate);
                }
                else
                {
                    EntryForm[NumberEntry].ReleaseDate = null;
                }
                bool ResaltAppointmentDate = DateOnly.TryParse(EntryForm[NumberEntry].AppointmentDate, out DateOnly AppointmentDate);
                if (ResaltAppointmentDate == true)
                {
                    EntryForm[NumberEntry].AppointmentDate = Convert.ToString(AppointmentDate);
                }
                else
                {
                    EntryForm[NumberEntry].AppointmentDate = null;
                }
                if (ModelState.IsValid == false || ResaltAppointmentDate == false)
                {
                    return Redirect("https://localhost:7240/Vaccination/Main");
                }


                if (User.Identity == null)
                {
                    throw new Exception("Autentication Error");
                }


                var Db = new PolyclinicContext();
                var Patient = await Db.Patients.FirstOrDefaultAsync(z => z.Email == User.Identity.Name);
                if (Patient == null)
                {
                    throw new Exception("Patient Error");
                }


                var Vaccination = await Db.Vaccinations.FirstOrDefaultAsync(z => z.Id == EntryForm[NumberEntry].Id);
                if (Vaccination == null)
                {
                    throw new Exception("Vaccination Error");
                }


                Vaccination.ReleaseDate = EntryForm[NumberEntry].ReleaseDate;
                Vaccination.AppointmentDate = EntryForm[NumberEntry].AppointmentDate;
                Vaccination.Type = EntryForm[NumberEntry].Type;
                Vaccination.Dose = EntryForm[NumberEntry].Dose;
                Vaccination.Series = EntryForm[NumberEntry].Series;
                Vaccination.Name = EntryForm[NumberEntry].Name;
                Vaccination.Production = EntryForm[NumberEntry].Production;
                Vaccination.AdditionalInformation = EntryForm[NumberEntry].AdditionalInformation;
                Db.SaveChanges();
                return Redirect("https://localhost:7240/Vaccination/Main");
            }
            catch (Exception ex)
            {
                Log.Error($"{Environment.NewLine}" +
                          $"Текст: {ex.Message}.{Environment.NewLine}" +
                          $"Имя объекта: {ex.Source}.{Environment.NewLine}" +
                          $"Трассировка стека: {ex.StackTrace}.{Environment.NewLine}" +
                          $"Метод: {ex.TargetSite}.");

                return StatusCode(500, ex.Message);
            }
        }
    }
}
