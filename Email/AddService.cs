using Microsoft.Extensions.DependencyInjection;

namespace Email;

public static class AddService
{
    public static IServiceCollection AddEmailService(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}
