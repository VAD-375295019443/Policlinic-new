using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyclinicWeb.Classes;
using PolyclinicWeb.Data;
using PolyclinicWeb.Migrations;
using PolyclinicWeb.Models;
using Serilog;

namespace PolyclinicWeb.Controllers
{
    public class PatientController : Controller
    {
        [HttpGet]
        [Authorize]
        public IActionResult StartMain()
        {
            try
            {
                if (User.Identity == null)
                {
                    throw new Exception("Autentication Error");
                }
            
                var PatientModel = new PatientModel();
                PatientModel.Patient.Email = User.Identity.Name;
                return View(PatientModel);
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
        public IActionResult SaveNewEntry(Patient EntryForm)
        {
            try
            {
                bool ResaltDateOfBirth = DateOnly.TryParse(EntryForm.DateOfBirth, out DateOnly DateOfBirth);
                if (ModelState.IsValid == false || ResaltDateOfBirth == false)
                {
                    return Redirect("https://localhost:7240/Patient/StartMain");
                }


                var Db = new PolyclinicContext();
                var NewEntryDb = new Patient()
                {
                    Id = Guid.NewGuid(),
                    Password = "12345",
                    Email = EntryForm.Email,
                    Surname = EntryForm.Surname,
                    Name = EntryForm.Name,
                    MiddleName = EntryForm.MiddleName,
                    DateOfBirth = Convert.ToString(DateOfBirth),
                    Floor = EntryForm.Floor,
                    Telephone = EntryForm.Telephone,
                    Country = EntryForm.Country,
                    Region = EntryForm.Region,
                    District = EntryForm.District,
                    LocalityType = EntryForm.LocalityType,
                    Locality = EntryForm.Locality,
                    StreetType = EntryForm.StreetType,
                    Street = EntryForm.Street,
                    House = EntryForm.House,
                    Flat = EntryForm.Flat,
                    AdditionalInformation = EntryForm.AdditionalInformation
                };
                Db.Patients.Add(NewEntryDb);
                Db.SaveChanges();

                return Redirect("https://localhost:7240/Patient/Main");
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

                var PatientModel = new PatientModel();
                PatientModel.Patient = Patient;
                return View(PatientModel);
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
        public async Task<IActionResult> SaveEntry(Patient EntryForm)
        {
            try
            {
                bool ResaltDateOfBirth = DateOnly.TryParse(EntryForm.DateOfBirth, out DateOnly DateOfBirth);
                if (ModelState.IsValid == false || ResaltDateOfBirth == false)
                {
                    return Redirect("https://localhost:7240/Patient/Main");
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


                Patient.Email = EntryForm.Email;
                Patient.Surname = EntryForm.Surname;
                Patient.Name = EntryForm.Name;
                Patient.MiddleName = EntryForm.MiddleName;
                Patient.DateOfBirth = Convert.ToString(DateOfBirth);
                Patient.Floor = EntryForm.Floor;
                Patient.Telephone = EntryForm.Telephone;
                Patient.Country = EntryForm.Country;
                Patient.Region = EntryForm.Region;
                Patient.District = EntryForm.District;
                Patient.LocalityType = EntryForm.LocalityType;
                Patient.Locality = EntryForm.Locality;
                Patient.StreetType = EntryForm.StreetType;
                Patient.Street = EntryForm.Street;
                Patient.House = EntryForm.House;
                Patient.Flat = EntryForm.Flat;
                Patient.AdditionalInformation = EntryForm.AdditionalInformation;
                Db.SaveChanges();

                return Redirect("https://localhost:7240/Patient/Main");
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
