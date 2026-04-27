step 1 :create an web api project in Day 75 folder 


step 2: Add the following packages in the project of version all 8.0.24

Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Identity.EntityFrameworkCore  
Microsoft.AspNetCore.Authentication.JwtBearer   

step 3:IN the project which u created earlier check the db in sql server there and take out script file from sql sever of your database once 


create database EmpModified 

CREATE TABLE [dbo].[employees](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](max) NOT NULL,
	[LastName] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Age] [int] NOT NULL,
	[ImagePath] [nvarchar](max) NULL,
 CONSTRAINT [PK_employees] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

step 4 : 
--------
	here in your project first create Models folder as in the command below Models i had mentioned 
	
	Scaffold-DbContext 'Data Source=LAPTOP-4G8BHPK9\SQLEXPRESS;initial catalog=EmpModified;Integrated Security=true;TrustServerCertificate=True;' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context ApplicationDbContext -Force

step 5:
---------
	Modify the applciation class with this one 

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

Step 6:
---------
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
Finally run the migrations and this is DBFirst appraoch 

---->Now put this prompt to chatgpt it is big prompt so 

----------------------prompt as per your namespace ----------------------
	I am using DbFirst approach of Entity framework and i am having a table Employee so on this employee i want to implement crud operation so I will use IEmployee interface and EmployeeService class in Services folder and in EmpController i want to use referecne of IEmployee obj so all async methods I want to use
so now the problem is in DBfirst approach the class generated is autogenrated partial class so i cannot use that Employee class now i want to create a corresponding EmployeeDto and through EmployeeDto I want to do crud operations on Employe object so for this give me the complete code all in steps

Now this is autogenrated class 


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



