using AlphaPortal.Hubs;
using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => !context.Request.Cookies.ContainsKey("cookieConsent");
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));
builder.Services.AddIdentity<UserEntity, IdentityRole>(x =>
{
    x.User.RequireUniqueEmail = true;
    x.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(x =>
{
    x.LoginPath = "/auth/login";
    x.AccessDeniedPath = "/auth/adminlogin";
    x.Cookie.HttpOnly = true;
    x.Cookie.IsEssential = true;
    x.SlidingExpiration = true;
    x.Cookie.SameSite = SameSiteMode.None;
    x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
});

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<INotificationService, NotificationService>();


var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Administrator", "User" };

    foreach(var roleName in roleNames)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if(!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

    var user = new UserEntity { UserName = "admin@domain.com", Email = "admin@domain.com", FirstName="Admin", LastName="Admin" };

    var userExists = await userManager.Users.AnyAsync(x => x.Email == user.Email);
    if (!userExists)
    {
        var result = await userManager.CreateAsync(user, "BytMig123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, "Administrator");
    }
}
    


app.MapStaticAssets();
app.UseRewriter(new RewriteOptions().AddRedirect("^$", "/alpha/overview"));
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Overview}/{action=Index}/{id?}")
    .WithStaticAssets();


//Tog hjälp utav ChatGPT för att fylla ut statustabellen, var tvungen att använda mig utav SET IDENTITY_Insert Statuses ON och OFF.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Statuses.Any())
   {
       using var transaction = context.Database.BeginTransaction();

       context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Statuses ON");

       context.Statuses.AddRange(new List<StatusEntity>
       {
           new StatusEntity { Id = 1, StatusName = "ONGOING" },
           new StatusEntity { Id = 2, StatusName = "COMPLETED" },
           new StatusEntity { Id = 3, StatusName = "NOT STARTED" }
       });

       context.SaveChanges();

       context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Statuses OFF");

       transaction.Commit();
   }
}



app.MapHub<NotificationHub>("/notificationHub");


app.Run();
