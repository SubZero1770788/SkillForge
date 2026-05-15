using quiz_project.Database;
using quiz_project.Hubs;
using quiz_project.Infrastructure;

var builder = WebApplication.CreateBuilder();

// Adding all services through ServiceConfigurer helper class
var finishedBuilder = ServiceConfigurer.Configure(builder);
var app = finishedBuilder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<PresenceHub>("/presencehub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Adding fallback to routing table
app.MapControllerRoute(
    name: "fallback",
    pattern: "{*Home}",
    defaults: new { controller = "User", action = "Register" });

// Seeding database if data is not available
using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<QuizDb>();
    DbSeeder.Initialize(scope, context);
}
app.Run();
