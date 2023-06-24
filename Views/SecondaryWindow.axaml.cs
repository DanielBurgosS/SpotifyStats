using Avalonia.Controls;
using Avalonia.Interactivity;
using SpotifyAPI.Web;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using System;
using System.Windows;

namespace SpotifyStats.Views
{
    public partial class SecondaryWindow : Window
    {
        Avalonia.Controls.Label tracks;
        Avalonia.Controls.Label artists;
        Avalonia.Controls.Label genres;
        StatisticsController controller_;
        //defaulting to 5 so that there are no bugs
        private int rank_ = 5;
        public SecondaryWindow()
        {
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            artists = this.FindControl<Avalonia.Controls.Label>("Artists");
            tracks = this.FindControl<Avalonia.Controls.Label>("Tracks");
            genres = this.FindControl<Avalonia.Controls.Label>("Genres");

        }
        private async void Top5Button_Click(object? sender, RoutedEventArgs e)
        {
            rank_ = 5;
            try
            {
                await UpdateRanking();
            }
            catch (Exception ex)
            {
                tracks.Content = ex.Message;
            }

        }
        private async void Top10Button_Click(object? sender, RoutedEventArgs e)

        {
            rank_ = 10;
            try
            {
                await UpdateRanking();
            }
            catch (Exception ex)
            {
                tracks.Content = ex.Message;
            }

        }

        private async Task UpdateRanking()
        {
            (artists.Content, tracks.Content, genres.Content) = ("", "", "");
            (artists.Content, tracks.Content, genres.Content) = await controller_.Update(rank_);

        }


        public override void Show()
        {
            base.Show();
            //TODO: INITIALIZE Spotify Class
            if (controller_ == null)
            {
                controller_ = new();
            }

        }
    }
}
