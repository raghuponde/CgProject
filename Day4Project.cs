
you have to be in this link only to further continue with the project and to give inputs means u should not change the window as it will not give you complete information 
I am saying you to be in track with this https://www.perplexity.ai/search/6ee15a24-5b5a-444f-bd35-b555a288cfd4  

Now again open the project which u have done in Day61 to give some features of it now i am going for front end design of my code which is there now in Day 76 ModifiedProject okay 
here in Day 76 backend is ready now so create a new folder now with name Day 77 and in this add Day 76 project as from where to where you have moved some tracking idea will be 
there so now from   Day 77 open the project okay and start doing coding 

---------------------------------------Design Prompt ------------------------------------------------------------------------------------------

first paste
------------
My Backend is ready so for the Backend which I had created i want to create a new asp.net core mvc application of .net core 8.0 with name ModifiedMVCProject which will consume the 
web api of mine and i want to use bootswatch Quartz theme of bootstrap 5.0 for layout Now here i want to  use HttpClient method of asp.net core to consume the web api so I want all the 
action methods which will connect to my web api and in views of each action method  i want to use tag helpers of asp.net core along with bootstrap controls of bootswatch Quartz theme 

second paste in the same area where u pasted but some space u give it (shift enter)
---------------------------------------------------------------------

I want to see the DashBoard like this ( paste the image also which i had given in Day 77 this is for you people dont put this bracket message in chatgpt )  

so as per image i want search option and pagination option and left side navigation menu using accordian control which contrains list of employees data displayed through 
EmployeeData link and Employee Export to export data into excel sheet and add employee feature and images in small size as shown in dashboard has to be displayed 

third paste in the same area where u pasted but some space u give it (shift enter)
---------------------------------------------------------------------

so now i am pasting complete code  details of my web api down analyze it and give me the complete code for the scenario 

using Microsoft.AspNetCore.Http;
using ModifiedProject.DTO;

namespace ModifiedProject.Services
{
    public interface IEmployee
    {
        Task<List<EmployeeDto>> GetAllEmployeesAsync(int pageNumber, int pageSize);
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> AddEmployeeAsync(EmployeeDto employeeDto, IFormFile? image);
        Task<EmployeeDto?> UpdateEmployeeAsync(int id, EmployeeUpdateDto employeeDto, IFormFile? image);
        Task<EmployeeDto?> DeleteEmployeeAsync(int id);
        Task<List<EmployeeBasicDto>> GetAllEmployeeBasicInfoAsync(int pageNumber, int pageSize, string? searchTerm);
    }
}

using Microsoft.EntityFrameworkCore;
using ModifiedProject.DTO;
using ModifiedProject.Models;
using System;

namespace ModifiedProject.Services
{
    public class EmployeeService : IEmployee
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeService(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetBaseUrl()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("No HttpContext available.");

            var request = httpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync(int pageNumber, int pageSize)
        {
            var employees = await _context.employees
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return employees.Select(MapEmployeeToDto).ToList();
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.employees.FindAsync(id);
            if (employee == null)
                return null;

            return MapEmployeeToDto(employee);
        }

        public async Task<EmployeeDto> AddEmployeeAsync(EmployeeDto employeeDto, IFormFile? image)
        {
            var employee = new Employee
            {
                FirstName = employeeDto.FirstName!,
                LastName = employeeDto.LastName!,
                Email = employeeDto.Email!,
                Age = employeeDto.Age,
                ImagePath = "/uploads/default.jpg"
            };

            if (image != null && image.Length > 0)
            {
                employee.ImagePath = await SaveImageToUploadsAsync(image);
            }

            await _context.employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return MapEmployeeToDto(employee);
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, EmployeeUpdateDto employeeDto, IFormFile? image)
        {
            var existingEmployee = await _context.employees.FindAsync(id);
            if (existingEmployee == null)
                return null;

            existingEmployee.FirstName = employeeDto.FirstName!;
            existingEmployee.LastName = employeeDto.LastName!;
            existingEmployee.Email = employeeDto.Email!;
            existingEmployee.Age = employeeDto.Age;

            if (image != null && image.Length > 0)
            {
                DeleteImageFile(existingEmployee.ImagePath);
                existingEmployee.ImagePath = await SaveImageToUploadsAsync(image);
            }

            await _context.SaveChangesAsync();

            return MapEmployeeToDto(existingEmployee);
        }

        public async Task<EmployeeDto?> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.employees.FindAsync(id);
            if (employee == null)
                return null;

            var deletedEmployeeDto = MapEmployeeToDto(employee);

            DeleteImageFile(employee.ImagePath);
            _context.employees.Remove(employee);
            await _context.SaveChangesAsync();

            return deletedEmployeeDto;
        }

