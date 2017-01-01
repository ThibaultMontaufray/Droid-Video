namespace Assistant
{
    partial class GUI
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            this.imageList32 = new System.Windows.Forms.ImageList(this.components);
            this.imageList16 = new System.Windows.Forms.ImageList(this.components);
            this.imageListTreeView = new System.Windows.Forms.ImageList(this.components);
            this.imageListVideo = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // imageList32
            // 
            this.imageList32.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList32.ImageStream")));
            this.imageList32.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList32.Images.SetKeyName(0, "screen169");
            this.imageList32.Images.SetKeyName(1, "screen15");
            this.imageList32.Images.SetKeyName(2, "screenFull");
            this.imageList32.Images.SetKeyName(3, "navRewind");
            this.imageList32.Images.SetKeyName(4, "navPlay");
            this.imageList32.Images.SetKeyName(5, "navPause");
            this.imageList32.Images.SetKeyName(6, "navStop");
            this.imageList32.Images.SetKeyName(7, "navForward");
            this.imageList32.Images.SetKeyName(8, "showRead");
            this.imageList32.Images.SetKeyName(9, "showExplorer");
            this.imageList32.Images.SetKeyName(10, "showLibrary");
            this.imageList32.Images.SetKeyName(11, "openVideo");
            // 
            // imageList16
            // 
            this.imageList16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList16.ImageStream")));
            this.imageList16.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList16.Images.SetKeyName(0, "navForward");
            this.imageList16.Images.SetKeyName(1, "navPause");
            this.imageList16.Images.SetKeyName(2, "navPlay");
            this.imageList16.Images.SetKeyName(3, "navRewind");
            this.imageList16.Images.SetKeyName(4, "navStop");
            this.imageList16.Images.SetKeyName(5, "showExplorer");
            this.imageList16.Images.SetKeyName(6, "showRead");
            this.imageList16.Images.SetKeyName(7, "showLibrary");
            this.imageList16.Images.SetKeyName(8, "screen169");
            this.imageList16.Images.SetKeyName(9, "screen15");
            this.imageList16.Images.SetKeyName(10, "screenFull");
            this.imageList16.Images.SetKeyName(11, "openVideo");
            // 
            // imageListTreeView
            // 
            this.imageListTreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTreeView.ImageStream")));
            this.imageListTreeView.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTreeView.Images.SetKeyName(0, "default");
            this.imageListTreeView.Images.SetKeyName(1, "computer");
            this.imageListTreeView.Images.SetKeyName(2, "favorite");
            this.imageListTreeView.Images.SetKeyName(3, "network");
            // 
            // imageListVideo
            // 
            this.imageListVideo.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListVideo.ImageStream")));
            this.imageListVideo.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListVideo.Images.SetKeyName(0, "0003");
            this.imageListVideo.Images.SetKeyName(1, "0021");
            this.imageListVideo.Images.SetKeyName(2, "browse");
            this.imageListVideo.Images.SetKeyName(3, "btnplay");
            this.imageListVideo.Images.SetKeyName(4, "pause");
            this.imageListVideo.Images.SetKeyName(5, "stop");
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 261);
            this.Name = "GUI";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ImageList imageList32;
        public System.Windows.Forms.ImageList imageList16;
        public System.Windows.Forms.ImageList imageListTreeView;
        public System.Windows.Forms.ImageList imageListVideo;


    }
}

