using MuniAnnouncements.Api.Repositories;
using MuniAnnouncements.Api.Services;

public class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		ConfigureServices(builder.Services);

		WebApplication app = builder.Build();
		app.UseCors();
		app.MapControllers();
		app.Run();
	}

	private static void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddCors(options => options.AddDefaultPolicy(policy =>
			policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
		services.AddScoped<ICmsAnnouncementClient, HardcodedCmsAnnouncementClient>();
		services.AddSingleton<IAuthoritySiteSettingsContext, HardcodedAuthoritySiteSettingsContext>();
		services.AddScoped<IAuthoritySiteSettingsRepository, AuthoritySiteSettingsRepository>();
		services.AddScoped<IAnnouncementService, AnnouncementService>();
	}
}