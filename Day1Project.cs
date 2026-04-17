
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
