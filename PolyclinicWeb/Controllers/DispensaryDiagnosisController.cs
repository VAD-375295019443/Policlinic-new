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
    public class DispensaryDiagnosisController : Controller
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

                var DispensaryDiagnosisModel = new DispensaryDiagnosisModel();
                DispensaryDiagnosisModel.DispensaryDiagnosesDeregistration = await Db.DispensaryDiagnoses
                    .Where(z => z.PatientId == Patient.Id && z.DeregistrationDate != null)
                    .ToListAsync();
                DispensaryDiagnosisModel.DispensaryDiagnosesDeregistration = DispensaryDiagnosisModel.DispensaryDiagnosesDeregistration.TakeLast(50).ToList();

                DispensaryDiagnosisModel.DispensaryDiagnosesRegistration = await Db.DispensaryDiagnoses
                    .Where(z => z.PatientId == Patient.Id && z.DeregistrationDate == null)
                    .ToListAsync();
                return View(DispensaryDiagnosisModel);
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
        public async Task<IActionResult> SaveNewEntry(DispensaryDiagnosis EntryForm)
        {
            try
            {
                bool ResaltDeregistrationDate = DateOnly.TryParse(EntryForm.DeregistrationDate, out DateOnly DeregistrationDate);
                if (ResaltDeregistrationDate == true)
                {
                    EntryForm.DeregistrationDate = Convert.ToString(DeregistrationDate);
                }
                else
                {
                    EntryForm.DeregistrationDate = null;
                }
                bool ResaltRegistrationDate = DateOnly.TryParse(EntryForm.RegistrationDate, out DateOnly RegistrationDate);
                if (ResaltRegistrationDate == true)
                {
                    EntryForm.RegistrationDate = Convert.ToString(RegistrationDate);
                }
                else
                {
                    EntryForm.RegistrationDate = null;
                }
                if (ModelState.IsValid == false || ResaltRegistrationDate == false)
                {
                    return Redirect("https://localhost:7240/DispensaryDiagnosis/Main");
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


                var EntryDb = new DispensaryDiagnosis()
                {
                    Id = Guid.NewGuid(),
                    PatientId = Patient.Id,
                    DeregistrationDate = EntryForm.DeregistrationDate,
                    RegistrationDate = EntryForm.RegistrationDate,
                    Diagnosis = EntryForm.Diagnosis
                };
                Db.DispensaryDiagnoses.Add(EntryDb);
                Db.SaveChanges();
                return Redirect("https://localhost:7240/DispensaryDiagnosis/Main");
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
        public async Task<IActionResult> SaveEntry(List<DispensaryDiagnosis> EntryForm, int NumberEntry)
        {
            try
            {
                bool ResaltDeregistrationDate = DateOnly.TryParse(EntryForm[NumberEntry].DeregistrationDate, out DateOnly DeregistrationDate);
                if (ResaltDeregistrationDate == true)
                {
                    EntryForm[NumberEntry].DeregistrationDate = Convert.ToString(DeregistrationDate);
                }
                else
                {
                    EntryForm[NumberEntry].DeregistrationDate = null;
                }
                bool ResaltRegistrationDate = DateOnly.TryParse(EntryForm[NumberEntry].RegistrationDate, out DateOnly RegistrationDate);
                if (ResaltRegistrationDate == true)
                {
                    EntryForm[NumberEntry].RegistrationDate = Convert.ToString(RegistrationDate);
                }
                else
                {
                    EntryForm[NumberEntry].RegistrationDate = null;
                }
                if (ModelState.IsValid == false || ResaltRegistrationDate == false)
                {
                    return Redirect("https://localhost:7240/DispensaryDiagnosis/Main");
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


                var DispensaryDiagnosis = await Db.DispensaryDiagnoses.FirstOrDefaultAsync(z => z.Id == EntryForm[NumberEntry].Id);
                if (DispensaryDiagnosis == null)
                {
                    throw new Exception("DispensaryDiagnosis Error");
                }


                DispensaryDiagnosis.DeregistrationDate = EntryForm[NumberEntry].DeregistrationDate;
                DispensaryDiagnosis.RegistrationDate = EntryForm[NumberEntry].RegistrationDate;
                DispensaryDiagnosis.Diagnosis = EntryForm[NumberEntry].Diagnosis;
                Db.SaveChanges();
                return Redirect("https://localhost:7240/DispensaryDiagnosis/Main");
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
