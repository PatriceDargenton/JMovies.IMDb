﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JMovies.IMDb.Entities.Settings.Presets
{
    /// <summary>
    /// A preset of data fetch settings that fetches all the data available for productions
    /// </summary>
    public class FullProductionDataFetchSettings : ProductionDataFetchSettings
    {
        /// <summary>
        /// Fetch the detailed cast
        /// </summary>
        public override bool FetchDetailedCast { get => true; }

        /// <summary>
        /// Fetch the image contents
        /// </summary>
        public override bool FetchImageContents { get => true; }

        /// <summary>
        /// Fetch 25 media images
        /// </summary>
        public override int MediaImagesFetchCount { get => 25; }
    }
}
