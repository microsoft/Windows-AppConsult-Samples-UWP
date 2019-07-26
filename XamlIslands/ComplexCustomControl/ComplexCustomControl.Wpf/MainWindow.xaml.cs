using ComplexCustomControl.Uwp;
using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Windows;

namespace ComplexCustomControl.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            if (sender is WindowsXamlHost host && host.Child is MarkdownView markdownView)
            {
                markdownView.MarkdownText = "This is **markdown** rendered by the UWP control included in the [Windows Community Toolkit](https://docs.microsoft.com/en-us/windows/communitytoolkit/)";
            }
        }
    }
}
