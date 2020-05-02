using System;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.api.Data;
using Microsoft.AspNetCore.Mvc;
using DatingApp.api.Models;
using DatingApp.api.DTOs;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase 
    {
        private readonly IAuthRepository _iauthRepository;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository iauthRepository,IConfiguration  config)
        {
            _iauthRepository = iauthRepository;
            _config = config;
        }
       [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO)
        {
           userForRegisterDTO.Name=userForRegisterDTO.Name.ToLower();
           if(await _iauthRepository.UserExists(userForRegisterDTO.Name))
            return BadRequest("User already exists");

            var userToCreate=new User
            {
              Name=userForRegisterDTO.Name
            };
            var user =_iauthRepository.Register(userToCreate,userForRegisterDTO.Password);
            return StatusCode(201);
        }

         [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {
         var userFromRepo= await _iauthRepository.Login(userForLoginDTO.Name.ToLower(),userForLoginDTO.Password);
         if(userFromRepo ==null)
         return Unauthorized();
         var claim= new []
         {
             new Claim[]
             {
                 new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                 new Claim(ClaimTypes.Name,userFromRepo.Name)
             }
         };
             var key=new SymmetricSecurityKey(Encoding.UTF8.
             GetBytes(_config.GetSection("AppSettings:Token").Value));

             var cred=new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            
             var tokenDescriptor=new SecurityTokenDescriptor
             {
                 Subject=new ClaimsIdentity(),
                 Expires=DateTime.Now.AddDays(1),
                 SigningCredentials=cred
             };
             var tokenHandler=new JwtSecurityTokenHandler();
             var token=tokenHandler.CreateToken(tokenDescriptor);
             return Ok(new {
                 token=tokenHandler.WriteToken(token)
             });

        }
        
    }
}