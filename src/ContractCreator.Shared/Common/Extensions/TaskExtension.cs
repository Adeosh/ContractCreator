using ContractCreator.Shared.Common.Exceptions;
using Serilog;

namespace ContractCreator.Shared.Common.Extensions
{
    public static class TaskExtension
    {
        public static async void SafeFireAndForget(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (ex is UserMessageException userEx)
                {
                    Log.Information($"Ошибка валидации: {userEx.Message}");
                }
                else
                {
                    Log.Error(ex, "Не удалось выполнить асинхронную задачу");
                }
            }
        }
    }
}
