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
using System.Xml.Serialization;
using Microsoft.Win32;

namespace Media_Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Song> songsList;
        public List<Uri> tracks;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private bool draggSlider = false;
        private bool volumeDarggSlider = false;
        private bool isPaused = false;
        

        public MainWindow()
        {
            InitializeComponent();
           
            tracks = new List<Uri>();
          
        }
        private void AddToList()
        {
          this.playLista.Items.Clear();

          for (int i = 0; i < tracks.Count; i++)
            this.playLista.Items.Add(tracks.ElementAt(i));

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TickTimer;
            timer.Start();
        }
        
        private void TickTimer(object sender, EventArgs e)
        {
            if(mediaPlayer.Source != null)
            {
                elapsedTime.Content = string.Format("{0}", mediaPlayer.Position.ToString(@"mm\:ss"));
                duration.Content = string.Format("{0}", mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));

                durationProgressBar.Minimum = 0;
                durationProgressBar.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                durationProgressBar.Value = mediaPlayer.Position.TotalSeconds;

                durationSliderProgress.Minimum = 0;
                durationSliderProgress.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                durationSliderProgress.Value = mediaPlayer.Position.TotalSeconds;

                volumeProgressBar.Minimum = 0;
                volumeProgressBar.Maximum = 1.0;
                volumeProgressBar.SmallChange = 0.1;
                volumeProgressBar.Value = mediaPlayer.Volume;

                volumeSlider.Minimum = 0;
                volumeSlider.Maximum = 1;
                volumeSlider.Value = mediaPlayer.Volume;
            }
            else
            {
                elapsedTime.Content = "No file selected";
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            int trackNumber = playLista.SelectedIndex;
            this.textBox.Text = "Track nr: " + trackNumber.ToString(); ;

            if (tracks.Count != 0)
            {
                if(trackNumber == -1)
                {
                    this.textBox.Text = "No Track Selected";
                }
                else
                {
                    if (!isPaused)
                    {
                        mediaPlayer.Open(tracks[trackNumber]);
                        mediaPlayer.Play();
                    }
                    else
                    {
                        mediaPlayer.Play();
                        isPaused = false;
                    }               
                }
            }  
            else
            {
                this.textBox.Text = "No Track Selected";
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (!isPaused)
            {
                mediaPlayer.Pause();
                isPaused = true;
            }
            else
            {
                mediaPlayer.Play();
                isPaused = false;
            }      
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Please Select Source File(s) for a List";

            openFileDialog.Filter = "Media files (*.mp3;*.wav;)|*.mp3;*.wav;|All files (*.*)|*.*";
 
            if (openFileDialog.ShowDialog() == true)
            {

                foreach (String file in openFileDialog.FileNames)
                {
                    try
                    {
                        Uri uriAddress = new Uri(file);
                        tracks.Add(uriAddress);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    }
                }
            }
            AddToList();
        }

        private void SliderProgress_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            draggSlider = true;
        }

        private void SliderProgress_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            draggSlider = false;
            mediaPlayer.Position = TimeSpan.FromSeconds(durationSliderProgress.Value);
        }

        private void SliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            elapsedTime.Content = TimeSpan.FromSeconds(durationSliderProgress.Value).ToString(@"mm\:ss");
            draggSlider = true;
        }

        private void SliderVolume_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            volumeDarggSlider = true;
        }

        private void SliderVolume_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            volumeDarggSlider = false;
            mediaPlayer.Volume = volumeSlider.Value;
            this.textBox.Text = "volume:" + mediaPlayer.Volume.ToString(); 
        }

        private void VolumeUp_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Volume += 0.1;
        }

        private void VolumeDown_Click(object sender, RoutedEventArgs e)
        {    
            mediaPlayer.Volume -= 0.1;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int trackNumber = playLista.SelectedIndex;
            this.textBox.Text = "Track nr: " + trackNumber.ToString();

            if (tracks.Count != 0)
            {
                if (trackNumber == -1)
                {
                    this.textBox.Text = "No Track Selected";
                }
                else
                {
                    mediaPlayer.Open(tracks[trackNumber]);
                    mediaPlayer.Play();
                }
            }
            else
            {
                this.textBox.Text = "No Track Selected";
            }

        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mediaPlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Media files (*.xml)|*.xml;*|All files (*.*)|*.*";

            if(saveFileDialog.ShowDialog() == true)
            using (Stream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                List<string> stringTracks = new List<string>();

                for(int i = 0; i < tracks.Count(); i++)
                {
                    stringTracks.Add(tracks[i].ToString());
                }
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                serializer.Serialize(fileStream, stringTracks);
            }    
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Media files (*.xml)|*xml;|All files(*.*)|*.*";

            List<string> stringTracks = new List<string>();

            if (openFileDialog.ShowDialog() == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<String>));

                using (FileStream fileStream = File.OpenRead(openFileDialog.FileName))
                {
                    stringTracks = (List<string>)serializer.Deserialize(fileStream);
                }
            }

            tracks.Clear();

            for (int i = 0; i < stringTracks.Count(); i++)
            {
                tracks.Add(new Uri(stringTracks[i]));
            }
            AddToList();
        }

 
    }
}
