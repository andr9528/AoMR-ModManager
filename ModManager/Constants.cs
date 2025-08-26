using ModManager.Extensions;

namespace ModManager;

public static class Constants
{
    public static class Glyphs
    {
        public const string EDIT_SYMBOL_UNICODE = "\uE104";
        public const string DELETE_SYMBOL_UNICODE = "\uE107";
        public const string TRASH_CAN_SYMBOL_UNICODE = "\uE74D";
        public const string RENAME_SYMBOL_UNICODE = "\uE8AC";
        public const string LEFT_ARROW_SYMBOL_UNICODE = "\u2190";
        public const string CHECKMARK_SYMBOL_UNICODE = "\uE8FB";
        public const string CROSS_SYMBOL_UNICODE = "\uE8BB";
        public const string FOLDER_SYMBOL_UNICODE = "\uE8B7";
        public const string CLOUD_SYMBOL_UNICODE = "\uE753";
    }

    public static class UiColors
    {
        public static Color OnRowColor = Colors.LimeGreen.WithAlpha(0.2);
        public static Color OnButtonColor = Colors.LimeGreen.WithAlpha(0.4);
        public static Color OffRowColor = Colors.IndianRed.WithAlpha(0.2);
        public static Color OffButtonColor = Colors.IndianRed.WithAlpha(0.4);
        public static Color DisabledButtonColor = Colors.Gray.WithAlpha(0.4);
        public static Color InteractableButtonColor = Colors.Teal.WithAlpha(0.4);
        public static Color InformationButtonColor = Colors.CadetBlue.WithAlpha(0.4);
        public static Color RowBorderColor = Colors.Black;
        public static Color MissingRowColor = Colors.Teal.WithAlpha(0.2);
        public static Color MissingLocalRowColor = Colors.DimGray.WithAlpha(0.2);
    }

    public static class Fonts
    {
        public const int SECTION_HEADER_FONT_SIZE = 18;
    }
}
