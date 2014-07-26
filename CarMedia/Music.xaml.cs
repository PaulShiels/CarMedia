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
        private bool mediaPlayerIsPlaying = false, mediaPaused = false, mediaStopped=false;
        private TimeSpan pausedPosition;
        private bool sliderBeingDragged = false;
        private List<Song> lstSongs = new List<Song>();
        private MediaElement mePlayer = new MediaElement();
        private DispatcherTimer timer = new DispatcherTimer(), sliderChanging = new DispatcherTimer();
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
            txtsongTimeLeft.Text = String.Format(@"{0:mm\:ss}", TimeSpan.FromSeconds(TimeSpan.Zero.TotalSeconds));
            txtsongRunningTime.Text = String.Format(@"{0:mm\:ss}", TimeSpan.FromSeconds(TimeSpan.Zero.TotalSeconds));

            //Set up buttons
            btnPlay.IsEnabled = false;
            btnStop.IsEnabled = false;
            btnPlay.Content = "Play";

            sldrTrack.IsMoveToPointEnabled = true;

            sliderChanging.Interval = TimeSpan.FromMilliseconds(10);
            sliderChanging.Tick += new EventHandler(SliderMoving);
            timer.Interval = TimeSpan.FromMilliseconds(1000); //one second
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
                        
            int id = 0;
            foreach (var song in Directory.GetFiles("C:\\Music\\"))
            {
                s = new MediaPlayer();
                s.Open(new Uri(song, UriKind.Relative));
                //s.ScrubbingEnabled = true;
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
                btnPlay.Content = "Pause";

                if (!sliderBeingDragged)
                {
                    prbrSong.Value = s.Position.TotalSeconds;
                    sldrTrack.Value = prbrSong.Value;
                }                

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
            else if (!mediaPaused)
            {
                btnStop.IsEnabled = false;
            }
        }

        private void lvSelectionDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PlaySelectedSong();
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

        private void PlaySelectedSong()
        {
            //timer.Stop();            
            s.Stop();
            //Quickly reset the progress bar to 0 as soon as the song stops look good.
            prbrSong.Value = 0;
            sldrTrack.Value = 0;
            
            songPlaying = (Song)lvSelectionDetails.SelectedValue;

            if (songPlaying != null)
            {
                s = songs[songPlaying.songId];
                s.Play();
                mediaPlayerIsPlaying = true;
                prbrSong.Maximum = s.NaturalDuration.TimeSpan.TotalSeconds;
                sldrTrack.Maximum = prbrSong.Maximum;

                lvSelectionDetails.Visibility = Visibility.Hidden;
                nowPlaying.Visibility = Visibility.Visible;
                npSongTitle.Text = songPlaying.songName;
                npArtistName.Text = songPlaying.artist;
                npAlbumTitle.Text = songPlaying.album;
                //timer.Start();
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

        private void lvSelectionDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnPlay.IsEnabled = true;
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            //Was Pause pressed
            if (mediaPlayerIsPlaying)
            {
                s.Pause();
                PlayState(false, true, false, s.Position);
                btnPlay.Content = "Resume";
                btnStop.IsEnabled = true;
            }

            //Was Resume pressed
            else if (mediaPaused)
            {
                s.Position = pausedPosition;
                s.Play();
                mediaPaused = false;
                mediaPlayerIsPlaying = true;
                btnPlay.Content = "Pause";
            }

            //Just play the song
            else
            {
                PlaySelectedSong();
                btnStop.IsEnabled = true;
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            PlayState(false, false, true, TimeSpan.Zero);
            s.Stop();
            prbrSong.Value = 0;
            sldrTrack.Value = 0;
            btnPlay.Content = "Play";
        }

        private void PlayState(bool playing, bool paused, bool stopped, TimeSpan position)
        {
            this.mediaPlayerIsPlaying = playing;
            this.mediaPaused = paused;
            this.mediaStopped = stopped;

            if (paused)
            {
                this.pausedPosition = position;
            }
            else
            {
                this.s.Position = position;
            }
        }

        private void sldrTrack_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            s.Position = TimeSpan.FromSeconds(sldrTrack.Value);
            sliderChanging.Stop();
            sliderBeingDragged = false;
        }

        private void SliderMoving(object sender, EventArgs e)
        {
            prbrSong.Value = sldrTrack.Value;
        }

        private void sldrTrack_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderChanging.Start();            
        }
        
    }

    public class Song
    {
        public int songId { get; set; }

        public string songName { get; set; }

        public string album { get; set; }

        public string artist { get; set; }
    }
}
