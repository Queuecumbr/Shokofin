using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shokofin.API.Models;

public class Series
{
    /// <summary>
    /// The preferred name of the series based on the selected series language
    /// settings on the server.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The preferred description of the series based on the selected series
    /// language settings on the server.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    public int Size { get; set; }

    /// <summary>
    /// All identifiers related to the series entry, e.g. the Shoko, AniDB,
    /// TMDB, etc.
    /// </summary>
    public SeriesIDs IDs { get; set; } = new();

    /// <summary>
    /// The default or random pictures for a series. This allows the client to
    /// not need to get all images and pick one.
    ///
    /// There should always be a poster, but no promises on the rest.
    /// </summary>
    public Images Images { get; set; } = new();

    /// <summary>
    /// The user's rating, if any.
    /// </summary>
    public Rating? UserRating { get; set; } = new();

    /// <summary>
    /// The AniDB entry.
    /// </summary>
    [JsonPropertyName("AniDB")]
    public AniDBWithDate AniDBEntity { get; set; } = new();

    public SeriesSizes Sizes { get; set; } = new();

    /// <summary>
    /// When the series entry was created during the process of the first file
    /// being added to Shoko.
    /// </summary>
    [JsonPropertyName("Created")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the series entry was last updated.
    /// </summary>
    [JsonPropertyName("Updated")]
    public DateTime LastUpdatedAt { get; set; }

    public class AniDB
    {
        /// <summary>
        /// AniDB Id
        /// </summary>
        [JsonPropertyName("ID")]
        public int Id { get; set; }

        /// <summary>
        /// <see cref="Series"/> Id if the series is available locally.
        /// </summary>
        [JsonPropertyName("ShokoID")]
        public int? ShokoId { get; set; }

        /// <summary>
        /// Series type. Series, OVA, Movie, etc
        /// </summary>
        public SeriesType Type { get; set; }

        /// <summary>
        /// Main Title, usually matches x-jat
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// There should always be at least one of these, the <see cref="Title"/>. May be omitted if needed.
        /// </summary>
        public List<Title>? Titles { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Restricted content. Mainly porn.
        /// </summary>
        public bool Restricted { get; set; }

        /// <summary>
        /// The main or default poster.
        /// </summary>
        public Image Poster { get; set; } = new();

        /// <summary>
        /// Number of <see cref="EpisodeType.Normal"/> episodes contained within the series if it's known.
        /// </summary>
        public int? EpisodeCount { get; set; }

        /// <summary>
        /// The average rating for the anime. Only available on
        /// </summary>
        public Rating? Rating { get; set; }

        /// <summary>
        /// User approval rate for the similar submission. Only available for similar.
        /// </summary>
        public Rating? UserApproval { get; set; }

        /// <summary>
        /// Relation type. Only available for relations.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RelationType? Relation { get; set; }
    }

    public class AniDBWithDate : AniDB
    {
        /// <summary>
        /// Description.
        /// </summary>
        public new string Description { get; set; } = string.Empty;

        /// <summary>
        /// There should always be at least one of these, the <see cref="Title"/>. May be omitted if needed.
        /// </summary>
        public new List<Title> Titles { get; set; } = [];

        /// <summary>
        /// The average rating for the anime. Only available on
        /// </summary>
        public new Rating Rating { get; set; } = new();

        /// <summary>
        /// Number of <see cref="EpisodeType.Normal"/> episodes contained within the series if it's known.
        /// </summary>
        public new int EpisodeCount { get; set; }

        [JsonIgnore]
        private DateTime? InternalAirDate { get; set; } = null;

        /// <summary>
        /// Air date (2013-02-27). Anything without an air date is going to be missing a lot of info.
        /// </summary>
        public DateTime? AirDate
        {
            get
            {
                return InternalAirDate;
            }
            set
            {
                InternalAirDate = value.HasValue && (value.Value == DateTime.UnixEpoch || value.Value == DateTime.MinValue || value.Value == DateTime.MaxValue) ? null : value;
            }
        }

        [JsonIgnore]
        private DateTime? InternalEndDate { get; set; } = null;

        /// <summary>
        /// End date, can be omitted. Omitted means that it's still airing (2013-02-27)
        /// </summary>
        public DateTime? EndDate
        {
            get
            {
                return InternalEndDate;
            }
            set
            {
                InternalEndDate = value.HasValue && (value.Value == DateTime.UnixEpoch || value.Value == DateTime.MinValue || value.Value == DateTime.MaxValue) ? null : value;
            }
        }
    }

    public class SeriesIDs : IDs
    {
        public int ParentGroup { get; set; } = 0;

        public int TopLevelGroup { get; set; } = 0;

        public int AniDB { get; set; } = 0;
    }

    /// <summary>
    /// Downloaded, Watched, Total, etc
    /// </summary>
    public class SeriesSizes
    {
        /// <summary>
        /// Combined count of all files across all file sources within the series or group.
        /// </summary>
        public int Files =>
            FileSources.Unknown +
            FileSources.Other +
            FileSources.TV +
            FileSources.DVD +
            FileSources.BluRay +
            FileSources.Web +
            FileSources.VHS +
            FileSources.VCD +
            FileSources.LaserDisc +
            FileSources.Camera;

        /// <summary>
        /// Counts of each file source type available within the local collection
        /// </summary>
        public FileSourceCounts FileSources { get; set; } = new();

        /// <summary>
        /// What is downloaded and available
        /// </summary>
        public EpisodeTypeCounts Local { get; set; } = new();

        /// <summary>
        /// What is local and watched.
        /// </summary>
        public EpisodeTypeCounts Watched { get; set; } = new();

        /// <summary>
        /// Total count of each type
        /// </summary>
        public EpisodeTypeCounts Total { get; set; } = new();

        /// <summary>
        /// Lists the count of each type of episode.
        /// </summary>
        public class EpisodeTypeCounts
        {
            public int Unknown { get; set; }
            public int Episodes { get; set; }
            public int Specials { get; set; }
            public int Credits { get; set; }
            public int Trailers { get; set; }
            public int Parodies { get; set; }
            public int Others { get; set; }
        }

        public class FileSourceCounts
        {
            public int Unknown;
            public int Other;
            public int TV;
            public int DVD;
            public int BluRay;
            public int Web;
            public int VHS;
            public int VCD;
            public int LaserDisc;
            public int Camera;
        }
    }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SeriesType
{
    /// <summary>
    /// The series type is unknown.
    /// </summary>
    Unknown,
    /// <summary>
    /// A catch-all type for future extensions when a provider can't use a current episode type, but knows what the future type should be.
    /// </summary>
    Other,
    /// <summary>
    /// Standard TV series.
    /// </summary>
    TV,
    /// <summary>
    /// TV special.
    /// </summary>
    TVSpecial,
    /// <summary>
    /// Web series.
    /// </summary>
    Web,
    /// <summary>
    /// All movies, regardless of source (e.g. web or theater)
    /// </summary>
    Movie,
    /// <summary>
    /// Original Video Animations, AKA standalone releases that don't air on TV or the web.
    /// </summary>
    OVA,
}

