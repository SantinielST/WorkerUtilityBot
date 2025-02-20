using Telegram.Bot.Types;

namespace WorkerUtilityBot.Services;

/// <summary>
/// Интерфейс для подсчёта суммы чисел
/// </summary>
interface INumbersCalculator
{
    public int CalculateNumbers(Message message, CancellationToken cancellationToken);
}
