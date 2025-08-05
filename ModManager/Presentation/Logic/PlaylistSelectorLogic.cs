using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Services;

namespace ModManager.Presentation;

public class PlaylistSelectorLogic
{
    private readonly IFileService fileService;
    private readonly IStateService stateService;

    public PlaylistSelectorLogic(IFileService fileService, IStateService stateService)
    {
        this.fileService = fileService;
        this.stateService = stateService;
    }

    public void DeleteButtonClicked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void RenameButtonClicked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void EditButtonClicked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void UpdateDataGridColumnVisibility(bool isOpen, DataGrid dataGrid)
    {
        for (var i = 0; i < dataGrid.Columns.Count; i++)
        {
            dataGrid.Columns[i].Visibility = !isOpen && i > 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
