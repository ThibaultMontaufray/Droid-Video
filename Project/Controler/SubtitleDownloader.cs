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
        private static string _currentFile = string.Empty;
        private static string _currentLang = string.Empty;

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
            _currentFile = file;
            Task<List<string>> lst = new Task<List<string>>(GetLanguagesAvailable);
            lst.Start();

            List<string> res = await lst;
            return res;
        }
        public static async Task<string> SearchSubtitle(string file, string lang)
        {
            _currentLang = lang;
            _currentFile = file;

            Task<string> sub = new Task<string>(GetSubtitle);
            sub.Start();

            string res = await sub;
            return res;
        }
        #endregion
        
        #region Methods private
        private static string GetSubtitle()
        {
            Subtitle subtitle;
            string subtitleFinalFile = string.Empty;
            try
            {
                IList<Subtitle> subtitles = _client.SearchSubtitlesFromFile(Language, _currentFile);
                subtitles = subtitles.Where(s => s.LanguageName.Equals(_currentLang)).ToList();
                subtitle = FindBestFit(_currentFile, subtitles);

                if (subtitle == null)
                {
                    if (subtitles.Count == 0) { return string.Empty; }
                    // we try to give a file even if that doesn't fit exactly.
                    subtitleFinalFile = DownloadSubtitle(_currentFile, subtitles[0], _currentLang);
                }
                else
                {
                    subtitleFinalFile = DownloadSubtitle(_currentFile, subtitle, _currentLang);
                }
            }
            //catch (System.Exception exception)
            //{
            //    Tools4Libraries.Log.write(string.Format("[INF: 0000] : Error trying to download subtitle for {0}./n Exception {1}", _currentFile, exception.Message));
            //}
            finally
            {

            }
            return subtitleFinalFile;
        }
        private static List<string> GetLanguagesAvailable()
        {
            List<string> languages = new List<string>();
            try
            {
                IList<Subtitle> subtitles = _client.SearchSubtitlesFromFile(Language, _currentFile);
                foreach (var item in subtitles)
                {
                    if (!languages.Contains(item.LanguageName))
                    {
                        languages.Add(item.LanguageName);
                    }
                }
                languages.Sort();
            }
            //catch (System.Exception exp)
            //{
            //    Tools4Libraries.Log.write(string.Format("[INF: 0000] : Error trying searching languages for {0}./n Exception {1}", _currentFile, exp.Message));
            //}
            finally
            {

            }
            return languages;
        }
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
