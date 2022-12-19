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
    public class VaccinationController : ControllerBase
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

                var VaccinationsRelease = await Db.Vaccinations
                                                  .Where(z => z.PatientId == Patient.Id && z.ReleaseDate != null)
                                                  .Select(zz => new { zz.Id, zz.ReleaseDate, zz.AppointmentDate, zz.Type, zz.Dose, zz.Series, zz.Name, zz.Production, zz.AdditionalInformation })
                                                  .ToListAsync();

                VaccinationsRelease = VaccinationsRelease.TakeLast(50).ToList();

                var Vaccinations = await Db.Vaccinations
                                          .Where(z => z.PatientId == Patient.Id && z.ReleaseDate == null)
                                          .Select(zz => new { zz.Id, zz.ReleaseDate, zz.AppointmentDate, zz.Type, zz.Dose, zz.Series, zz.Name, zz.Production, zz.AdditionalInformation })
                                          .ToListAsync();

                var VaccinationGetResponse = VaccinationsRelease.Union(Vaccinations);
                
                return Ok(VaccinationGetResponse);
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
                var Vaccination = await Db.Vaccinations.FirstOrDefaultAsync(z => z.Id == id);

                if (Vaccination == null)
                {
                    return NotFound(); //404.
                }

                var VaccinationGetIdResponse = new VaccinationGetIdResponse()
                {
                    Id = Vaccination.Id,
                    ReleaseDate = Vaccination.ReleaseDate, //Дата выполнения.
                    AppointmentDate = Vaccination.AppointmentDate, //Дата назначения.
                    Type = Vaccination.Type, //Тип вакцинации.
                    Dose = Vaccination.Dose, //Доза вакцины.
                    Series = Vaccination.Series, //Серия препарата.
                    Name = Vaccination.Name, //Наименование вакцины.
                    Production = Vaccination.Production, //Место производства вакцины.
                    AdditionalInformation = Vaccination.AdditionalInformation //Дополнительная информация.
                };

                return Ok(VaccinationGetIdResponse); //200.
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
        public async Task<IActionResult> Post(string Email, [FromBody] VaccinationPostRequest VaccinationPostRequest)
        {
            try
            {
                if (Email == null || VaccinationPostRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(VaccinationPostRequest.ReleaseDate, out DateOnly Date);
                if (Resalt == true)
                {
                    VaccinationPostRequest.ReleaseDate = Convert.ToString(Date);
                }
                else
                {
                    VaccinationPostRequest.ReleaseDate = null;
                }

                Resalt = DateOnly.TryParse(VaccinationPostRequest.AppointmentDate, out Date);
                if (Resalt == true)
                {
                    VaccinationPostRequest.AppointmentDate = Convert.ToString(Date);
                }
                else
                {
                    VaccinationPostRequest.AppointmentDate = null;
                }

                if (
                    VaccinationPostRequest.AppointmentDate == null ||
                    VaccinationPostRequest.Type == null ||
                    VaccinationPostRequest.Name == null
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

                    var Vaccination = new Vaccination()
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Patient.Id,
                        ReleaseDate = VaccinationPostRequest.ReleaseDate, //Дата выполнения.
                        AppointmentDate = VaccinationPostRequest.AppointmentDate, //Дата назначения.
                        Type = VaccinationPostRequest.Type, //Тип вакцинации.
                        Dose = VaccinationPostRequest.Dose, //Доза вакцины.
                        Series = VaccinationPostRequest.Series, //Серия препарата.
                        Name = VaccinationPostRequest.Name, //Наименование вакцины.
                        Production = VaccinationPostRequest.Production, //Место производства вакцины.
                        AdditionalInformation = VaccinationPostRequest.AdditionalInformation //Дополнительная информация.
                    };
                    
                    Db.Vaccinations.Add(Vaccination);
                    Db.SaveChanges();

                    Vaccination = await Db.Vaccinations.FirstOrDefaultAsync(z => z.Id == Vaccination.Id);

                    if (Vaccination == null)
                    {
                        return NotFound(); //404.
                    }

                    var VaccinationPostResponse = new VaccinationPostResponse()
                    {
                        Id = Vaccination.Id,
                        ReleaseDate = Vaccination.ReleaseDate, //Дата выполнения.
                        AppointmentDate = Vaccination.AppointmentDate, //Дата назначения.
                        Type = Vaccination.Type, //Тип вакцинации.
                        Dose = Vaccination.Dose, //Доза вакцины.
                        Series = Vaccination.Series, //Серия препарата.
                        Name = Vaccination.Name, //Наименование вакцины.
                        Production = Vaccination.Production, //Место производства вакцины.
                        AdditionalInformation = Vaccination.AdditionalInformation //Дополнительная информация.
                    };

                    return CreatedAtAction(nameof(GetId), new { id = VaccinationPostResponse.Id }, VaccinationPostResponse); //201.
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
        public async Task<IActionResult> PutId(Guid id, [FromBody] VaccinationPutIdRequest VaccinationPutIdRequest)
        {
            try
            {
                if (VaccinationPutIdRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(VaccinationPutIdRequest.ReleaseDate, out DateOnly Date);
                if (Resalt == true)
                {
                    VaccinationPutIdRequest.ReleaseDate = Convert.ToString(Date);
                }
                else
                {
                    VaccinationPutIdRequest.ReleaseDate = null;
                }

                Resalt = DateOnly.TryParse(VaccinationPutIdRequest.AppointmentDate, out Date);
                if (Resalt == true)
                {
                    VaccinationPutIdRequest.AppointmentDate = Convert.ToString(Date);
                }
                else
                {
                    VaccinationPutIdRequest.AppointmentDate = null;
                }

                if (
                    VaccinationPutIdRequest.AppointmentDate == null ||
                    VaccinationPutIdRequest.Type == null ||
                    VaccinationPutIdRequest.Name == null
                    )
                {
                    return BadRequest(); //400.
                }
                else
                {
                    var Db = new PolyclinicContext();
                    var Vaccination = await Db.Vaccinations.FirstOrDefaultAsync(z => z.Id == id);

                    if (Vaccination == null)
                    {
                        return NotFound(); //404.
                    }

                    Vaccination.ReleaseDate = VaccinationPutIdRequest.ReleaseDate; //Дата выполнения.
                    Vaccination.AppointmentDate = VaccinationPutIdRequest.AppointmentDate; //Дата назначения.
                    Vaccination.Type = VaccinationPutIdRequest.Type; //Тип вакцинации.
                    Vaccination.Dose = VaccinationPutIdRequest.Dose; //Доза вакцины.
                    Vaccination.Series = VaccinationPutIdRequest.Series; //Серия препарата.
                    Vaccination.Name = VaccinationPutIdRequest.Name; //Наименование вакцины.
                    Vaccination.Production = VaccinationPutIdRequest.Production; //Место производства вакцины.
                    Vaccination.AdditionalInformation = VaccinationPutIdRequest.AdditionalInformation; //Дополнительная информация.
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
                var Vaccination = await Db.Vaccinations.FirstOrDefaultAsync(z => z.Id == id);

                if (Vaccination == null)
                {
                    return NotFound(); //404.
                }

                Db.Remove(Vaccination);
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