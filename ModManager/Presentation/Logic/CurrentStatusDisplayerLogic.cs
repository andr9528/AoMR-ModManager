using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;

namespace ModManager.Presentation.Logic;

public class CurrentStatusDisplayerLogic
{
    private readonly IStateService stateService;

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

        mod.Hidden = false;
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
        if (e.Row.DataContext is not IMod mod)
        {
            return;
        }

        Color enabledColor = Colors.LimeGreen.WithAlpha(0.4);
        Color disabledColor = Colors.IndianRed.WithAlpha(0.4);

        if (mod.IsLocalMod)
        {
            Color localEnabledColor = Colors.LightBlue.LerpColors(enabledColor, 0.3);
            Color localDisabledColor = Colors.LightBlue.LerpColors(disabledColor, 0.3);

            e.Row.Background = mod.Enabled
                ? new SolidColorBrush(localEnabledColor)
                : new SolidColorBrush(localDisabledColor);
            return;
        }

        e.Row.Background = mod.Enabled ? new SolidColorBrush(enabledColor) : new SolidColorBrush(disabledColor);
    }
}
