# ZPP-API
Simple ASP.NET Core WebApi. 

## SPA client
- [ZPP-Blazor](https://github.com/dejvids/zpp-blazor)
- [ZPP-Angular](https://github.com/dejvids/zpp-angular)

## Features
- SQL Server
- JWT Authentication
- Sign in By Google and Facebook
- Integration with Swagger UI

## Configuration
Required configuration can be placed in appsettings.json configuration file.

|Section          |Description|
|-----------------|-----------|
|ConnectionStrings|privides connection string SQL Server
|jwt              | JWT parameters such as HMAC secret and toke expiration time
|lectures         | Describes how to load and presents data in client application
|users            | Allow to set number of user accounts loaded to admin panel
|Authentication   | Configuration of Google and Facebook Sign in

## Instalation
You need to have running SQL Server locally or via Docker.
Applicaiton uses Entity Framework Core for comunication with DB.

Before the first run apply the migration of databse. Command `Update-Database` in Visual Studio Package Manager or `dotnet ef database update` if you use dotnet CLI. 

Run the project using Visual Studio or dotnet cli typing `dotnet run`. App should start by default on port 5000.

Navigates to `hhtp://localhost:5000/swagger` to explore the API.