using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiExamples.Domain.CDCollection;
using WebApiExamples.Services.CDCollection;

namespace WebApiExamples.Controllers
{
    public class AlbumController : ApiController
    {
        protected AlbumDataService  AlbumDataService = new AlbumDataService();

        // GET http://localhost:4848/api/album/
        public IEnumerable<Album> GetAllAlbums()
        {
            var allAlbums = AlbumDataService.GetAll();
            return allAlbums;
        }

        // GET http://localhost:4848/api/album/5
        public Album GetById(int id)
        {
            var album = AlbumDataService.Get(id);
            if (album == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return album;
        }

        // GET http://localhost:4848/api/album?albumTitle=AlbumTitleGoesHere
        public Album GetByTitle(string albumTitle)
        {
            var matchingArtist = AlbumDataService.Get(albumTitle);
            return matchingArtist;
        }

        //POST http://localhost:4848/api/album
        public HttpResponseMessage PostAlbum(Album albumToCreate)
        {
            var createdAlbum =
                AlbumDataService.Create(
                    albumToCreate.Title,
                    albumToCreate.ReleaseDate,
                    albumToCreate.ArtistIds);

            var response = Request.CreateResponse(HttpStatusCode.Created, createdAlbum);

            string uri = Url.Link("DefaultApi", new { id = createdAlbum.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        // PUT http://localhost:4848/api/album/5
        public void PutAlbum(int id, Album albumToUpdate)
        {
            albumToUpdate.Id = id;
            var successfullyUpdated = AlbumDataService.Update(albumToUpdate);
            if (!successfullyUpdated)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        // DELETE http://localhost:4848/api/album/5
        public void DeleteAlbum(int id)
        {
            AlbumDataService.Delete(id);
        }
    }
}
