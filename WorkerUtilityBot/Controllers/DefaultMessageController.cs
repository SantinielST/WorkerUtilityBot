using Telegram.Bot;
using Telegram.Bot.Types;

namespace WorkerUtilityBot.Controllers;

class DefaultMessageController
{
    private readonly ITelegramBotClient _telegramBotClient;

    public DefaultMessageController(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public async Task Handle(Message message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Контроллер {GetType().Name} получил сообщение");
        await _telegramBotClient.SendMessage(message.Chat.Id, $"Получено сообщение не поддерживаемого формата", cancellationToken: cancellationToken);
    }
}
