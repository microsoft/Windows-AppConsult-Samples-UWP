using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ComplexCustomControl.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MarkdownView : UserControl
    {
        public MarkdownView()
        {
            this.InitializeComponent();
            this.Markdown.Text = MarkdownText;
        }

        public string MarkdownText
        {
            get { return (string)GetValue(MarkdownTextProperty); }
            set { SetValue(MarkdownTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkdownText.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty MarkdownTextProperty =
            DependencyProperty.Register("MarkdownText", typeof(string), typeof(MarkdownView), new PropertyMetadata(string.Empty, (obj, args) =>
                {
                    var control = obj as MarkdownView;
                    var markdown = control.FindName("Markdown") as MarkdownTextBlock;
                    markdown.Text = args.NewValue.ToString();
                }
            ));
    }
}
