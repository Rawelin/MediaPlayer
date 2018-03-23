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
                volumeProgressBar.Maximum = 1;
                volumeProgressBar.SmallChange = 0.05;
                volumeProgressBar.Value = mediaPlayer.Volume;
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
                    mediaPlayer.Open(tracks[trackNumber]);
                    mediaPlayer.Play();
                }
            }  
            else
            {
                this.textBox.Text = "No Track Selected";
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Media files (*.mp3;*.wav;)|*.mp3;*.wav;|All files (*.*)|*.*";

          
            if (openFileDialog.ShowDialog() == true)
            {
                Uri uriAddress = new Uri(openFileDialog.FileName);
                tracks.Add(uriAddress);
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TickTimer;
            timer.Start();

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
        }

        private void VolumeUp_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Volume += 0.05;
        }

        private void VolumeDown_Click(object sender, RoutedEventArgs e)
        {
           
            mediaPlayer.Volume -= 0.05;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int trackNumber = playLista.SelectedIndex;
            this.textBox.Text = "Track nr: " + trackNumber.ToString(); ;

            mediaPlayer.Open(tracks[trackNumber]);
            mediaPlayer.Play();

        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mediaPlayer.Volume += (e.Delta > 0) ? 0.05 : -0.05;
        }
        private void Serialization()
        {
          /*  List<Animal> theAnimals = new List<Animal>
            {
                new Animal("Mario", 60, 30),
                new Animal("Luigi", 55, 24),
                new Animal("Peach", 40, 20),
             };
           */
            using (Stream fs = new FileStream(@"C:\test\animals.xml", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlSerializer serializer2 = new XmlSerializer(typeof(List<Uri>));
                serializer2.Serialize(fs, tracks);

            }

            tracks = null;

            XmlSerializer serializer3 = new XmlSerializer(typeof(List<Uri>));

            using (FileStream fs2 = File.OpenRead(@"C:\test\animals.xml"))
            {
                tracks = (List<Uri>)serializer3.Deserialize(fs2);
            }
        }



    }
}
