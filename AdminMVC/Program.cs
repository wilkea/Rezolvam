using Microsoft.AspNetCore.Identity;
using rezolvam.Application;
using rezolvam.Domain.Common;
using rezolvam.Infrastructure;
using Rezolvam.Infrastructure.Mapping;
using AutoMapper;
using rezolvam.Application.Interfaces;
using rezolvam.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(typeof(ReportMappingProfile).Assembly);
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();


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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
