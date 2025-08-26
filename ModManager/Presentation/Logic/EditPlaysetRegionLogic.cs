using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Core;

namespace ModManager.Presentation.Logic;

public class EditPlaysetRegionLogic : BaseLogic, IDisplayerLogic
{
    private readonly ViewModel.EditPlaysetRegionViewModel viewModel;
    private readonly ILogger<EditPlaysetRegionLogic> logger;

    public EditPlaysetRegionLogic(IStateService stateService, ViewModel.EditPlaysetRegionViewModel viewModel) :
        base(stateService)
    {
        this.viewModel = viewModel;

        logger =
            ActivatorUtilities.GetServiceOrCreateInstance<ILogger<EditPlaysetRegionLogic>>(App.Startup.ServiceProvider);
    }

    /// <inheritdoc />
    public void EnabledIndicatorButtonClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.Tag is not IMod taggedMod)
        {
            logger.LogWarning(
                $"Expected button in '{nameof(EnabledIndicatorButtonClicked)}' to be tagged with a '{nameof(IMod)}'");

            return;
        }

        IMod? mod = StateService.EditingPlayset?.ModStatus?.Mods.FirstOrDefault(x => x.IsMatchingMod(taggedMod));

        if (mod != null)
        {
            mod.IsEnabled = !mod.IsEnabled;
        }
    }

    public void RemoveModClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.Tag is not IMod taggedMod)
        {
            logger.LogWarning($"Expected button in '{nameof(RemoveModClicked)}' to be tagged with a '{nameof(IMod)}'");

            return;
        }

        IMod? mod = StateService.EditingPlayset?.ModStatus.Mods.FirstOrDefault(x => x.IsMatchingMod(taggedMod));
        if (mod == null)
        {
            return;
        }

        mod.IsEnabled = false;
        mod.IsHidden = true;
    }
}
