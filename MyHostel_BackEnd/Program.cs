using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyHostel_BackEnd.Models;
using MyHostel_BackEnd.Quartz;
using Quartz;
using System.Text;
using System.Text.Json.Serialization;
using MyHostel_BackEnd.ChatHubController;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressConsumesConstraintForFormFileParameters = true;
    options.SuppressInferBindingSourcesForParameters = true;
    options.SuppressModelStateInvalidFilter = true;
    options.SuppressMapClientErrors = true;
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
    var jobKey_QuartzJob = new JobKey("jobKey_QuartzJob");
    q.AddJob<QuartzJob>(opts => opts.WithIdentity(jobKey_QuartzJob));
    q.AddTrigger(opts => opts
        .ForJob(jobKey_QuartzJob)
        .WithIdentity("Job_trigger_QuartzJob")
        .WithCronSchedule("0 0 */12 * * ?"));

    var jobKey_CreateNotificationJob = new JobKey("jobKey_CreateNotificationJob");
    q.AddJob<CreateNotificationJob>(opts => opts.WithIdentity(jobKey_CreateNotificationJob));
    q.AddTrigger(opts => opts
        .ForJob(jobKey_CreateNotificationJob)
        .WithIdentity("Job_trigger_CreateNotificationJob")
        .WithCronSchedule("0 0 0 * * ?"));

    var jobKey_SendAtHourJob = new JobKey("jobKey_SendAtHourJob");
    q.AddJob<SendAtHourJob>(opts => opts.WithIdentity(jobKey_SendAtHourJob));
    q.AddTrigger(opts => opts
        .ForJob(jobKey_SendAtHourJob)
        .WithIdentity("Job_trigger_SendAtHourJob")
        .WithCronSchedule("* * */1 * * ?"));
});

string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
string sFile = System.IO.Path.Combine(sCurrentDirectory, @"..\..\..\GoogleCredential\googlekey.json");
string sFilePath = Path.GetFullPath(sFile);
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(sFilePath),
});
builder.Services.AddSignalR().AddJsonProtocol();
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddCors();
builder.Services.AddDbContext<MyHostelContext>(opt => opt.UseSqlServer(
            builder.Configuration.GetConnectionString("MyDB")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});
var app = builder.Build();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/ws");
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                );


app.Run();