for the above class i have decided the below class as my EmployeeDto which is there in DTO folder 

  public class EmployeeDto
  {
      public int Id { get; set; }

      [Required(ErrorMessage = "Please enter your firstname")]
      public string? FirstName { set; get; }

      [Required(ErrorMessage = "Please enter your lastname")]
      public string? LastName { set; get; }

      [Required(ErrorMessage = "Please enter email id")]
      [EmailAddress(ErrorMessage = "Please enter valid email id")]
      public string? Email { set; get; }

      [Required(ErrorMessage = "Please enter your age")]
      [Range(0, 100, ErrorMessage = "Please enter your age betwen 1 to 100 only ")]

      public int Age { set; get; }

      public string? ImagePath { set; get; }
  }
  
  and now  i am having IEmployee interface like this in Services folder 
  
  
   public interface IEmployee
 {
     Task<List<Employee>> GetAllEmployeesAsync(int pageNumber,int pageSize);
     Task<Employee?> GetEmployeeByIdAsync(int id);
     Task<Employee> AddEmployeeAsync(Employee employee,IFormFile image);
     Task<Employee?> UpdateEmployeeAsync(Employee employee,IFormFile? image);
     Task<Employee?> DeleteEmployeeAsync(int id);
     Task<List<EmployeeBasicDto>> GetAllEmployeeBasicInfoAsync(int pageNumber, int pageSize,
           string? searchTerm);
 }
 
 and EmployeeService class like this in Services folder 
 
 
 
    public class EmployeeService : IEmployee
    {
        private readonly EmpContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmployeeService(EmpContext context,IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetBaseUrl()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) throw new InvalidOperationException("No HttpContext");
            var request = httpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }
       // FileStream fs;
        public async Task<Employee> AddEmployeeAsync(Employee employee,IFormFile? image)
        {
            if(image!=null && image.Length > 0)
            {
                employee.ImagePath = SaveImageToUploads(image);
          
                
            }
            else
            {
                employee.ImagePath = "/uploads/default.jpg";
            }
                await _context.employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            employee.ImagePath = GetBaseUrl() + employee.ImagePath;
            return employee;
        }

        public async Task<Employee?> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.employees.FindAsync(id);
            if (employee == null) return null;
            DeleteImageFile(employee.ImagePath);
            _context.employees.Remove(employee);
            await _context.SaveChangesAsync();
            employee.ImagePath = null; // optional to avoid exposing deleted image URL
            return employee;

           
        }

        public async Task<List<Employee>> GetAllEmployeesAsync(int pageNumber,int pageSize)
        {
            var employees = await _context.employees.
                Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            string baseUrl = GetBaseUrl();
            foreach(var e in employees)
            {
                e.ImagePath = string.IsNullOrEmpty(e.ImagePath) ?
                    baseUrl + "/uploads/default.jpg" : baseUrl + e.ImagePath;
            }
            return employees;
            
        }
        public async Task<List<EmployeeBasicDto>> GetAllEmployeeBasicInfoAsync(int pageNumber, int pageSize,
             string? searchTerm)
        {
            var query = _context.employees.AsQueryable();
            if(!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.FirstName!.Contains(searchTerm) || e.LastName!.Contains(searchTerm) ||
                e.Email!.Contains(searchTerm));

            }

            var employees = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            string baseUrl = GetBaseUrl();

            var basicList = employees.Select(e => new EmployeeBasicDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                ImageUrl = string.IsNullOrEmpty(e.ImagePath)
                    ? baseUrl + "/uploads/default.jpg"
                    : baseUrl + e.ImagePath
            }).ToList();

            return basicList;
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
           var emp=  await _context.employees.FindAsync(id);
            if (emp != null)
            {
                emp.ImagePath = string.IsNullOrEmpty(emp.ImagePath)
                    ? GetBaseUrl() + "/uploads/default.jpg"
                    : GetBaseUrl() + emp.ImagePath;
            }
            return emp;
        }
        private void DeleteImageFile(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)||imagePath.Contains("default.jpg"))
                return;

            var fullPath = Path.Combine
                (_env.WebRootPath, imagePath.TrimStart('/').Replace
                ('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private string SaveImageToUploads(IFormFile image)
        {
            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fullPath = Path.Combine(uploadPath, imageName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            image.CopyTo(stream);

            return "/uploads/" + imageName;
        }
        public async Task<Employee?> UpdateEmployeeAsync(Employee employee,IFormFile? image)
        {
            var existing = await _context.employees.FindAsync(employee.Id);
            if (existing == null) return null;

            existing.FirstName = employee.FirstName;
            existing.LastName = employee.LastName;
            existing.Email = employee.Email;
            existing.Age = employee.Age;

            if (image != null && image.Length > 0)
            {
                DeleteImageFile(existing.ImagePath);
                existing.ImagePath = SaveImageToUploads(image);
            }

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(existing.ImagePath))
                existing.ImagePath = GetBaseUrl() + existing.ImagePath;

            return existing;

        }
    }
	
	
	and EmpController like this 
	
	
	[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EmpController : ControllerBase
{
    private readonly IEmployee _employeeService;
    public EmpController(IEmployee employeeService)
    {
        _employeeService = employeeService;
    }
    [HttpGet]
   
    public async Task<ActionResult<List<Employee>>>  GetAll(int page=1,int pageSize=5)
    {
        
        return Ok(await _employeeService.GetAllEmployeesAsync(page,pageSize));
    }

    [HttpGet("{id}")]
   public async Task<ActionResult<Employee>> GetById(int id)
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
    [Authorize(Roles ="Admin,HR")]
    public async Task<ActionResult<Employee>> Create([FromForm]Employee employee,
        IFormFile? image)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var added = await _employeeService.AddEmployeeAsync(employee,image);
        return Ok(added);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<ActionResult<Employee>> Update(int id,[FromForm]EmployeeUpdateDto  employeeDto
        ,IFormFile? image)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // map dto to entity
        var employee = new Employee
        {
            Id = id, // take from route only
            FirstName = employeeDto.FirstName,
            LastName = employeeDto.LastName,
            Email = employeeDto.Email,
            Age = employeeDto.Age,
            ImagePath = employeeDto.ImagePath
        };

        var updated = await _employeeService.UpdateEmployeeAsync(employee, image);
        if (updated == null)
            return NotFound("Employee not found to update");

        return Ok(updated);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Employee>> Delete(int id)
    {
        var deleted = await _employeeService.DeleteEmployeeAsync(id);
        if (deleted == null)
            return NotFound("Employee not foudn to delete");
        return Ok(deleted);
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("export/excel")]
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

        return File(stream.ToArray(), 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Employees.xlsx");
    }



}


also add these classes in  in DTO folder 

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
      [Range(0, 100, ErrorMessage = "Please enter your age betwen 1 to 100 only ")]
      public int Age { get; set; }

      public string? ImagePath { get; set; }
  }
  
  
    public class EmployeeBasicDto
  {
      public int Id { set; get; }
      public string? FirstName { get; set; }
      public string? LastName { get; set; }
      public string? Email { get; set; }
      public string? ImageUrl { get; set; }

  }
  
  which i am using in EmpController 
  
  
  so considering all this i want a code where in EmployeeService In Employee partial class the values should go from EmployeeDto into Employee class 
   so do change in code of EmployeeService class and where ever in EmpController  and IEmployee any change needed means add it otherwise not needed so 
   so now give me complete code which I need as per requirement 
	-----------------------prompt-------------------------------------------------------------

	and my link of perplexity is like this if your chatgpt is not giving full information u can use mine 

	https://www.perplexity.ai/search/6ee15a24-5b5a-444f-bd35-b555a288cfd4


next do our job now as per prompt 

add DTO folder in the project and add Services folder in the project and 
in 

so DTO classes code is like this 

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

Next IEmployee interface code is like this 

	IEmployee interface
Now modify your interface so it returns DTOs, not Employee entity objects.

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


EmployeeService class
This is the most important part. Here we use the DB First autogenerated Employee entity internally and map 
	between DTO and entity manually. Microsoft’s DTO guidance also shows projecting and shaping data explicitly instead of returning EF entities directly

	Note : i was getting error on EmpContext but here my context is ApplicationDbContext next error was i have to add -->public DbSet<Employee> employees { set; get; } in ApplicationDbContext 
	        then errors have gone ..

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
                ImagePath = image != null && image.Length > 0
                    ? SaveImageToUploads(image)
                    : "/uploads/default.jpg"
            };

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
                existingEmployee.ImagePath = SaveImageToUploads(image);
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
            if (string.IsNullOrEmpty(imagePath) || imagePath.Contains("default.jpg"))
                return;

            var fullPath = Path.Combine(
                _env.WebRootPath,
                imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private string SaveImageToUploads(IFormFile image)
        {
            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fullPath = Path.Combine(uploadPath, imageName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            image.CopyTo(stream);

            return "/uploads/" + imageName;
        }
    }
}

