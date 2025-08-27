using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Core;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.ViewModel;
using ModManager.Strings;

namespace ModManager.Presentation.UserInterface;

public class NewPlaysetDialogContentUserInterface : BaseUserInterface
{
    private readonly NewPlaysetDialogContentLogic logic;
    private readonly NewPlaysetDialogContentViewModel viewModel;
    private readonly ITranslationService translationService;

    public NewPlaysetDialogContentUserInterface(
        NewPlaysetDialogContentLogic logic, NewPlaysetDialogContentViewModel viewModel,
        ITranslationService translationService)
    {
        this.logic = logic;
        this.viewModel = viewModel;
        this.translationService = translationService;
    }

    /// <inheritdoc />
    protected override void ConfigureContentGrid(Grid grid)
    {
        grid.DefineRows(sizes: [50, 50, 50, 50,]);
    }

    /// <inheritdoc />
    protected override void AddChildrenToGrid(Grid grid)
    {
        TextBlock messageBlockOne = CreateMessageBlockOne();
        TextBox nameTextBox = CreateNameTextBox();
        TextBlock messageBlockTwo = CreateMessageBlockTwo();
        ComboBox modsSourceComboBox = CreateModsSourceComboBox();

        grid.Children.Add(messageBlockOne.SetRow(0));
        grid.Children.Add(nameTextBox.SetRow(1));
        grid.Children.Add(messageBlockTwo.SetRow(2));
        grid.Children.Add(modsSourceComboBox.SetRow(3));
    }

    private ComboBox CreateModsSourceComboBox()
    {
        string[] options =
        [
            translationService[ResourceKeys.Dialog.Create.OPTION_ONE],
            translationService[ResourceKeys.Dialog.Create.OPTION_TWO],
            translationService[ResourceKeys.Dialog.Create.OPTION_THREE],
        ];

        ComboBox comboBox = ComboBoxFactory.CreateDefaultComboBox(options);

        var selectedIndexBinding = new Binding()
        {
            Path = nameof(viewModel.SelectedModsSource),
            Mode = BindingMode.TwoWay,
        };

        comboBox.SetBinding(ComboBox.SelectedIndexProperty, selectedIndexBinding);

        comboBox.SelectionChanged += (sender, args) => logic.ModsSourceSelectionChanged(sender, args);

        return comboBox;
    }

    private TextBlock CreateMessageBlockTwo()
    {
        TextBlock block = TextBlockFactory.CreateDefaultTextBlock();

        block.Text = translationService[ResourceKeys.Dialog.Create.MESSAGE_TWO];
        block.Foreground = new SolidColorBrush(Constants.UiColors.DialogLabelColor);

        return block;
    }

    private TextBox CreateNameTextBox()
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

    private TextBlock CreateMessageBlockOne()
    {
        TextBlock block = TextBlockFactory.CreateDefaultTextBlock();

        block.Text = translationService[ResourceKeys.Dialog.Create.MESSAGE_ONE];
        block.Foreground = new SolidColorBrush(Constants.UiColors.DialogLabelColor);

        return block;
    }
}
