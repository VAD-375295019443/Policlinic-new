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
    public class PatientController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetId(Guid id)
        {
            try
            {
                var Db = new PolyclinicContext();
                var Patient = await Db.Patients.FirstOrDefaultAsync(z => z.Id == id);

                if (Patient == null)
                {
                    return NotFound(); //404.
                }

                var PatientGetIdResponse = new PatientGetIdResponse()
                {
                    Id = Patient.Id, //Код.
                    Email = Patient.Email, //Логин.
                    Surname = Patient.Surname, //Фамилия.
                    Name = Patient.Name, //Имя.
                    MiddleName = Patient.MiddleName, //Отчество.
                    DateOfBirth = Patient.DateOfBirth, //Дата рождения.
                    Floor = Patient.Floor, //Пол.
                    Telephone = Patient.Telephone, //Телефон.
                    Country = Patient.Country, //Страна.
                    Region = Patient.Region, //Область.
                    District = Patient.District, //Район.
                    LocalityType = Patient.LocalityType, //Статус населенного пункта.
                    Locality = Patient.Locality, //Населенный пункт.
                    StreetType = Patient.StreetType, //Статус улицы.
                    Street = Patient.Street, //Улица.
                    House = Patient.House, //Дом.
                    Flat = Patient.Flat, //Квартира.
                    AdditionalInformation = Patient.AdditionalInformation //Дополнительная информация.
                };

                return Ok(PatientGetIdResponse); //200.
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
        public async Task<IActionResult> Post([FromBody] PatientPostRequest PatientPostRequest)
        {
            try
            {
                if (PatientPostRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(PatientPostRequest.DateOfBirth, out DateOnly Date);
                if (Resalt == true)
                {
                    PatientPostRequest.DateOfBirth = Convert.ToString(Date);
                }
                else
                {
                    PatientPostRequest.DateOfBirth = null;
                }

                if (
                    PatientPostRequest.Email == null ||
                    PatientPostRequest.Password == null ||
                    PatientPostRequest.Surname == null ||
                    PatientPostRequest.Name == null ||
                    PatientPostRequest.MiddleName == null ||
                    PatientPostRequest.DateOfBirth == null ||
                    PatientPostRequest.Floor == null ||
                    PatientPostRequest.Telephone == null ||
                    PatientPostRequest.Country == null ||
                    PatientPostRequest.Region == null ||
                    PatientPostRequest.District == null ||
                    PatientPostRequest.LocalityType == null ||
                    PatientPostRequest.Locality == null ||
                    PatientPostRequest.StreetType == null ||
                    PatientPostRequest.Street == null ||
                    PatientPostRequest.Flat == null
                    )
                {
                    return BadRequest(); //400.
                }
                else
                {
                    var Patient = new Patient()
                    {
                        Id = Guid.NewGuid(),
                        Email = PatientPostRequest.Email,
                        Password = PatientPostRequest.Password,
                        Surname = PatientPostRequest.Surname,
                        Name = PatientPostRequest.Name,
                        MiddleName = PatientPostRequest.MiddleName,
                        DateOfBirth = PatientPostRequest.DateOfBirth,
                        Floor = PatientPostRequest.Floor,
                        Telephone = PatientPostRequest.Telephone,
                        Country = PatientPostRequest.Country,
                        Region = PatientPostRequest.Region,
                        District = PatientPostRequest.District,
                        LocalityType = PatientPostRequest.LocalityType,
                        Locality = PatientPostRequest.Locality,
                        StreetType = PatientPostRequest.StreetType,
                        Street = PatientPostRequest.Street,
                        House = PatientPostRequest.House,
                        Flat = PatientPostRequest.Flat,
                        AdditionalInformation = PatientPostRequest.AdditionalInformation
                    };

                    var Db = new PolyclinicContext();
                    Db.Patients.Add(Patient);
                    Db.SaveChanges();

                    Patient = await Db.Patients.FirstOrDefaultAsync(z => z.Id == Patient.Id);

                    if (Patient == null)
                    {
                        return NotFound(); //404.
                    }
                    
                    var PatientPostResponse = new PatientPostResponse()
                    {
                        Id = Patient.Id, //Код.
                        Email = Patient.Email, //Логин.
                        Surname = Patient.Surname, //Фамилия.
                        Name = Patient.Name, //Имя.
                        MiddleName = Patient.MiddleName, //Отчество.
                        DateOfBirth = Patient.DateOfBirth, //Дата рождения.
                        Floor = Patient.Floor, //Пол.
                        Telephone = Patient.Telephone, //Телефон.
                        Country = Patient.Country, //Страна.
                        Region = Patient.Region, //Область.
                        District = Patient.District, //Район.
                        LocalityType = Patient.LocalityType, //Статус населенного пункта.
                        Locality = Patient.Locality, //Населенный пункт.
                        StreetType = Patient.StreetType, //Статус улицы.
                        Street = Patient.Street, //Улица.
                        House = Patient.House, //Дом.
                        Flat = Patient.Flat, //Квартира.
                        AdditionalInformation = Patient.AdditionalInformation //Дополнительная информация.
                    };

                    return CreatedAtAction(nameof(GetId), new { id = PatientPostResponse.Id }, PatientPostResponse); //201.
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
        public async Task<IActionResult> PutId(Guid id, [FromBody] PatientPutIdRequest PatientPutIdRequest)
        {
            try
            {
                if (PatientPutIdRequest == null)
                {
                    return BadRequest(); //400.
                }

                bool Resalt = DateOnly.TryParse(PatientPutIdRequest.DateOfBirth, out DateOnly Date);
                if (Resalt == true)
                {
                    PatientPutIdRequest.DateOfBirth = Convert.ToString(Date);
                }
                else
                {
                    PatientPutIdRequest.DateOfBirth = null;
                }

                if (
                    PatientPutIdRequest.Email == null ||
                    PatientPutIdRequest.Password == null ||
                    PatientPutIdRequest.Surname == null ||
                    PatientPutIdRequest.Name == null ||
                    PatientPutIdRequest.MiddleName == null ||
                    PatientPutIdRequest.DateOfBirth == null ||
                    PatientPutIdRequest.Floor == null ||
                    PatientPutIdRequest.Telephone == null ||
                    PatientPutIdRequest.Country == null ||
                    PatientPutIdRequest.Region == null ||
                    PatientPutIdRequest.District == null ||
                    PatientPutIdRequest.LocalityType == null ||
                    PatientPutIdRequest.Locality == null ||
                    PatientPutIdRequest.StreetType == null ||
                    PatientPutIdRequest.Street == null ||
                    PatientPutIdRequest.Flat == null
                    )
                {
                    return BadRequest(); //400.
                }
                else
                {
                    var Db = new PolyclinicContext();
                    var Patient = await Db.Patients.FirstOrDefaultAsync(z => z.Id == id);

                    if (Patient == null)
                    {
                        return NotFound(); //404.
                    }

                    Patient.Email = PatientPutIdRequest.Email;
                    Patient.Password = PatientPutIdRequest.Password;
                    Patient.Surname = PatientPutIdRequest.Surname;
                    Patient.Name = PatientPutIdRequest.Name;
                    Patient.MiddleName = PatientPutIdRequest.MiddleName;
                    Patient.DateOfBirth = PatientPutIdRequest.DateOfBirth;
                    Patient.Floor = PatientPutIdRequest.Floor;
                    Patient.Telephone = PatientPutIdRequest.Telephone;
                    Patient.Country = PatientPutIdRequest.Country;
                    Patient.Region = PatientPutIdRequest.Region;
                    Patient.District = PatientPutIdRequest.District;
                    Patient.LocalityType = PatientPutIdRequest.LocalityType;
                    Patient.Locality = PatientPutIdRequest.Locality;
                    Patient.StreetType = PatientPutIdRequest.StreetType;
                    Patient.Street = PatientPutIdRequest.Street;
                    Patient.House = PatientPutIdRequest.House;
                    Patient.Flat = PatientPutIdRequest.Flat;
                    Patient.AdditionalInformation = PatientPutIdRequest.AdditionalInformation;
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
                var Patient = await Db.Patients.FirstOrDefaultAsync(z => z.Id == id);

                if (Patient == null)
                {
                    return NotFound(); //404.
                }

                Db.Remove(Patient);
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