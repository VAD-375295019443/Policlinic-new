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
    public class DispensaryDiagnosisController : ControllerBase
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

                var DispensaryDiagnosesRelease = await Db.DispensaryDiagnoses
                                                  .Where(z => z.PatientId == Patient.Id && z.DeregistrationDate != null)
                                                  .Select(z => new { z.Id, z.DeregistrationDate, z.RegistrationDate, z.Diagnosis })
                                                  .ToListAsync();

                DispensaryDiagnosesRelease = DispensaryDiagnosesRelease.TakeLast(50).ToList();

                var DispensaryDiagnoses = await Db.DispensaryDiagnoses
                                          .Where(z => z.PatientId == Patient.Id && z.DeregistrationDate == null)
                                          .Select(z => new { z.Id, z.DeregistrationDate, z.RegistrationDate, z.Diagnosis })
                                          .ToListAsync();

                var DispensaryDiagnosisGetResponse = DispensaryDiagnosesRelease.Union(DispensaryDiagnoses);

                return Ok(DispensaryDiagnosisGetResponse);
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
                var DispensaryDiagnosis = await Db.DispensaryDiagnoses.FirstOrDefaultAsync(z => z.Id == id);

                if (DispensaryDiagnosis == null)
                {
                    return NotFound(); //404.
                }

                var DispensaryDiagnosisGetIdResponse = new DispensaryDiagnosisGetIdResponse()
                {
                    Id = DispensaryDiagnosis.Id,
                    DeregistrationDate = DispensaryDiagnosis.DeregistrationDate, //Дата выполнения.
                    RegistrationDate = DispensaryDiagnosis.RegistrationDate, //Дата назначения.
                    Diagnosis = DispensaryDiagnosis.Diagnosis
                };

                return Ok(DispensaryDiagnosisGetIdResponse); //200.
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
        public async Task<IActionResult> Post(string Email, [FromBody] DispensaryDiagnosisPostRequest DispensaryDiagnosisPostRequest)
        {
            try
            {
                if (Email == null || DispensaryDiagnosisPostRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(DispensaryDiagnosisPostRequest.DeregistrationDate, out DateOnly Date);
                if (Resalt == true)
                {
                    DispensaryDiagnosisPostRequest.DeregistrationDate = Convert.ToString(Date);
                }
                else
                {
                    DispensaryDiagnosisPostRequest.DeregistrationDate = null;
                }

                Resalt = DateOnly.TryParse(DispensaryDiagnosisPostRequest.RegistrationDate, out Date);
                if (Resalt == true)
                {
                    DispensaryDiagnosisPostRequest.RegistrationDate = Convert.ToString(Date);
                }
                else
                {
                    DispensaryDiagnosisPostRequest.RegistrationDate = null;
                }

                if (
                    DispensaryDiagnosisPostRequest.RegistrationDate == null ||
                    DispensaryDiagnosisPostRequest.Diagnosis == null
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

                    var DispensaryDiagnosis = new DispensaryDiagnosis()
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Patient.Id,
                        DeregistrationDate = DispensaryDiagnosisPostRequest.DeregistrationDate, //Дата выполнения.
                        RegistrationDate = DispensaryDiagnosisPostRequest.RegistrationDate, //Дата назначения.
                        Diagnosis = DispensaryDiagnosisPostRequest.Diagnosis
                    };

                    Db.DispensaryDiagnoses.Add(DispensaryDiagnosis);
                    Db.SaveChanges();

                    DispensaryDiagnosis = await Db.DispensaryDiagnoses.FirstOrDefaultAsync(z => z.Id == DispensaryDiagnosis.Id);

                    if (DispensaryDiagnosis == null)
                    {
                        return NotFound(); //404.
                    }

                    var DispensaryDiagnosisPostResponse = new DispensaryDiagnosisPostResponse()
                    {
                        Id = DispensaryDiagnosis.Id,
                        DeregistrationDate = DispensaryDiagnosis.DeregistrationDate, //Дата выполнения.
                        RegistrationDate = DispensaryDiagnosis.RegistrationDate, //Дата назначения.
                        Diagnosis = DispensaryDiagnosis.Diagnosis
                    };

                    return CreatedAtAction(nameof(GetId), new { id = DispensaryDiagnosisPostResponse.Id }, DispensaryDiagnosisPostResponse); //201.
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
        public async Task<IActionResult> PutId(Guid id, [FromBody] DispensaryDiagnosisPutIdRequest DispensaryDiagnosisPutIdRequest)
        {
            try
            {
                if (DispensaryDiagnosisPutIdRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(DispensaryDiagnosisPutIdRequest.DeregistrationDate, out DateOnly Date);
                if (Resalt == true)
                {
                    DispensaryDiagnosisPutIdRequest.DeregistrationDate = Convert.ToString(Date);
                }
                else
                {
                    DispensaryDiagnosisPutIdRequest.DeregistrationDate = null;
                }

                Resalt = DateOnly.TryParse(DispensaryDiagnosisPutIdRequest.RegistrationDate, out Date);
                if (Resalt == true)
                {
                    DispensaryDiagnosisPutIdRequest.RegistrationDate = Convert.ToString(Date);
                }
                else
                {
                    DispensaryDiagnosisPutIdRequest.RegistrationDate = null;
                }

                if (
                    DispensaryDiagnosisPutIdRequest.RegistrationDate == null ||
                    DispensaryDiagnosisPutIdRequest.Diagnosis == null
                    )
                {
                    return BadRequest(); //400.
                }
                else
                {
                    var Db = new PolyclinicContext();
                    var DispensaryDiagnosis = await Db.DispensaryDiagnoses.FirstOrDefaultAsync(z => z.Id == id);

                    if (DispensaryDiagnosis == null)
                    {
                        return NotFound(); //404.
                    }

                    DispensaryDiagnosis.DeregistrationDate = DispensaryDiagnosisPutIdRequest.DeregistrationDate; //Дата выполнения.
                    DispensaryDiagnosis.RegistrationDate = DispensaryDiagnosisPutIdRequest.RegistrationDate; //Дата назначения.
                    DispensaryDiagnosis.Diagnosis = DispensaryDiagnosisPutIdRequest.Diagnosis;
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
                var DispensaryDiagnosis = await Db.DispensaryDiagnoses.FirstOrDefaultAsync(z => z.Id == id);

                if (DispensaryDiagnosis == null)
                {
                    return NotFound(); //404.
                }

                Db.Remove(DispensaryDiagnosis);
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