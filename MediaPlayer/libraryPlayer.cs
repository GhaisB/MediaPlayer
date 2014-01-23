using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using MediaPlayer.Annotations;
using WMPLib;

namespace MediaPlayer
{

    class libraryPlayer : INotifyPropertyChanged
    {
        #region variables
        private ObservableCollection<media> _soundLibrary, _videoLibrary, _pictureLibrary, _allLibrary, _researchLibrary;

        public ObservableCollection<media> SoundLibrary
        {
            get
            {
                return _soundLibrary;
            }
            set
            {
                _soundLibrary = value;
                OnPropertyChanged("SoundLibrary");
            }
        }
        public ObservableCollection<media> VideoLibrary
        {
            get
            {
                return _videoLibrary;
            }
            set
            {
                _soundLibrary = value;
                OnPropertyChanged("VideoLibrary");
            }
        }
        public ObservableCollection<media> PicturesLibrary
        {
            get
            {
                return _pictureLibrary;
            }
            set
            {
                _soundLibrary = value;
                OnPropertyChanged("PictureLibrary");
            }
        }
        public ObservableCollection<media> AllLibrary
        {
            get
            {
                return _allLibrary;
            }
            set
            {
                _allLibrary = value;
                OnPropertyChanged("AllLibrary");
            }
        }
        public ObservableCollection<media> ResearchLibrary
        {
            get
            {
                return _researchLibrary;
            }
            set
            {
                _researchLibrary = value;
                OnPropertyChanged("ResearchLibrary");
            }
        }

        #endregion

        public libraryPlayer()
        {
            
           

            this._soundLibrary = new ObservableCollection<media>();
            this._videoLibrary = new ObservableCollection<media>();
            this._pictureLibrary = new ObservableCollection<media>();
            this._allLibrary = new ObservableCollection<media>();
            this._researchLibrary = new ObservableCollection<media>();

            this.fillLibrary();
            try
            {

               
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region operationLibrary

        public async void fillLibrary()
        {
            await doAsyncFill();
        }

        private async Task<bool> doAsyncFill()
        {
            string[] musicFilter, videosFilter, picturesFilter;


            musicFilter = new string[2] { "*.mp3", "*.wmv" };
            videosFilter = new string[4] { "*.mp4", "*avi", "*.wmv", "*.mov" };
            picturesFilter = new string[3] { "*.jpg", "*.bmp", "*.png" };

            this.MakeFileList(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), videosFilter, VideoLibrary);
            this.MakeFileList(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), picturesFilter, PicturesLibrary);
            this.MakeFileList(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), musicFilter, SoundLibrary);

            foreach (var p in this.VideoLibrary.Union(this.PicturesLibrary.Union(this.SoundLibrary)))
                this.AllLibrary.Add(p);
            return true;
        }

        private void MakeFileList(string path, string[] filter, ObservableCollection<media> tabToFill)
        {
            foreach (string a in filter)
            {
                string[] filePaths = Directory.GetFiles(path, a, SearchOption.AllDirectories);
                foreach (string filePath in filePaths)
                {
                    FileInfo tmp;
                    tmp = new FileInfo(filePath);
                    tabToFill.Add(new media(Path.GetFileNameWithoutExtension(filePath), filePath, getDuration(filePath), getArtist(filePath), getGenre(filePath), getAlbum(filePath)));
                }
            }
       }

        public void researchInCollection(ObservableCollection<media> collection, string toFind)
        {
            this._researchLibrary.Clear();
            IEnumerable<media> requeteFiltree = from i in collection where Regex.IsMatch(i.Title, toFind, RegexOptions.IgnoreCase) || 
                                                    Regex.IsMatch(i.Genre, toFind, RegexOptions.IgnoreCase) ||
                                                    Regex.IsMatch(i.Author, toFind, RegexOptions.IgnoreCase) ||
                                                    Regex.IsMatch(i.Album, toFind, RegexOptions.IgnoreCase)
                                                select i;
                                             
            this._researchLibrary = requeteFiltree.ToObservableCollection();
        }

        #endregion

        #region getter
        public string getDuration(String file)
        {
            string result;
            WindowsMediaPlayer wmp = new WindowsMediaPlayer();
            IWMPMedia mediainfo = wmp.newMedia(file);
            var timespan = TimeSpan.FromSeconds(mediainfo.duration);
            result = timespan.ToString(@"hh\:mm\:ss");
            return result;
        }

        public string getArtist(string file)
        {
            string result;
            WindowsMediaPlayer wmp = new WindowsMediaPlayer();
            IWMPMedia mediainfo = wmp.newMedia(file);
            result = mediainfo.getItemInfo("Actor");
            return result;
        }

        public string getGenre(string file)
        {
            string result;
            WindowsMediaPlayer wmp = new WindowsMediaPlayer();
            IWMPMedia mediainfo = wmp.newMedia(file);
            result = mediainfo.getItemInfo("Genre");
            return result;
        }

        public string getAlbum(string file)
        {
            string result;
            WindowsMediaPlayer wmp = new WindowsMediaPlayer();
            IWMPMedia mediainfo = wmp.newMedia(file);
            result = mediainfo.getItemInfo("Album");
            return result;
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

    }
}
