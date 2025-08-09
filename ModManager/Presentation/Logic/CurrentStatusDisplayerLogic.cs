using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;

namespace ModManager.Presentation.Logic;

public class CurrentStatusDisplayerLogic
{
    private readonly IStateService stateService;
    private readonly Dictionary<IMod, EventHandler<bool>> siblingEventHandlers = new();

    public CurrentStatusDisplayerLogic(IStateService stateService)
    {
        this.stateService = stateService;
    }


    public void AddModClicked(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        if (button?.Tag is not long workshopId)
        {
            return;
        }

        IMod? mod = stateService.EditingPlayset?.ModStatus.Mods.FirstOrDefault(x => x.WorkshopId == workshopId);

        if (mod == null)
        {
            return;
        }

        mod.IsHidden = false;
    }

    public void DataGridRowSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid dataGrid)
        {
            return;
        }

        dataGrid.SelectionChanged -= DataGridRowSelectionChanged;
        dataGrid.SelectedItem = null;
        dataGrid.SelectionChanged += DataGridRowSelectionChanged;
    }

    public void DataGridLoadedRow(object? sender, DataGridRowEventArgs e)
    {
        if (e.Row.DataContext is not IMod mod || sender is not DataGrid dataGrid)
        {
            return;
        }

        UpdateButtonAppearance(dataGrid, e.Row, mod);
        UpdateRowColor(e.Row, mod);
    }

    private void UpdateButtonAppearance(DataGrid dataGrid, DataGridRow row, IMod mod)
    {
        return;
        FrameworkElement? cell = dataGrid.Columns.First().GetCellContent(row);
        var button = cell?.FindDescendantOfType<Button>();

        if (button == null)
        {
            return;
        }

        // Unsubscribe any previous handler if exists
        if (siblingEventHandlers.TryGetValue(mod, out var oldHandler))
        {
            mod.IsHiddenSiblingChanged -= oldHandler;
        }

        // Create and store a new named handler
        EventHandler<bool> handler = (sender, enabled) => UpdateButtonStyle(button, enabled);
        siblingEventHandlers[mod] = handler;
        mod.IsHiddenSiblingChanged += handler;

        // Apply current style immediately
        UpdateButtonStyle(button, mod.IsHiddenSibling);
    }

    private void UpdateButtonStyle(Button button, bool enabled)
    {
        if (enabled)
        {
            button.Background = new SolidColorBrush(Colors.Black);
            button.Foreground = new SolidColorBrush(Colors.White);
        }
        else
        {
            button.Background = new SolidColorBrush(Colors.DarkGray);
            button.Foreground = new SolidColorBrush(Colors.LightGray);
        }
    }

    private void UpdateRowColor(DataGridRow row, IMod mod)
    {
        return;
        Color enabledColor = Colors.LimeGreen.WithAlpha(0.4);
        Color disabledColor = Colors.IndianRed.WithAlpha(0.4);
        //Color localColor = Colors.LightBlue;

        //if (mod.IsLocalMod)
        //{
        //    Color localEnabledColor = localColor.LerpColors(enabledColor, 0.4);
        //    Color localDisabledColor = localColor.LerpColors(disabledColor, 0.4);
        //    row.Background = mod.IsEnabled
        //        ? new SolidColorBrush(localEnabledColor)
        //        : new SolidColorBrush(localDisabledColor);
        //    return;
        //}

        row.Background = mod.IsEnabled ? new SolidColorBrush(enabledColor) : new SolidColorBrush(disabledColor);
    }
}
