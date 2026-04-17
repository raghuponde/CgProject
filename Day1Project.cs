
step 1 : create and open asp.net core web api project and give some name like ResortAPI 

step 2: Add the following packages in the project 

Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Identity.EntityFrameworkCore    of version 8.0.24
Microsoft.AspNetCore.Authentication.JwtBearer     of version 8.0.24


step 3 : go to app settings and set the sql settings like this for the project after putting , add this 

 "AllowedHosts": "*",
 "ConnectionStrings": {
   "constring": "Data Source=LAPTOP-4G8BHPK9\\SQLEXPRESS;initial catalog=resortdb;Integrated Security=true;Encrypt=true;TrustServerCertificate=true;"
 }
means your project will look like this app settings file 

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
  }
}

add this also by putting ,


  "JWT": {
    "ValidAudience": "https://localhost:7230",
    "ValidIssuer": "https://localhost:7230",
    "Secret": "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyrssjkdajad122"
  }

so finally it will look like this 

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
give your url of web api here issuer and audience is same becasue we have not yet develped the front end 

step 4 : 
--------
 add  Models folder and what and all classes are given in project add those in Models fodler except AppDbContext class which has be added in Data 
 fodler 
 
so as per the project i had added the following classes 

using System.ComponentModel.DataAnnotations;

namespace ResortAPI.Models
{
    public class User
    {

        [Key]
        public long? UserId { get; set; }//n

        public string Email { get; set; }

        public string Password { get; set; }

        public string? Username { get; set; }

        public string? MobileNumber { get; set; }

        public string? UserRole { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResortAPI.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public long UserId { get; set; }
        public string Subject { get; set; }

        public string Body { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime DateCreated { get; set; }
        //   [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ResortAPI.Models
{
    public class Resort
    {
        [Key]
        public long ResortId { get; set; }

        public string ResortName { get; set; }

        public string ResortImageUrl { get; set; }

        public string ResortLocation { get; set; }

        public string ResortAvailableStatus { get; set; }

        public long Price { get; set; }

        public int Capacity { get; set; }

        public string Description { get; set; }
        // [JsonIgnore]
        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResortAPI.Models
{
    public class Booking
    {
        [Key]
        public long? BookingId { get; set; }

        public int NoOfPersons { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Status { get; set; }

        public double TotalPrice { get; set; }

        public string Address { get; set; }

        // Foreign key for the Many-to-One relationship with User
        public long? UserId { get; set; } // Nullable foreign key
                                          // [JsonIgnore]

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; } // Nullable navigation property
                                                // [JsonIgnore]

        public long? ResortId { get; set; } // Nullable foreign key
                                            // [JsonIgnore]
        [ForeignKey(nameof(ResortId))]
        public virtual Resort? Resort { get; set; } // Nullable navigation property
    }
}



step 5: Creaate a Data folder and add a class ApplicationDbContext class and add the following code into it 


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResortAPI.Models;

namespace ResortAPI.Data
{
  

        public class ApplicationDbContext : IdentityDbContext<IdentityUser>
        {
            public ApplicationDbContext(DbContextOptions dbContextOptions) :
                base(dbContextOptions)
            {

            }

        public DbSet<User> Users { get; set; }
        public DbSet<Resort> Resorts { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {

                base.OnModelCreating(modelBuilder);
                SeedRoles(modelBuilder);

            }
            private static void SeedRoles(ModelBuilder builder)
            {
                builder.Entity<IdentityRole>().HasData
                (
                new IdentityRole()
                {
                    Name = "Admin",
                    ConcurrencyStamp = "1",
                    NormalizedName = "Admin"
                },
                new IdentityRole()
                {
                    Name = "Customer",
                    ConcurrencyStamp = "2",
                    NormalizedName = "User"
                }
               
                );
            }


        }
    
}


  
