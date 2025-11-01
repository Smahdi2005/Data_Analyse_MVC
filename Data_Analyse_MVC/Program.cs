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
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
    name: "Upload",
    pattern: "Upload",
    defaults: new { controller = "Upload", action = "Index" }
);

// ============================
// ✅ روت‌های FileController
app.MapControllerRoute(
    name: "StartAnalyse",
    pattern: "File/StartAnalyse/{fileId?}",
    defaults: new { controller = "File", action = "StartAnalyse" }
);

app.MapControllerRoute(
    name: "UserFiles",
    pattern: "File/UserFiles",
    defaults: new { controller = "File", action = "UserFiles" }
);

// ============================
// ✅ روت‌های AnalyseResultController
app.MapControllerRoute(
    name: "AnalyseResult",
    pattern: "AnalyseResult/LoadPartial/{analyseResultId?}",
    defaults: new { controller = "AnalyseResult", action = "LoadAnalyseResultPartial" }
);

app.MapControllerRoute(
    name: "AnalyseResultFull",
    pattern: "AnalyseResult/View/{analyseResultId?}",
    defaults: new { controller = "AnalyseResult", action = "AnalyseResult" }
);

// ============================
// ✅ روت‌های AdminController
app.MapControllerRoute(
    name: "Admin",
    pattern: "Admin/ManageFiles",
    defaults: new { controller = "Admin", action = "ManageFiles" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
