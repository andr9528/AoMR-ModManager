using ModManager.Abstractions.Services;
using ModManager.Strings;

namespace ModManager.Presentation.Factory;

public static class DialogFactory
{
    public static ContentDialog CreateConfirmationDialog(
        string title, string content, ITranslationService translationService)
    {
        return CreateDefaultDialog(title, content, translationService[ResourceKeys.Dialog.CONFIRM],
            translationService[ResourceKeys.Dialog.CANCEL]);
    }

    private static ContentDialog CreateDefaultDialog(string title, object content, string yesButton, string noButton)
    {
        return new ContentDialog
        {
            Title = title,
            Content = content,
            PrimaryButtonText = yesButton,
            CloseButtonText = noButton,
            DefaultButton = ContentDialogButton.Close,
        };
    }
}
