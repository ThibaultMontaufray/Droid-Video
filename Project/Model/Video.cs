﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Droid_web;

namespace Droid_video
{
    public class Video
    {
        #region Attribute
        private string _path;
        private string _cleanName;
        private List<string> _videoSubtitleLanguages;
        private List<string> _subtitlePath;
        private Subtitle _subtitle;
        private int _date;
        private string _source;
        private string _info;
        private string _language;
        private string _season;
        private string _episod;
        private bool _vo;
        private string _format;
        private long _length;
        private long _position;
        private List<string> _listSubtitleLanguages;

        private List<string[]> _lang;
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
        public long Length
        {
            get { return _length; }
            set { _length = value; }
        }
        public long Position
        {
            get { return _position; }
            set { _position = value; }
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
        public int Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public Subtitle Subtitle
        {
            get { return _subtitle; }
            set { _subtitle = value; }
        }
        public List<string> SubtitlePath
        {
            get { return _subtitlePath; }
            set { _subtitlePath = value; }
        }
        public List<string> SubtitleLanguages
        {
            get { return _videoSubtitleLanguages; }
            set { _videoSubtitleLanguages = value; }
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
            get { return _listSubtitleLanguages; }
        }
        #endregion

        #region Constructor
        public Video()
        {
            Init();
        }
        #endregion

        #region Methods public
        #endregion

        #region Methods private
        private void Init()
        {
            _vo = false;

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
            _cleanName = System.IO.Path.GetFileName(_path);
            _cleanName = _cleanName.Replace(_format, string.Empty);

            DetectDate(ref filePart);
            DetectDate(ref filePart);
            DetectSeries(ref filePart);
            DetectSource(ref filePart);
            DetectInfo(ref filePart);
            DetectSubtitles();
        }
        private void DetectLanguage(ref List<string> filePart)
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
                Tools4Libraries.Log.write("[INF: 0001] Error while parsing the file name. \n" + exp.Message);
            }
        }
        private void DetectDate(ref List<string> filePart)
        {
            try { 
                filePart.Add(_path);
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
                            _language = lang[lang.Length - 1];
                            _vo = true;
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".vost{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".vost{0}.", item)).ToList();
                            _language = lang[lang.Length - 1];
                            _vo = true;
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".VoSt{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".VoSt{0}.", item)).ToList();
                            _language = lang[lang.Length - 1];
                            _vo = true;
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".sub{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".sub{0}.", item)).ToList();
                            _language = lang[lang.Length - 1];
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".SUB{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".SUB{0}.", item)).ToList();
                            _language = lang[lang.Length - 1];
                            break;
                        }
                        if (_cleanName.Contains(string.Format(".Sub{0}.", item)))
                        {
                            filePart = Regex.Split(_cleanName, string.Format(".Sub{0}.", item)).ToList();
                            _language = lang[lang.Length - 1];
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(_language)) { break; }
                }
            }
            catch (Exception exp)
            {
                Tools4Libraries.Log.write("[INF: 0001] Error while parsing the file name. \n" + exp.Message);
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
                Tools4Libraries.Log.write("[INF: 0001] Error while parsing the file name. \n" + exp.Message);
            }
        }
        private void DetectSource(ref List<string> filePart)
        {
            try { 
                string tmpStr;
                List<string> newFilePart = new List<string>();
                foreach (string part in filePart)
                {
                    tmpStr = string.Empty;
                    foreach (string s in part.Split(']'))
                    {
                        if (s.StartsWith("["))
                        {
                            _source = s.Replace("[", string.Empty).Trim();
                        }
                        else if (s.Contains("["))
                        {
                            _source = s.Split('[')[1].Trim();
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
                Tools4Libraries.Log.write("[INF: 0001] Error while parsing the file name. \n" + exp.Message);
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
                Tools4Libraries.Log.write("[INF: 0001] Error while parsing the file name. \n" + exp.Message);
            }
        }
        private void DetectSubtitles()
        {
            try
            {
                _listSubtitleLanguages = ParserSubtitle.Languages(_cleanName);
            }
            catch (Exception exp)
            {
                Tools4Libraries.Log.write("[INF: 0001] Error while listing downloadable subtitles. \n" + exp.Message);
            }
        }
        #endregion
    }
}
