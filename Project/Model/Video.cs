﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OSDBnet;
using Droid_web;
using System.Drawing;
using System.Net;
using System.IO;

namespace Droid_video
{
    public delegate void VideoEventHandler();
    public class Video
    {
        #region Attribute
        public event VideoEventHandler SubtitleResearchCompleted;
        public event VideoEventHandler SubtitleChanged;

        private string _path;
        private string _cleanName;
        private List<string> _videoSubtitleLanguages;
        private List<string> _subtitlePath;
        //private SubtitleFile _subtitle;
        private Subtitle _subtitle;
        private int? _date;
        private string _source;
        private string _info;
        private string _language;
        private string _subtitleLanguage;
        private string _season;
        private string _episod;
        private bool _vo;
        private string _format;
        private long _length;
        private long _time;
        private string _subtitleRequested;
        private double _audioAdjustment;
        private string _currentSubtitlePath;
        private string _coverPath;
        
        private List<string[]> _lang;
        // take this out in xml file !
        private string[] _langFr = { "fr", "Fr", "FR", "french", "FRENCH", "French" };
        private string[] _langEn = { "en", "En", "EN", "english", "ENGLISH", "English" };
        private string[] _langGe = { "ge", "Ge", "GE", "german", "GERMAN", "German" };
        private string[] _langSp = { "sp", "Sp", "SP", "spanish", "SPANISH", "Spanish" };
        private string[] _langIt = { "it", "It", "IT", "italian", "ITALIAN", "Italian" };
        private string[] _langFi = { "fi", "Fi", "FI", "finnish", "FINNISH", "Finnish" };
        private string[] _langAr = { "ar", "Ar", "AR", "arabic", "ARABIC", "Arabic" };
        private string[] _langCh = { "ch", "Ch", "CH", "chinese", "CHINESE", "Chinese" };
        #endregion

