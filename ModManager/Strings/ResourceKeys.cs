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
        public const string SAVE = nameof(Resources.Dialog_Save);
        public const string DISCARD = nameof(Resources.Dialog_Discard);

        public static class Delete
        {
            public const string TITLE = nameof(Resources.Dialog_Deletion_Title);
            public const string MESSAGE = nameof(Resources.Dialog_Deletion_Message);
        }

        public static class Rename
        {
            public const string TITLE = nameof(Resources.Dialog_Rename_Title);
            public const string MESSAGE = nameof(Resources.Dialog_Rename_Message);
        }

        public static class Create
        {
            public const string TITLE = nameof(Resources.Dialog_Create_Title);
            public const string MESSAGE_ONE = nameof(Resources.Dialog_Create_Message_One);
            public const string MESSAGE_TWO = nameof(Resources.Dialog_Create_Message_Two);

            public const string OPTION_ONE = nameof(Resources.Dialog_Create_Option_One);
            public const string OPTION_TWO = nameof(Resources.Dialog_Create_Option_Two);
            public const string OPTION_THREE = nameof(Resources.Dialog_Create_Option_Three);
        }
    }
}
