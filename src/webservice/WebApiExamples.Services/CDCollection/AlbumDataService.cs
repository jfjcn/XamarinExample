using System;
using System.Collections.Generic;
using WebApiExamples.Domain.CDCollection;

namespace WebApiExamples.Services.CDCollection
{
    /// <summary>
    /// Album data service serves the purpose of performing CRUD operations
    /// for Albums and is NOT thread-safe.  Used to showcase REST API built
    /// on top of these basic CRUD operations.
    /// </summary>
    public class AlbumDataService
    {
        private static Dictionary<int, Album> AlbumDatabase;

        private static int NextId = 1;

        static AlbumDataService()
        {
            InitializeDB();
        }

        /// <summary>
        /// Creates a new Album in the "DB" with a name of albumTitle, 
        /// a release date of releaseDate, and assigns an Id to the Album.
        /// </summary>
        /// <param name="albumTitle"></param>
        /// <param name="releaseDate"> </param>
        /// <param name="artistIds"> </param>
        /// <returns>The newly created Album</returns>
        public Album Create(string albumTitle, DateTime releaseDate, List<int> artistIds)
        {
            var newAlbum = 
                new Album
                    {
                        Id = NextId++, 
                        Title = albumTitle,
                        ReleaseDate = releaseDate
                    };
            if(artistIds != null)
            {
                foreach (var artistId in artistIds)
                {
                    newAlbum.ArtistIds.Add(artistId);
                }
            }
            AddAlbumToDB(newAlbum);
            return newAlbum;
        }

        /// <summary>
        /// Updates albumToUpdate in the "DB"
        /// </summary>
        /// <param name="albumToUpdate"></param>
        /// <returns>true if Album exists, false otherwise</returns>
        public bool Update(Album albumToUpdate)
        {
            if(!AlbumDatabase.ContainsKey(albumToUpdate.Id))
            {
                return false;
            }
            UpdateAlbumFromDBWith(albumToUpdate);
            return true;
        }

        /// <summary>
        /// Get the Album with albumId from the "DB"
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns>The Album with Id albumId, otherwise null</returns>
        public Album Get(int albumId)
        {
            if(!AlbumDatabase.ContainsKey(albumId))
            {
                return null;
            }
            return new Album(AlbumDatabase[albumId]);
        }

        /// <summary>
        /// Get the Album with albumTitle from the "DB"
        /// </summary>
        /// <param name="albumTitle"></param>
        /// <returns>The Album with Name albumTitle, otherwise null</returns>
        public Album Get(string albumTitle)
        {
            foreach (var Album in AlbumDatabase.Values)
            {
                if(albumTitle.Equals(Album.Title, StringComparison.InvariantCulture))
                {
                    return new Album(Album);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all the Albums form the DB
        /// </summary>
        /// <returns></returns>
        public List<Album> GetAll()
        {
            var copyOfAllAlbums = new List<Album>();
            foreach (var albumToCopy in AlbumDatabase.Values)
            {
                copyOfAllAlbums.Add(new Album(albumToCopy));
            }
            return copyOfAllAlbums;
        }

        /// <summary>
        /// Delete the Album with Id albumId from the "DB"
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns>true if albumId is in the "DB," otherwise null</returns>
        public bool Delete(int albumId)
        {
            if (!AlbumDatabase.ContainsKey(albumId))
            {
                return false;
            }
            WipeOutAlbumFromDBWithId(albumId);
            return true;
        }

        /// <summary>
        /// Gets the total number of Albums in the "DB"
        /// </summary>
        public int TotalCount { get { return AlbumDatabase.Count; } }

        /// <summary>
        /// Clears out all the records from the "DB"
        /// </summary>
        public void ClearAll()
        {
            InitializeDB();
        }

        private static void InitializeDB()
        {
            AlbumDatabase = new Dictionary<int, Album>();
        }

        private static void AddAlbumToDB(Album newAlbum)
        {
            AlbumDatabase.Add(newAlbum.Id, newAlbum);
        }

        private static void UpdateAlbumFromDBWith(Album albumToUpdate)
        {
            AlbumDatabase[albumToUpdate.Id] = albumToUpdate;
        }

        private static void WipeOutAlbumFromDBWithId(int albumId)
        {
            AlbumDatabase.Remove(albumId);
        }
    }
}
