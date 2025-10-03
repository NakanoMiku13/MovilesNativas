using BlazorBootstrap;

namespace Kmila.Shared.Models;

public class ToastMessages
{
    public static ToastMessage Message(string Title, string Message, ToastType Type = ToastType.Success) =>
        new()
        {
            Title = Title,
            Message = Message,
            Type = Type,
            HelpText = $"{DateTime.Now}"
        };
}