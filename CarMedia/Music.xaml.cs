using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CarMedia
{
    /// <summary>
    /// Interaction logic for Music.xaml
    /// </summary>
    public partial class Music : Page
    {
        private List<MediaPlayer> songs = new List<MediaPlayer>();
        private MediaPlayer s = new MediaPlayer();
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        private List<Song> lstSongs = new List<Song>();
        private MediaElement mePlayer = new MediaElement();
        private DispatcherTimer timer = new DispatcherTimer();
        private Song songPlaying;
        //<MediaElement Name="mePlayer" Grid.Row="1" LoadedBehavior="Manual" Stretch="None" />

        public Music()
        {
            InitializeComponent();
                      
            
            //items.Add(new Song() { songName = "John Doe", col2 = 42, col3 = "john@doe-family.com" });
            //lvSelectionDetails.ItemsSource = items;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            songName.Header = "Name";
            album.Header = "Album";
            artist.Header = "Artist";
            timer.Interval = TimeSpan.FromMilliseconds(1000); //one second
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
                        
            int id = 0;
            foreach (var song in Directory.GetFiles("C:\\Music\\"))
            {
                s = new MediaPlayer();
                s.Open(new Uri(song, UriKind.Relative));
                s.ScrubbingEnabled = true;
                songs.Add(s);
                
                TagLib.File tagFile = TagLib.File.Create(song);
                Song sng = new Song();
                sng.songId = id;
                id++;
                sng.songName = tagFile.Tag.Title;
                sng.artist = "PVD";
                sng.album = "Best of cd1";

                //FileInfo f = new FileInfo(song);                

                //sng.songName = f.Name;
                //sng.album = 0;
                //sng.artist = s.NaturalDuration.ToString();
                lstSongs.Add(sng);

            }

            lvSelectionDetails.ItemsSource = lstSongs;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayerIsPlaying)
            {
                prbrSong.Value = s.Position.TotalSeconds;

                try
                {
                    double timeLeft = s.NaturalDuration.TimeSpan.TotalSeconds - s.Position.TotalSeconds;
                    txtsongRunningTime.Text = String.Format(@"{0:mm\:ss}", s.Position);
                    txtsongTimeLeft.Text = String.Format(@"{0:mm\:ss}", TimeSpan.FromSeconds(timeLeft));
                }
                catch
                {

                }
            }
        }

        private void lvSelectionDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();            
            s.Stop();
            prbrSong.Maximum = s.NaturalDuration.TimeSpan.TotalSeconds;
            songPlaying = (Song)lvSelectionDetails.SelectedValue;
            if (songPlaying != null)
            {
                s = songs[songPlaying.songId];
                s.Play();
                mediaPlayerIsPlaying = true;
                //prbrSong.Maximum = s.Position.TotalSeconds;

                lvSelectionDetails.Visibility = Visibility.Hidden;
                nowPlaying.Visibility = Visibility.Visible;
                npSongTitle.Text = songPlaying.songName;
                npArtistName.Text = songPlaying.artist;
                npAlbumTitle.Text = songPlaying.album;
                timer.Start();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (nowPlaying.Visibility == Visibility.Visible && lvSelectionDetails.Visibility == Visibility.Hidden)
            {
                nowPlaying.Visibility = Visibility.Hidden;
                lvSelectionDetails.Visibility = Visibility.Visible;
            }
            else if(mediaPlayerIsPlaying)
            {
                nowPlaying.Visibility = Visibility.Visible;
                lvSelectionDetails.Visibility = Visibility.Hidden;
            }
        }

        #region unused
        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = (mePlayer != null) && (mePlayer.Source != null);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //mePlayer.Play();
            //mediaPlayerIsPlaying = true;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
           // e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //mePlayer.Pause();
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //mePlayer.Stop();
            //mediaPlayerIsPlaying = false;
        }
        #endregion
    }

    public class Song
    {
        public int songId { get; set; }

        public string songName { get; set; }

        public string album { get; set; }

        public string artist { get; set; }
    }
}
