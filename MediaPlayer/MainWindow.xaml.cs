using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml.Serialization;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using MahApps.Metro;


namespace MediaPlayer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {

        #region variables

        private player mediaPlayer;
        private bool _playlistVisible, _libraryVisible;
        private bool _isPlaying;
        private string _mediaName;
     
            
            
        public ObservableCollection<media> Playlist
        {
            get
            {
                return this.mediaPlayer.Playlist;
            }
            set
            {
                this.mediaPlayer.Playlist = value;
                OnPropertyChanged("Playlist");
            }
        }

        public ObservableCollection<media> soundLibrary
        {
            get
            {
                return this.mediaPlayer.LibraryPlayer.SoundLibrary;
            }
        }

        public ObservableCollection<media> videosLibrary
        {
            get
            {
                return this.mediaPlayer.LibraryPlayer.VideoLibrary;
            }
        }

        public ObservableCollection<media> picturesLibrary
        {
            get
            {
                return this.mediaPlayer.LibraryPlayer.PicturesLibrary;
            }
        }

        public ObservableCollection<media> allLibrary
        {
            get
            {
                return this.mediaPlayer.LibraryPlayer.AllLibrary;
            }
        }

        public ObservableCollection<media> researchLibrary
        {
            get
            {
                return this.mediaPlayer.LibraryPlayer.ResearchLibrary;
            }
        }

        public string MediaName
        {
            get
            {
                if (this._mediaName == "")
                    return ("Simplayer");
                return this._mediaName;
            }
            set
            {
                this._mediaName = value;
                OnPropertyChanged("MediaName");
            }
        }

        #endregion

        #region abonnement
        void mediaElementFailed(object sender, RoutedEventArgs e)
        {
            this.mediaPlayer.IsPlaying = false;
            this.mediaPlayer.IsPausing = false;
            BackgroundImage.Visibility = Visibility.Visible;
            this.mediaPlayer.mediaElement.Visibility = Visibility.Hidden;
        }

        void mediaElementOpened(object sender, RoutedEventArgs e)
        {
            this.MediaName = this.mediaPlayer.Playlist[this.mediaPlayer.IndexList].Title;
            if (this.mediaPlayer.mediaElement.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = MediaElement.NaturalDuration.TimeSpan;
                SeekBar.Maximum = ts.TotalSeconds;
                SeekBar.SmallChange = 1;
                SeekBar.LargeChange = Math.Min(10, ts.Seconds/10);
            }
            this.mediaPlayer.Timer.Start();
            BackgroundImage.Visibility = Visibility.Hidden;
            this.mediaPlayer.mediaElement.Visibility = Visibility.Visible;
        }

        void mediaElementEnded(object sender, RoutedEventArgs e)
        {
            BackgroundImage.Visibility = Visibility.Visible;
            this.mediaPlayer.mediaElement.Visibility = Visibility.Hidden;
           this.mediaPlayer.handleMediaEnded();
        }

        
        #endregion

        public MainWindow()
        {
           
            InitializeComponent();
            this.DataContext = this;
            this._libraryVisible = false;
            this._playlistVisible = false;
            this._isPlaying = false;
            this._mediaName = "SimPlayer";
            
            

            this.mediaPlayer = new player(ref MediaElement, ref SeekBar);

            MediaElement.MediaFailed += this.mediaElementFailed;
            MediaElement.MediaOpened += this.mediaElementOpened;
            MediaElement.MediaEnded += this.mediaElementEnded;

            
        }

        #region click
        private void openFileOnClick(object sender, RoutedEventArgs e)
        {
            this.mediaPlayer.openFileOnClick();
        }

        private void addToPlaylistOnClick(object sender, RoutedEventArgs e)
        {
            this.mediaPlayer.addToPlaylistOnClick();
        }

        private void playPauseClick(object sender, RoutedEventArgs e)
        {
            if (!this.mediaPlayer.IsPlaying && !this.mediaPlayer.IsPausing)
            {
                BackgroundImage.Visibility = Visibility.Hidden;
                this.mediaPlayer.mediaElement.Visibility = Visibility.Visible;
            }
            if (this._isPlaying)
            {
                if (this.mediaPlayer.IsPlaying)
                    this.handleStateButtonPlayPause(false);
                this.mediaPlayer.pauseClick();
                this._isPlaying = false;
            }
            else
            {
                if (this.mediaPlayer.IsPausing)
                    this.handleStateButtonPlayPause(true);
                this.mediaPlayer.playClick();
                this._isPlaying = true;
            }
        }
        