        #region Properties
        public string CurrentSubtitlePath
        {
            get { return _currentSubtitlePath; }
            set
            {
                _currentSubtitlePath = value;
                if (SubtitleChanged != null) SubtitleChanged();
            }
        }
        public double AudioAdjustment
        {
            get { return _audioAdjustment; }
            set { _audioAdjustment = value; }
        }
        public string SubtitleRequested
        {
            get { return _subtitleRequested; }
            set { _subtitleRequested = value; }
        }
        public string SubtitleLanguage
        {
            get { return _subtitleLanguage; }
            set { _subtitleLanguage = value; }
        }
        public long Length
        {
            get { return _length; }
            set { _length = value; }
        }
        public long Time
        {
            get { return _time; }
            set { _time = value; }
        }
        public string Episod
        {
            get { return _episod; }
            set { _episod = value; }
        }
        public string Season
        {
            get { return _season; }
            set { _season = value; }
        }
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }
        public string Info
        {
            get { return _info; }
            set { _info = value; }
        }
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }
        public int? Date    
        {
            get { return _date; }
            set { _date = value; }
        }
        public Subtitle Subtitle
        {
            get { return _subtitle; }
            set
            {
                _subtitle = value;
                if (SubtitleChanged != null) SubtitleChanged();
            }
        }
        public List<string> SubtitlePath
        {
            get { return _subtitlePath; }
            set { _subtitlePath = value; }
        }
        public string NameClean
        {
            get { return _cleanName; }
            set { _cleanName = value; }
        }
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                CleanVideoName();
            }
        }
        public bool VO
        {
            get { return _vo; }
            set { _vo = value; }
        }
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }
        public List<string> DownloadableSubtilteLanguages
        {
            get { return _videoSubtitleLanguages; }
            set { _videoSubtitleLanguages = value; }
        }
        public string CoverPath
        {
            get { return _coverPath; }
            set { _coverPath = value; }
        }
        #endregion

        #region Constructor
        public Video()
        {
            Init();
        }
        #endregion

        #region Methods public
        public async void LoadWebDetails()
        {
            Task t = new Task( () => { DownloadCover(); });
            t.Start();

            _videoSubtitleLanguages = await DetectSubtitles();
            if (SubtitleResearchCompleted != null) SubtitleResearchCompleted();

        }
        #endregion

        #region Methods private
        private void Init()
        {
            _vo = false;
            _coverPath = string.Empty;

            _videoSubtitleLanguages = new List<string>();

            _lang = new List<string[]>();
            _lang.Add(_langFr);
            _lang.Add(_langEn);
            _lang.Add(_langGe);
            _lang.Add(_langSp);
            _lang.Add(_langIt);
            _lang.Add(_langFi);
            _lang.Add(_langAr);
            _lang.Add(_langCh);
        }
        private void CleanVideoName()
        {
            List<string> filePart = new List<string>();
            _language = string.Empty;
            _format = System.IO.Path.GetExtension(_path);
            _cleanName = System.IO.Path.GetFileNameWithoutExtension(_path);
            _cleanName = _cleanName.Replace("FANSUB", string.Empty);
            _cleanName = _cleanName.Replace("fansub", string.Empty);
            _cleanName = _cleanName.Replace("FanSub", string.Empty);
            _cleanName = _cleanName.Replace("FASTSUB", string.Empty);
            _cleanName = _cleanName.Replace("fastsub", string.Empty);
            _cleanName = _cleanName.Replace("FastSub", string.Empty);

            DetectLanguage(ref filePart);
            DetectDate(ref filePart);
            DetectSeries(ref filePart);
            DetectSource(ref filePart);
            DetectInfo(ref filePart);
        }
        private void DetectDate(ref List<string> filePart)
        {
            try { 
                string tmpStr;
                List<string> newFilePart = new List<string>();
                foreach (string part in filePart)
                {
                    tmpStr = string.Empty;
                    foreach (string s in part.Split('.'))
                    {
                        if (Regex.Split(s, "^(19|20)\\d\\d$").Count() > 1)
                        {
                            _date = int.Parse(s);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(s)) tmpStr += ".";
                            tmpStr += s;
                        }
                    }
                    newFilePart.Add(tmpStr);
                }
                filePart = newFilePart;
            }
            catch (Exception exp)
            {
                Tools4Libraries.Log.Write("[INF: 0001] Error while parsing the file name. \n" + exp.Message);
            }
        }
        private void DetectLanguage(ref List<string> filePart)
        {
            try { 
                filePart.Add(_cleanName);
                foreach (string[] lang in _lang)
                {
                    foreach (string item in lang)
                    {
                        if (_cleanName.Contains(string.Format(".{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".{0}.", item)).ToList();
                            _language = lang[lang.Length - 1];
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".TRUE{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".TRUE{0}.", item)).ToList();
                            _language = lang[lang.Length - 1];
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".True{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".True{0}.", item)).ToList();
                            _language = lang[lang.Length - 1];
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".true{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".true{0}.", item)).ToList();
                            _language = lang[lang.Length - 1];
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".VOST{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".VOST{0}.", item)).ToList();
                            _subtitleLanguage = lang[lang.Length - 1];
                            _vo = true;
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".vost{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".vost{0}.", item)).ToList();
                            _subtitleLanguage = lang[lang.Length - 1];
                            _vo = true;
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".VoSt{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".VoSt{0}.", item)).ToList();
                            _subtitleLanguage = lang[lang.Length - 1];
                            _vo = true;
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".sub{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".sub{0}.", item)).ToList();
                            _subtitleLanguage = lang[lang.Length - 1];
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".SUB{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".SUB{0}.", item)).ToList();
                            _subtitleLanguage = lang[lang.Length - 1];
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".Sub{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".Sub{0}.", item)).ToList();
                            _subtitleLanguage = lang[lang.Length - 1];
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(_language)) { break; }
                }
            }
            catch (Exception exp)
            {
                Tools4Libraries.Log.Write("[INF: 0001] Error while parsing the file name. \n" + exp.Message);
            }
        }
        private void DetectSeries(ref List<string> filePart)
        {
            try { 
                string tmpStr;
                List<string> newFilePart = new List<string>();
                foreach (string part in filePart)
                {
                    tmpStr = string.Empty;
                    foreach (string s in part.Split('.'))
                    {
                        if (Regex.Split(s, "^[S-s][\\d]{1,2}[E-e][\\d]{1,2}$").Count() > 1)
                        {
                            _season = s.ToLower().Split('e')[0].Replace("s", string.Empty);
                            _episod = s.ToLower().Split('e')[1];
                        }
                        //else if (Regex.Split(s, "^[\\[][\\d]{1,2}[X-x][\\d]{1,2}[\\]]$").Count() > 1)
                        //{
                        //    _season = s.ToLower().Split('[')[0].Split('x')[0];
                        //    _episod = s.ToLower().Split('[')[0].Split('x')[1];
                        //}
                        else
                        {
                            if (!string.IsNullOrEmpty(s)) tmpStr += ".";
                            tmpStr += s;
                        }
                    }
                    newFilePart.Add(tmpStr);
                }
                filePart = newFilePart;
            }
            catch (Exception exp)
            {
                Tools4Libraries.Log.Write("[INF: 0001] Error while parsing the file name. \n" + exp.Message);
            }
        }
        private void DetectSource(ref List<string> filePart)
        {
            try { 
                string tmpStr;
                List<string> newFilePart = new List<string>();
                tmpStr = string.Empty;
                foreach (string part in filePart)
                {
                    foreach (string s in part.Split(']'))
                    {
                        if (s.StartsWith("["))
                        {
                            _source = s.Replace("[", string.Empty).Trim();
                        }
                        else if (s.Contains("["))
                        {
                            _source = s.Split('[')[1].Trim();
                            tmpStr += s.Split('[')[0].Trim();
                        }
                        else
                        {
                            tmpStr += s;
                        }
                    }
                    newFilePart.Add(tmpStr);
                }
                filePart = newFilePart;
            }
            catch (Exception exp)
            {
                Tools4Libraries.Log.Write("[INF: 0001] Error while parsing the file name. \n" + exp.Message);
            }
        }
        private void DetectInfo(ref List<string> filePart)
        {
            try
            {
                _cleanName = filePart[0].Replace('.', ' ');
                _cleanName = _cleanName.Trim();

                _info = string.Empty;
                for (int i = 1; i < filePart.Count; i++)
                {
                    if (!string.IsNullOrEmpty(_info)) _info += ".";
                    _info += filePart[i];
                }
            }
            catch (Exception exp)
            {
                Tools4Libraries.Log.Write("[INF: 0001] Error while parsing the file name. \n" + exp.Message);
                _cleanName = System.IO.Path.GetFileName(_path);
            }
        }
        private async Task<List<string>> DetectSubtitles()
        {
            List<string> subList = new List<string>();
            string search = string.Empty;
            try
            {
                search = _cleanName;
                if (!string.IsNullOrEmpty(_season))
                {
                    search += string.Format(" S{0}E{1}", _season, _episod);
                }
                Task<List<string>> retList = SubtitleDownloader.SearchLanguagesAvailable(_path);
                subList = await retList;
            }
            catch (Exception)
            {
                // normal if no connection
                //Tools4Libraries.Log.Write("[INF: 0001] Error while listing downloadable subtitles. \n" + exp.Message);
            }
            finally
            {
            }
            return subList;
        }
        private string DownloadCover()
        {
            string finalPath, picPath;
            if (!string.IsNullOrEmpty(_season)) { picPath = Web.GetLuckyImage(string.Format("cover movie {0} season {1}", _cleanName, _season)); }
            else { picPath = Web.GetLuckyImage(string.Format("cover movie {0} {1}", _cleanName, _date)); }
            
            using (WebClient client = new WebClient())
            {
                finalPath = string.Format("{0}{1}.{2}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Servodroid\Droid-Video\", _cleanName, picPath.Split('.')[picPath.Split('.').Length - 1]);
                client.DownloadFileAsync(new Uri(picPath), finalPath);
            }
            _coverPath = finalPath;
            return finalPath;
        }
        #endregion
    }
}
