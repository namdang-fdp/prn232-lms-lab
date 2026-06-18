using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PRN232.LMS.API.Common.Middleware;
using PRN232.LMS.API.Common.Response;
using PRN232.LMS.API.Mapping;
using PRN232.LMS.API.Swagger;
using PRN232.LMS.API.Validators;
using PRN232.LMS.Services.Extensions;
using PRN232.LMS.Services.Mapping;
using PRN232.LMS.Services.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    {
        options.ReturnHttpNotAcceptable = true;
    })
    .AddXmlDataContractSerializerFormatters()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(item => item.Value?.Errors.Count > 0)
            .ToDictionary(
                item => item.Key,
                item => item.Value!.Errors.First().ErrorMessage);

        return new BadRequestObjectResult(new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed.",
            Data = null,
            Errors = errors
        });
    };
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentRequestValidator>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? new JwtOptions();
var jwtSecretBytes = Encoding.UTF8.GetBytes(jwtOptions.Secret);

if (jwtSecretBytes.Length < 32)
{
    throw new InvalidOperationException("JWT secret must be configured and at least 32 bytes long.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtSecretBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Unauthorized.",
                        Data = null,
                        Errors = null
                    });
                }
            },
            OnForbidden = async context =>
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Forbidden.",
                        Data = null,
                        Errors = null
                    });
                }
            }
        };
    });

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=prn232_lms;Username=postgres;Password=postgres";

builder.Services.AddLmsApplication(connectionString);
builder.Services.AddAutoMapper(
    _ => { },
    typeof(ApiMappingProfile).Assembly,
    typeof(RepositoryMappingProfile).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "PRN232 LMS API", Version = "v1" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "PRN232 LMS API", Version = "v2" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter a JWT access token. Swagger sends it as: Bearer {token}."
    });
    options.OperationFilter<AuthorizeCheckOperationFilter>();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.Services.InitializeLmsDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PRN232 LMS API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "PRN232 LMS API v2");
    });
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
