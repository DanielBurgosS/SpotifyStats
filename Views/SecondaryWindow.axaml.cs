using Avalonia.Controls;
using Avalonia.Interactivity;
using SpotifyAPI.Web;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.Media.Imaging;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Models;
using System.Linq;
using MessageBoxAvaloniaEnums = MessageBox.Avalonia.Enums;
using EmbedIO.Utilities;
using System.IO;

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
            UpdateFontSize(50);
            rank_ = 5;
            try
            {
                await UpdateRanking();
            }
            catch (NullReferenceException ex)
            {
                ShowMessageBox("Error", "The API hasn't gotten a response. Try again!");
            }
            catch (Exception ex)
            {
                ShowMessageBox("Error", ex.Message);
            }

        }
        private async void Top10Button_Click(object? sender, RoutedEventArgs e)
        {
            UpdateFontSize(30);
            rank_ = 10;
            try
            {
                await UpdateRanking();
            }
            catch (NullReferenceException ex)
            {
                ShowMessageBox("Error", "The API hasn't gotten a response. Try again!");
                await LogException(ex);
            }
            catch (Exception ex)
            {
                ShowMessageBox("Error", ex.Message);
            }

        }
        private async Task LogException(Exception ex)
        {
            string date = DateTime.Now.ToString("dd_mm_yyy_hh_mm_ss");
            using (StreamWriter writer = new StreamWriter($"../../../Log/log_{date}.txt"))
            {
                await writer.WriteAsync($"=>{DateTime.Now} An Error occurred: {ex.StackTrace}  Message: {ex.Message}");
            }
        }
        private void ShowMessageBox(string title, string message)
        {
            var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(title, message);
            messageBoxStandardWindow.Show();
        }
        private async Task UpdateRanking()
        {
            (artists.Content, tracks.Content, genres.Content) = ("", "", "");
            (artists.Content, tracks.Content, genres.Content) = await controller_.Update(rank_);

        }

        private void UpdateFontSize(int size)
        {
            (artists.FontSize, tracks.FontSize, genres.FontSize) = (size, size, size);
        }


        public override void Show()
        {
            base.Show();
            //TODO: INITIALIZE Spotify Class
            try
            {
                if (controller_ == null)
                {
                    controller_ = new();
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox("Error", ex.Message);
            }

        }

        private void BackButton_Click(object? sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