        public async Task<List<EmployeeBasicDto>> GetAllEmployeeBasicInfoAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            var query = _context.employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e =>
                    e.FirstName.Contains(searchTerm) ||
                    e.LastName.Contains(searchTerm) ||
                    e.Email.Contains(searchTerm));
            }

            var employees = await query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            string baseUrl = GetBaseUrl();

            return employees.Select(e => new EmployeeBasicDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                ImageUrl = string.IsNullOrEmpty(e.ImagePath)
                    ? $"{baseUrl}/uploads/default.jpg"
                    : $"{baseUrl}{e.ImagePath}"
            }).ToList();
        }

        private EmployeeDto MapEmployeeToDto(Employee employee)
        {
            string baseUrl = GetBaseUrl();

            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Age = employee.Age,
                ImagePath = string.IsNullOrEmpty(employee.ImagePath)
                    ? $"{baseUrl}/uploads/default.jpg"
                    : $"{baseUrl}{employee.ImagePath}"
            };
        }
        private void DeleteImageFile(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || imagePath.Contains("default.jpg"))
                return;

            if (imagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(imagePath);
                imagePath = uri.AbsolutePath;
            }

            var fullPath = Path.Combine(
                _env.WebRootPath!,
                imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private async Task<string> SaveImageToUploadsAsync(IFormFile image)
        {
            if (_env.WebRootPath == null)
            {
                var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (!Directory.Exists(wwwrootPath))
                    Directory.CreateDirectory(wwwrootPath);

                _env.WebRootPath = wwwrootPath;
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(image.FileName);
            var imageName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadsFolder, imageName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await image.CopyToAsync(stream);

            return $"/uploads/{imageName}";
        }
    }
}
app settings file 
------------------
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=LAPTOP-4G8BHPK9\\SQLEXPRESS;Initial Catalog=EmpModified;Integrated Security=True;TrustServerCertificate=True;"
  }
}

Models classses 
----------------
using System;
using System.Collections.Generic;

namespace ModifiedProject.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Age { get; set; }

    public string? ImagePath { get; set; }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ModifiedProject.Models;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    public DbSet<Employee> employees { set; get; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SeedRoles(modelBuilder);
    }

    private static void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole()
            {
                Name = "Admin",
                ConcurrencyStamp = "1",
                NormalizedName = "Admin"
            },
            new IdentityRole()
            {
                Name = "User",
                ConcurrencyStamp = "2",
                NormalizedName = "User"
            },
            new IdentityRole()
            {
                Name = "HR",
                ConcurrencyStamp = "3",
                NormalizedName = "HR"
            }
        );
    }
}

DTO classes 
-------------
namespace ModifiedProject.DTO
{
    public class EmployeeBasicDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ImageUrl { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ModifiedProject.DTO
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your firstname")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your lastname")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please enter email id")]
        [EmailAddress(ErrorMessage = "Please enter valid email id")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your age")]
        [Range(1, 100, ErrorMessage = "Please enter your age between 1 to 100 only")]
        public int Age { get; set; }

        public string? ImagePath { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace ModifiedProject.DTO
{
    public class EmployeeUpdateDto
    {
        [Required(ErrorMessage = "Please enter your firstname")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your lastname")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please enter email id")]
        [EmailAddress(ErrorMessage = "Please enter valid email id")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your age")]
        [Range(1, 100, ErrorMessage = "Please enter your age between 1 to 100 only")]
        public int Age { get; set; }

        public string? ImagePath { get; set; }
    }
}

EmpController of web api 
------------------------
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModifiedProject.DTO;
using ModifiedProject.Services;

namespace ModifiedProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class EmpController : ControllerBase
    {
        private readonly IEmployee _employeeService;

        public EmpController(IEmployee employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeDto>>> GetAll(int page = 1, int pageSize = 5)
        {
            var result = await _employeeService.GetAllEmployeesAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
                return NotFound("Employee not found");

            return Ok(employee);
        }

        [HttpGet("basic")]
        public async Task<ActionResult<List<EmployeeBasicDto>>> GetBasicEmployeeList(
            int page = 1, int pageSize = 5, string? search = null)
        {
            var result = await _employeeService.GetAllEmployeeBasicInfoAsync(page, pageSize, search);
            return Ok(result);
        }

        [HttpPost]
       // [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<EmployeeDto>> Create([FromForm] EmployeeDto employeeDto, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var added = await _employeeService.AddEmployeeAsync(employeeDto, image);
            return Ok(added);
        }

        [HttpPut("{id}")]
       // [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<EmployeeDto>> Update(
            int id,
            [FromForm] EmployeeUpdateDto employeeDto,
            IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _employeeService.UpdateEmployeeAsync(id, employeeDto, image);

            if (updated == null)
                return NotFound("Employee not found to update");

            return Ok(updated);
        }

        [HttpDelete("{id}")]
      //  [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeeDto>> Delete(int id)
        {
            var deleted = await _employeeService.DeleteEmployeeAsync(id);

            if (deleted == null)
                return NotFound("Employee not found to delete");

            return Ok(deleted);
        }

        [HttpGet("export/excel")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportToExcel(string? search = null)
        {
            var employees = await _employeeService.GetAllEmployeeBasicInfoAsync(1, int.MaxValue, search);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Employees");

            worksheet.Cell(1, 1).Value = "First Name";
            worksheet.Cell(1, 2).Value = "Last Name";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Image URL";

            int row = 2;
            foreach (var emp in employees)
            {
                worksheet.Cell(row, 1).Value = emp.FirstName;
                worksheet.Cell(row, 2).Value = emp.LastName;
                worksheet.Cell(row, 3).Value = emp.Email;
                worksheet.Cell(row, 4).Value = emp.ImageUrl;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Employees.xlsx");
        }
    }
}

program.cs 
-----------

using Microsoft.EntityFrameworkCore;
using ModifiedProject.Models;
using ModifiedProject.Services;

namespace ModifiedProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpContextAccessor();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddScoped<IEmployee, EmployeeService>();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

           // app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

fourth paste in the same area where u pasted but some space u give it 
---------------------------------------------------------------------
Also i am pasting the views of Edit ,Details,Export and AddEmployee,directory and swagger etc  and Delete views so like that only create the code for views 

Also providing you the image of my web api swagger and all urls as well for your referecne so that you can read it in front end 

and this is my url u can use in programming code 

https://localhost:7238/api/Emp?page=1&pageSize=5
https://localhost:7238/api/Emp/1 

so now give me all the steps in sequence to perform with complete coding of it in detail




   
