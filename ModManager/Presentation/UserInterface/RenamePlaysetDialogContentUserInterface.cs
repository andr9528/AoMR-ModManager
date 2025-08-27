using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Core;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.ViewModel;
using ModManager.Strings;

namespace ModManager.Presentation.UserInterface;

public class RenamePlaysetDialogContentUserInterface : BaseUserInterface
{
    private readonly RenamePlaysetDialogContentLogic logic;
    private readonly RenamePlaysetDialogContentViewModel viewModel;
    private readonly ITranslationService translationService;

    public RenamePlaysetDialogContentUserInterface(
        RenamePlaysetDialogContentLogic logic, RenamePlaysetDialogContentViewModel viewModel,
        ITranslationService translationService)
    {
        this.logic = logic;
        this.viewModel = viewModel;
        this.translationService = translationService;
    }

    /// <inheritdoc />
    protected override void ConfigureContentGrid(Grid grid)
    {
        grid.DefineRows(sizes: [50, 50,]);
    }

    /// <inheritdoc />
    protected override void AddChildrenToGrid(Grid grid)
    {
        TextBlock messageBlock = CreateMessageBlock();
        TextBox renameTextBox = CreateRenameTextBox();

        grid.Children.Add(messageBlock.SetRow(0));
        grid.Children.Add(renameTextBox.SetRow(1));
    }

    private TextBox CreateRenameTextBox()
    {
        TextBox box = TextBoxFactory.CreateDefaultTextBox();

        var textBinding = new Binding()
        {
            Path = nameof(viewModel.PlaysetName),
            Mode = BindingMode.TwoWay,
        };

        box.SetBinding(TextBox.TextProperty, textBinding);

        return box;
    }

    private TextBlock CreateMessageBlock()
    {
        var message = $"{translationService[ResourceKeys.Dialog.Rename.MESSAGE]} '{viewModel.Playset.FileName}'";

        TextBlock block = TextBlockFactory.CreateDefaultTextBlock();

        block.Text = message;
        block.Foreground = new SolidColorBrush(Constants.UiColors.DialogLabelColor);

        return block;
    }
}