        private void stopClick(object sender, RoutedEventArgs e)
        {
            this.handleStateButtonPlayPause(false);
            this.mediaPlayer.stopClick();
            BackgroundImage.Visibility = Visibility.Visible;
            this.mediaPlayer.mediaElement.Visibility = Visibility.Hidden;
        }

        private void previousClik(object sender, RoutedEventArgs e)
        {
            this.mediaPlayer.previousClick();
        }

        private void nextClik(object sender, RoutedEventArgs e)
        {
           this.mediaPlayer.nextClick();
        }

        private void quitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void muteClick(object sender, RoutedEventArgs e)
        {
            if (this.mediaPlayer.mediaElement != null)
            {
               if (this.mediaPlayer.IsMute)
                {
                    this.mediaPlayer.IsMute = false;
                    this.changeImageButton(ButtonVolume, "/Resources/images/audioIcon.png");
                    this.mediaPlayer.mediaElement.Volume = 50;
                }
                else
                {
                    this.mediaPlayer.IsMute = true;
                    this.changeImageButton(ButtonVolume, "/Resources/images/audioIconActive.png");
                    this.mediaPlayer.mediaElement.Volume = 0;
                } 
            }
        }

        private void handlePlaylistClick(object sender, RoutedEventArgs e)
        {
            if (this._playlistVisible)
            {
                myPlayList.Visibility = Visibility.Hidden;
                this._playlistVisible = false;
                this.changeImageButton(OpenPlaylist, "/Resources/images/listIcon.png");
            }
            else
            {
                myPlayList.Visibility = Visibility.Visible;
                this._playlistVisible = true;
                this.changeImageButton(OpenPlaylist, "/Resources/images/listIconActive.png");
            }
        }

        private void handleLibraryClick(object sender, RoutedEventArgs e)
        {
            if (!this._libraryVisible)
            {
                choiceLibrary.Visibility = Visibility.Visible;
                mediaLibrary.Visibility = Visibility.Visible;
                MediaElement.Visibility = Visibility.Hidden;
                this._libraryVisible = true;
                this.changeImageButton(OpenLibrary, "/Resources/images/libraryIconActive.png");
            }
            else
            {
                choiceLibrary.Visibility = Visibility.Hidden;
                mediaLibrary.Visibility = Visibility.Hidden;
                MediaElement.Visibility = Visibility.Visible;
                this._libraryVisible = false;
                this.changeImageButton(OpenLibrary, "/Resources/images/libraryIcon.png");

            }
        }

        private void ClickAllCategoriesLibrary(object sender, MouseButtonEventArgs e)
        {
            mediaLibrary.ItemsSource = this.allLibrary;
        }

        private void ClickSoundLibrary(object sender, MouseButtonEventArgs e)
        {
            mediaLibrary.ItemsSource = this.soundLibrary;

        }

        private void ClickVideosCategoriesLibrary(object sender, MouseButtonEventArgs e)
        {
            mediaLibrary.ItemsSource = this.videosLibrary;
        }

        private void ClickPicturesLibrary(object sender, MouseButtonEventArgs e)
        {
            mediaLibrary.ItemsSource = this.picturesLibrary;
        }

        private void addToPlaylistFromLibraryClick(object sender, MouseButtonEventArgs e)
        {
            media media = new media();
            media = (media)mediaLibrary.SelectedItem;
            this.mediaPlayer.addToPlaylistFromLibraryClick(media);
        }

        private void clickPlayFromPlaylist(Object sender, EventArgs e)
        {
            this.mediaPlayer.clickPlayFromPlaylist(myPlayList.SelectedIndex);
        }

        private void clickRemoveFromPlaylist(Object sender, EventArgs e)
        {
            this.mediaPlayer.clickRemoveFromPlaylist(myPlayList.SelectedIndex, (media)myPlayList.SelectedItem);
        }

