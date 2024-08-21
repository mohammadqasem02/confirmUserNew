using confirmUser;
using confirmUser.HelperRepo;
using confirmUser.repository;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Localization;
using System.Globalization;
using userInformation.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("No connection string was found");

builder.Services.AddTransient<manageAPI>();
builder.Services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailHelperRepo, EmailHelperRepo>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddLocalization();
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        factory.Create(typeof(JsonStringLocalizerFactory));
    });
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCulture = new[]
    {
      new CultureInfo("en-US"),
      new CultureInfo("ar-JO")
    };
    options.DefaultRequestCulture = new RequestCulture(culture: supportedCulture[0], uiCulture: supportedCulture[0]);
    options.SupportedCultures = supportedCulture;
    options.SupportedUICultures = supportedCulture;
});
builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Ensures the cookie is only accessible via HTTP
    options.Cookie.IsEssential = true; // Ensures the session cookie is always sent
});
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(
//        policy =>
//        {
//            policy.AllowAnyOrigin()
//            .AllowAnyMethod()
//            .AllowAnyHeader();
//        });
//});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();
var supportedCultures = new[] { "en-US", "ar-JO" };
var localizationOptions=new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);
app.UseCors();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
