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
        private VideoPlayer _videoFrame;
        private Form _formFrame;
        
        private bool _explorerShown;
        private bool _libraryShown;
        private Video _currentVideo;
        #endregion

        #region Properties
        public Video CurrentVideo
        {
            get { return _currentVideo; }
            set { _currentVideo = value; }
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
                case "screen169":
                    LaunchFullScreen();
                    break;
                case "screen15":
                    LaunchDisableFullScreen();
                    break;
                case "browseSubtitle":
                    LaunchBrowseSubtitle();
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
            
            _sheet = new Panel();
            _sheet.Controls.Add(_panelVideo);
            _sheet.Disposed += _sheet_Disposed;
            //}
        }
        #endregion

        #region Methods Launcher
        private void LaunchOpenVideo()
        {
            _videoFrame.OpenFile();
        }
        private void LaunchFullScreen()
        {
            try
            {
                _panelVideo.Dock = DockStyle.None;
                _sheet.Controls.Remove(_panelVideo);

                if (_formFrame != null) _formFrame.Dispose();
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
                _videoFrame.FullScreen = true;

                _formFrame.ShowDialog();
            }
            catch (Exception exp8401)
            {
                Log.write("[ CRT : 8401 ] Error during the full screen frame execution.\nPlease close the video sheet and relaunch it.\nYou can send the following message to support teams :\n\n" + exp8401.Message);
            }
        }
        private void LaunchDisableFullScreen()
        {
            _videoFrame.FullScreen = false;
            _panelVideo.Dock = DockStyle.None;
            if (_formFrame != null && !_formFrame.IsDisposed)
            {
                _formFrame.Controls.Remove(_panelVideo);
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
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _currentVideo.Subtitle = new Subtitle(ofd.FileName);
                _currentVideo.Path = ofd.FileName;
            }
        }
        private void LaunchGetSubtitle()
        {

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
                _videoFrame = new VideoPlayer(this);
                _videoFrame.FullScreenRequested += _videoFrame_FullScreenRequested;
                _videoFrame.FullScreenExit += _videoFrame_FullScreenExit;
                _videoFrame.Dock = DockStyle.Fill;

                _panelVideo.Controls.Add(_videoFrame);
                _panelVideo.BackColor = Color.WhiteSmoke;
            }
            catch (Exception exp8400)
            {
                Log.write("[ CRT : 8400 ] Cannot create the video frame. \n" + exp8400.Message);
            }
        }
        #endregion

        #region Event
        private void _formFrame_KeyPress(object sender, KeyPressEventArgs e)
        {
            _videoFrame.KeyEvent(e.KeyChar);
        }
        private void _videoFrame_FullScreenExit()
        {
            LaunchFullScreen();
        }
        private void _videoFrame_FullScreenRequested()
        {
            LaunchFullScreen();
        }
        private void f_Disposed(object sender, EventArgs e)
        {
            LaunchDisableFullScreen();
        }
        private void _formFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            LaunchDisableFullScreen();
        }
        private void _sheet_Disposed(object sender, EventArgs e)
        {
            _videoFrame.Dispose();
        }
        #endregion
    }
}
