using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ControlPeriod.Views
{
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
