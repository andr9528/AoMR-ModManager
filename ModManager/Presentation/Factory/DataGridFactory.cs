using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;

namespace ModManager.Presentation.Factory;

public static class DataGridFactory
{
    public static DataGrid CreateDataGrid<TViewModel>(
        TViewModel viewModel, string itemsSourcePath, IList<DataGridColumn> columns)
    {
        var dataGrid = new DataGrid
        {
            CanUserReorderColumns = false,
            CanUserResizeColumns = false,
            CanUserSortColumns = false,
            SelectionMode = DataGridSelectionMode.Single,
            ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader,
            AutoGenerateColumns = false,
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsReadOnly = true,
        };

        columns.ForEach(column => dataGrid.Columns.Add(column));

        dataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding {Path = itemsSourcePath, Source = viewModel,});

        return dataGrid;
    }
}
