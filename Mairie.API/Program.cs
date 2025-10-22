using Mairie.API.Helpers;
using Mairie.API.Infrastructure.Security;
using Mairie.DAL.Configuration;
using Mairie.DAL.Services;
using Mairie.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using OwaspHeaders.Core.Extensions;
using Serilog; 
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
//Configurer les logs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() //Le minimum global
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error) //Définir le minimum pour une source particulière
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Error)
    //Enrichissement
    .Enrich.FromLogContext() //Donne des infos complémentaires pour le log
    .Enrich.WithProperty("Application", "Mairie.API") //Ajoute l'info Application dans les logs
                                                      //Destination
    .WriteTo.File(
                //path vers le fichier de log
                path: @"Logs\MairieApi-.log",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}[{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 40 * 1024 * 1024)
    .CreateLogger();
       
    //"Remplacer" le log par défaut par notre serilog
    builder.Host.UseSerilog();
    

    




//Configuration l'"authentification" Windows
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
//Ajout de mon handler pour l'authorisation par requirement
builder.Services.AddScoped<IAuthorizationHandler, DemandeRequirementHandler>();
//Configuration politique d'autorisation
builder.Services.AddAuthorization(
    options =>
    {
        options.AddPolicy("AgentPolicy", p => p.RequireClaim(ClaimTypes.Role, "Agent"));
        options.AddPolicy("ChefServicePolicy", p => p.RequireClaim(ClaimTypes.Role, "ChefService"));
        options.AddPolicy("AdministrateurPolicy", p => p.RequireClaim(ClaimTypes.Role, "Administrateur"));
        options.AddPolicy("AdminOrAgentPolicy", p => p.RequireClaim(ClaimTypes.Role, "Administrateur", "Agent"));
        options.AddPolicy("HasARolePolicy", p => p.RequireClaim(ClaimTypes.Role, "Administrateur", "ChefService", "Agent"));
        //Ajout d'une policy custom
        
        options.AddPolicy("DepartementMember", p => p.RequireClaim("https://www.mairie.fr/claims/Departement", "EtatCivil"));

        //requirements
        options.AddPolicy("DemandeOwner", p =>
                    p.AddRequirements(new DemandeOwnerRequirement("DemandeOwner")));
        



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

//La politique cross origin
builder.Services.AddCors(
    options=>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("https://localhost:7210") // Port de notre Blazor App
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // IMPORTANT pour Windows Auth
        });
    });




var app = builder.Build();
app.UseCors("BlazorCors");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
if(!app.Environment.IsDevelopment())
{
    //FORCER HTTPS
    app.UseHsts();
}
app.UseSecureHeadersMiddleware();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
