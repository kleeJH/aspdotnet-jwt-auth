using JWTBackendAuth.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Create a static variable to use Configuration anywhere else
// Can be used to access appsettings.json from elsewhere
ConfigurationHelper.Initialize(builder.Configuration);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
ConfigurationHelper.ConfigureAuthenticationServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication(); // Used to enable JWT Authentication

app.UseAuthorization();

app.MapControllers();

app.Run();
