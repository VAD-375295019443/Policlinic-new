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
    public class InstrumentalTestController : ControllerBase
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

                var InstrumentalTestsRelease = await Db.InstrumentalTests
                                                  .Where(z => z.PatientId == Patient.Id && z.ReleaseDate != null)
                                                  .Select(z => new { z.Id, z.ReleaseDate, z.AppointmentDate, z.Name, z.Result })
                                                  .ToListAsync();

                InstrumentalTestsRelease = InstrumentalTestsRelease.TakeLast(50).ToList();

                var InstrumentalTests = await Db.InstrumentalTests
                                          .Where(z => z.PatientId == Patient.Id && z.ReleaseDate == null)
                                          .Select(z => new { z.Id, z.ReleaseDate, z.AppointmentDate, z.Name, z.Result })
                                          .ToListAsync();

                var InstrumentalTestGetResponse = InstrumentalTestsRelease.Union(InstrumentalTests);

                return Ok(InstrumentalTestGetResponse);
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
                var InstrumentalTest = await Db.InstrumentalTests.FirstOrDefaultAsync(z => z.Id == id);

                if (InstrumentalTest == null)
                {
                    return NotFound(); //404.
                }

                var InstrumentalTestGetIdResponse = new InstrumentalTestGetIdResponse()
                {
                    Id = InstrumentalTest.Id,
                    ReleaseDate = InstrumentalTest.ReleaseDate, //Дата выполнения.
                    AppointmentDate = InstrumentalTest.AppointmentDate, //Дата назначения.
                    Name = InstrumentalTest.Name,
                    Result = InstrumentalTest.Result
                };

                return Ok(InstrumentalTestGetIdResponse); //200.
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
        public async Task<IActionResult> Post(string Email, [FromBody] InstrumentalTestPostRequest InstrumentalTestPostRequest)
        {
            try
            {
                if (Email == null || InstrumentalTestPostRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(InstrumentalTestPostRequest.ReleaseDate, out DateOnly Date);
                if (Resalt == true)
                {
                    InstrumentalTestPostRequest.ReleaseDate = Convert.ToString(Date);
                }
                else
                {
                    InstrumentalTestPostRequest.ReleaseDate = null;
                }

                Resalt = DateOnly.TryParse(InstrumentalTestPostRequest.AppointmentDate, out Date);
                if (Resalt == true)
                {
                    InstrumentalTestPostRequest.AppointmentDate = Convert.ToString(Date);
                }
                else
                {
                    InstrumentalTestPostRequest.AppointmentDate = null;
                }

                if (
                    InstrumentalTestPostRequest.AppointmentDate == null ||
                    InstrumentalTestPostRequest.Name == null
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

                    var InstrumentalTest = new InstrumentalTest()
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Patient.Id,
                        ReleaseDate = InstrumentalTestPostRequest.ReleaseDate, //Дата выполнения.
                        AppointmentDate = InstrumentalTestPostRequest.AppointmentDate, //Дата назначения.
                        Name = InstrumentalTestPostRequest.Name,
                        Result = InstrumentalTestPostRequest.Result
                    };

                    Db.InstrumentalTests.Add(InstrumentalTest);
                    Db.SaveChanges();

                    InstrumentalTest = await Db.InstrumentalTests.FirstOrDefaultAsync(z => z.Id == InstrumentalTest.Id);

                    if (InstrumentalTest == null)
                    {
                        return NotFound(); //404.
                    }

                    var InstrumentalTestPostResponse = new InstrumentalTestPostResponse()
                    {
                        Id = InstrumentalTest.Id,
                        ReleaseDate = InstrumentalTest.ReleaseDate, //Дата выполнения.
                        AppointmentDate = InstrumentalTest.AppointmentDate, //Дата назначения.
                        Name = InstrumentalTest.Name,
                        Result = InstrumentalTest.Result
                    };

                    return CreatedAtAction(nameof(GetId), new { id = InstrumentalTestPostResponse.Id }, InstrumentalTestPostResponse); //201.
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
        public async Task<IActionResult> PutId(Guid id, [FromBody] InstrumentalTestPutIdRequest InstrumentalTestPutIdRequest)
        {
            try
            {
                if (InstrumentalTestPutIdRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(InstrumentalTestPutIdRequest.ReleaseDate, out DateOnly Date);
                if (Resalt == true)
                {
                    InstrumentalTestPutIdRequest.ReleaseDate = Convert.ToString(Date);
                }
                else
                {
                    InstrumentalTestPutIdRequest.ReleaseDate = null;
                }

                Resalt = DateOnly.TryParse(InstrumentalTestPutIdRequest.AppointmentDate, out Date);
                if (Resalt == true)
                {
                    InstrumentalTestPutIdRequest.AppointmentDate = Convert.ToString(Date);
                }
                else
                {
                    InstrumentalTestPutIdRequest.AppointmentDate = null;
                }

                if (
                    InstrumentalTestPutIdRequest.AppointmentDate == null ||
                    InstrumentalTestPutIdRequest.Name == null
                    )
                {
                    return BadRequest(); //400.
                }
                else
                {
                    var Db = new PolyclinicContext();
                    var InstrumentalTest = await Db.InstrumentalTests.FirstOrDefaultAsync(z => z.Id == id);

                    if (InstrumentalTest == null)
                    {
                        return NotFound(); //404.
                    }

                    InstrumentalTest.ReleaseDate = InstrumentalTestPutIdRequest.ReleaseDate; //Дата выполнения.
                    InstrumentalTest.AppointmentDate = InstrumentalTestPutIdRequest.AppointmentDate; //Дата назначения.
                    InstrumentalTest.Name = InstrumentalTestPutIdRequest.Name;
                    InstrumentalTest.Result = InstrumentalTestPutIdRequest.Result;
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
                var InstrumentalTest = await Db.InstrumentalTests.FirstOrDefaultAsync(z => z.Id == id);

                if (InstrumentalTest == null)
                {
                    return NotFound(); //404.
                }

                Db.Remove(InstrumentalTest);
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