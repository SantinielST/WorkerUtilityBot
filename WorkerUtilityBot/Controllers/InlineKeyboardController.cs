using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WorkerUtilityBot.Controllers;

class InlineKeyboardController
{
    private readonly ITelegramBotClient _telegramBotClient;

    public string Operation { get; protected set; }

    public InlineKeyboardController(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public async Task Handle(CallbackQuery? callbackQuery, CancellationToken cancellationToken)
    {
        if (callbackQuery?.Data == null)
        {
            return;
        }

        // Обновление пользовательской сессии новыми данными
        string operationText = callbackQuery.Data switch
        {
            "countSymbols" => "Подсчёт символов ",
            "sumNumbers" => "Сумма чисел ",
            _ => string.Empty
        };

        // Отправляем в ответ уведомление о выборе
        await _telegramBotClient.SendMessage(callbackQuery.From.Id,
            $"<b>Выбранная операция - {operationText}.{Environment.NewLine}</b>" +
            $"{Environment.NewLine}Можно поменять в главном меню.", cancellationToken: cancellationToken,
            parseMode: ParseMode.Html);

        Operation = callbackQuery.Data;

        Console.WriteLine($"Контроллер {GetType().Name} получил сообщение {callbackQuery.From.Username}");
    }
}
