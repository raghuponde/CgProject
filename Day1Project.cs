CODE FIRST APPROACH
*********************
Refer Day 70 folder for this demos 
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
  }

 
}

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

Step 6 : Register in Program .cs 

     
            builder.Services.AddDbContext<ApplicationDbContext>
(options => options.UseSqlServer(builder.Configuration.GetConnectionString("constring")));

no need to configure json etc will do later now i want your tables in project and also identity 8 tables with the project tables 


step 7: add migration and update database 
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
DBFIRST APPROACH 
********************
Now what they are asking is you have to do the above task using db first apprach of entity framework so here they will give u script file means sql server tables they will give you 
that .sql script file you have to run in your local database of sql server and then in web api you have to write one command to do reverse engineering of those tables 
Now another question is that the script file which is given to you whether it will contain identity tables or not mostly identity tables wont be there we have to generate it from our end 


step 1 : create and open asp.net core web api project and give some name like ResortAPI2

step 2: Add the following packages in the project of version all 8.0.24

Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Identity.EntityFrameworkCore  
Microsoft.AspNetCore.Authentication.JwtBearer     

Go to sql server create new  database resortdb2 and add one sample table create table hello (id int primary key ) and i am doing this because they have told they will give script file so 
as per script file gvining db name and also one dummy table for chekcing 

so add these packages again 

--->then in package manager console fire this command 

I think u have to create folder here for Models before firing this command as web api project will not have Models folder into it as in built 

Scaffold-DbContext 'Data Source=LAPTOP-4G8BHPK9\SQLEXPRESS;initial catalog=resortdb2;Integrated Security=true;TrustServerCertificate=True;' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context ApplicationDbContext -Force

see now i had not brought the line down okay sigle line u have to paste and then only hello table and ApplicationDbContext will be created 


change above as per your server and as per your db okay 

so in models all classes will be generated 

Next Identity tables are needed along with the Hello table so 

my class was looking liek this 

 using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ResortAPI2.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Hello> Hellos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer
        ("Data Source=LAPTOP-4G8BHPK9\\SQLEXPRESS;initial catalog=resortdb2;Integrated Security=true;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hello>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__hello__3213E83F635C32FC");

            entity.ToTable("hello");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
        });



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}


and then do like this as this class is partial class so u cannot use do like this overide this code in this file 

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResortAPI2.Models;

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
            new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1"
            },
            new IdentityRole
            {
                Id = "2",
                Name = "Customer",
                NormalizedName = "CUSTOMER",
                ConcurrencyStamp = "2"
            }
        );
    }
}

and also app setting set it 

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",

  "ConnectionStrings": {
    "DefaultConnection": "Data Source=LAPTOP-4G8BHPK9\\SQLEXPRESS;Initial Catalog=resortdb2;Integrated Security=True;TrustServerCertificate=True;"
  }
}


and in program.cs also set it 

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


and finally run the migrations it will work so this is the apprach for DBFirst approach okay ..
---------------------------------------------------------------------------------------------------------------------------------------------------------
 Now the Next thing is for some project like 
 When IdentityUser is also a table in your database (e.g., when you want to store users along with other domain entities like employees, posts, etc.),
so what  i mean to say is that when u are registering with Authenticcaton contoller then register data will go in Asp.netUsers table of identity and now i am saying that the same table 
user is also a user in your database who's duty is just not to register and login activity he will have its own columns also which may not be there in Asp.netusers table and at that time what 
 u will do is you will extend Identity user and these values will go in Asp.netUsers table along with 

 FullName and ImageUrl like that so extra columsn may be added in Asp.netuser table now i am taking one project which i have given as 

  and for that project i will do the same task which i had done for Resort project okay 

  public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public string? ImageUrl { get; set; }  // Optional: for profile pictures
}

here I can give any name ApplicationUser or User or anything but it shoudl be meaningful Now again 
it is your choice in the project  u want to maintain seperate user means no need to extend Identity user okay 

so for this projecct i am going with DFirst Approach now DBfirst wont work for this scenaario okay so tables which are not related to User those you can scafffold it no issue 
and for other which is acting as User and that is not related in other tables that much scaffold it okay from the script file which they give okay 

so from the document i am taking classes and i am asking chatgpt to give me sql script file for this to run in sprint they will only will give script file 
but now i am not having so i am doing like this 


so i had removed User as it will come from identity okay 

public class Course
{
public int CourseID { get; set; } 
 public string CourseName { get; set; } 
 public string Description { get; set; } 
 public string Duration { get; set; }
 public int Cost { get; set; }
}

public class Enquiry
{
public int EnquiryID { get; set; }
public DateTime EnquiryDate { get; set; } 
 public string userId { get; set; }
public string Title { get; set; }
 public string Description { get; set; }
 public string EmailID { get; set; }
public string EnquiryType { get; set; }
public string Status { get; set; }
}

public class Payment
{
public int PaymentID {get; set;} 
 public int CourseId { get; set; } 
 public string UserName { get; set; }
 public int TotalAmount {get; set;}
public string Status { get; set; }
public DateTime PaymentDate {get; set;}
public string ModeOfPayment {get; set;}
}


now open visual studio of web api with the name BEC and add Models folder into it 

and add following packages etc 


Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Identity.EntityFrameworkCore  
Microsoft.AspNetCore.Authentication.JwtBearer     


--->then in package manager console fire this command 


Scaffold-DbContext 'Data Source=LAPTOP-4G8BHPK9\SQLEXPRESS;initial catalog=BEC3;Integrated Security=true;TrustServerCertificate=True;' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context ApplicationDbContext -Force

see now i had not brought the line down okay sigle line u have to paste and then only hello table and ApplicationDbContext will be created so overwrite the class with this as 
 partial class wnt work for creating identity tables but instead of IdentityUser u have to say User 

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
public class ApplicationDbContext : IdentityDbContext<User>
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
            new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1"
            },
            new IdentityRole
            {
                Id = "2",
                Name = "Customer",
                NormalizedName = "STUDENT",
                ConcurrencyStamp = "2"
            }
        );
    }
}
so was gettign some error so 

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BEC.Models;

public  class User:IdentityUser
{
   

    public string UserRole { get; set; } = null!;

    public virtual ICollection<Enquiry> Enquiries { get; set; } = new List<Enquiry>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

so this column userrole u can see in Asp.netUser Table okay .

 
and now no errors as User is inheritng 

so my class is now like this 

 using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BEC.Models;

namespace BEC.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext <User>
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
            new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1"
            },
            new IdentityRole
            {
                Id = "2",
                Name = "Customer",
                NormalizedName = "STUDENT",
                ConcurrencyStamp = "2"
            }
        );
    }
}


finall now do app setting also 

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=LAPTOP-4G8BHPK9\\SQLEXPRESS;Initial Catalog=BEC;Integrated Security=True;TrustServerCertificate=True;"
  }
}

and progam.cs also 

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

and finally run the migrations now 

but the tables are not generated identtity so now what i will do is i will remove partial for all classes now and will run the migration okay 

so refer Day70 BEC project for this okay 

