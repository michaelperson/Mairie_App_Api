using Mairie.API.Helpers;
using Mairie.API.Infrastructure.Security;
using Mairie.DAL.Configuration;
using Mairie.DAL.Services;
using Mairie.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Configuration l'"authentification" Windows
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

//Configuration politique d'autorisation
builder.Services.AddAuthorization(
    options =>
    {
        options.AddPolicy("AgentPolicy", p => p.RequireClaim(ClaimTypes.Role, "Agent"));
        options.AddPolicy("ChefServicePolicy", p => p.RequireClaim(ClaimTypes.Role, "ChefService"));
        options.AddPolicy("AdministrateurPolicy", p => p.RequireClaim(ClaimTypes.Role, "Administrateur"));
        options.AddPolicy("AdminOrAgentPolicy", p => p.RequireClaim(ClaimTypes.Role, "Administrateur", "Agent"));
    }
    );

//Récupération de la connection string
#pragma warning disable CA1416 // Validate platform compatibility
string cnstr = SecretManager.Decrypt(
      builder.Configuration["ConnectionStrings:DefaultConnection"]??
      builder.Configuration.GetConnectionString("DefaultConnection")?? 
      throw new InvalidProgramException("No Connection string provided"),
      System.Security.Cryptography.DataProtectionScope.LocalMachine);
#pragma warning restore CA1416 // Validate platform compatibility

// Add services to the container.
// Enregistrer la configuration de la db en transmettant notre connexion
builder.Services.AddSingleton(new DatabaseConfiguration(cnstr));
//Enregistrer les repositories
builder.Services.AddScoped<IDemandeRepository, DemandeRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();

//Enregistrer le service qui se charge de récupérer les informations de l'utilisateur
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<IClaimsTransformation, RoleClaimsTransformation>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "API Mairie",
            Description = "API interne de la Mairie"
        });
        c.EnableAnnotations();
        c.SchemaFilter<EnumSchemaFilter>();
    }
   

    
    );
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
