using BusinessLogic.Repository;

namespace OnlineChat;

public static class AppExtensions
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IAdministratorsRepository, AdministratorsRepository>()
            .AddScoped<IChatroomRepository, ChatroomRepository>()
            .AddScoped<IChatroomTicketRepository, ChatroomTicketRepository>()
            .AddScoped<IMessageRepository, MessageRepository>()
            .AddScoped<IUserAdministratorRepository, UserAdministratorRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IUserTokenRepository, UserTokenRepository>();
        return serviceCollection;
    }
}

public static class Utils
{
    public static Type[] GetAutoMapperProfiles()
    {
        return Array.Empty<Type>();
    }
}