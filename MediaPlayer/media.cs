using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer
{
    public class media
    {

           public media(string title, string path, string duration, string author ="", string genre ="", string album="")
            {
               this.Path = path;
               this.Title = title;
               this.Duration = duration;
               this.Author = author;
               this.Genre = genre;
               this.Album = album;
            }

            public media()
            {
            
            }

            public string Title { get;  set; }
            public string Path { get;  set; }
            public string Duration { get;  set; }
            public string Author { get;  set; }
            public string Genre { get; set; }
            public string Album { get; set; }

            public override string ToString()
            {
                return this.Title;
            }
        }
}
