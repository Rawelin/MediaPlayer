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
        
        public List<Uri> trackList;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private bool draggSlider = false;
        private bool volumeDarggSlider = false;
        private bool isPaused = false;
        private bool playListFlag = true;

        private int globaTrackNumber;

        public MainWindow()
        {
            InitializeComponent();

            trackList = new List<Uri>();
        }
        private void AddToList()
        {
            this.playLista.Items.Clear();

            string path, trimmedPath;

            for (int i = 0; i < trackList.Count; i++)
            {
                path = trackList[i].AbsolutePath.ToString();
                trimmedPath = Utility.TrimPath(path);
                this.playLista.Items.Add(trimmedPath);
            }
              

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TickTimer;
            timer.Start();
        }
        
        private void TickTimer(object sender, EventArgs e)
        {
            if(mediaPlayer.Source != null)
            {

                if(mediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    elapsedTime.Content = string.Format("{0}", mediaPlayer.Position.ToString(@"m\:ss"));
                    duration.Content = string.Format("{0}", mediaPlayer.NaturalDuration.TimeSpan.ToString(@"m\:ss"));

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
            }
            else
            {
                this.textBox.Text = "No file selected";
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            globaTrackNumber = playLista.SelectedIndex;

            if (trackList.Count != 0)
            {
                int trackNumber = playLista.SelectedIndex;
                string path = trackList[trackNumber].AbsolutePath.ToString();

                string trimmedPath = Utility.TrimPath(path);

                this.textBox.Text = trimmedPath;

                if (trackNumber == -1)
                {
                    this.textBox.Text = "No Track Selected";
                }
                else
                {
                    if (!isPaused)
                    {
                        mediaPlayer.Open(trackList[trackNumber]);
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

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            globaTrackNumber++;

            if (globaTrackNumber == trackList.Count)
                globaTrackNumber = 0;

            mediaPlayer.Open(trackList[globaTrackNumber]);
            mediaPlayer.Play();

            string path = trackList[globaTrackNumber].AbsolutePath.ToString();
            string trimmedPath = Utility.TrimPath(path);

            this.textBox.Text = trimmedPath;

        }

        private void Backward_Click(object sender, RoutedEventArgs e)
        {
            globaTrackNumber--;
            if (globaTrackNumber == -1)
                globaTrackNumber = trackList.Count - 1;

            mediaPlayer.Open(trackList[globaTrackNumber]);
            mediaPlayer.Play();

            string path = trackList[globaTrackNumber].AbsolutePath.ToString();
            string trimmedPath = Utility.TrimPath(path);

            this.textBox.Text = trimmedPath;
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
                        trackList.Add(uriAddress);
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
          //  this.textBox.Text = "volume:" + mediaPlayer.Volume.ToString(); 
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
            globaTrackNumber = playLista.SelectedIndex;

            int trackNumber = playLista.SelectedIndex;
            string path = trackList[trackNumber].AbsolutePath.ToString();

            string trimmedPath = Utility.TrimPath(path);

            this.textBox.Text = trimmedPath;

            if (trackList.Count != 0)
            {
                if (trackNumber == -1)
                {
                    this.textBox.Text = "No Track Selected";
                }
                else
                {
                    mediaPlayer.Open(trackList[trackNumber]);
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

                for(int i = 0; i < trackList.Count(); i++)
                {
                    stringTracks.Add(trackList[i].ToString());
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

            trackList.Clear();

            for (int i = 0; i < stringTracks.Count(); i++)
            {
                trackList.Add(new Uri(stringTracks[i]));
            }
            AddToList();

            playLista.MaxHeight = 260;
            listStack.Height = 260;
            Application.Current.MainWindow.Height = 393;

            playListFlag = false;

        }


        private void Deletet_Click(object sender, RoutedEventArgs e)
        {
            int trackNumber = -1;
            trackNumber = playLista.SelectedIndex;

            if(trackNumber != -1)
                 trackList.RemoveAt(trackNumber);
            AddToList();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_app(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Minimize_app(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ShowPlayList(object sender, RoutedEventArgs e)
        {
            if(playListFlag)
            {
                playLista.MaxHeight = 260;
                listStack.Height = 260;
                Application.Current.MainWindow.Height = 393;

                playListFlag = false;
            }
            else
            {
                playLista.MaxHeight = 0;
                listStack.Height = 0;
                Application.Current.MainWindow.Height = 135;

                playListFlag = true;
            }
        }

        private void ListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.textBox.Text = "Drag";
            DataObject data = new DataObject(DataFormats.Text, ((ListViewItem)e.Source).Content);
            DragDrop.DoDragDrop((DependencyObject)e.Source, data, DragDropEffects.Copy);       
        }
    }
}
