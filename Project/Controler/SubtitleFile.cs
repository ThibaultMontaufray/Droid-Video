using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Droid_video
{
    public class SubtitleFile
    {
        #region Structures
        public struct SubLine
        {
            public TimeSpan StartDate;
            public TimeSpan EndDate;
            public string Text;
            public int Id;
        }
        #endregion

        #region Attribute
        private string _filePath;
        private bool _fileLoaded;
        private List<SubLine> _lines;
        #endregion

        #region Properties
        public List<SubLine> Lines
        {
            get { return _lines; }
            set { _lines = value; }
        }
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        public bool FileLoaded
        {
            get { return _fileLoaded; }
            set { _fileLoaded = value; }
        }
        #endregion

        #region Constructor
        public SubtitleFile(string filePath)
        {
            _lines = new List<SubLine>();
            _filePath = filePath;
            LoadFile();
        }
        #endregion

        #region Methods public
        public string GetText(TimeSpan now)
        {
            string resultText = "<html><style>*{margin-top:0px;background-color:#000;color:#aaa;font-family: Calibri, \"Times New Roman\", Times, serif;font-size: 20px;opacity:0.7;}</style><body><center>";
            List<SubLine> result = _lines.Where(l => l.StartDate < now && l.EndDate > now).ToList();

            if (result.Count >= 1)
            {
                foreach (SubLine SubLine in result)
                {
                    resultText += SubLine.Text;
                    if (!string.IsNullOrEmpty(resultText)) resultText += "</br>";
                }
            }
            resultText += "</center></body></html>";
            return resultText;
        }
        #endregion

        #region Methods private
        private void LoadFile()
        {
            int id;
            string line;
            string[] tab;
            TimeSpan dateStart;
            TimeSpan dateEnd;
            SubLine subLine = new SubLine();
            _fileLoaded = false;
            _lines.Clear();

            try
            {
                if (File.Exists(_filePath))
                {
                    using (StreamReader sr = new StreamReader(_filePath))
                    {
                        while (sr.Peek() > 0)
                        {
                            line = sr.ReadLine();
                            if (string.IsNullOrEmpty(line)) { subLine = new SubLine(); }
                            else if (line.Contains("-->"))
                            {
                                tab = Regex.Split(line, " --> ");
                                if (tab.Length == 2 && TimeSpan.TryParse(tab[0], out dateStart) && TimeSpan.TryParse(tab[1], out dateEnd))
                                {
                                    subLine.StartDate = dateStart;
                                    subLine.EndDate = dateEnd;
                                }
                            }
                            else if (int.TryParse(line, out id)) { subLine.Id = id; }
                            else
                            {
                                subLine.Text = line;
                                _lines.Add(subLine);
                            }
                        }
                        _fileLoaded = true;
                    }
                }
            }
            catch (Exception exp0001)
            {
                Tools4Libraries.Log.Write("[INF: 0001] Cannot read the file. \n" + exp0001.Message);
            }
        }
        #endregion

        #region Event
        #endregion
    }
}
