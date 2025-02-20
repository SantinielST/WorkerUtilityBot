using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WorkerUtilityBot.Services;

namespace WorkerUtilityBot.Controllers;

class TextMessageController
{
    private readonly ITelegramBotClient _telegramBotClient;
    private NumbersCalculator numbersCalculator;

    public TextMessageController(ITelegramBotClient telegramBotClient, InlineKeyboardController inlineKeyboardController)
    {
        _telegramBotClient = telegramBotClient;
        numbersCalculator = new(_telegramBotClient);
    }

    public async Task Handle(Message message, string opteration, CancellationToken cancellationToken)
    {
        string input = message.Text;

        switch (message.Text)
        {
            case "/start":
                // Объект, представляющий кнопки
                var buttons = new List<InlineKeyboardButton[]>();
                buttons.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData($"Подсчёт символов ", "countSymbols"),
                    InlineKeyboardButton.WithCallbackData($"Сумма чисел", "sumNumbers")
                });

                // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                await _telegramBotClient.SendMessage(message.Chat.Id,
                    $"<b>Наш бот может подсчитывать количество символов в тексте{Environment.NewLine}" +
                    $"или вычислять сумму чисел!</b>{Environment.NewLine}", cancellationToken: cancellationToken,
                    parseMode: ParseMode.Html,
                    replyMarkup: new InlineKeyboardMarkup(buttons));
                break;

            default:
                break;
        }

        if (!string.IsNullOrEmpty(opteration) && opteration == "countSymbols" && message.Text != "/start") // Проверяем выбранную операцию
        {
            // Подсчитываем количество символов в тексте
            await _telegramBotClient.SendMessage(message.Chat.Id, $"Длина сообщения: {message.Text.Length} знаков", cancellationToken: cancellationToken);
        }
        else if (!string.IsNullOrEmpty(opteration) && opteration == "sumNumbers" && message.Text != "/start")  // Проверяем выбранную операцию
        {
            var result = numbersCalculator.CalculateNumbers(message, cancellationToken); // Отправляем в работу сообщение с числами

            await _telegramBotClient.SendMessage(message.Chat.Id, $"Сумма чисел: {result}", cancellationToken: cancellationToken);
        }
    }
}
