using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyclinicWeb.Classes;
using PolyclinicWebAPI.Models.Request;
using PolyclinicWebAPI.Models.Response;
using Serilog;
using System.Linq;

namespace PolyclinicWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LaboratoryTestController : ControllerBase
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

                var LaboratoryTestsRelease = await Db.LaboratoryTests
                                                  .Where(z => z.PatientId == Patient.Id && z.ReleaseDate != null)
                                                  .Select(z => new { z.Id, z.ReleaseDate, z.AppointmentDate, z.Name, z.Result })
                                                  .ToListAsync();

                LaboratoryTestsRelease = LaboratoryTestsRelease.TakeLast(50).ToList();

                var LaboratoryTests = await Db.LaboratoryTests
                                          .Where(z => z.PatientId == Patient.Id && z.ReleaseDate == null)
                                          .Select(z => new { z.Id, z.ReleaseDate, z.AppointmentDate, z.Name, z.Result })
                                          .ToListAsync();

                var LaboratoryTestGetResponse = LaboratoryTestsRelease.Union(LaboratoryTests);

                return Ok(LaboratoryTestGetResponse);
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
                var LaboratoryTest = await Db.LaboratoryTests.FirstOrDefaultAsync(z => z.Id == id);

                if (LaboratoryTest == null)
                {
                    return NotFound(); //404.
                }

                var LaboratoryTestGetIdResponse = new LaboratoryTestGetIdResponse()
                {
                    Id = LaboratoryTest.Id,
                    ReleaseDate = LaboratoryTest.ReleaseDate, //Дата выполнения.
                    AppointmentDate = LaboratoryTest.AppointmentDate, //Дата назначения.
                    Name = LaboratoryTest.Name,
                    Result = LaboratoryTest.Result
                };

                return Ok(LaboratoryTestGetIdResponse); //200.
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
        public async Task<IActionResult> Post(string Email, [FromBody] LaboratoryTestPostRequest LaboratoryTestPostRequest)
        {
            try
            {
                if (Email == null || LaboratoryTestPostRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(LaboratoryTestPostRequest.ReleaseDate, out DateOnly Date);
                if (Resalt == true)
                {
                    LaboratoryTestPostRequest.ReleaseDate = Convert.ToString(Date);
                }
                else
                {
                    LaboratoryTestPostRequest.ReleaseDate = null;
                }

                Resalt = DateOnly.TryParse(LaboratoryTestPostRequest.AppointmentDate, out Date);
                if (Resalt == true)
                {
                    LaboratoryTestPostRequest.AppointmentDate = Convert.ToString(Date);
                }
                else
                {
                    LaboratoryTestPostRequest.AppointmentDate = null;
                }

                if (
                    LaboratoryTestPostRequest.AppointmentDate == null ||
                    LaboratoryTestPostRequest.Name == null
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

                    var LaboratoryTest = new LaboratoryTest()
                    {
                        Id = Guid.NewGuid(),
                        PatientId = Patient.Id,
                        ReleaseDate = LaboratoryTestPostRequest.ReleaseDate, //Дата выполнения.
                        AppointmentDate = LaboratoryTestPostRequest.AppointmentDate, //Дата назначения.
                        Name = LaboratoryTestPostRequest.Name,
                        Result = LaboratoryTestPostRequest.Result
                    };

                    Db.LaboratoryTests.Add(LaboratoryTest);
                    Db.SaveChanges();

                    LaboratoryTest = await Db.LaboratoryTests.FirstOrDefaultAsync(z => z.Id == LaboratoryTest.Id);

                    if (LaboratoryTest == null)
                    {
                        return NotFound(); //404.
                    }

                    var LaboratoryTestPostResponse = new LaboratoryTestPostResponse()
                    {
                        Id = LaboratoryTest.Id,
                        ReleaseDate = LaboratoryTest.ReleaseDate, //Дата выполнения.
                        AppointmentDate = LaboratoryTest.AppointmentDate, //Дата назначения.
                        Name = LaboratoryTest.Name,
                        Result = LaboratoryTest.Result
                    };

                    return CreatedAtAction(nameof(GetId), new { id = LaboratoryTestPostResponse.Id }, LaboratoryTestPostResponse); //201.
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
        public async Task<IActionResult> PutId(Guid id, [FromBody] LaboratoryTestPutIdRequest LaboratoryTestPutIdRequest)
        {
            try
            {
                if (LaboratoryTestPutIdRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(LaboratoryTestPutIdRequest.ReleaseDate, out DateOnly Date);
                if (Resalt == true)
                {
                    LaboratoryTestPutIdRequest.ReleaseDate = Convert.ToString(Date);
                }
                else
                {
                    LaboratoryTestPutIdRequest.ReleaseDate = null;
                }

                Resalt = DateOnly.TryParse(LaboratoryTestPutIdRequest.AppointmentDate, out Date);
                if (Resalt == true)
                {
                    LaboratoryTestPutIdRequest.AppointmentDate = Convert.ToString(Date);
                }
                else
                {
                    LaboratoryTestPutIdRequest.AppointmentDate = null;
                }

                if (
                    LaboratoryTestPutIdRequest.AppointmentDate == null ||
                    LaboratoryTestPutIdRequest.Name == null
                    )
                {
                    return BadRequest(); //400.
                }
                else
                {
                    var Db = new PolyclinicContext();
                    var LaboratoryTest = await Db.LaboratoryTests.FirstOrDefaultAsync(z => z.Id == id);

                    if (LaboratoryTest == null)
                    {
                        return NotFound(); //404.
                    }

                    LaboratoryTest.ReleaseDate = LaboratoryTestPutIdRequest.ReleaseDate; //Дата выполнения.
                    LaboratoryTest.AppointmentDate = LaboratoryTestPutIdRequest.AppointmentDate; //Дата назначения.
                    LaboratoryTest.Name = LaboratoryTestPutIdRequest.Name;
                    LaboratoryTest.Result = LaboratoryTestPutIdRequest.Result;
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
                var LaboratoryTest = await Db.LaboratoryTests.FirstOrDefaultAsync(z => z.Id == id);

                if (LaboratoryTest == null)
                {
                    return NotFound(); //404.
                }

                Db.Remove(LaboratoryTest);
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