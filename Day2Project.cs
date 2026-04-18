Now the program or project which is there in Day 70 i had copied it in Day 71 and continuing th next task on it so u also add the project from 
  Day 70 int day 71 and start coding further and now using Authentication Controller and using register and login model
classes  i will insert values in Asp.net users table of identity which i had created in earlier section of Day1Project of code share  so just follow the steps here now okay 


go to day 58 
  and add this things 
go to project in Models folder add this class 

  using System.ComponentModel.DataAnnotations;

namespace ResortAPI.Models
{
    public class RegisterUser
    {

        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

    }
}

go to day 60 and add thises things in project 
namespace ResortAPI.Models
{
    public class Response
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}


Now add one folder with the name Services and add one Interface with name IAuth 

and add these methods into it 

using Microsoft.AspNetCore.Mvc;
using ResortAPI.Models;

namespace ResortAPI.Services
{
    public interface IAuth
    {
        Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role);
        Task<IActionResult> Login([FromBody] LoginModel loginModel);

    }
}

As in the interface i had kept LoginModel also so add one class now in Models folder of LoginModel like this 
  
using System.ComponentModel.DataAnnotations;

namespace ResortAPI.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

    }
}


 Now  In servcie class add AuthService class like this so 

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ResortAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ResortAPI.Services
{
    public class AuthService : IAuth
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<IActionResult> Register(RegisterUser registerUser, string role)
        {
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);

            if (userExist != null)
            {
                return new ObjectResult(new Response
                {
                    Status = "Error",
                    Message = "User already exists!"
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            IdentityUser user = new IdentityUser
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username
            };

            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.Password);

                if (!result.Succeeded)
                {
                    return new ObjectResult(new Response
                    {
                        Status = "Error",
                        Message = "User Failed to Create"
                    })
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }

                await _userManager.AddToRoleAsync(user, role);

                return new ObjectResult(new Response
                {
                    Status = "Success",
                    Message = "User created SuccessFully"
                })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            else
            {
                return new ObjectResult(new Response
                {
                    Status = "Error",
                    Message = "This Role Doesnot Exist."
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var userRoles = await _userManager.GetRolesAsync(user);

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var jwtToken = GetToken(authClaims);

                return new OkObjectResult(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                });
            }

            return new UnauthorizedResult();
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddYears(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}

Next create AuthenticationController of Api empty in Controller folder okay 

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResortAPI.Models;
using ResortAPI.Services;

namespace ResortAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuth _authService;

        public AuthenticationController(IAuth authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {
            return await _authService.Register(registerUser, role);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            return await _authService.Login(loginModel);
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            return Ok(new Response
            {
                Status = "Success",
                Message = "Logged out successfully"
            });
        }
    }
}


Next add this in program.cs 

builder.Services.AddScoped<IAuth, AuthService>(); // this line has to be added before sql connection code only it should be second line in program.cs 

and then go to app settings and write the JWT ..code here


{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "constring": "Data Source=LAPTOP-4G8BHPK9\\SQLEXPRESS;initial catalog=resortdb;Integrated Security=true;Encrypt=true;TrustServerCertificate=true;"
  },

  "JWT": {
    "ValidAudience": "https://localhost:7198",
    "ValidIssuer": "https://localhost:7198",
    "Secret": "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyrssjkdajad122"
  }
}


Now Program.cs file further jwt code is written like this and for authorize button in swagger i am
writing some code in the middleware because in swagger authorize button is not there like
postman so explicitly u have to add code and finally down u have to add one method which is
app.UseAuthentication();



so after db connection add this code 

   // For Identity
 builder.Services.AddIdentity<IdentityUser, IdentityRole>()
 .AddEntityFrameworkStores<ApplicationDbContext>()
 .AddDefaultTokenProviders();


 // adding basic authentication
 builder.Services.AddAuthentication(options =>
 {
     options.DefaultAuthenticateScheme =
     JwtBearerDefaults.AuthenticationScheme;
     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
     options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
 }).AddJwtBearer(options =>
 {
     options.SaveToken = true;
     options.RequireHttpsMetadata = false;
     options.TokenValidationParameters = new TokenValidationParameters()
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidAudience = builder.Configuration["JWT:ValidAudience"],
         ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
     };
 }); ;


Then for swagger button add this code 

builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
            });

you have to add this code after this line builder.Services.AddEndpointsApiExplorer();

add this code  app.UseAuthentication();


  app.UseHttpsRedirection(); // afetr this line add this 
  app.UseAuthentication();

so my total program.cs file will look like this now 


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ResortAPI.Data;
using ResortAPI.Models;
using ResortAPI.Services;
using System;
using System.Text;


namespace ResortAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IAuth, AuthService>();

            builder.Services.AddDbContext<ApplicationDbContext>
(options => options.UseSqlServer(builder.Configuration.GetConnectionString("constring")));

            // For Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            // adding basic authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
            }); ;






            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
            });
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}


so i had checked the program is working fine i am able to  create users and values are going in Asp.net users tabe and when i am doing login 
token is generated 

Now for Identtity user where identity user is also a table for those projects what you have to do is you have to take the above code and you have to tell in chatgpt 
the scenario that my class is extedning like this to identtity user so paste that class and paste above code for IAuth ,AuthService and also AuthenticationController to chatgpt 
and tell how for my sceanrion give me same classes code it will give here where ever Identity user is there there your class comes which is inheriting Identityuser 

so this is all about coding so i had kept the code in day71 ResortApi the code which i had changed step by step in this file that code only i had kept you have to take code from day 70 
  and do it step by step 

  


