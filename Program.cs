using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using personal.api.Data;
using personal.api.Mappings;
using personal.api.Models;
using personal.api.Repositories.Implementation;
using personal.api.Repositories.Interface;

var builder = WebApplication.CreateBuilder(args);

// Activar licencia gratuita
QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });

    // Autenticación con JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Token JWT. Ejemplo: 'Bearer {tu_token_aquí}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<PersonalContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SeguridadConnection"));
});

builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IOrganizacionRepository, OrganizacionRepository>();
/*
builder.Services.AddScoped<IAspNetUsersRepository, AspNetUsersRepository>();

builder.Services.AddScoped<IEstadoRepository, EstadoRepository>();
builder.Services.AddScoped<ITipoEnfermeraRepository, TipoEnfermeraRepository>();
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IContactoRepository, ContactoRepository>();
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
builder.Services.AddScoped<IBancoRepository, BancoRepository>();
builder.Services.AddScoped<IColaboradorDocumentoRepository, ColaboradorDocumentoRepository>();
builder.Services.AddScoped<ITipoLugarRepository, TipoLugarRepository>();
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
builder.Services.AddScoped<IHorarioRepository, HorarioRepository>();
builder.Services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
builder.Services.AddScoped<IServicioFechaRepository, ServicioFechaRepository>();
builder.Services.AddScoped<IServicioFechasOfertaRepository, ServicioFechasOfertaRepository>();
builder.Services.AddScoped<IEncuestaPlantillaRepository, EncuestaPlantillaRepository>();
builder.Services.AddScoped<IEncuestaPlantillaPreguntaRepository, EncuestaPlantillaPreguntaRepository>();
builder.Services.AddScoped<IPagoLoteRepository, PagoLoteRepository>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IMunicipioRepository, MunicipioRepository>();
builder.Services.AddScoped<IMensajeRepository, MensajeRepository>();
builder.Services.AddScoped<IAvisoRepository, AvisoRepository>();
*/
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            AuthenticationType = "Jwt",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = "roles"   // ?? IMPORTANTE
        };
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
            "https://localhost",
            "https://localhost:8100",
            "ionic://localhost",
            "capacitor://localhost",
            "https://quatro0-001-site2.ktempurl.com",
            "https://quatro0-001-site3.ktempurl.com",
            "https://quatro0-001-site4.ktempurl.com",
            "https://quatro0-001-site5.ktempurl.com", // <-- Agrega este
            "https://quatro0-001-site6.ktempurl.com",
            "http://quatro0-001-site6.ktempurl.com",
            "http://localhost:51293",
            "http://localhost:8100",
            "http://localhost:4200") // Origen permitido
              .AllowAnyHeader() // Permitir cualquier encabezado
              .AllowAnyMethod(); // Permitir cualquier método HTTP
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (
    app.Environment.IsDevelopment()
    //|| app.Environment.IsProduction()
    )
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();