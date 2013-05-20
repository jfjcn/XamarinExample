using System;
using System.Collections.Generic;

namespace WebApiExamples.Domain.CDCollection
{
    public class Album
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime ReleaseDate { get; set; }

        public List<int> ArtistIds { get; set; }











        public Album()
        {
            ArtistIds = new List<int>();
        }

        public Album(Album albumToCopy) : this()
        {
            Id = albumToCopy.Id;
            Title = albumToCopy.Title;
            ReleaseDate = albumToCopy.ReleaseDate;
            foreach (var artistId in albumToCopy.ArtistIds)
            {
                ArtistIds.Add(artistId);
            }
        }

        protected bool Equals(Album other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Album)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format(
                "{0}:{1}",
                Id, Title);
        }
    }
}
