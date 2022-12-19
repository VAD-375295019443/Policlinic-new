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
    public class ClinicalDiagnosisController : ControllerBase
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

                var ClinicalDiagnosesRelease = await Db.ClinicalDiagnoses
                                                  .Where(z => z.PatientId == Patient.Id && z.DeregistrationDate != null)
                                                  .Select(z => new { z.Id, z.DeregistrationDate, z.RegistrationDate, z.Diagnosis })
                                                  .ToListAsync();

                ClinicalDiagnosesRelease = ClinicalDiagnosesRelease.TakeLast(50).ToList();

                var ClinicalDiagnoses = await Db.ClinicalDiagnoses
                                          .Where(z => z.PatientId == Patient.Id && z.DeregistrationDate == null)
                                          .Select(z => new { z.Id, z.DeregistrationDate, z.RegistrationDate, z.Diagnosis })
                                          .ToListAsync();

                var ClinicalDiagnosisGetResponse = ClinicalDiagnosesRelease.Union(ClinicalDiagnoses);

                return Ok(ClinicalDiagnosisGetResponse);
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
                var ClinicalDiagnosis = await Db.ClinicalDiagnoses.FirstOrDefaultAsync(z => z.Id == id);

                if (ClinicalDiagnosis == null)
                {
                    return NotFound(); //404.
                }

                var ClinicalDiagnosisGetIdResponse = new ClinicalDiagnosisGetIdResponse()
                {
                    Id = ClinicalDiagnosis.Id,
                    DeregistrationDate = ClinicalDiagnosis.DeregistrationDate, //Дата выполнения.
                    RegistrationDate = ClinicalDiagnosis.RegistrationDate, //Дата назначения.
                    Diagnosis = ClinicalDiagnosis.Diagnosis
                };

                return Ok(ClinicalDiagnosisGetIdResponse); //200.
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
        public async Task<IActionResult> Post(string Email, [FromBody] ClinicalDiagnosisPostRequest ClinicalDiagnosisPostRequest)
        {
            try
            {
                if (Email == null || ClinicalDiagnosisPostRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(ClinicalDiagnosisPostRequest.DeregistrationDate, out DateOnly Date);
                if (Resalt == true)
                {
                    ClinicalDiagnosisPostRequest.DeregistrationDate = Convert.ToString(Date);
                }
                else
                {
                    ClinicalDiagnosisPostRequest.DeregistrationDate = null;
                }

                Resalt = DateOnly.TryParse(ClinicalDiagnosisPostRequest.RegistrationDate, out Date);
                if (Resalt == true)
                {
                    ClinicalDiagnosisPostRequest.RegistrationDate = Convert.ToString(Date);
                }
                else
                {
                    ClinicalDiagnosisPostRequest.RegistrationDate = null;
                }

                if (
                    ClinicalDiagnosisPostRequest.RegistrationDate == null ||
                    ClinicalDiagnosisPostRequest.Diagnosis == null
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

                    var ClinicalDiagnosis = new ClinicalDiagnosis()
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Patient.Id,
                        DeregistrationDate = ClinicalDiagnosisPostRequest.DeregistrationDate, //Дата выполнения.
                        RegistrationDate = ClinicalDiagnosisPostRequest.RegistrationDate, //Дата назначения.
                        Diagnosis = ClinicalDiagnosisPostRequest.Diagnosis
                    };

                    Db.ClinicalDiagnoses.Add(ClinicalDiagnosis);
                    Db.SaveChanges();

                    ClinicalDiagnosis = await Db.ClinicalDiagnoses.FirstOrDefaultAsync(z => z.Id == ClinicalDiagnosis.Id);

                    if (ClinicalDiagnosis == null)
                    {
                        return NotFound(); //404.
                    }

                    var ClinicalDiagnosisPostResponse = new ClinicalDiagnosisPostResponse()
                    {
                        Id = ClinicalDiagnosis.Id,
                        DeregistrationDate = ClinicalDiagnosis.DeregistrationDate, //Дата выполнения.
                        RegistrationDate = ClinicalDiagnosis.RegistrationDate, //Дата назначения.
                        Diagnosis = ClinicalDiagnosis.Diagnosis
                    };

                    return CreatedAtAction(nameof(GetId), new { id = ClinicalDiagnosisPostResponse.Id }, ClinicalDiagnosisPostResponse); //201.
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
        public async Task<IActionResult> PutId(Guid id, [FromBody] ClinicalDiagnosisPutIdRequest ClinicalDiagnosisPutIdRequest)
        {
            try
            {
                if (ClinicalDiagnosisPutIdRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(ClinicalDiagnosisPutIdRequest.DeregistrationDate, out DateOnly Date);
                if (Resalt == true)
                {
                    ClinicalDiagnosisPutIdRequest.DeregistrationDate = Convert.ToString(Date);
                }
                else
                {
                    ClinicalDiagnosisPutIdRequest.DeregistrationDate = null;
                }

                Resalt = DateOnly.TryParse(ClinicalDiagnosisPutIdRequest.RegistrationDate, out Date);
                if (Resalt == true)
                {
                    ClinicalDiagnosisPutIdRequest.RegistrationDate = Convert.ToString(Date);
                }
                else
                {
                    ClinicalDiagnosisPutIdRequest.RegistrationDate = null;
                }

                if (
                    ClinicalDiagnosisPutIdRequest.RegistrationDate == null ||
                    ClinicalDiagnosisPutIdRequest.Diagnosis == null
                    )
                {
                    return BadRequest(); //400.
                }
                else
                {
                    var Db = new PolyclinicContext();
                    var ClinicalDiagnosis = await Db.ClinicalDiagnoses.FirstOrDefaultAsync(z => z.Id == id);

                    if (ClinicalDiagnosis == null)
                    {
                        return NotFound(); //404.
                    }

                    ClinicalDiagnosis.DeregistrationDate = ClinicalDiagnosisPutIdRequest.DeregistrationDate; //Дата выполнения.
                    ClinicalDiagnosis.RegistrationDate = ClinicalDiagnosisPutIdRequest.RegistrationDate; //Дата назначения.
                    ClinicalDiagnosis.Diagnosis = ClinicalDiagnosisPutIdRequest.Diagnosis;
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
                var ClinicalDiagnosis = await Db.ClinicalDiagnoses.FirstOrDefaultAsync(z => z.Id == id);

                if (ClinicalDiagnosis == null)
                {
                    return NotFound(); //404.
                }

                Db.Remove(ClinicalDiagnosis);
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