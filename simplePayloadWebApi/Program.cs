using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml; 
using simplePayloadWebApi.Data;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<simplePayloadContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("simplePayloadContext")
    ?? throw new InvalidOperationException("Connection string 'simplePayloadContext' not found.")));

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
