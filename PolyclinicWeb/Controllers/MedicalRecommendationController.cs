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
    public class MedicalRecommendationController : Controller
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

                var MedicalRecommendationModel = new MedicalRecommendationModel();
                MedicalRecommendationModel.MedicationRecommendationsRelease = await Db.MedicationRecommendations
                    .Where(z => z.PatientId == Patient.Id && z.ReleaseDate != null)
                    .ToListAsync();
                MedicalRecommendationModel.MedicationRecommendationsRelease = MedicalRecommendationModel.MedicationRecommendationsRelease.TakeLast(50).ToList();

                MedicalRecommendationModel.MedicationRecommendations = await Db.MedicationRecommendations
                    .Where(z => z.PatientId == Patient.Id && z.ReleaseDate == null)
                    .ToListAsync();
                return View(MedicalRecommendationModel);
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
        public async Task<IActionResult> SaveNewEntry(MedicalRecommendation EntryForm)
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
                    return Redirect("https://localhost:7240/MedicalRecommendation/Main");
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


                var EntryDb = new MedicalRecommendation()
                {
                    Id = Guid.NewGuid(),
                    PatientId = Patient.Id,
                    ReleaseDate = EntryForm.ReleaseDate,
                    AppointmentDate = EntryForm.AppointmentDate,
                    Drug = EntryForm.Drug,
                    ReleaseForm = EntryForm.ReleaseForm,
                    Dose = EntryForm.Dose,
                    DosesNumber = EntryForm.DosesNumber,
                    Signature = EntryForm.Signature
                };
                Db.MedicationRecommendations.Add(EntryDb);
                Db.SaveChanges();
                return Redirect("https://localhost:7240/MedicalRecommendation/Main");
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
        public async Task<IActionResult> SaveEntry(List<MedicalRecommendation> EntryForm, int NumberEntry)
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
                    return Redirect("https://localhost:7240/MedicalRecommendation/Main");
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


                var MedicalRecommendation = await Db.MedicationRecommendations.FirstOrDefaultAsync(z => z.Id == EntryForm[NumberEntry].Id);
                if (MedicalRecommendation == null)
                {
                    throw new Exception("MedicalRecommendation Error");
                }


                MedicalRecommendation.ReleaseDate = EntryForm[NumberEntry].ReleaseDate;
                MedicalRecommendation.AppointmentDate = EntryForm[NumberEntry].AppointmentDate;
                MedicalRecommendation.Drug = EntryForm[NumberEntry].Drug;
                MedicalRecommendation.ReleaseForm = EntryForm[NumberEntry].ReleaseForm;
                MedicalRecommendation.Dose = EntryForm[NumberEntry].Dose;
                MedicalRecommendation.DosesNumber = EntryForm[NumberEntry].DosesNumber;
                MedicalRecommendation.Signature = EntryForm[NumberEntry].Signature;
                Db.SaveChanges();
                return Redirect("https://localhost:7240/MedicalRecommendation/Main");
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
