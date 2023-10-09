- Solution
  - WebApp (ASP.NET Core MVC)
      - Controllers
          -AccountController
      - Views
          -Account
             -Signup
      - wwwroot
          -Images
          -css
          -js
      - ...
  - WebAPI (ASP.NET Core Web API)
      - Controllers
          -AccountApiController
      - ...
  - Core
      - Entities
      - Interfaces
      - DTOs
      - Utilities
          -JsonContent
      - ViewModels
          -LoginModel.cs
      - Services
          - AccountService.cs
  - Infrastructure
      - Data
          - AppDbContext.cs
      - Migrations
      - Repositories
      - Models
          - ApplicationUser : IdentityUser


so this is my Project A application that is made for customers and restaurants this is made for easy ordering for the customer to scan a QR code and place a Order so how would that work (just the web app) we need to register shops and customers both 

Add-Migration First_1 -Context MasterDbContext
update-database -Context MasterDbContext
Add-Migration First_1 -Context DynamicDbContext
update-database -Context AppDbContext

