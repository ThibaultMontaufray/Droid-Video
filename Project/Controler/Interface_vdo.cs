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
                    LaunchFullScreen();
                    break;
                case "screen15":
                    LaunchDisableFullScreen();
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
                _sheet.Controls.Remove(_panelVideo);

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
        
        private void LaunchFullScreen()
        {
            _panelVideo.Dock = DockStyle.None;
            if (_formFrame != null && !_formFrame.IsDisposed)
            {
                _formFrame.Controls.Remove(_panelVideo);
                _formFrame.Dispose();
            }

            _sheet.Controls.Add(_panelVideo);
            _panelVideo.Dock = DockStyle.Fill;
        }
        private void LaunchDisableFullScreen()
        {
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
                _panelVideo.BackColor = Color.WhiteSmoke;
                //_panelVideo.Resize += _panelVideo_Resize;
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
            LaunchScreenFull();
        }
        private void f_Disposed(object sender, EventArgs e)
        {
            LaunchFullScreen();
        }
        private void _formFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            LaunchFullScreen();
        }
        #endregion
    }
}
