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
    public class AdditionalRecommendationController : ControllerBase
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

                var AdditionalRecommendationsRelease = await Db.AdditionalRecommendations
                                                  .Where(z => z.PatientId == Patient.Id && z.ReleaseDate != null)
                                                  .Select(z => new { z.Id, z.ReleaseDate, z.AppointmentDate, z.Recommendation })
                                                  .ToListAsync();

                AdditionalRecommendationsRelease = AdditionalRecommendationsRelease.TakeLast(50).ToList();

                var AdditionalRecommendations = await Db.AdditionalRecommendations
                                          .Where(z => z.PatientId == Patient.Id && z.ReleaseDate == null)
                                          .Select(z => new { z.Id, z.ReleaseDate, z.AppointmentDate, z.Recommendation })
                                          .ToListAsync();

                var AdditionalRecommendationGetResponse = AdditionalRecommendationsRelease.Union(AdditionalRecommendations);

                return Ok(AdditionalRecommendationGetResponse);
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
                var AdditionalRecommendation = await Db.AdditionalRecommendations.FirstOrDefaultAsync(z => z.Id == id);

                if (AdditionalRecommendation == null)
                {
                    return NotFound(); //404.
                }

                var AdditionalRecommendationGetIdResponse = new AdditionalRecommendationGetIdResponse()
                {
                    Id = AdditionalRecommendation.Id,
                    ReleaseDate = AdditionalRecommendation.ReleaseDate, //Дата выполнения.
                    AppointmentDate = AdditionalRecommendation.AppointmentDate, //Дата назначения.
                    Recommendation = AdditionalRecommendation.Recommendation,
                };

                return Ok(AdditionalRecommendationGetIdResponse); //200.
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
        public async Task<IActionResult> Post(string Email, [FromBody] AdditionalRecommendationPostRequest AdditionalRecommendationPostRequest)
        {
            try
            {
                if (Email == null || AdditionalRecommendationPostRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(AdditionalRecommendationPostRequest.ReleaseDate, out DateOnly Date);
                if (Resalt == true)
                {
                    AdditionalRecommendationPostRequest.ReleaseDate = Convert.ToString(Date);
                }
                else
                {
                    AdditionalRecommendationPostRequest.ReleaseDate = null;
                }

                Resalt = DateOnly.TryParse(AdditionalRecommendationPostRequest.AppointmentDate, out Date);
                if (Resalt == true)
                {
                    AdditionalRecommendationPostRequest.AppointmentDate = Convert.ToString(Date);
                }
                else
                {
                    AdditionalRecommendationPostRequest.AppointmentDate = null;
                }

                if (
                    AdditionalRecommendationPostRequest.AppointmentDate == null ||
                    AdditionalRecommendationPostRequest.Recommendation == null
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

                    var AdditionalRecommendation = new AdditionalRecommendation()
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Patient.Id,
                        ReleaseDate = AdditionalRecommendationPostRequest.ReleaseDate, //Дата выполнения.
                        AppointmentDate = AdditionalRecommendationPostRequest.AppointmentDate, //Дата назначения.
                        Recommendation = AdditionalRecommendationPostRequest.Recommendation
                    };

                    Db.AdditionalRecommendations.Add(AdditionalRecommendation);
                    Db.SaveChanges();

                    AdditionalRecommendation = await Db.AdditionalRecommendations.FirstOrDefaultAsync(z => z.Id == AdditionalRecommendation.Id);

                    if (AdditionalRecommendation == null)
                    {
                        return NotFound(); //404.
                    }

                    var AdditionalRecommendationPostResponse = new AdditionalRecommendationPostResponse()
                    {
                        Id = AdditionalRecommendation.Id,
                        ReleaseDate = AdditionalRecommendation.ReleaseDate, //Дата выполнения.
                        AppointmentDate = AdditionalRecommendation.AppointmentDate, //Дата назначения.
                        Recommendation = AdditionalRecommendation.Recommendation
                    };

                    return CreatedAtAction(nameof(GetId), new { id = AdditionalRecommendationPostResponse.Id }, AdditionalRecommendationPostResponse); //201.
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
        public async Task<IActionResult> PutId(Guid id, [FromBody] AdditionalRecommendationPutIdRequest AdditionalRecommendationPutIdRequest)
        {
            try
            {
                if (AdditionalRecommendationPutIdRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(AdditionalRecommendationPutIdRequest.ReleaseDate, out DateOnly Date);
                if (Resalt == true)
                {
                    AdditionalRecommendationPutIdRequest.ReleaseDate = Convert.ToString(Date);
                }
                else
                {
                    AdditionalRecommendationPutIdRequest.ReleaseDate = null;
                }

                Resalt = DateOnly.TryParse(AdditionalRecommendationPutIdRequest.AppointmentDate, out Date);
                if (Resalt == true)
                {
                    AdditionalRecommendationPutIdRequest.AppointmentDate = Convert.ToString(Date);
                }
                else
                {
                    AdditionalRecommendationPutIdRequest.AppointmentDate = null;
                }

                if (
                    AdditionalRecommendationPutIdRequest.AppointmentDate == null ||
                    AdditionalRecommendationPutIdRequest.Recommendation == null
                    )
                {
                    return BadRequest(); //400.
                }
                else
                {
                    var Db = new PolyclinicContext();
                    var AdditionalRecommendation = await Db.AdditionalRecommendations.FirstOrDefaultAsync(z => z.Id == id);

                    if (AdditionalRecommendation == null)
                    {
                        return NotFound(); //404.
                    }

                    AdditionalRecommendation.ReleaseDate = AdditionalRecommendationPutIdRequest.ReleaseDate; //Дата выполнения.
                    AdditionalRecommendation.AppointmentDate = AdditionalRecommendationPutIdRequest.AppointmentDate; //Дата назначения.
                    AdditionalRecommendation.Recommendation = AdditionalRecommendationPutIdRequest.Recommendation; //Дополнительная информация.
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
                var AdditionalRecommendation = await Db.AdditionalRecommendations.FirstOrDefaultAsync(z => z.Id == id);

                if (AdditionalRecommendation == null)
                {
                    return NotFound(); //404.
                }

                Db.Remove(AdditionalRecommendation);
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