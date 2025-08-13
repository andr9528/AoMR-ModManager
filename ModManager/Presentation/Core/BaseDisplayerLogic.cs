using System.Collections;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Presentation.Core;

public abstract class BaseDisplayerLogic
{
    protected readonly IStateService StateService;

    private readonly Dictionary<IMod, EventHandler<bool>> enabledEventHandlers = new();
    private readonly DispatcherQueue? uiQueue;

    protected BaseDisplayerLogic(IStateService stateService)
    {
        StateService = stateService;
        uiQueue = DispatcherQueue.GetForCurrentThread();
    }

    public void DataGridRowSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid dataGrid)
        {
            return;
        }

        dataGrid.SelectionChanged -= DataGridRowSelectionChanged;
        dataGrid.SelectedItem = null;
        dataGrid.SelectedIndex = -1;
        dataGrid.SelectionChanged += DataGridRowSelectionChanged;
    }

    public void DataGridLoadedRow(object? sender, DataGridRowEventArgs e)
    {
        if (e.Row.DataContext is not IMod mod || sender is not DataGrid dataGrid)
        {
            return;
        }

        SetupRowColorHandling(e.Row, mod, dataGrid);
    }

    private void SetupRowColorHandling(DataGridRow row, IMod mod, DataGrid dataGrid)
    {
        UpdateRowColor(row, mod);

        if (enabledEventHandlers.TryGetValue(mod, out var existingHandler))
        {
            mod.IsEnabledChanged -= existingHandler;
        }

        EventHandler<bool> handler = (s, args) =>
        {
            uiQueue?.TryEnqueue(() => UpdateRowColorWithForcedRenderUpdate(row, mod, dataGrid));
        };

        mod.IsEnabledChanged += handler;

        enabledEventHandlers[mod] = handler;

        row.Unloaded += (s, args) =>
        {
            if (!enabledEventHandlers.TryGetValue(mod, out var handlerToRemove))
            {
                return;
            }

            mod.IsEnabledChanged -= handlerToRemove;
            enabledEventHandlers.Remove(mod);
        };
    }

    private void UpdateRowColorWithForcedRenderUpdate(DataGridRow row, IMod mod, DataGrid dataGrid)
    {
        UpdateRowColor(row, mod);

        IEnumerable? temp = dataGrid.ItemsSource;
        dataGrid.ItemsSource = null;
        dataGrid.ItemsSource = temp;

        var dummy = 1;
    }


    private void UpdateRowColor(DataGridRow row, IMod mod)
    {
        row.Background = mod.IsEnabled
            ? new SolidColorBrush(Constants.UiColors.OnRowColor)
            : new SolidColorBrush(Constants.UiColors.OffRowColor);
    }

    protected bool IsClickedMod(IMod mod, IMod taggedMod)
    {
        if (mod.WorkshopId != 0 && taggedMod.WorkshopId != 0 && mod.WorkshopId == taggedMod.WorkshopId)
        {
            return true;
        }

        return mod.Title.Equals(taggedMod.Title, StringComparison.InvariantCultureIgnoreCase) &&
               mod.IsLocalMod == taggedMod.IsLocalMod;
    }
}
