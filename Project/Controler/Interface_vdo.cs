// log code 84 02

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Tools4Libraries;
using System.Threading.Tasks;
using System.Linq;
using OSDBnet;
using System.ComponentModel;
using Droid_Image;
using System.Text;

namespace Droid_video
{
    /// <summary>
    /// Interface for Tobi Assistant application : take care, some french word here to allow Tobi to speak with natural langage.
    /// </summary>            
    public class Interface_vdo : GPInterface
    {
		#region Attributes
        private ToolStripMenuVDO _tsm;
        private string _currentDirectory;
        private List<Video> _currentDirectoryFiles;
        private string _seriePathNext;
        private string _seriePathPreview;

        private Panel _sheet;
        private Panel _panelVideo;
        private VideoPlayer _videoFrame;
        private Form _formFrame;
        
        private bool _explorerShown;
        private bool _libraryShown;
        private Video _currentVideo;
        private List<string> _moviesProgression;
        private Screen _currentScreen;
        private TransparentPanel _panelMouseControl;
        private Welcome _welcome;
        #endregion

        #region Properties
        public Screen CurrentScreen
        {
            get { return _currentScreen; }
            set { _currentScreen = value; }
        }
        public string CurrentDirectory
        {
            get { return _currentDirectory; }
            set { _currentDirectory = value; }
        }
        public string SeriePathPreview
        {
            get { return _seriePathPreview; }
            set { _seriePathPreview = value; }
        }
        public string SeriePathNext
        {
            get { return _seriePathNext; }
            set { _seriePathNext = value; }
        }
        public List<Video> CurrentDirectoryFiles
        {
            get { return _currentDirectoryFiles; }
            set { _currentDirectoryFiles = value; }
        }
        public List<string> MoviesProgression
        {
            get { return _moviesProgression; }
            set { _moviesProgression = value; }
        }
        public Video CurrentVideo
        {
            get { return _currentVideo; }
            set { _currentVideo = value; }
        }
        public ToolStripMenuVDO Tsm
		{
			get { return _tsm; }
			set { _tsm = value as ToolStripMenuVDO; }
		}
        public Panel Sheet
        {
            get { return _sheet; }
            set { _sheet = value; }
        }
		public override bool Openned
		{
			get { return _currentVideo != null; }
        }
        #endregion

        #region Constructor
        public Interface_vdo()
        {
            Init();
        }
        public Interface_vdo(List<String> lts)
        {
            _explorerShown = true;
            _libraryShown = false;
            BuildToolBar();
            BuildPanel();
            //separationChar = ';';
            //displayMode = "textbox";
            //openned = false;
            //listToolStrip = lts;
        }
        #endregion

        #region ACTION
        [Description("french[lancer.video(nom)];english[launch.video(name)]")]
        public static void ACTION_130_launch_video(string objet)
        {
            Demo d = new Demo(null);
            d.Show();
        }
        #endregion

        #region Methods Public
        public override bool Open(object o)
        {
            if (_videoFrame == null) BuildPanel();
            if (o is string)
            {
                _videoFrame.OpenFile(o as string);
                _tsm.UpdateVideoDetails();
                AddMovieAdvancement();
            }
            else
            {
                _videoFrame.OpenFile();
                _tsm.UpdateVideoDetails();
                AddMovieAdvancement();
            }
			return false;
		}
		public override void Print()
		{
			
		}
		public override void Close()
		{
			try 
			{
                SaveMovieProgression();
                SaveUserParams();
            }
			catch
			{
				
			}
		}
        public override bool Save()
		{
			return false;
		}
		public override void ZoomIn()
		{
			
		}
		public override void ZoomOut()
		{
			
		}
		public override void Copy()
		{
			
		}
		public override void Cut()
		{
			
		}
		public override void Paste()
		{
			
		}
		public override void Resize()
        {

        }
		public override void GlobalAction(object sender, EventArgs e)
		{
			ToolBarEventArgs tbea = e as ToolBarEventArgs;
			string action = tbea.EventText;
			switch (action)
            {
                case "openVideo":
                    LaunchOpenVideo();
                    break;
                case "showExplorer":
                    LaunchShowExplorer();
                    break;
                case "showLibrary":
                    LaunchShowLibrary();
                    break;
                case "screenFull":
                    LaunchFullScreen();
                    break;
                case "browseSubtitle":
                    LaunchBrowseSubtitle();
                    break;
                case "subtitleDownloadRequest":
                    LaunchDownloadSubtitle();
                    break;
                case "disableSubtitle":
                    LaunchDisableSubtitle();
                    break;
                case "relaunchVideo":
                    LaunchSetOldPosition();
                    break;
                case "serieNext":
                    LaunchNextEpisod();
                    break;
                case "seriePreview":
                    LaunchPreviewEpisod();
                    break;
                case "moveVideo":
                    break;
                case "rotationScreen0":
                    LaunchRotationVideo(0);
                    break;
                case "rotationScreen90":
                    LaunchRotationVideo(90);
                    break;
                case "rotationScreen180":
                    LaunchRotationVideo(180);
                    break;
                case "rotationScreen270":
                    LaunchRotationVideo(270);
                    break;
                case "analyse":
                    LaunchAnalyse();
                    break;
                case "report an issue":
                    LaunchReportIssue();
                    break;
            }
        }

