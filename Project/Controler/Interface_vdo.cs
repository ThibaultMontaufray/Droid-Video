// log code 84 02

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Tools4Libraries;

namespace Droid_video
{
    /// <summary>
    /// Interface for Tobi Assistant application : take care, some french word here to allow Tobi to speak with natural langage.
    /// </summary>            
    public class Interface_vdo : GPInterface
    {
		#region Attributes
        private ToolStripMenuVDO _tsm;
        private Stream _stream;
        private bool _openned;
        
        private Panel _sheet;
        private Panel _panelVideo;
        private Panel _panelExplorer;
        private Panel _panelFiles;
        private VideoPlayer _videoFrame;
        private Form _formFrame;
        private SplitContainer _splitMain;
        private SplitContainer _splitExplore;
        private ExplorerTree _explorertree;
        private Microsoft.VisualBasic.Compatibility.VB6.FileListBox _fileListBox;
        private ComboBox _comboListFile;

        private string _currentVideoPath;
        private bool _explorerShown;
        private bool _libraryShown;
		#endregion
		
		#region Properties
        public string CurrentVideoPath
        {
            get { return _currentVideoPath; }
            set { _currentVideoPath = value; }
        }
        public new ToolStripMenuVDO Tsm
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
			get { return _openned; }
		}
        #endregion

