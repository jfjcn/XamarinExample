using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiExamples.Domain.CDCollection;
using WebApiExamples.Services.CDCollection;

namespace WebApiExamples.Controllers
{
    public class ArtistController : ApiController
    {

        protected ArtistDataService ArtistDataService = new ArtistDataService();

        // GET http://localhost:4848/api/artist
        public IEnumerable<Artist> GetAllArtists()
        {
            var allArtists = ArtistDataService.GetAll();
            return allArtists;
        }

        // GET http://localhost:4848/api/artist/5
        public Artist GetById(int id)
        {
            var artist = ArtistDataService.Get(id);
            if (artist == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return artist;
        }

        // GET http://localhost:4848/api/artist?artistName=NameGoesHere
        public Artist GetByName(string artistName)
        {
            var matchingArtist = ArtistDataService.Get(artistName);
            return matchingArtist;
        }

        // POST http://localhost:4848/api/artist
        public HttpResponseMessage PostProduct(Artist artistToCreate)
        {
            var createdArtist = ArtistDataService.Create(artistToCreate.Name);

            var response = Request.CreateResponse(HttpStatusCode.Created, createdArtist);

            string uri = Url.Link("DefaultApi", new {id = createdArtist.Id});
            response.Headers.Location = new Uri(uri);
            return response;
        }

        // PUT http://localhost:4848/api/artist/5
        public void PutArtist(int id, Artist artistToUpdate)
        {
            artistToUpdate.Id = id;
            var successfullyUpdated = ArtistDataService.Update(artistToUpdate);
            if (!successfullyUpdated)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        // DELETE http://localhost:4848/api/artist/5
        public void DeleteArtist(int id)
        {
            ArtistDataService.Delete(id);
        }
    }
}