        public void Dispose()
        {
            if (_videoFrame != null) _videoFrame.Dispose();
            if (_formFrame != null) _formFrame.Dispose();
        }
        public RibbonTab BuildToolBar()
        {
            _tsm = new ToolStripMenuVDO(this);
            return _tsm;
        }
        public void BuildPanel()
        {
            BuildPanelVideo();
            
            _sheet = new Panel();
            _sheet.Controls.Add(_panelVideo);
            _sheet.Disposed += _sheet_Disposed;

            _welcome = new Welcome();
            _welcome.Dock = DockStyle.Fill;
            //_sheet.Controls.Add(_welcome);
        }
        public long GetMovieAdvancement(string movieTitle)
        {
            string[] tab;
            string instMovie;

            try
            {
                foreach (var movie in _moviesProgression)
                {
                    tab = movie.Split('#');
                    instMovie = tab[0];
                    if (instMovie.Equals(movieTitle) && tab.Length > 2)
                    {
                        return long.Parse(tab[2]);
                    }
                }
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 0000 ] User settings have been modified out of the program ! \n\n" + exp.Message);
            }

            return 0;
        }
        public void SaveMovieProgression()
        {
            string thisMovie = string.Format("{0}#{1}#{2}", CurrentVideo.Path, DateTime.Now, CurrentVideo.Time);
            string instMovie;
            DateTime date;
            List<string> finalList = new List<string>();
            foreach (var movie in _moviesProgression)
            {
                instMovie = movie.Split('#')[0];
                if (File.Exists(instMovie))
                { 
                    if (instMovie.Equals(CurrentVideo.Path))
                    {
                        if (((((double)CurrentVideo.Time * 100) / (double)CurrentVideo.Length) * 10000 < 95) && ((((double)CurrentVideo.Time * 100) / (double)CurrentVideo.Length) * 10000 > 10))
                        {
                            finalList.Add(thisMovie);
                        }
                    }
                    else
                    {
                        if (DateTime.TryParse(movie.Split('#')[1], out date))
                        {
                            if (date >= DateTime.Now.AddMonths(-2))
                            { 
                                finalList.Add(movie);
                            }
                        }
                    }
                }
            }
            _moviesProgression = finalList;

            Properties.Settings.Default.movies = new System.Collections.Specialized.StringCollection();
            foreach (var movie in _moviesProgression)
            {
                Properties.Settings.Default.movies.Add(movie);
            }
            Properties.Settings.Default.Save();
        }
        public bool IsMovieProgressionAvailable()
        {
            string moviePath;
            foreach (var movie in _moviesProgression)
            {
                moviePath = movie.Split('#')[0];
                if (moviePath.Equals(CurrentVideo.Path)) return true;
            }
            return false;
        }
        public void LoadDirectoryFiles()
        {
            if (CurrentVideo != null && !string.IsNullOrEmpty(CurrentVideo.Path))
            {
                if (Path.GetDirectoryName(CurrentVideo.Path) != _currentDirectory) { _currentDirectoryFiles = DetectMovies(); }
                _seriePathNext = DetectNextEpisod();
                _seriePathPreview = DetectPreviewEpisod();

                _tsm.UpdateVideoDetails();
            }
            else
            {
                _seriePathPreview = string.Empty;
                _seriePathNext = string.Empty;
            }
        }
        public void SaveUserParams()
        {
            Properties.Settings.Default.language = GetText.CurrentLanguage.ToString();
            Properties.Settings.Default.volume = _videoFrame.TrackBarSound.Value;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Methods Launcher
        private void LaunchDisableSubtitle()
        {
            _currentVideo.CurrentSubtitlePath = string.Empty;
            _tsm.UpdateVideoDetails();
        }
        private async void LaunchDownloadSubtitle()
        {
            try
            {
                Task<string> taskSubtitle = SubtitleDownloader.SearchSubtitle(_currentVideo.Path, _currentVideo.SubtitleRequested);
                string subtitle = await taskSubtitle;
                if (!string.IsNullOrEmpty(subtitle))
                { 
                    _currentVideo.Subtitle = new Subtitle();
                    _currentVideo.Subtitle.SubtitleFileName = subtitle;
                }
                _tsm.UpdateVideoDetails();
            }
            catch (Exception exp)
            {
                Log.Write("[ CRT : 0123 ] Cannot download subtitle files \n\n" + exp.Message);
            }
        }
        private void LaunchOpenVideo()
        {
            if (_videoFrame == null) BuildPanel();
            if (_videoFrame.IsPlaying) _videoFrame.Pause();
            _videoFrame.OpenFile();
            _tsm.UpdateVideoDetails();
            AddMovieAdvancement();
        }
        private void LaunchFullScreen()
        {
            try
            {
                _panelVideo.Dock = DockStyle.None;
                _sheet.Controls.Remove(_panelVideo);

                Rectangle bounds = _currentScreen.Bounds;
                if (_formFrame != null) _formFrame.Dispose();
                _formFrame = new Form();
                _formFrame.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                _formFrame.FormBorderStyle = FormBorderStyle.None;
                _formFrame.StartPosition = FormStartPosition.CenterParent;
                _formFrame.WindowState = FormWindowState.Maximized;
                _formFrame.ShowIcon = false;
                _formFrame.ShowInTaskbar = false;

                _panelMouseControl.Width = _formFrame.Width;
                _panelMouseControl.Height = _formFrame.Height - 100;
                _formFrame.Controls.Add(_panelMouseControl);

                _formFrame.Controls.Add(_panelVideo);
                _formFrame.KeyPress += _formFrame_KeyPress; ;
                _formFrame.FormClosing += _formFrame_FormClosing;
                _panelVideo.Dock = DockStyle.Fill;
                _videoFrame.FullScreen = true;
                _formFrame.ShowDialog();
            }
            catch (Exception exp8401)
            {
                Log.Write("[ CRT : 8401 ] Error during the full screen frame execution.\nPlease close the video sheet and relaunch it.\nYou can send the following message to support teams :\n\n" + exp8401.Message);
            }
        }
        private void LaunchDisableFullScreen()
        {
            _videoFrame.FullScreen = false;
            _panelVideo.Dock = DockStyle.None;
            if (_formFrame != null && !_formFrame.IsDisposed)
            {
                _formFrame.Controls.Remove(_panelVideo);
                _formFrame.Controls.Remove(_panelMouseControl);
                _formFrame.Dispose();
            }

            _sheet.Controls.Add(_panelVideo);
            _panelVideo.Dock = DockStyle.Fill;
        }
        private void LaunchShowExplorer()
        {
            _explorerShown = !_explorerShown;
            if (_explorerShown)
            {
            }
            else
            {
            }
        }
        private void LaunchShowLibrary()
        {
            _libraryShown = !_libraryShown;
            if (_libraryShown)
            {
            }
            else
            {
            }
        }
        private void LaunchBrowseSubtitle()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Subtitles Files (.srt)|*.srt|All Files (*.*)|*.*";
            if (_currentDirectory!= null) ofd.InitialDirectory = _currentDirectory;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _videoFrame.Pause();
                _currentVideo.CurrentSubtitlePath = ofd.FileName;
                _tsm.UpdateVideoDetails();
                _videoFrame.Pause();
            }
        }
        private void LaunchSetOldPosition()
        {
            _videoFrame.LoadPosition();
        }
        private void LaunchNextEpisod()
        {
            if (!string.IsNullOrEmpty(_seriePathNext))
            {
                _videoFrame.Stop();
                Open(_seriePathNext);
            }
        }
        private void LaunchPreviewEpisod()
        {
            if (!string.IsNullOrEmpty(_seriePathPreview))
            {
                _videoFrame.Stop();
                Open(_seriePathPreview);
            }
        }
        private void LaunchRotationVideo(int rotation)
        {
            _videoFrame.Rotation(rotation);
        }
        private void LaunchReportIssue()
        {

        }
        private void LaunchAnalyse()
        {
            bool alternate = true;
            int diff;
            StringBuilder analyseResult = new StringBuilder();
            Image currentSnap, lastSnap = null;

            if (!Directory.Exists(string.Format("VideoAnalayse_{0}", _currentVideo.NameClean.Replace(" ", "_"))))
            {
                Directory.CreateDirectory(string.Format("VideoAnalayse_{0}", _currentVideo.NameClean.Replace(" ", "_")));
            }

            if (_videoFrame != null)
            {
                VideoPlayer videoAnalyzer = new VideoPlayer(this);
                videoAnalyzer.OpenFile(_currentVideo.Path);
                System.Threading.Thread.Sleep(100);
                videoAnalyzer.Mute();

                analyseResult.AppendLine("<xml>");
                for (int i = 0; i < videoAnalyzer.VideoLength / 10000; i += 100)
                {
                    try
                    {
                        alternate = !alternate;
                        videoAnalyzer.SetPosition(i);
                        currentSnap = videoAnalyzer.GetScreenShot(string.Format(string.Format("VideoAnalayse_{0}\\videoSnap_{1}.jpg", _currentVideo.NameClean.Replace(" ", "_"), alternate)));
                        if (lastSnap != null)
                        {
                            diff = Interface_image.ACTION_139_compare(currentSnap, lastSnap);
                            analyseResult.AppendLine(string.Format("    <moment time=\"{0}\" match=\"{1}\" />", i / 100, diff));
                        }
                        lastSnap = currentSnap;
                    }
                    catch (Exception exp)
                    {
                        break;
                    }
                }
                analyseResult.AppendLine("</xml>");
                using (StreamWriter sw = new StreamWriter(string.Format("VideoAnalayse_{0}\\AnalyseMovie{0}.xml", _currentVideo.NameClean.Replace(" ", "_"))))
                {
                    sw.Write(analyseResult.ToString());
                }
            }
        }
        private void LaunchWelcomeView()
        {
            _sheet.Controls.Clear();
            _sheet.Controls.Add(_welcome);
        }
        private void LaunchVideoPlayView()
        {
            _sheet.Controls.Clear();
            _sheet.Controls.Add(_videoFrame);
        }
        #endregion

        #region Methods	private
        private void Init()
        {
            _currentDirectoryFiles = new List<Video>();
            _explorerShown = true;
            _libraryShown = false;

            LoadMoviesAdvancement();
            BuildToolBar();
            BuildPanel();
            InitPanelMouseControl();
        }
        private void InitPanelMouseControl()
        {
            _panelMouseControl = new TransparentPanel();
            _panelMouseControl.Top = 0;
            _panelMouseControl.Left = 0;
            _panelMouseControl.MouseClick += _panelMouseControl_MouseClick;
            _panelMouseControl.DoubleClick += _panelMouseControl_DoubleClick;
            _panelMouseControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        }
        private void AddMovieAdvancement()
        {
            if (CurrentVideo == null) return;
            bool done = false;
            string thisMovie = string.Format("{0}#{1}#{2}", CurrentVideo.Path, DateTime.Now, CurrentVideo.Time);
            string instMovie;
            List<string> finalList = new List<string>();
            foreach (var movie in _moviesProgression)
            {
                instMovie = movie.Split('#')[0];
                if (instMovie.Equals(CurrentVideo.Path))
                {
                    if (CurrentVideo.Length == 0 || (((((double)CurrentVideo.Time / (double)CurrentVideo.Length) * 100) * 10000 < 95) && (((double)CurrentVideo.Time / (double)CurrentVideo.Length) * 100) * 10000 > 10))
                    {
                        finalList.Add(thisMovie);
                        done = true;
                    }
                }
                else
                {
                    finalList.Add(movie);
                }
            }
            if (!done)
            {
                _moviesProgression.Add(thisMovie);
            }
        }
        private void LoadMoviesAdvancement()
        {
            _moviesProgression = new List<string>();
            if (Properties.Settings.Default.movies != null)
            {
                foreach (var movie in Properties.Settings.Default.movies)
                {
                    _moviesProgression.Add(movie);
                }
            }
        }
        private void BuildPanelVideo()
        {
            _panelVideo = new Panel();
            _panelVideo.Visible = false;
            _panelVideo.Dock = DockStyle.Fill;
            BuildVideoFrame();
            _panelVideo.Visible = true;
        }
        private void BuildVideoFrame()
        {
            try
            {
                _videoFrame = new VideoPlayer(this);
                _videoFrame.FullScreenExit += _videoFrame_FullScreenExit;
                _videoFrame.FullScreenRequested += _videoFrame_FullScreenRequested;
                _videoFrame.Dock = DockStyle.Fill;

                _panelVideo.Controls.Add(_videoFrame);
                _panelVideo.BackColor = Color.WhiteSmoke;
            }
            catch (Exception exp8400)
            {
                Log.Write("[ CRT : 8400 ] Cannot create the video frame. \n" + exp8400.Message);
            }
        }
        private string DetectNextEpisod()
        {
            _seriePathNext = string.Empty;
            if (_currentVideo != null && !string.IsNullOrEmpty(_currentVideo.Season))
            {
                int currentEpisod = int.Parse(_currentVideo.Episod);
                int currentSeason = int.Parse(_currentVideo.Season);
                int nextEpisod = currentEpisod + 1;
                int nextSeason = currentSeason + 1;
                List<Video> videos;

                videos = _currentDirectoryFiles.Where(v =>
                    v.NameClean.Equals(_currentVideo.NameClean) &&
                    (v.Season.Equals(currentSeason.ToString()) || v.Season.Equals("0" + currentSeason.ToString())) &&
                    (v.Episod.Equals(nextEpisod.ToString()) || v.Episod.Equals("0" + nextEpisod.ToString()))
                ).ToList();

                if (videos.Count() == 1)
                {
                    _seriePathNext = videos[0].Path;
                }
                else
                { 
                    videos = _currentDirectoryFiles.Where(v =>
                        v.NameClean.Equals(_currentVideo.NameClean) &&
                        (v.Season.Equals(nextSeason.ToString()) || v.Season.Equals("0" + nextSeason.ToString())) &&
                        (v.Episod.Equals("1") || v.Episod.Equals("01"))
                    ).ToList();

                    if (videos.Count() == 1) { _seriePathNext = videos[0].Path; }
                }
            }
            return _seriePathNext;
        }
        private string DetectPreviewEpisod()
        {
            _seriePathPreview = string.Empty;
            if (_currentVideo != null && !string.IsNullOrEmpty(_currentVideo.Season))
            {
                int currentEpisod = int.Parse(_currentVideo.Episod);
                int currentSeason = int.Parse(_currentVideo.Season);
                int previewEpisod = currentEpisod - 1;
                int previewSeason = currentSeason - 1;
                List<Video> videos;

                videos = _currentDirectoryFiles.Where(v =>
                    v.NameClean.Equals(_currentVideo.NameClean) &&
                    (v.Season.Equals(currentSeason.ToString()) || v.Season.Equals("0" + currentSeason.ToString())) &&
                    (v.Episod.Equals(previewEpisod.ToString()) || v.Episod.Equals("0" + previewEpisod.ToString()))
                ).ToList();

                if (videos.Count() == 1)
                {
                    _seriePathPreview = videos[0].Path;
                }
                else
                { 
                    for (int i = 50; i > 0; i--)
                    {
                        videos = _currentDirectoryFiles.Where(v =>
                            v.NameClean.Equals(_currentVideo.NameClean) &&
                            (v.Season.Equals(previewSeason) || v.Season.Equals("0" + previewSeason)) &&
                            (v.Episod.Equals(i.ToString()) || v.Episod.Equals("0" + i))
                        ).ToList();

                        if (videos.Count() == 1)
                        {
                            _seriePathPreview = videos[0].Path;
                            break;
                        }
                    }
                }
            }
            return _seriePathPreview;
        }
        private List<Video> DetectMovies()
        {
            List<Video> listVid = new List<Video>();
            Video vid;
            string directory = Path.GetDirectoryName(CurrentVideo.Path);

            foreach (var file in Directory.GetFiles(directory))
            {
                vid = new Video();
                vid.Path = file;
                listVid.Add(vid);
            }

            return listVid;
        }
        #endregion

        #region Event
        private void _formFrame_KeyPress(object sender, KeyPressEventArgs e)
        {
            _videoFrame.KeyEvent(e.KeyChar);
        }
        private void _videoFrame_FullScreenExit()
        {
            LaunchDisableFullScreen();
        }
        private void _videoFrame_FullScreenRequested()
        {
            LaunchFullScreen();
        }
        private void _sheet_Disposed(object sender, EventArgs e)
        {
            _videoFrame.Dispose();
            Close();
        }
        private void _panelMouseControl_DoubleClick(object sender, EventArgs e)
        {
            _videoFrame.Pause();
            LaunchDisableFullScreen();
        }
        private void _panelMouseControl_MouseClick(object sender, MouseEventArgs e)
        {
            _videoFrame.Pause();
        }
        private void _formFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            LaunchDisableFullScreen();
        }
        #endregion
    }
}
