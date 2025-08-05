using System.Reflection;
using Uno.Extensions.Configuration;
using Uno.Resizetizer;

namespace ModManager;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }
    internal static Startup Startup { get; set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Startup = new Startup();

        // Load WinUI Resources
        Resources.Build(r => r.Merged(new XamlControlsResources()));

        // Load Uno.UI.Toolkit and Material Resources
        Resources.Build(r => r.Merged(
            new MaterialToolkitTheme(new Styles.ColorPaletteOverride(), new Styles.MaterialFontsOverride())));

        IApplicationBuilder builder = this.CreateBuilder(args);

        Startup.SetupApplication(builder)
            .Configure(host => host.ConfigureServices(collection => Startup.SetupServices(collection)));

        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        //MainWindow.SetWindowIcon();

        Host = builder.Build();

        ActivateStartingWindow(MainWindow, args);
    }

    private void ActivateStartingWindow(Window mainWindow, LaunchActivatedEventArgs args)
    {
        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (mainWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            // Place the frame in the current Window
            mainWindow.Content = rootFrame;
        }

        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(MainPage), args.Arguments);
        }

        // Ensure the current window is active
        mainWindow.Activate();
    }
}
