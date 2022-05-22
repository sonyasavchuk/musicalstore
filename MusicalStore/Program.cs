using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MusicalStore;
using MusicalStore.Models.Entities.Identity;
using MusicalStore.Options;
using MusicalStore.Services;
using MusicalStore.Services.ExternalData;
using MusicalStore.Services.Identity;
using MusicalStore.Services.Identity.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<MailOptions>().BindConfiguration("MailOptions");

var authConnectionString = builder.Configuration.GetConnectionString("AuthConnection");
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(authConnectionString));

var dataConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MusicalStoreContext>(options =>
    options.UseSqlServer(dataConnectionString));

builder.Services.AddScoped<MaterialsService>();

builder.Services.AddScoped<DataTransformationService>();

builder.Services.AddScoped<ExcelDataService>();
builder.Services.AddScoped<DocxDataService>();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.Admin, policyBuilder => policyBuilder
        .RequireAuthenticatedUser()
        .RequireRole(RoleNames.Admin)
        .Build());
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Identity/Account/Login");
    options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
});

builder.Services.AddRazorPages(options => {
    options.Conventions.AuthorizeFolder("/charts", PolicyNames.Admin);
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Instruments}/{action=Index}/{id?}");
app.MapRazorPages();

await app.EnsureIdentityInitialized();

await app.RunAsync();
