using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PolyclinicWeb.Classes;
using PolyclinicWebAPI.Classes;
using PolyclinicWebAPI.Models.Request;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PolyclinicWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegisterUserController : ControllerBase
    {
        // POST api/<RegisterUserController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterUserRequestModel RegisterData)
        {
            var Db = new PolyclinicContext();
            var Patient = await Db.Patients.FirstOrDefaultAsync(z => z.Email == RegisterData.email && z.Password == RegisterData.password);

            if (Patient == null)
            {
                return Unauthorized(); //401.
            }

            if (Patient.Email == null)
            {
                return BadRequest(); //400.
            }

            var Claims = new List<Claim> { new Claim(ClaimTypes.Name, Patient.Email) };
            
            //Создание токена.
            var Jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: Claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var EncodedJwt = new JwtSecurityTokenHandler().WriteToken(Jwt);

            //Формирование ответа.
            var Response = new
            {
                Access_token = EncodedJwt,
                Username = Patient.Email
            };

            return Ok(Response);
        }
    }
}
