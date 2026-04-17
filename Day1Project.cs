
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
 


  
