/*  
 *  This code is based on information provided by http://greatmaps.codeplex.com
 *  
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace BruTile.Web
{
    /// <summary>
    /// This enum contains the map types offered by Google Maps
    /// </summary>
    [Flags]
    public enum GoogleMapType
    {
        GoogleMap = 1,
        GoogleSatellite = 4,
        GoogleLabels = 8,
        GoogleTerrain = 16
    }

    public class GoogleRequest : IRequest
    {        
#if !SILVERLIGHT
        static GoogleRequest()
        {
            TryCorrectGoogleVersions();
        }
#endif
        
        /// <summary>
        /// This enum contains all possible languages for the Google maps. 
        /// You can find latest information about supported languages in the:
        /// http://spreadsheets.google.com/pub?key=p9pdwsai2hDMsLkXsoM05KQ&gid=1
        /// </summary>
        public enum LanguageType
        {
            [Description("ar")]
            Arabic,

            [Description("bg")]
            Bulgarian,

            [Description("bn")]
            Bengali,

            [Description("ca")]
            Catalan,

            [Description("cs")]
            Czech,

            [Description("da")]
            Danish,

            [Description("de")]
            German,

            [Description("el")]
            Greek,

            [Description("en")]
            English,

            [Description("en-AU")]
            EnglishAustralian,

            [Description("en-GB")]
            EnglishGreatBritain,

            [Description("es")]
            Spanish,

            [Description("eu")]
            Basque,

            [Description("fi")]
            Finnish,

            [Description("fil")]
            Filipino,

            [Description("fr")]
            French,

            [Description("gl")]
            Galician,

            [Description("gu")]
            Gujarati,
            [Description("hi")]
            Hindi,

            [Description("hr")]
            Croatian,

            [Description("hu")]
            Hungarian,

            [Description("id")]
            Indonesian,

            [Description("it")]
            Italian,

            [Description("iw")]
            Hebrew,

            [Description("ja")]
            Japanese,

            [Description("kn")]
            Kannada,

            [Description("ko")]
            Korean,

            [Description("lt")]
            Lithuanian,

            [Description("lv")]
            Latvian,

            [Description("ml")]
            Malayalam,

            [Description("mr")]
            Marathi,

            [Description("nl")]
            Dutch,

            [Description("nn")]
            NorwegianNynorsk,

            [Description("no")]
            Norwegian,

            [Description("or")]
            Oriya,

            [Description("pl")]
            Polish,

            [Description("pt")]
            Portuguese,

            [Description("pt-BR")]
            PortugueseBrazil,

            [Description("pt-PT")]
            PortuguesePortugal,

            [Description("rm")]
            Romansch,
            [Description("ro")]
            Romanian,

            [Description("ru")]
            Russian,

            [Description("sk")]
            Slovak,

            [Description("sl")]
            Slovenian,

            [Description("sr")]
            Serbian,

            [Description("sv")]
            Swedish,

            [Description("ta")]
            Tamil,

            [Description("te")]
            Telugu,

            [Description("th")]
            Thai,

            [Description("tr")]
            Turkish,

            [Description("uk")]
            Ukrainian,

            [Description("vi")]
            Vietnamese,

            [Description("zh-CN")]
            ChineseSimplified,

            [Description("zh-TW")]
            ChineseTraditional,
        }

        // Google version strings
        private static string VersionGoogleMap = "m@130";
        private static string VersionGoogleSatellite = "66";
        private static string VersionGoogleLabels = "h@130";
        private static string VersionGoogleTerrain = "t@125,r@130";
        private static readonly string SecGoogleWord = "Galileo";

        private static readonly System.Globalization.CultureInfo FormatProvider =
            System.Globalization.CultureInfo.InvariantCulture;

        /// <summary>
        /// 0 ... Name of Server [mt|khm]
        /// 1 ... Number of Server [0, 3]
        ///       Calculated by GetServerNum -function
        /// 2 ... Request [vt|kh]
        /// 3 ... Version
        /// 4 ... Language [LanguageType]
        /// 5 ... TileIndex.Col
        /// 6 ... ???
        /// 7 ... TileIndex.Row
        /// 8 ... Zoomlevel
        /// 9 ... ???
        /// 10 .. [lyrs|v]
        /// </summary>
        private const string UrlFormatString =
            "http://{0}{1}.google.com/{2}/{10}={3}&hl={4}&x={5}{6}&y={7}&z={8}&s={9}";

        private string _server;
        private string _request;
        private string _version;
        private string _versionKey;

        private GoogleMapType _mapType;
        /// <summary>
        /// MapType
        /// </summary>
        public GoogleMapType MapType
        {
            get { return _mapType; }
            set
            {
                _mapType = value;
                switch (MapType)
                {
                    case GoogleMapType.GoogleMap:
                        _server = "mt";
                        _request = "vt";
                        _version = VersionGoogleMap;
                        _versionKey = "lyrs";
                        break;
                    case GoogleMapType.GoogleSatellite:
                        _server = "khm";
                        _request = "kh";
                        _version = VersionGoogleSatellite;
                        _versionKey = "v";
                        break;
                    case GoogleMapType.GoogleLabels:
                        _server = "mt";
                        _request = "vt";
                        _version = VersionGoogleLabels;
                        _versionKey = "lyrs";
                        break;
                    case GoogleMapType.GoogleTerrain:
                        _server = "mt";
                        _request = "vt";
                        _version = VersionGoogleTerrain;
                        _versionKey = "lyrs";
                        break;
                }
                _server = _mapType != GoogleMapType.GoogleSatellite ? "mt" : "khm";
                _request = _mapType != GoogleMapType.GoogleSatellite ? "vt" : "kh";
            }
        }

        private LanguageType _language;

        public LanguageType Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public GoogleRequest()
            : this(GoogleMapType.GoogleMap)
        {
        }

        public GoogleRequest(GoogleMapType googleMapType, LanguageType languageType)
        {
            Language = languageType;
            MapType = googleMapType;
        }

        public GoogleRequest(GoogleMapType googleMapType)
            : this(googleMapType, LanguageType.English)
        {
        }

        private static Int32 GetServerNum(TileIndex tileIndex, Int32 max)
        {
            return (tileIndex.Col + 2 * tileIndex.Row) % max;
        }

        public Uri GetUri(TileInfo tileInfo)
        {

            string sec1; // after &x=...
            string sec2; // after &zoom=...
            GetSecGoogleWords(tileInfo.Index, out sec1, out sec2);

            TileIndex tileIndex = tileInfo.Index;

            return new Uri(
                string.Format(FormatProvider, UrlFormatString,
                              _server, GetServerNum(tileIndex, 4), _request, _version, _language,
                              tileIndex.Col, sec1, tileIndex.Row, tileIndex.Level, sec2, _versionKey));
        }

        /// <summary>
        /// gets secure google words based on position
        /// </summary>
        /// <param name="tileIndex"></param>
        /// <param name="sec1"></param>
        /// <param name="sec2"></param>
        internal void GetSecGoogleWords(TileIndex tileIndex, out string sec1, out string sec2)
        {
            sec1 = ""; // after &x=...
            //sec2 = ""; // after &zoom=...
            int seclen = ((tileIndex.Col * 3) + tileIndex.Row) % 8;
            sec2 = SecGoogleWord.Substring(0, seclen);
            if (tileIndex.Row >= 10000 && tileIndex.Row < 100000)
            {
                sec1 = "&s=";
            }
        }

#if !SILVERLIGHT
        private static void TryCorrectGoogleVersions()
        {
                string url = @"http://maps.google.com";
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
//                    if (Proxy != null)
//                    {
//                        request.Proxy = Proxy;
//#if !PocketPC
//                        request.PreAuthenticate = true;
//#endif
//                    }

                    request.UserAgent = GoogleTileSource.UserAgent;
                    request.Timeout = 60000;
                    request.ReadWriteTimeout = 360000;

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response != null)
                        {
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                if (responseStream != null)
                                {
                                    using (StreamReader read = new StreamReader(responseStream))
                                    {
                                        string html = read.ReadToEnd();

                                        Regex reg = new Regex("\"*http://mt0.google.com/vt/lyrs=m@(\\d*)",
                                                              RegexOptions.IgnoreCase);
                                        Match mat = reg.Match(html);
                                        if (mat.Success)
                                        {
                                            GroupCollection gc = mat.Groups;
                                            int count = gc.Count;
                                            if (count > 0)
                                            {
                                                VersionGoogleMap = string.Format("m@{0}", gc[1].Value);
                                                //VersionGoogleMapChina = VersionGoogleMap;
                                                Debug.WriteLine("TryCorrectGoogleVersions, VersionGoogleMap: " +
                                                                VersionGoogleMap);
                                            }
                                        }

                                        reg = new Regex("\"*http://mt0.google.com/vt/lyrs=h@(\\d*)",
                                                        RegexOptions.IgnoreCase);
                                        mat = reg.Match(html);
                                        if (mat.Success)
                                        {
                                            GroupCollection gc = mat.Groups;
                                            int count = gc.Count;
                                            if (count > 0)
                                            {
                                                VersionGoogleLabels = string.Format("h@{0}", gc[1].Value);
                                                //VersionGoogleLabelsChina = VersionGoogleLabels;
                                                Debug.WriteLine("TryCorrectGoogleVersions, VersionGoogleLabels: " +
                                                                VersionGoogleLabels);
                                            }
                                        }

                                        reg = new Regex("\"*http://khm0.google.com/kh/v=(\\d*)", RegexOptions.IgnoreCase);
                                        mat = reg.Match(html);
                                        if (mat.Success)
                                        {
                                            GroupCollection gc = mat.Groups;
                                            int count = gc.Count;
                                            if (count > 0)
                                            {
                                                VersionGoogleSatellite = gc[1].Value;
                                                //VersionGoogleSatelliteKorea = VersionGoogleSatellite;
                                                //VersionGoogleSatelliteChina = "s@" + VersionGoogleSatellite;
                                                Debug.WriteLine("TryCorrectGoogleVersions, VersionGoogleSatellite: " +
                                                                VersionGoogleSatellite);
                                            }
                                        }

                                        reg = new Regex("\"*http://mt0.google.com/vt/lyrs=t@(\\d*),r@(\\d*)",
                                                        RegexOptions.IgnoreCase);
                                        mat = reg.Match(html);
                                        if (mat.Success)
                                        {
                                            GroupCollection gc = mat.Groups;
                                            int count = gc.Count;
                                            if (count > 1)
                                            {
                                                VersionGoogleTerrain = string.Format("t@{0},r@{1}", gc[1].Value,
                                                                                     gc[2].Value);
                                                //VersionGoogleTerrainChina = VersionGoogleTerrain;
                                                Debug.WriteLine(
                                                    "TryCorrectGoogleVersions, VersionGoogleTerrain: " +
                                                    VersionGoogleTerrain);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("TryCorrectGoogleVersions failed: " + ex);
                }
            }
#endif
    }
}
