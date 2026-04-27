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


