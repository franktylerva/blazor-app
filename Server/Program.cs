using BlazorApp.Models;
using Microsoft.AspNetCore.ResponseCompression;
using BlazorApp;
using Steeltoe.Connector.PostgreSql;
using Steeltoe.Connector.PostgreSql.EFCore;
using Steeltoe.Extensions.Configuration.Kubernetes.ServiceBinding;
using Steeltoe.Management.Endpoint;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddKubernetesServiceBindings();
builder.Services.AddDbContext<BlazorAppContext>((services, options) => options.UseNpgsql(builder.Configuration));
builder.Services.AddPostgresHealthContributor(builder.Configuration);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.AddAllActuators();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

await PostgreSqlSeeder.CreateSampleDataAsync(app.Services);

app.Run();
