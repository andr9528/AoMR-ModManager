using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Presentation.Core;

public abstract class BaseDisplayerLogic : IDisplayerLogic
{
    protected readonly IStateService StateService;

    protected BaseDisplayerLogic(IStateService stateService)
    {
        StateService = stateService;
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

        SetupRowColorHandling(e.Row, mod);
    }

    private void SetupRowColorHandling(DataGridRow row, IMod mod)
    {
        UpdateRowColor(row, mod);

        // Subscribe using a lambda that captures both row and mod
        EventHandler<bool> handler = (s, args) => { UpdateRowColor(row, mod); };
        mod.IsEnabledChanged += handler;

        // Unsubscribe when the row unloads to prevent memory leaks
        row.Unloaded += (s, args) => { mod.IsEnabledChanged -= handler; };
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

        IMod? mod = StateService.CurrentModStatus?.Mods.FirstOrDefault(x => x.WorkshopId == workshopId);

        if (mod != null)
        {
            mod.IsEnabled = !mod.IsEnabled;
        }
    }

    private void UpdateRowColor(DataGridRow row, IMod mod)
    {
        row.Background = mod.IsEnabled
            ? new SolidColorBrush(Constants.UiColors.OnRowColor)
            : new SolidColorBrush(Constants.UiColors.OffRowColor);
    }
}
