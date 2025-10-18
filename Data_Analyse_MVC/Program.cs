using Data_Analyse_MVC.Models;
using Data_Analyse_MVC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IFileAnalyseService, FileAnalyseService>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
    name: "uploads",
    pattern: "File/uploads",
    defaults: new { controller = "File", action = "uploads" });



app.MapControllerRoute(
    name: "ManageFiles",
    pattern: "File/ManageFiles",
    defaults: new { controller = "File", action = "ManageFiles" });

app.MapControllerRoute(
    name: "AnalyseResult",
    pattern: "File/AnalyseResult",
    defaults: new { controller = "File", action = "AnalyseResult" });


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
