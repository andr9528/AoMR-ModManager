using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Presentation.Factory;
using ModManager.Strings;

namespace ModManager.Presentation.Logic;

public class PlaysetSelectorLogic
{
    private readonly IFileService fileService;
    private readonly IStateService stateService;
    private readonly ITranslationService translationService;
    private readonly UIElement parentElement;
    private readonly ILogger<PlaysetSelectorLogic> logger;
    private bool suppressSelectionChangeDueToButtonClick;

    public PlaysetSelectorLogic(
        IFileService fileService, IStateService stateService, ITranslationService translationService,
        UIElement parentElement)
    {
        this.fileService = fileService;
        this.stateService = stateService;
        this.translationService = translationService;
        this.parentElement = parentElement;
        logger =
            ActivatorUtilities.GetServiceOrCreateInstance<ILogger<PlaysetSelectorLogic>>(App.Startup.ServiceProvider);
    }

    public void DeleteButtonClicked(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        if (button?.Tag is not IPlayset taggedPlayset)
        {
            logger.LogWarning(
                $"Expected button in '{nameof(DeleteButtonClicked)}' to be tagged with a '{nameof(IPlayset)}'.");

            return;
        }

        var message = $"{translationService[ResourceKeys.Dialog.Delete.MESSAGE]}{taggedPlayset.FileName})";

        ContentDialog dialog =
            DialogFactory.CreateConfirmationDialog(translationService[ResourceKeys.Dialog.Delete.TITLE], message,
                translationService);
        dialog.XamlRoot = parentElement.XamlRoot;
        dialog.Tag = taggedPlayset;

        dialog.PrimaryButtonClick += DeleteDialogOnPrimaryButtonClick;

        _ = dialog.ShowAsync();
    }

    private void DeleteDialogOnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (sender?.Tag is not IPlayset taggedPlayset)
        {
            logger.LogWarning(
                $"Expected Content Dialog in '{nameof(DeleteDialogOnPrimaryButtonClick)}' to be tagged with a '{nameof(IPlayset)}'.");

            return;
        }

        bool wasDeleted = fileService.DeletePlayset(taggedPlayset);
        if (!wasDeleted)
        {
            return;
        }

        logger.LogInformation("Deleted playset: {PlaysetName}", taggedPlayset.FileName);

        stateService.Playsets.Remove(taggedPlayset);
        if (stateService.EditingPlayset?.FileName != taggedPlayset.FileName)
        {
            return;
        }

        stateService.EditingPlayset = stateService.Playsets.FirstOrDefault();

        logger.LogInformation("Updated Selected playset for editing: {PlaysetName}",
            stateService.EditingPlayset?.FileName ?? "null");
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

        if (suppressSelectionChangeDueToButtonClick)
        {
            if (e.RemovedItems.Count > 0)
            {
                dataGrid.SelectionChanged -= DataGridRowSelectionChanged;
                dataGrid.SelectedItem = e.RemovedItems[0];
                dataGrid.SelectionChanged += DataGridRowSelectionChanged;
            }

            suppressSelectionChangeDueToButtonClick = false;
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

        suppressSelectionChangeDueToButtonClick = true;
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
