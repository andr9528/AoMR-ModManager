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
    }

    public void EnabledIndicatorButtonClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.Tag is not long workshopId)
        {
            return;
        }

        IMod? mod = stateService.CurrentModStatus?.Mods.FirstOrDefault(x => x.WorkshopId == workshopId);

        if (mod != null)
        {
            mod.IsEnabled = !mod.IsEnabled;
        }
    }
}
