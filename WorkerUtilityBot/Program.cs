using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using Telegram.Bot;
using WorkerUtilityBot.Configuration;

namespace WorkerUtilityBot;

internal class Program
{
    public static async Task Main()
    {
        Console.OutputEncoding = Encoding.Unicode;

        // Объект, отвечающий за постоянный жизненный цикл приложения
        var host = new HostBuilder()
            .ConfigureServices((hostContext, services) => ConfigureServices(services)) // Задаем конфигурацию
            .UseConsoleLifetime() // Позволяет поддерживать приложение активным в консоли
            .Build(); // Собираем

        Console.WriteLine("Сервис запущен");

        await host.RunAsync(); // Запускаем сервис

        Console.WriteLine("Сервис остановлен");
    }

    static void ConfigureServices(IServiceCollection services)
    {
        AppSettings appSettings = BuildAppSettings();
        services.AddSingleton(appSettings);

        // Регистрируем объект TelegramBotClient c токеном подключения
        services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(appSettings.BotToken));

        // Регистрируем постоянно активный сервис бота
        services.AddHostedService<Bot>();
    }

    static AppSettings BuildAppSettings()
    {
        return new AppSettings()
        {
            BotToken = ""
        };
    }
}
