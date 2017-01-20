using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using NLog;
using OSDBnet;
using Droid_video.Comparers;
using System.Threading.Tasks;

namespace Droid_video
{
    public partial class SubtitleDownloader
    {
        #region Attribute
        private static Thread _thread;
        private static readonly string UserAgent = ConfigurationManager.AppSettings["UserAgent"];
        private static readonly string Language = ConfigurationManager.AppSettings["Language"];
        private static readonly IAnonymousClient _client = Osdb.Login(Language, UserAgent);

        private static readonly IList<IEqualityComparer<string>> Comparers = new List<IEqualityComparer<string>>()
            {
                new ExactEqualyComparer(),
                new IgnoreCaseComparer(),
                new SeasonEpisodeLastComparer(),
                new SplitLastComparer(),
                new SeasonEpisodeComparer()
            };
        #endregion

        #region Constructor
        public SubtitleDownloader()
        {
        }
        #endregion

        #region Methods public
        public static async Task<List<string>> SearchLanguagesAvailable(string file)
        {
            List<string> languages = new List<string>();
            try
            {
                IList<Subtitle> subtitles = _client.SearchSubtitlesFromFile(Language, file);
                foreach (var item in subtitles)
                {
                    if (!languages.Contains(item.LanguageName))
                    {
                        languages.Add(item.LanguageName);
                    }
                }
                languages.Sort();
            }
            catch (System.Exception exp)
            {
                Tools4Libraries.Log.write(string.Format("Error trying searching languages for {0}./n Exception {1}", file, exp.Message));
            }
            return languages;
        }
        public static async Task<string> SearchSubtitle(string file, string lang)
        {
            Subtitle subtitle;
            string subtitleFinalFile = string.Empty;
            try
            {
                IList<Subtitle> subtitles = _client.SearchSubtitlesFromFile(Language, file);
                subtitles = subtitles.Where(s => s.LanguageName.Equals(lang)).ToList();
                subtitle = FindBestFit(file, subtitles);

                if (subtitle == null)
                {
                    if (subtitles.Count == 0) { return string.Empty; }
                    // we try to give a file even if that doesn't fit exactly.
                    subtitleFinalFile= DownloadSubtitle(file, subtitles[0], lang);
                }
                else
                { 
                    subtitleFinalFile = DownloadSubtitle(file, subtitle, lang);
                }
            }
            catch (System.Exception exception)
            {
                Tools4Libraries.Log.write(string.Format("Error trying to download subtitle for {0}./n Exception {1}", file, exception.Message));
            }
            return subtitleFinalFile;
        }
        #endregion
        
        #region Methods private
        private static string DownloadSubtitle(string file, OSDBnet.Subtitle subtitle, string lang)
        {
            file = string.Format("{0}\\{1}_{2}{3}", Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file), lang, Path.GetExtension(file));
            string downloaded = _client.DownloadSubtitleToPath(Path.GetDirectoryName(file), subtitle);
            var comp = new ExactEqualyComparer();
            if (comp.Equals(file, downloaded)) return Path.ChangeExtension(file, Path.GetExtension(downloaded));
            File.Move(downloaded, Path.ChangeExtension(file, Path.GetExtension(downloaded)));
            return Path.ChangeExtension(file, Path.GetExtension(downloaded));
        }
        private static OSDBnet.Subtitle FindBestFit(string file, IList<OSDBnet.Subtitle> subtitles)
        {
            var filename = Path.GetFileNameWithoutExtension(file);
            foreach (var comparer in Comparers)
            {
                var subtitle = subtitles.FirstOrDefault(x => comparer.Equals(Path.GetFileNameWithoutExtension(x.SubtitleFileName), filename));
                if (subtitle != null)
                    return subtitle;
            }
            return null;
        }
        #endregion
    }
}
