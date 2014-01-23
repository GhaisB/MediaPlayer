using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace MediaPlayer
{
    class player : INotifyPropertyChanged
    {
        #region propertychanged

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

        #region variables

        private int _indexList = 0;
        public Uri _mediaSource;
        private String _mediaName;

        private Boolean _isMute;
        private Boolean _isPlaying;
        private Boolean _isPausing;
        private Boolean _isRandom;
        private Boolean _isShuffle;

        private Boolean _isDragging;
        private DispatcherTimer _timer;

        private ObservableCollection<media> _playlist;

        private libraryPlayer _libraryPlayer;

        private Slider _seekBar;

        private string[] _fileFilter;

        

        public int IndexList
        {
            get
            {
                return _indexList;
            }
            set
            {
                _indexList = value;
                OnPropertyChanged("IndexList");
            }
        }
        
        public String MediaName
        {
            get
            {
                return _mediaName;
            }
            set
            {
                _mediaName = value;
                OnPropertyChanged("MediaName");
            }
        }

        public Boolean IsMute
        {
            get
            {
                return _isMute;
            }
            set
            {
                _isMute = value;
                OnPropertyChanged("IsMute");
            }
        }
        public Boolean IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                _isPlaying = value;
                OnPropertyChanged("IsMute");
            }
        }
        public Boolean IsPausing
        {
            get
            {
                return _isPausing;
            }
            set
            {
                _isPausing = value;
                OnPropertyChanged("IsMute");
            }
        }
        public Boolean IsRandom
        {
            get
            {
                return _isRandom;
            }
            set
            {
                _isRandom = value;
                OnPropertyChanged("IsRandom");
            }
        }
        public Boolean IsShuffle
        {
            get
            {
                return _isShuffle;
            }
            set
            {
                _isShuffle = value;
                OnPropertyChanged("IsShuffle");
            }
        }

        public Boolean IsDragging
        {
            get
            {
                return _isDragging;
            }
            set
            {
                _isDragging = value;
                OnPropertyChanged("isDragging");
            }
        }

        public DispatcherTimer Timer
        {
            get
            {
                return _timer;
            }
            set
            {
                _timer = value;
                OnPropertyChanged("Timer");
            }
        }

        public ObservableCollection<media> Playlist
        {
            get
            {
                return _playlist;
            }
            set
            {
                _playlist = value;
                OnPropertyChanged("Playlist");
            }
        }

        public libraryPlayer LibraryPlayer
        {
            get
            {
                return _libraryPlayer;
            }
            set
            {
                _libraryPlayer = value;
                OnPropertyChanged("LibraryPlayer");
            }
        }

        public MediaElement mediaElement;

        public Uri MediaSource
        {
            get
            {
                //Console.WriteLine("on est dans le getter de MediaSource");
                return this._mediaSource;
            }
            set
            {
              
                this._mediaSource = value;
                OnPropertyChanged("MediaSource");
            }
        }


        #endregion

        public player(ref MediaElement media, ref Slider seekBar)
        {
            this.mediaElement = media;
            this._fileFilter = new string[8] { ".mp3", ".wmv", ".mp4", ".avi", ".mov", ".jpg", ".bmp", ".png" };
            this._libraryPlayer = new libraryPlayer();

            this._isPausing = false;
            this._isPlaying = false;
            this._isDragging = false;
            this._isRandom = false;
            this._isShuffle = false;
            this._isMute = false;
            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(200);
            this._seekBar = seekBar;
            this._timer.Tick += new EventHandler(timerTick);
            this._playlist = new ObservableCollection<media>();
        }

        #region timerEtUri
        private void timerTick(Object sender, EventArgs e)
        {
            if (!this.IsDragging)
            {
                _seekBar.Value = this.mediaElement.Position.TotalSeconds;
            }
        }

        public void handleMediaEnded()
        {
            if (this.IndexList + 1 == this._playlist.Count)
            {
               this.stop();
                this.IndexList = 0;
                return;
            }
            this.next();
        }

        private void createUri()
        {
            String pathMedia;
           
            pathMedia = this._playlist[this._indexList].Path;

            this._mediaSource = new Uri(pathMedia, UriKind.Absolute);
            this.mediaElement.Source = this._mediaSource;
        }

        #endregion 

        
        #region controleMedia

        private void play()
        {
            if (this.Playlist.Count > 0)
            {
                if (!this._isPausing && !this._isPlaying)
                {
                    Console.WriteLine("on est dans la creation");
                    createUri();
                }
                this._isPlaying = true;
                this._isPausing = false;

                this.mediaElement.Play();
            }
        }

        private void pause()
        {
            this.mediaElement.Pause();
            this._isPlaying = false;
            this._isPausing = true;

        }

        private void stop()
        {
            this.mediaElement.Stop();
            this._isPlaying = false;
            this._isPausing = false;
        }

        private void next()
        {
            this.stop();
            if (this._isRandom)
            {
                Random random = new Random();
                int randomNumber = this.IndexList;
                while (randomNumber == this.IndexList)
                    randomNumber = random.Next(0, this._playlist.Count);
                this.IndexList = randomNumber;
            }
            else if (this._isShuffle)
            {
                this._indexList = this._indexList;
            }
            else
            {
                this._indexList++;                    
            }
            Console.WriteLine(this._playlist.Count);
            if (this._indexList >= this._playlist.Count)
                this._indexList = this._playlist.Count - 1;
            this.play();
        }

        private void previous()
        {
            this.stop();
            this._indexList--;
            if (this._indexList <= 0)
                this._indexList = 0;
            this.play();
        }

        #endregion

        #region clickPlayer

        public void playClick()
        {
            if ((!this._isPlaying && this._isPausing) || (!this._isPlaying && !this._isPausing)) 
                this.play();
        }

        public void pauseClick()
        {
            if (this._isPlaying && !this._isPausing)
                this.pause();
        }

        public void stopClick()
        {
            this.stop();
        }

        public void previousClick()
        {
            this.previous();            
        }

        public void nextClick()
        {
            this.next();
        }

        public void addToPlaylistFromLibraryClick(media media)
        {
            if (media != null)
            {
                if (System.IO.File.Exists(media.Path))
                    this._playlist.Add(media);
            }
        }

        public void clickPlayFromPlaylist(int index)
        {
            this.stop();
            this._indexList = index;
            this.play();
        }

        public void clickRemoveFromPlaylist(int index, media media)
        {
            if (index >= 0)
            {
                this.Playlist.Remove(media);
                if (index == this.IndexList)
                {
                    this.stop();
                    this.next();
                }
            }
        }

        public void openFileOnClick()
        {
           
            this._playlist.Clear();
            OpenFileDialog filePicker = new OpenFileDialog();
            filePicker.Filter =
                "videos (*.mp4, *.avi, *.wmv, *.mov)|*.mp4;*.avi;.*.wmv;*.mov| pictures (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp | music (*.mp3, *.ogg, *.flac, *.wmv) | *.mp3;*.ogg;*.flac;*.wmv";
            if (filePicker.ShowDialog() == true)
            {
                foreach (String sourceFile in filePicker.FileNames)
                {
                    if (System.IO.File.Exists(sourceFile))
                    {
                        this._playlist.Add(new media(System.IO.Path.GetFileNameWithoutExtension(sourceFile), sourceFile, this.LibraryPlayer.getDuration(sourceFile)));
                    }
                }
            }
            else
                return;
            if (this._isPlaying)
                this.stop();
            this.play();
        }

        public void addToPlaylistOnClick()
        {
            OpenFileDialog filePicker = new OpenFileDialog();
            filePicker.Multiselect = true;
            filePicker.Filter = "videos (*.mp4, *.avi, *.wmv, *.mov)|*.mp4;*.avi;.*wmv;*.mov| pictures (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp | music (*.mp3, *.ogg, *.flac, *.wmv) | *.mp3;*.ogg;*.flac;*.wmv";
            if (filePicker.ShowDialog() == true)
            {
                foreach (String sourceFile in filePicker.FileNames)
                {
                    if (System.IO.File.Exists(sourceFile))
                    {
                        this._playlist.Add(new media(System.IO.Path.GetFileNameWithoutExtension(sourceFile), sourceFile, this.LibraryPlayer.getDuration(sourceFile)));
                    }
                }
            }
            else
                return;
        }

        public void addOnDrag(List<string> tmp)
        {
            foreach (String sourceFile in tmp)
            {
                if (System.IO.File.Exists(sourceFile) && this.isFileOk(sourceFile))
                {
                    this._playlist.Add(new media(System.IO.Path.GetFileNameWithoutExtension(sourceFile), sourceFile,
                        this.LibraryPlayer.getDuration(sourceFile)));
                }
            }
        }

        private bool isFileOk(string path)
        {
            string extensionFile;
            extensionFile = Path.GetExtension(path);
            Console.WriteLine("on a comme extension" + extensionFile);
            foreach (string filter in this._fileFilter)
            {
                if (filter == extensionFile)
                    return true;
            }
            Console.WriteLine("yop");
            return false;
        }

        public void fillXml(ObservableCollection<media> list, string name)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<media>));
            using (StreamWriter wr = new StreamWriter(name))
            {
                xs.Serialize(wr, list);
            }
        }

        #endregion
    }
}
