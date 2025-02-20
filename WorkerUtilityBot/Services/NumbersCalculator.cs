using Telegram.Bot;
using Telegram.Bot.Types;
using WorkerUtilityBot.Extensions;

namespace WorkerUtilityBot.Services;
/// <summary>
/// Класс посчитывает сумму чисел из сообщения, проверяет данные на корректность.
/// </summary>
class NumbersCalculator : INumbersCalculator
{
	private ITelegramBotClient _telegramBotClient;
	private InputException inputException = new InputException("Введите числа через пробел!!!");

    public NumbersCalculator(ITelegramBotClient telegramBotClient)
    {
		_telegramBotClient = telegramBotClient;
    }


    /// <summary>
    /// Метод разбивает строку на отдельные числа, проверяется строку на корректность данных.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public int CalculateNumbers(Message message, CancellationToken cancellationToken)
	{
		try
		{
			var result = message.Text.Split(' ').Select(int.Parse).Sum();

			return result;
		}
		catch (Exception)
		{
			_telegramBotClient.SendMessage(message.Chat.Id, inputException.Message, cancellationToken: cancellationToken);
			throw;
		}
	}
}
