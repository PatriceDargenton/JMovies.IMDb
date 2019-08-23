﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMovies.IMDb.Entities.Movies
{
    /// <summary>
    /// Class definition of a TV Character
    /// </summary>
    public class TVCharacter : Character
    {

        /// <summary>
        /// Count of episodes the character was in
        /// </summary>
        [MaxLength(4)]
        public int EpisodeCount { get; set; }

        /// <summary>
        /// Character first appereance year
        /// </summary>
        [MaxLength(4)]
        public int? StartYear { get; set; }

        /// <summary>
        /// Character last appereance year
        /// </summary>
        [MaxLength(4)]
        public int? EndYear { get; set; }

        public override CharacterTypeEnum CharacterType { get => CharacterTypeEnum.TVCharacter; set => base.CharacterType = value; }
    }
}
