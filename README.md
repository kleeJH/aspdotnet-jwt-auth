## ASP.NET CORE JWT Authentication
This is an example of a simple JWT authentication with the refresh token feature in ASP.NET Web API. It mostly uses ASP.NET Core Identity to help with the user management. ASP.NET Core is responsible for authentication (JWT expiry management), setting up routes and controllers. It also connects to a MySql database to store user's information and the refresh token information. Security for the user's credentials are done by ASP.NET Core Identity which will automatically generate a salt, hashes the user's password, and then stores the generated salt and password hash into the database.

### Changing the JWT Key, Issuer and Audience
In appsettings.json, change the appropriate properties under the `JWT` property to what you like. Also note, if the key length is too short, the code will produce an error.

### Configuring the database connection strings
In appsettings.json, change the `MySqlDatabase` property to your MySql DB correct initials.

### Changing the database
In ConfigurationHelper.cs, in IServiceCollection.AddDbContext, it is currently using MySql by taking the connecting strings from appsettings.json. If you would like to change the database for it, you need to change the code `options.UseMySql` .

### Notes on Database Migration
- Open the dotnet CLI (Packet Manager Console) and type the bottom few commands
  - Remove Migration: `Remove-Migration` (if you already have the migration files)
  - Add Migration: `Add-Migration <ClassName>`
  - Update Database: `Update-Database`

- Also note, once you already did `Update-Database`, you won't be able to `Remove-Migration` the current version.