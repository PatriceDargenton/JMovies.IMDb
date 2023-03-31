﻿using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using JMovies.IMDb.Common.Constants;
using JMovies.IMDb.Common.Extensions;
using JMovies.IMDb.Entities.Movies;
using JMovies.IMDb.Entities.PrivateAPI;
using JMovies.IMDb.Entities.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JMovies.IMDb.Helpers.Movies
{
    /// <summary>
    /// Helper Class responsible for parsing the Release Info Page
    /// </summary>
    public static class ReleaseInfoPageHelper
    {
        /// <summary>
        /// Parses the Release Info Page and persists the data on Movie instance
        /// </summary>
        /// <param name="movie">Movie object to be populated</param>
        /// <param name="releaseInfoPageDocument">HTML Document of the Release Info Page</param>
        /// <param name="settings">Object containing Data Fetch settings</param>
        public static void Parse(Movie movie, HtmlDocument releaseInfoPageDocument, ProductionDataFetchSettings settings)
        {
            #region Release Dates
            List<ReleaseDate> releaseDates = new List<ReleaseDate>();
            IEnumerable<HtmlNode> relaseDateRows = releaseInfoPageDocument.DocumentNode.QuerySelectorAll("[data-testid=sub-section-releases]>ul>li");
            if (relaseDateRows != null)
            {
                foreach (HtmlNode releaseDateRow in relaseDateRows)
                {
                    ReleaseDate releaseDate = new ReleaseDate();
                    releaseDate.Country = new Country();
                    HtmlNode[] releaseDateColumns = releaseDateRow.ChildNodes.ToArray();
                    HtmlNode releaseDateCountryLink = releaseDateColumns[0];
                    Match countryMatch = IMDbConstants.ReleaseDateCountryIdentifierRegex.Match(releaseDateCountryLink.GetAttributeValue("href", string.Empty));
                    releaseDate.Country.Name = releaseDateCountryLink.InnerText.Prepare();
                    if (countryMatch.Success)
                    {
                        releaseDate.Country.Identifier = countryMatch.Groups[1].Value;
                    }

                    // 31/03/2023
                    //string releaseDateString = releaseDateColumns[1].QuerySelector("label").InnerText.Prepare();
                    var a = releaseDateColumns[1];
                    var b = a.QuerySelector("label");
                    if (b != null) { 
                        var c = b.InnerText;
                        string releaseDateString =c.Prepare();

                        Match allNumericReleaseDateMatch = GeneralRegexConstants.AllNumericRegex.Match(releaseDateString);
                        if (allNumericReleaseDateMatch.Success)
                        {
                            releaseDate.Date = new DateTime(allNumericReleaseDateMatch.Groups[1].Value.ToInteger(), 1, 1);
                        }
                        else
                        {
                            releaseDate.Date = DateTime.Parse(releaseDateString);
                        }
                    }

                    HtmlNode releaseDateDescriptionNode = releaseDateColumns[1].QuerySelector("label+span");
                    if (releaseDateDescriptionNode != null)
                    {
                        releaseDate.Description = releaseDateDescriptionNode.InnerText.Prepare();
                        if (releaseDate.Description.Count(e => e == '(') == 1)
                        {
                            releaseDate.Description = releaseDate.Description.TrimStart('(').TrimEnd(')');
                        }
                    }
                    releaseDates.Add(releaseDate);
                }
            }
            movie.ReleaseDates = releaseDates;
            #endregion
            #region AKAs
            IEnumerable<HtmlNode> akaRows = releaseInfoPageDocument.DocumentNode.QuerySelectorAll("[data-testid=sub-section-akas]>ul>li");
            if (akaRows != null)
            {
                List<AKA> akas = new List<AKA>();
                foreach (HtmlNode akaRow in akaRows)
                {
                    AKA aka = new AKA();
                    
                    // 10/03/2023
                    //aka.Description = akaRow.QuerySelector("button").InnerText.Prepare().TrimStart('(').TrimEnd(')');
                    var a = akaRow.QuerySelector("button");
                    if (a == null) continue;
                    var b = a.InnerText;
                    var c = b.Prepare();
                    var d = c.TrimStart('(');
                    var e = d.TrimStart(')');
                    aka.Description = e;
                    
                    aka.Name = akaRow.QuerySelector("label").InnerText.Prepare();
                    akas.Add(aka);
                }

                HtmlNode moreButton = releaseInfoPageDocument.DocumentNode.QuerySelector("[data-testid=sub-section-akas]>ul>div button");
                if (moreButton != null && settings.FetchPrivateData)
                {
                    //Has More
                    string getMoreURL = $"https://caching.graphql.imdb.com/?operationName=TitleAkasPaginated&" +
                        $"variables={{\"after\":\"NA==\",\"const\":\"{IMDbConstants.MovieIDPrefix}{IMDBIDHelper.GetPaddedIMDBId(movie.IMDbID)}\",\"first\":150,\"locale\":\"{settings.GetActiveCulture()}\",\"originalTitleText\":false}}&extensions={{\"persistedQuery\":{{\"sha256Hash\":\"180f0f5df1b03c9ee78b1f410d65928ec22e7aca590e5321fbb6a6c39b802695\",\"version\":1}}}}";
                    using (HttpClient client = new HttpClient())
                    {
                        HttpRequestMessage getMoreRequest = new HttpRequestMessage()
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri(getMoreURL)
                        };
                        getMoreRequest.Headers.Add("Accept", "application/json");
                        getMoreRequest.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                        string moreAKAs = client.SendAsync(getMoreRequest).Result.Content.ReadAsStringAsync().Result;
                        APIResponse akaAPIResponse = JsonSerializer.Deserialize<APIResponse>(moreAKAs);
                        if (akaAPIResponse?.Data?.ProductionData?.AKAsData?.AKAEdges != null)
                        {
                            foreach (var akaEdge in akaAPIResponse.Data.ProductionData.AKAsData.AKAEdges)
                            {
                                AKA aka = new AKA();
                                aka.Description = akaEdge.AKANode.Country?.Description;
                                aka.Name = akaEdge.AKANode.DisplayableProperty.Value.PlainText;
                                akas.Add(aka);
                            }
                        }
                    }
                }

                movie.AKAs = akas.ToArray();
            }
            #endregion
        }
    }
}
