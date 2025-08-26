namespace ModManager.Strings;

public static class ResourceKeys
{
    public static class Column
    {
        public const string ACTIONS = nameof(Resources.Column_Actions);
        public const string MODS = nameof(Resources.Column_Mods);
        public const string INDICATORS = nameof(Resources.Column_Indicators);
        public const string AUTHOR = nameof(Resources.Column_Author);
    }

    public static class Status
    {
        public const string HEADER = nameof(Resources.Status_Header);
    }

    public static class Edit
    {
        public const string HEADER = nameof(Resources.Edit_Header);
    }

    public static class Actions
    {
        public const string ACTIVATE = nameof(Resources.Actions_Activate);
        public const string INDICATOR = nameof(Resources.Actions_Indicator);
    }

    public static class Dialog
    {
        public const string CONFIRM = nameof(Resources.Dialog_Confirm);
        public const string CANCEL = nameof(Resources.Dialog_Cancel);

        public static class Delete
        {
            public const string TITLE = nameof(Resources.Dialog_Deletion_Title);
            public const string MESSAGE = nameof(Resources.Dialog_Deletion_Message);
        }
    }
}
