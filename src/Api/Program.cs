using Api;
using Api.Hubs;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.AddAuthentication
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();
// Initialise and seed database
using (var scope = app.Services.CreateScope())
{
    //var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    //await initialiser.InitialiseAsync();
    //await initialiser.SeedAsync();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chatHub");
});

app.Run();

public partial class Program
{
}