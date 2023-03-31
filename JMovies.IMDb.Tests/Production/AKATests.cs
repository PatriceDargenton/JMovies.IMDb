﻿using JMovies.IMDb.Entities.Interfaces;
using JMovies.IMDb.Entities.Settings;
using JMovies.IMDb.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JMovies.IMDb.Tests.Production
{    /// <summary>
     /// Test class that contains the tests which are targeted for scraping AKAs related tests
     /// </summary>
    [TestClass]
    public class AKATests
    {
        /// <summary>
        /// Test Method that tests AKAs of the movie "Song of the South"
        /// </summary>
        [TestMethod]
        public void TestAKAsOfSongOfTheSouth()
        {
            IIMDbDataProvider imdbDataProvider = new IMDbScraperDataProvider();
            ProductionDataFetchSettings settings = new ProductionFetchSettingsBuilder().WithDetailedCast().WithPrivateData().
                WithFetchingImageContents().WithLimitedImageFetching(55).WithFetchingImageContents().Build();
            Entities.Movies.Movie movie = imdbDataProvider.GetMovie(38969, settings); // https://www.imdb.com/title/tt0038969/
            Assert.IsNotNull(movie);
            Assert.IsNotNull(movie.AKAs);
            Assert.AreEqual(32, movie.AKAs.Count);
        }
    }
}