so above code is copied from my code after checking erorrs 

Next EmpController code is like this 

EmpController here u should add empty web api controller with the name EmpController 
Now controller should also use DTOs instead of Employee. This keeps your API contract clean and independent from the DB First entity shape.

	In the below code i was getting error in closedXml so installed a package of verion which u can check in your earlier done proejct which is 0.105.0 

	using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModifiedProject.DTO;
using ModifiedProject.Services;

namespace ModifiedProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<EmployeeDto>> Create([FromForm] EmployeeDto employeeDto, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var added = await _employeeService.AddEmployeeAsync(employeeDto, image);
            return Ok(added);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR")]
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeeDto>> Delete(int id)
        {
            var deleted = await _employeeService.DeleteEmployeeAsync(id);

            if (deleted == null)
                return NotFound("Employee not found to delete");

            return Ok(deleted);
        }

        [HttpGet("export/excel")]
        [Authorize(Roles = "Admin")]
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

so upto this my code is done and till now compile time errors i am not getting now by checking the project which u have done of name WebApiInAsp.netcoreMvcDemo
do some settings in order which i am providing now 

in program.cs add this 

    builder.Services.AddHttpContextAccessor();
 builder.Services.AddScoped<IEmployee, EmployeeService>();

Next prompt is 

	--------------------prompt------------------------

here uploads folder i had created in asp.net core mvc template progam now i am in web api project so how do the code will chnage now for now so
	that when i add throggh web api it shouuld be uplaoded


Yes — in an ASP.NET Core Web API project, file upload still works, but you must make sure wwwroot/uploads exists and static files are enabled so uploaded images can be accessed 
by URL. IWebHostEnvironment.WebRootPath points to the web root, which by default is wwwroot.

So the main change is: in Web API, create wwwroot/uploads inside the API project, enable UseStaticFiles(), and save files into that folder using IFormFile.CopyToAsync

YourWebApiProject
 ┣ Controllers
 ┣ DTO
 ┣ Models
 ┣ Services
 ┣ wwwroot
 ┃  ┗ uploads
 ┣ Program.cs

 so now first add wwwroot folder in the project and in that add uploads as sub folder okay 

 in service class the changes needed 

so i am telling which methods i had modified okay and then full EmployeeService code as a whhole i will keep 


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

Now my final code is like this from my project 

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

so Now my web api is ready let us see how it is working 





