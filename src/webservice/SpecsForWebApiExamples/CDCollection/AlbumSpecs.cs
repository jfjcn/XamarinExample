using System;
using System.Collections.Generic;
using NUnit.Framework;
using WebApiExamples.Domain.CDCollection;

namespace SpecsForWebApiExamples.CDCollection
{
    [TestFixture]
    public class When_using_the_album_api
    {

        protected List<Album> _albums;
        protected static RestClientForTesting _albumRestClient;
        protected static RestClientForTesting _artistRestClient;
        protected static readonly string BaseUrl = @"http://localhost/api/";
        protected static readonly string AlbumRelativePath = @"album";
        protected static readonly string ArtistRelativePath = @"artist";

        [TestFixtureSetUp]
        public static void SetUpForFixture()
        {
            _albumRestClient = new RestClientForTesting(BaseUrl);
            _artistRestClient = new RestClientForTesting(BaseUrl);
        }

        [Test]
        public void _001_there_should_be_no_albums_in_the_API()
        {
            var response = _albumRestClient.GetMany<Album>(AlbumRelativePath);
            var allArtists = response.ReturnedObject;
            Assert.That(allArtists, Is.Not.Null);
            Assert.That(allArtists.Count, Is.EqualTo(0));
        }

        [Test]
        public void _010_we_should_be_able_to_add_an_album_named_Raising_Snd_with_no_artists_into_our_API()
        {
            var raisingSand = new Album { Title = "Raising Snd", ReleaseDate = new DateTime(2007, 10, 23)};
            var response = _albumRestClient.Post(AlbumRelativePath, raisingSand);
            Assert.That(response.Success, Is.True);
            Assert.That(response.ResourceUri, Is.Not.Null);
        }

        [Test]
        public void _020_we_should_be_able_to_find_an_album_titled_Raising_Snd_from_our_API()
        {
            var raisingSandAlbumTitle = "Raising Snd";
            var response = _albumRestClient.GetSingle<Album>(AlbumRelativePath + "?albumTitle=" + raisingSandAlbumTitle);
            var raisingSand = response.ReturnedObject;
            Assert.That(raisingSand, Is.Not.Null);
            Assert.That(raisingSand.Title, Is.EqualTo(raisingSandAlbumTitle));
        }

        [Test]
        public void _030_we_should_be_able_to_rename_our_album_titled_Raising_Sand_to_the_correct_spelling_in_our_API()
        {
            var raisingSandAlbumTitleMisspelled = "Raising Snd";
            var response = _albumRestClient.GetSingle<Album>(AlbumRelativePath + "?albumTitle=" + raisingSandAlbumTitleMisspelled);
            var raisingSand = response.ReturnedObject;
            Assert.That(raisingSand, Is.Not.Null);
            Assert.That(raisingSand.Title, Is.EqualTo(raisingSandAlbumTitleMisspelled));

            var raisingSandProperlySpelled = "Raising Sand";
            raisingSand.Title = raisingSandProperlySpelled;
            var secondResponse = _albumRestClient.Put(AlbumRelativePath + "/" + raisingSand.Id, raisingSand);

            var thirdResponse = _albumRestClient.GetSingle<Album>(AlbumRelativePath + "/" + raisingSand.Id);
            var newlyUpdatedRaisingSand = thirdResponse.ReturnedObject;
            Assert.That(newlyUpdatedRaisingSand, Is.Not.Null);
            Assert.That(newlyUpdatedRaisingSand.Title, Is.EqualTo(raisingSandProperlySpelled));

        }

        [Test]
        public void _100_when_we_clear_out_the_database_the_count_should_be_zero()
        {
            ClearOutAllEntitiesInTheRemoteApi();
        }

        [Test]
        [Ignore]
        public void _500_we_should_be_able_to_add_a_few_artists_and_albums_into_our_API()
        {
            var newArtist = new Artist { Name = "Robert Plant" };
            var response = _artistRestClient.Post(ArtistRelativePath, newArtist);
            Assert.That(response.Success, Is.True);
            Assert.That(response.ResourceUri, Is.Not.Null);
            var robertPlantId = int.Parse(response.ResourceParsedId);

            newArtist = new Artist { Name = "Alison Krauss" };
            response = _artistRestClient.Post(ArtistRelativePath, newArtist);
            Assert.That(response.Success, Is.True);
            Assert.That(response.ResourceUri, Is.Not.Null);
            var alisonKraussId = int.Parse(response.ResourceParsedId);

            var newAlbum =
                new Album
                    {
                        Title = "Raising Sand",
                        ReleaseDate = new DateTime(2007, 10, 23),
                        ArtistIds = new List<int> {robertPlantId, alisonKraussId}
                    };
            response = _albumRestClient.Post(AlbumRelativePath, newAlbum);
            Assert.That(response.Success, Is.True);
            Assert.That(response.ResourceUri, Is.Not.Null);

            newAlbum =
                new Album
                {
                    Title = "Pictures at Eleven",
                    ReleaseDate = new DateTime(1982, 6, 28),
                    ArtistIds = new List<int> { robertPlantId }
                };
            response = _albumRestClient.Post(AlbumRelativePath, newAlbum);
            Assert.That(response.Success, Is.True);
            Assert.That(response.ResourceUri, Is.Not.Null);

        }

        private static void ClearOutAllEntitiesInTheRemoteApi()
        {
            var response = _albumRestClient.GetMany<Album>(AlbumRelativePath);
            var allAlbums = response.ReturnedObject;
            if (allAlbums == null)
            {
                return;
            }
            foreach (var album in allAlbums)
            {
                var deleteResponse = _albumRestClient.Delete(AlbumRelativePath, album.Id);
                Assert.That(deleteResponse, Is.Not.Null);
            }
        }
    }
}
