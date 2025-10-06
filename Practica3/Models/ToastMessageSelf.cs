using BlazorBootstrap;

namespace Practica3.Models;
public static class ToastMessageSelf{
    public static ToastMessage Message(string message, string title = "Success", ToastType type = ToastType.Success) =>
        new(){
            Message = message,
            Title = title,
            Type = type,
            AutoHide = true,
            HelpText = $"{DateTime.Now}"
        };
}