        #region Constructor
        public Interface_vdo()
        {
            _explorerShown = true;
            _libraryShown = false;
            BuildToolBar();
            BuildPanel();
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

        #region Methods Public
        public override bool Open(object o)
		{
            //stream = s;
            //openned = LoadData();
			return false;
		}
		public override void Print()
		{
			
		}
		public override void Close()
		{
			try 
			{
				_stream.Close();
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
            _splitMain.Width = _tsm.CurrentTabPage.Width;
            _splitMain.Height = _tsm.CurrentTabPage.Height;

            if (!_libraryShown && !_explorerShown) _splitMain.SplitterDistance = 1;
            else _splitMain.SplitterDistance = 250;

            if (_splitExplore.Height - 200 > 0) _splitExplore.SplitterDistance = 200;
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
                    LaunchScreenFull();
                    break;
                case "screen169":
                    LaunchScreen169();
                    break;
                case "screen15":
                    LaunchScreen15();
                    break;
			}
		}

        public RibbonTab BuildToolBar()
        {
            _tsm = new ToolStripMenuVDO(this);
            return _tsm;
        }
        public void BuildPanel()
        {
            //if (_tsm.CurrentTabPage != null)
            //{
            BuildPanelVideo();
            BuildPanelExplorer();
            BuildPanelFiles();

            _splitExplore = new SplitContainer();
            _splitExplore.Dock = DockStyle.Fill;
            _splitExplore.Panel1.Controls.Add(_panelExplorer);
            _splitExplore.Panel2.Controls.Add(_panelFiles);
            _splitExplore.Orientation = Orientation.Horizontal;

            _splitMain = new SplitContainer();
            _splitMain.Dock = DockStyle.Fill;
            _splitMain.SplitterDistance = 250;
            _splitMain.Panel2.Controls.Add(_splitExplore);
            _splitMain.Panel1.Controls.Add(_panelVideo);

            _sheet = new Panel();
            _sheet.Controls.Add(_splitMain);
            //}
        }
        #endregion

        #region Methods Launcher
        private void LaunchOpenVideo()
        {
            _videoFrame.OpenFile();
        }
        private void LaunchScreenFull()
        {
            try
            {
                _panelVideo.Dock = DockStyle.None;
                
                _splitMain.Panel2.Controls.Remove(_panelVideo);

                _formFrame = new Form();
                _formFrame.FormBorderStyle = FormBorderStyle.None;
                _formFrame.StartPosition = FormStartPosition.CenterParent;
                _formFrame.WindowState = FormWindowState.Maximized;
                _formFrame.ShowIcon = false;
                _formFrame.ShowInTaskbar = false;
                _formFrame.Controls.Add(_panelVideo);
                _formFrame.Disposed += new EventHandler(f_Disposed);
                _formFrame.KeyPress += _formFrame_KeyPress;
                _formFrame.FormClosing += _formFrame_FormClosing;

                _panelVideo.Dock = DockStyle.Fill;
                _formFrame.ShowDialog();
            }
            catch (Exception exp8401)
            {
                Log.write("[ CRT : 8401 ] Error during the full screen frame execution.\nPlease close the video sheet and relaunch it.\nYou can send the following message to support teams :\n\n" + exp8401.Message);
            }
        }
        
        private void LaunchScreen169()
        {
            _panelVideo.Dock = DockStyle.None;
            if (_formFrame != null || !_formFrame.IsDisposed)
            {
                _formFrame.Controls.Remove(_panelVideo);
                _formFrame.Dispose();
            }

            _splitMain.Panel1.Controls.Add(_panelVideo);
            _panelVideo.Dock = DockStyle.Fill;
        }
        private void LaunchScreen15()
        {
            _panelVideo.Dock = DockStyle.None;
            if (_formFrame != null || !_formFrame.IsDisposed)
            {
                _formFrame.Controls.Remove(_panelVideo);
                _formFrame.Dispose();
            }

            _splitMain.Panel2.Controls.Add(_panelVideo);
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
        #endregion

        #region Methods	private
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
                _videoFrame = new VideoPlayer();
                _videoFrame.FullScreenRequested += _videoFrame_FullScreenRequested;
                _videoFrame.FullScreenExit += _videoFrame_FullScreenExit;
                _videoFrame.Dock = DockStyle.Fill;
                
                //_videoFrame.Left = 0;
                //_videoFrame.Width = _panelVideo.Width;
                //_videoFrame.Height = (_videoFrame.Width * 9) / 16;
                //_videoFrame.Top = (_panelVideo.Height / 2) - (_videoFrame.Height / 2);

                _panelVideo.Controls.Add(_videoFrame);
                _panelVideo.BackColor = Color.Black;
                //_panelVideo.Resize += _panelVideo_Resize;
            }
            catch (Exception exp8400)
            {
                Log.write("[ CRT : 8400 ] Cannot create the video frame. \n" + exp8400.Message);
            }
        }
        private void BuildPanelExplorer()
        {
            _panelExplorer = new Panel();
            _panelExplorer.Visible = false;
            _panelExplorer.Dock = DockStyle.Fill;
            _panelExplorer.BackColor = Color.WhiteSmoke;

            _explorertree = new ExplorerTree();
            _explorertree.PathChanged += new ExplorerTree.PathChangedEventHandler(explorertree_PathChanged);
            _explorertree.Dock = DockStyle.Fill;
            _panelExplorer.Controls.Add(_explorertree);
            _panelExplorer.Visible = true;
        }
        private void BuildPanelFiles()
        {
            _panelFiles = new Panel();
            _panelFiles.Dock = DockStyle.Fill;
            _panelFiles.BackColor = Color.WhiteSmoke;

            _fileListBox = new Microsoft.VisualBasic.Compatibility.VB6.FileListBox();
            _fileListBox.FormattingEnabled = true;
            _fileListBox.Pattern = "*.*";
            _fileListBox.Dock = DockStyle.Fill;
            _panelFiles.Controls.Add(_fileListBox);

            _comboListFile = new ComboBox();
            _comboListFile.Items.AddRange(new object[] 
            {
                "*.*",
                "*.avi;*.mkv;*.mov;*.mp4",
                "*.avi;*.mkv;*.mov;*.mp4;*.str;*.txt"
            });
            _comboListFile.Text = _comboListFile.Items[0].ToString();
            _comboListFile.Dock = DockStyle.Top;
            _comboListFile.TextChanged += new EventHandler(comboListFile_TextChanged);
            _panelFiles.Controls.Add(_comboListFile);

        }
        private void LoadPanelFiles()
        {
            _fileListBox.Path = _explorertree.SelectedPath;
        }
        #endregion

        #region Event
        private void _formFrame_KeyPress(object sender, KeyPressEventArgs e)
        {
            _videoFrame.KeyEvent(e.KeyChar);
        }
        private void _videoFrame_FullScreenExit()
        {
            LaunchScreen169();
        }
        private void _videoFrame_FullScreenRequested()
        {
            LaunchScreenFull();
        }
        private void f_Disposed(object sender, EventArgs e)
        {
            LaunchScreen169();
        }
        private void comboListFile_TextChanged(object sender, EventArgs e)
        {
            _fileListBox.Pattern = _comboListFile.Text;  
        }
        private void explorertree_PathChanged(object sender, EventArgs e)
        {
            LoadPanelFiles();
        }
        private void _formFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            LaunchScreen169();
        }
        #endregion
    }
}
