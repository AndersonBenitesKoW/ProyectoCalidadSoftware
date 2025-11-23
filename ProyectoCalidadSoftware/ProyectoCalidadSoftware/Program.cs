//using Microsoft.AspNetCore.Authentication.Cookies;
//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddControllersWithViews();

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services
//    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/UsuarioValidacion/Login";
//        options.AccessDeniedPath = "/Portal/Index";
//    });

//builder.Services.AddAuthorization();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Usuario}/{action=Listar}/{id?}");

//app.Run();
//----------------------
//------------------------
//using Microsoft.AspNetCore.Authentication.Cookies;

//bool isTesting = true;

//builder.Services
//    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Portal/Login";
//        options.AccessDeniedPath = "/Portal/Index";
//    });

//if (isTesting)
//{
//    // en modo pruebas: todo permitido
//    builder.Services.AddAuthorization(options =>
//    {
//        options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
//            .RequireAssertion(_ => true)
//            .Build();
//    });
//}
//else
//{
//    builder.Services.AddAuthorization();
//}
//-------------------------

using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// ?? SIEMPRE registra autenticación
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // aquí pones tu login real
        options.LoginPath = "/Portal/Login";
        options.AccessDeniedPath = "/Portal/Index";
    });

// si quieres, puedes dejar autorización básica
builder.Services.AddAuthorization();

var app = builder.Build();

// pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ?? IMPORTANTE: estos dos SIEMPRE
app.UseAuthentication();
app.UseAuthorization();

// ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Portal}/{action=Index}/{id?}");
  

app.Run();