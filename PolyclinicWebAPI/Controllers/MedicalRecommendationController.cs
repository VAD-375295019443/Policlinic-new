using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyclinicWeb.Classes;
using PolyclinicWebAPI.Models.Request;
using PolyclinicWebAPI.Models.Response;
using Serilog;

namespace PolyclinicWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MedicalRecommendationController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(string Email)
        {
            try
            {
                if (Email == null)
                {
                    return BadRequest(); //400.
                }

                var Db = new PolyclinicContext();
                var Patient = await Db.Patients.FirstOrDefaultAsync(z => z.Email == Email);
                if (Patient == null)
                {
                    return NotFound(); //404.
                }

                var MedicationRecommendationsRelease = await Db.MedicationRecommendations
                                                               .Where(z => z.PatientId == Patient.Id && z.ReleaseDate != null)
                                                               .Select(z => new { z.Id, z.ReleaseDate, z.AppointmentDate, z.ReleaseForm, z.Drug, z.Dose, z.DosesNumber, z.Signature })
                                                               .ToListAsync();

                MedicationRecommendationsRelease = MedicationRecommendationsRelease.TakeLast(50).ToList();

                var MedicationRecommendations = await Db.MedicationRecommendations
                                                        .Where(z => z.PatientId == Patient.Id && z.ReleaseDate == null)
                                                        .Select(z => new { z.Id, z.ReleaseDate, z.AppointmentDate, z.ReleaseForm, z.Drug, z.Dose, z.DosesNumber, z.Signature })
                                                        .ToListAsync();

                var MedicalRecommendationGetResponse = MedicationRecommendationsRelease.Union(MedicationRecommendations);

                return Ok(MedicalRecommendationGetResponse);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetId(Guid id)
        {
            try
            {
                var Db = new PolyclinicContext();
                var MedicalRecommendation = await Db.MedicationRecommendations.FirstOrDefaultAsync(z => z.Id == id);

                if (MedicalRecommendation == null)
                {
                    return NotFound(); //404.
                }

                var MedicalRecommendationGetIdResponse = new MedicalRecommendationGetIdResponse()
                {
                    Id = MedicalRecommendation.Id,
                    ReleaseDate = MedicalRecommendation.ReleaseDate, //Дата выполнения.
                    AppointmentDate = MedicalRecommendation.AppointmentDate, //Дата назначения.
                    ReleaseForm = MedicalRecommendation.ReleaseForm,
                    Drug = MedicalRecommendation.Drug,
                    Dose = MedicalRecommendation.Dose,
                    DosesNumber = MedicalRecommendation.DosesNumber,
                    Signature = MedicalRecommendation.Signature
                };

                return Ok(MedicalRecommendationGetIdResponse); //200.
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

        [HttpPost]
        public async Task<IActionResult> Post(string Email, [FromBody] MedicalRecommendationPostRequest MedicalRecommendationPostRequest)
        {
            try
            {
                if (Email == null || MedicalRecommendationPostRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(MedicalRecommendationPostRequest.ReleaseDate, out DateOnly Date);
                if (Resalt == true)
                {
                    MedicalRecommendationPostRequest.ReleaseDate = Convert.ToString(Date);
                }
                else
                {
                    MedicalRecommendationPostRequest.ReleaseDate = null;
                }

                Resalt = DateOnly.TryParse(MedicalRecommendationPostRequest.AppointmentDate, out Date);
                if (Resalt == true)
                {
                    MedicalRecommendationPostRequest.AppointmentDate = Convert.ToString(Date);
                }
                else
                {
                    MedicalRecommendationPostRequest.AppointmentDate = null;
                }

                if (
                    MedicalRecommendationPostRequest.AppointmentDate == null ||
                    MedicalRecommendationPostRequest.ReleaseForm == null ||
                    MedicalRecommendationPostRequest.Drug == null ||
                    MedicalRecommendationPostRequest.Dose == null ||
                    MedicalRecommendationPostRequest.DosesNumber == null ||
                    MedicalRecommendationPostRequest.Signature == null
                    )
                {
                    return BadRequest(); //400.
                }
                else
                {
                    var Db = new PolyclinicContext();

                    var Patient = await Db.Patients.FirstOrDefaultAsync(z => z.Email == Email);
                    if (Patient == null)
                    {
                        return NotFound(); //404.
                    }

                    var MedicalRecommendation = new MedicalRecommendation()
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Patient.Id,
                        ReleaseDate = MedicalRecommendationPostRequest.ReleaseDate, //Дата выполнения.
                        AppointmentDate = MedicalRecommendationPostRequest.AppointmentDate, //Дата назначения.
                        ReleaseForm = MedicalRecommendationPostRequest.ReleaseForm,
                        Drug = MedicalRecommendationPostRequest.Drug,
                        Dose = MedicalRecommendationPostRequest.Dose,
                        DosesNumber = MedicalRecommendationPostRequest.DosesNumber,
                        Signature = MedicalRecommendationPostRequest.Signature
                    };

                    Db.MedicationRecommendations.Add(MedicalRecommendation);
                    Db.SaveChanges();

                    MedicalRecommendation = await Db.MedicationRecommendations.FirstOrDefaultAsync(z => z.Id == MedicalRecommendation.Id);

                    if (MedicalRecommendation == null)
                    {
                        return NotFound(); //404.
                    }

                    var MedicalRecommendationPostResponse = new MedicalRecommendationPostResponse()
                    {
                        Id = MedicalRecommendation.Id,
                        ReleaseDate = MedicalRecommendation.ReleaseDate, //Дата выполнения.
                        AppointmentDate = MedicalRecommendation.AppointmentDate, //Дата назначения.
                        ReleaseForm = MedicalRecommendation.ReleaseForm,
                        Drug = MedicalRecommendation.Drug,
                        Dose = MedicalRecommendation.Dose,
                        DosesNumber = MedicalRecommendation.DosesNumber,
                        Signature = MedicalRecommendation.Signature
                    };

                    return CreatedAtAction(nameof(GetId), new { id = MedicalRecommendationPostResponse.Id }, MedicalRecommendationPostResponse); //201.
                }
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutId(Guid id, [FromBody] MedicalRecommendationPutIdRequest MedicalRecommendationPutIdRequest)
        {
            try
            {
                if (MedicalRecommendationPutIdRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(MedicalRecommendationPutIdRequest.ReleaseDate, out DateOnly Date);
                if (Resalt == true)
                {
                    MedicalRecommendationPutIdRequest.ReleaseDate = Convert.ToString(Date);
                }
                else
                {
                    MedicalRecommendationPutIdRequest.ReleaseDate = null;
                }

                Resalt = DateOnly.TryParse(MedicalRecommendationPutIdRequest.AppointmentDate, out Date);
                if (Resalt == true)
                {
                    MedicalRecommendationPutIdRequest.AppointmentDate = Convert.ToString(Date);
                }
                else
                {
                    MedicalRecommendationPutIdRequest.AppointmentDate = null;
                }

                if (
                    MedicalRecommendationPutIdRequest.AppointmentDate == null ||
                    MedicalRecommendationPutIdRequest.ReleaseForm == null ||
                    MedicalRecommendationPutIdRequest.Drug == null ||
                    MedicalRecommendationPutIdRequest.Dose == null ||
                    MedicalRecommendationPutIdRequest.DosesNumber == null ||
                    MedicalRecommendationPutIdRequest.Signature == null
                    )
                {
                    return BadRequest(); //400.
                }
                else
                {
                    var Db = new PolyclinicContext();
                    var MedicalRecommendation = await Db.MedicationRecommendations.FirstOrDefaultAsync(z => z.Id == id);

                    if (MedicalRecommendation == null)
                    {
                        return NotFound(); //404.
                    }

                    MedicalRecommendation.ReleaseDate = MedicalRecommendationPutIdRequest.ReleaseDate; //Дата выполнения.
                    MedicalRecommendation.AppointmentDate = MedicalRecommendationPutIdRequest.AppointmentDate; //Дата назначения.
                    MedicalRecommendation.ReleaseForm = MedicalRecommendationPutIdRequest.ReleaseForm;
                    MedicalRecommendation.Drug = MedicalRecommendationPutIdRequest.Drug;
                    MedicalRecommendation.Dose = MedicalRecommendationPutIdRequest.Dose;
                    MedicalRecommendation.DosesNumber = MedicalRecommendationPutIdRequest.DosesNumber;
                    MedicalRecommendation.Signature = MedicalRecommendationPutIdRequest.Signature;
                    Db.SaveChanges();

                    return Ok(); //200.
                }
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteId(Guid id)
        {
            try
            {
                var Db = new PolyclinicContext();
                var MedicalRecommendation = await Db.MedicationRecommendations.FirstOrDefaultAsync(z => z.Id == id);

                if (MedicalRecommendation == null)
                {
                    return NotFound(); //404.
                }

                Db.Remove(MedicalRecommendation);
                Db.SaveChanges();

                return Ok(); //200.
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