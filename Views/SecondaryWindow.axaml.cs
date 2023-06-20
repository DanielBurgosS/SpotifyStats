using Avalonia.Controls;
using Avalonia.Interactivity;
using SpotifyAPI.Web;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;

namespace SpotifyStats.Views
{
    public partial class SecondaryWindow : Window
    {
        public SecondaryWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
    }
}