        private void clickActivateRandom(Object sender, EventArgs e)
        {
            if (this.mediaPlayer.IsRandom)
            {
                this.mediaPlayer.IsRandom = false;
                this.changeImageButton(randomButton, "/Resources/images/randomIcon.png");
            }
            else
            {
                this.mediaPlayer.IsRandom = true;
                this.changeImageButton(randomButton, "/Resources/images/randomIconActive.png");

            }
        }

        private void clickActivateShuffle(Object sender, EventArgs e)
        {
            if (this.mediaPlayer.IsShuffle)
            {
                this.mediaPlayer.IsShuffle = false;
                this.changeImageButton(shuffleButton, "/Resources/images/repeatIcon.png");
            }
            else
            {
                this.mediaPlayer.IsShuffle= true;
                this.changeImageButton(shuffleButton, "/Resources/images/repeatIconActive.png");
            }
        }

        private void savePlaylistClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Playlist file (.xml)|*.xml";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                this.mediaPlayer.fillXml(this.Playlist, filename);
            }
        }

        private void openPlaylistClick(object sender, RoutedEventArgs e)
        {
            try
            {

                OpenFileDialog filePicker = new OpenFileDialog();
                filePicker.Filter = "savePlaylist (*.xml) | *.xml";
                if (filePicker.ShowDialog() == true)
                {
                    foreach (String sourceFile in filePicker.FileNames)
                    {
                        if (System.IO.File.Exists(sourceFile))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<media>));
                            using (StreamReader rd = new StreamReader(sourceFile))
                            {
                                ObservableCollection<media> test = xs.Deserialize(rd) as ObservableCollection<media>;
                                this.Playlist = test;
                            }
                            this.stopClick(null, null);
                        }
                    }
                }

            }
            catch (Exception)
            {

            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            int i = 0;
            string _filename;
            List<string> tmp = new List<string>();
            _filename = (string)((DataObject)e.Data).GetFileDropList()[i];
           
           while (_filename != null)
            {
                if (System.IO.File.Exists(_filename))
                {
                    tmp.Add(_filename);
                }
                i++;
                if (i >= (((DataObject)e.Data).GetFileDropList()).Count)
                    _filename = null;
                else
                    _filename = (string)((DataObject)e.Data).GetFileDropList()[i];
            }
            this.mediaPlayer.addOnDrag(tmp);
            if (!this.mediaPlayer.IsPausing && !this.mediaPlayer.IsPlaying)
                this.playPauseClick(null, null);
        }
        #endregion


        #region seekbarAndVolumeAndTools

        private void SeekBar_OnDragStarted(object sender, DragStartedEventArgs e)
        {
           this.mediaPlayer.IsDragging = true;
        }

        private void seekBar_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.mediaPlayer.IsDragging = false;
            this.mediaPlayer.mediaElement.Position = TimeSpan.FromSeconds(SeekBar.Value);
        }

        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {    
            if (this.mediaPlayer != null)
            this.mediaPlayer.mediaElement.Volume =  (double)volumeSlider.Value;
        }

        private void handleStateButtonPlayPause(Boolean value)
        {
            if (value)
            {
                this._isPlaying = true;
                this.changeImageButton(ButtonPlayPause, "/Resources/images/pauseIcon.png");
            }
            else
            {
                this._isPlaying = false;
                this.changeImageButton(ButtonPlayPause, "/Resources/images/playIcon.png");
            }
        }

        

        private void changeImageButton(Button button, string path)
        {
            button.Content = new Image
            {
                Source = new BitmapImage(new Uri(path, UriKind.Relative)),
            };
        }

       
        #endregion

        #region interfaceImplemented

        private void OnPropertyChanged(string p)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(p));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private void searchElementInLibrary(object sender, KeyEventArgs e)
        {
            TextBox objTextBox = (TextBox)sender;
            if (objTextBox.Text != "")
            {
                this.mediaPlayer.LibraryPlayer.researchInCollection(this.allLibrary, objTextBox.Text);
                mediaLibrary.ItemsSource = this.researchLibrary;                                     
            }
        }

        
    }
}
