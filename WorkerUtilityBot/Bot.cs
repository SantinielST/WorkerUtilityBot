using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WorkerUtilityBot.Controllers;

namespace WorkerUtilityBot;

class Bot : BackgroundService
{
    // Клиент к Telegram Bot API
    private ITelegramBotClient _telegramBotClient;

    // Контроллеры различных видов сообщений
    private DefaultMessageController _defaultMessageController;
    private InlineKeyboardController _inlineKeyboardController;
    private TextMessageController _textMessageController;


    public Bot(
        ITelegramBotClient telegramBotClient,
        DefaultMessageController defaultMessageController,
        InlineKeyboardController inlineKeyboardController,
        TextMessageController textMessageController)
    {
        _telegramBotClient = telegramBotClient;
        _defaultMessageController = defaultMessageController;
        _inlineKeyboardController = inlineKeyboardController;
        _textMessageController = textMessageController;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _telegramBotClient.StartReceiving(
            HandleUpdareAsync,
            HandleErrorAsync,
            new ReceiverOptions() { AllowedUpdates = { } }, //Здесь выбираем, какие обновления хотим получать. В данном случае разрешены все.
            cancellationToken: stoppingToken);

        Console.WriteLine("Бот запущен");
    }

    async Task HandleUpdareAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        //  Обрабатываем нажатия на кнопки из Telegram Bot API: https://core.telegram.org/bots/api#callbackquery
        if (update.Type == UpdateType.CallbackQuery)
        {
            await _inlineKeyboardController.Handle(update.CallbackQuery, cancellationToken);
            return;
        }

        // Обрабатываем входящие сообщения из Telegram Bot API: https://core.telegram.org/bots/api#message
        if (update.Type == UpdateType.Message)
        {
            switch (update.Message.Type)
            {
                case MessageType.Text:
                    await _textMessageController.Handle(update.Message, _inlineKeyboardController.Operation, cancellationToken);
                    return;

                default:
                    await _defaultMessageController.Handle(update.Message, cancellationToken);
                    return;
            }
        }
    }

    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Задаем сообщение об ошибке в зависимости от того, какая именно ошибка произошла
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
            $"Telegram API Error: \n[{apiRequestException.ErrorCode}\n{apiRequestException.Message}]",
            _ => exception.ToString()
        };

        // Выводим в консоль информацию об ошибке
        Console.WriteLine(errorMessage);

        // Задержка перед повторным подключением
        Console.WriteLine("Ожидаем 10 секунд перед повторным подключением.");
        Thread.Sleep(1000);

        return Task.CompletedTask;
    }
}
