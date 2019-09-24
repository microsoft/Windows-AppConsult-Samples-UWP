using Microsoft.Toolkit.Win32.UI.XamlHost;

namespace ComplexCustomControl.MyControls
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : XamlApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.Initialize();
        }
    }
}
