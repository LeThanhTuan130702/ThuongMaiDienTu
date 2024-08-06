using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.Helper;
using ThuongMaiDienTu.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Hshop2023Context>(op=>op.UseSqlServer(builder.Configuration.GetConnectionString("Hshop")));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.LoginPath = "/KhachHang/Login";
    option.AccessDeniedPath = "/AccessDenied";
});
builder.Services.AddSingleton(x => new PaypalClient(
    builder.Configuration["PayPal:AppId"],
    builder.Configuration["PayPal:AppSecret"],
    builder.Configuration["PayPal:Mode"]
    ));
builder.Services.AddSingleton<IVnPayService,VnPayService>();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(5);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
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

app.UseRouting();
app.UseSession();
app.UseAuthorization();
//app.UseAuthentication();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
