var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
    name: "default",
<<<<<<< HEAD
    pattern: "{controller=SeguimientoPuerperio}/{action=Listar}/{id?}");
=======
    pattern: "{controller=Embarazo}/{action=Index}/{id?}");
>>>>>>> b447e571425647127be49d6d033b0459a559055b

app.Run();
