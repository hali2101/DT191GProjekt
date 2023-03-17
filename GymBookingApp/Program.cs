using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GymBookingApp.Data;
using System;
using Microsoft.AspNetCore.Authorization;
using static System.Formats.Asn1.AsnWriter;
using AuthTest.Data;
using Microsoft.Extensions.Options;
using Microsoft.CodeAnalysis.Options;

var builder = WebApplication.CreateBuilder(args);

//lägger till DB-connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDbString")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

//seedar för att skapa nya roller
using (var scope = app.Services.CreateScope())
{
    //deklarerar rolemanager
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    //skapar lista med olika roller
    var roles = new[] { "Admin", "Member" };

    //loopar igenom listan med olika roller
    foreach (var role in roles)
    {
        //kontroll om rollen finns
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

app.Run();

