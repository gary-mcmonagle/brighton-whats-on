using Microsoft.Extensions.DependencyInjection;

namespace Email;

public static class AddService
{
    public static IServiceCollection AddEmailService(this IServiceCollection services, string rootUrl)
    {
        services.AddScoped<IEmailService>(new EmailService());
        return services;
    }
}
