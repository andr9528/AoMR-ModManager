using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Presentation;

public class PlaylistSelectorLogic
{
    private readonly IFileService fileService;
    private readonly IStateService stateService;
    private readonly ILogger<PlaylistSelectorLogic> logger;
    private bool suppressSelectionCahngeDueToButtonClick;

    public PlaylistSelectorLogic(IFileService fileService, IStateService stateService)
    {
        this.fileService = fileService;
        this.stateService = stateService;
        logger =
            ActivatorUtilities.GetServiceOrCreateInstance<ILogger<PlaylistSelectorLogic>>(App.Startup.ServiceProvider);
    }

    public void DeleteButtonClicked(object sender, RoutedEventArgs e)
    {
    }

    public void RenameButtonClicked(object sender, RoutedEventArgs e)
    {
    }

    public void UpdateDataGridColumnVisibility(bool isOpen, DataGrid dataGrid)
    {
        for (var i = 0; i < dataGrid.Columns.Count; i++)
        {
            dataGrid.Columns[i].Visibility = !isOpen && i > 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public void DataGridRowSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid dataGrid)
        {
            return;
        }

        if (suppressSelectionCahngeDueToButtonClick)
        {
            if (e.RemovedItems.Count > 0)
            {
                dataGrid.SelectionChanged -= DataGridRowSelectionChanged;
                dataGrid.SelectedItem = e.RemovedItems[0];
                dataGrid.SelectionChanged += DataGridRowSelectionChanged;
            }

            suppressSelectionCahngeDueToButtonClick = false;
            return;
        }

        object? selected = dataGrid.SelectedItem;

        if (selected is not IPlayset playset)
        {
            return;
        }

        stateService.EditingPlayset = playset;
        logger.LogInformation("Updated Selected playset for editing: {PlaysetName}", playset.FileName);
    }

    /// <summary>
    /// If click originates inside any Button within the row, mark the selection change to be suppressed.
    ///  </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DataGridPreviewPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (e.OriginalSource is not DependencyObject dep || FindParent<Button>(dep) is not Button)
        {
            return;
        }

        suppressSelectionCahngeDueToButtonClick = true;
    }

    private T? FindParent<T>(DependencyObject child) where T : class, DependencyObject
    {
        while (child != null && child is not T)
        {
            child = VisualTreeHelper.GetParent(child);
        }

        return child as T;
    }
}
