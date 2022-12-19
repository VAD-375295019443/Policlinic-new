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
    public class AdditionalRecommendationController : Controller
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

                var AdditionalRecommendationModel = new AdditionalRecommendationModel();
                AdditionalRecommendationModel.AdditionalRecommendationsRelease = await Db.AdditionalRecommendations
                    .Where(z => z.PatientId == Patient.Id && z.ReleaseDate != null)
                    .ToListAsync();
                AdditionalRecommendationModel.AdditionalRecommendationsRelease = AdditionalRecommendationModel.AdditionalRecommendationsRelease.TakeLast(50).ToList();
                
                AdditionalRecommendationModel.AdditionalRecommendations = await Db.AdditionalRecommendations
                    .Where(z => z.PatientId == Patient.Id && z.ReleaseDate == null)
                    .ToListAsync();
                return View(AdditionalRecommendationModel);
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
        public async Task<IActionResult> SaveNewEntry(AdditionalRecommendation EntryForm)
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
                    return Redirect("https://localhost:7240/AdditionalRecommendation/Main");
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


                var EntryDb = new AdditionalRecommendation()
                {
                    Id = Guid.NewGuid(),
                    PatientId = Patient.Id,
                    ReleaseDate = EntryForm.ReleaseDate,
                    AppointmentDate = EntryForm.AppointmentDate,
                    Recommendation = EntryForm.Recommendation
                };
                Db.AdditionalRecommendations.Add(EntryDb);
                Db.SaveChanges();
                return Redirect("https://localhost:7240/AdditionalRecommendation/Main");
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
        public async Task<IActionResult> SaveEntry(List<AdditionalRecommendation> EntryForm, int NumberEntry)
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
                    return Redirect("https://localhost:7240/AdditionalRecommendation/Main");
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


                var AdditionalRecommendation = await Db.AdditionalRecommendations.FirstOrDefaultAsync(z => z.Id == EntryForm[NumberEntry].Id);
                if (AdditionalRecommendation == null)
                {
                    throw new Exception("AdditionalRecommendation Error");
                }


                AdditionalRecommendation.ReleaseDate = EntryForm[NumberEntry].ReleaseDate;
                AdditionalRecommendation.AppointmentDate = EntryForm[NumberEntry].AppointmentDate;
                AdditionalRecommendation.Recommendation = EntryForm[NumberEntry].Recommendation;
                Db.SaveChanges();
                return Redirect("https://localhost:7240/AdditionalRecommendation/Main");
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
