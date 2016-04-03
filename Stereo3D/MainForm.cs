using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Diagnostics;
using System.Threading;

namespace Stereo3D
{
    public partial class MainForm : Form
    {
        Image<Gray, Byte> LeftImage;
        Image<Gray, Byte> RightImage;
        UserRect cropLeftRect;
        UserRect cropRightRect;

        UserRect searchAreaRect;
        UserRect templateLeftRect;
        UserRect templateRightRect;

        int TemplateSz = 50;    // In px
        int SearchAreaX = 200;  // In px
        int SearchAreaY = 20;   // In px
        int Step = 5;           // In px
        int[,] Disparity;       // Disparity array (height map)
        //int[,] MeshCoordsX;
        //int[,] MeshCoordsY;      
        Size meshSize;

        BackgroundWorker backgroundWorker = new BackgroundWorker();
        bool isWorking = false;
        AutoResetEvent waitHandle = new AutoResetEvent(false);
           
        string SamplesPath = @"E:\cadelias's Docs\Dropbox\Thesis\Model3DTests\";
        //string SamplesPath = @"D:\Dropbox\Thesis\Model3DTests\";
       
        public MainForm()
        {
            InitializeComponent();
            // Set up image boxes
            pictureBoxLeftImg.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            pictureBoxLeftImg.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxRightImg.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            pictureBoxRightImg.SizeMode = PictureBoxSizeMode.Zoom;
            // Autoload test image at beginning
            pictureBoxLeftImg.Image = LeftImage = new Image<Gray, byte>(SamplesPath + "demoL.bmp");
            pictureBoxRightImg.Image = RightImage = new Image<Gray, byte>(SamplesPath + "demoR.bmp");

            // Set up cropping image rectangles
            Rectangle Rect = new Rectangle(new Point(0, 0), pictureBoxLeftImg.Size);
            Rect.Inflate(-10,-10);
            cropLeftRect = new UserRect(Rect);
            cropLeftRect.SetPictureBox(this.pictureBoxLeftImg);
            cropLeftRect.allowDeformingDuringMovement = false;
            cropRightRect = new UserRect(Rect);
            cropRightRect.isFixed = true;
            cropRightRect.allowDeformingDuringMovement = false;
            cropRightRect.SetPictureBox(this.pictureBoxRightImg);
            cropRightRect.allowDeformingDuringMovement = false;

            pictureBoxLeftImg.MouseMove += new MouseEventHandler(pictureBoxLeftImg_MouseMove);
            pictureBoxLeftImg.MouseMoveOverImage += new PictureBoxExtended.MouseMoveOverImageHandler(mouseOverLeftImage);
            pictureBoxRightImg.MouseMoveOverImage += new PictureBoxExtended.MouseMoveOverImageHandler(mouseOverRightImage);
            // Set up background worker
            backgroundWorker.DoWork += new DoWorkEventHandler(CreateMesh); // This does the job ...
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            backgroundWorker.WorkerSupportsCancellation = true; // This allows cancellation.
            backgroundWorker.WorkerReportsProgress = true;

            // For debugging - set up search area, and templates
            templateLeftRect = new UserRect(new Rectangle(0, 0, 0, 0), Color.Green);
            templateLeftRect.SetPictureBox(this.pictureBoxLeftImg);
            templateLeftRect.isFixed = true;
            templateLeftRect.visible = false;

            templateRightRect = new UserRect(new Rectangle(0, 0, 0, 0), Color.Green);
            templateRightRect.SetPictureBox(this.pictureBoxRightImg);
            templateRightRect.isFixed = true;
            templateRightRect.visible = false;

            searchAreaRect = new UserRect(new Rectangle(0, 0, 0, 0), Color.Yellow);
            searchAreaRect.SetPictureBox(this.pictureBoxRightImg);
            searchAreaRect.isFixed = true;
            searchAreaRect.visible = false;

            // Set up parameters in textboxes
            textBoxPatternSz.Text = TemplateSz.ToString();
            textBoxStep.Text = Step.ToString();
            textBoxRangeX.Text = SearchAreaX.ToString();
            textBoxRangeY.Text = SearchAreaY.ToString();

            progressBar.Maximum = 100;  // using percetange

        }

        private void mouseOverLeftImage(object sender, MouseEventArgs e)
        {
            labelCoordsLeft.Text = string.Format("Coords: {0}, {1}", e.X, e.Y);
        }

        private void mouseOverRightImage(object sender, MouseEventArgs e)
        {
            labelCoordsRight.Text = string.Format("Coords: {0}, {1}", e.X, e.Y);
        }

        private void pictureBoxLeftImg_MouseMove(object sender, MouseEventArgs e)
        {
            if (cropLeftRect.mIsClick)
            {
                cropRightRect.rect = cropLeftRect.rect;
                pictureBoxRightImg.Invalidate();
            }
        }

        private void buttonLoadLeftImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenDlg = new OpenFileDialog();
            OpenDlg.Filter = "Image files (*.jpg, *.jpeg, *.bmp, *.png) | *.jpg; *.jpeg; *.bmp; *.png";
            OpenDlg.Title = "Select left side image";
            if (OpenDlg.ShowDialog() == DialogResult.OK)
            {
                pictureBoxLeftImg.Image = LeftImage = new Image<Gray, byte>(OpenDlg.FileName);
            }
        }

        private void buttonLoadRightImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenDlg = new OpenFileDialog();
            OpenDlg.Filter = "Image files (*.jpg, *.jpeg, *.bmp, *.png) | *.jpg; *.jpeg; *.bmp; *.png";
            OpenDlg.Title = "Select right side image";
            if (OpenDlg.ShowDialog() == DialogResult.OK)
            {
                pictureBoxRightImg.Image = RightImage = new Image<Gray, byte>(OpenDlg.FileName);
            }
        }

        private void ResetProgressStatus()
        {
            progressBar.Value = 0;
            labelPercentage.Text = "0 %";
        }

        private void buttonCreateMesh_Click(object sender, EventArgs e)
        {
            if (isWorking/*backgroundWorker.IsBusy*/)
            {
                backgroundWorker.CancelAsync();
            }
            else
            {
                ResetProgressStatus();
                if (pictureBoxLeftImg.Size.IsEmpty || pictureBoxRightImg.Size.IsEmpty)
                    return;

                TemplateSz = Convert.ToInt32(textBoxPatternSz.Text);
                Step = Convert.ToInt32(textBoxStep.Text);
                SearchAreaX = Convert.ToInt32(textBoxRangeX.Text);
                SearchAreaY = Convert.ToInt32(textBoxRangeY.Text);
                if (checkDebugMode.Checked == true)
                {
                    searchAreaRect.visible = true;
                    templateLeftRect.visible = true;
                    templateRightRect.visible = true;
                }
                else
                {
                    searchAreaRect.visible = false;
                    templateLeftRect.visible = false;
                    templateRightRect.visible = false;
                }
                backgroundWorker.RunWorkerAsync();
                buttonCreateMesh.Text = "Cancel";
            }
        }

        private void ShowMeshDlg()
        {
            MeshView MeshDlg = new MeshView();

            // Preparte texture bitmap
            Point Pnt1, Pnt2;
            GetCroppedImageCoords(out Pnt1, out Pnt2);
            // Crop Left image
            // Need to be from actual starting i,j to last one
            // size has to size of this bitmap, (which should be equal to number of items in disparity times step)
            // we found the height for a specific number of pixels, skipping by step
            Pnt1.X += TemplateSz / 2 + SearchAreaX / 2;
            Pnt1.Y += TemplateSz / 2 + SearchAreaY / 2;
            Pnt2.X -= (TemplateSz / 2 + SearchAreaX / 2);
            Pnt2.Y -= (TemplateSz / 2 + SearchAreaY / 2);
            LeftImage.ROI = new Rectangle(Pnt1.X, Pnt1.Y, Pnt2.X - Pnt1.X, Pnt2.Y - Pnt1.Y);
            
            MeshDlg.SetupMeshView(Disparity, meshSize, LeftImage.ToBitmap(), Step);

            // Restore original image
            LeftImage.ROI = Rectangle.Empty;

            MeshDlg.Show();
        }

        private void GetCroppedImageCoords(out Point Pnt1, out Point Pnt2)
        {
            Pnt1 = pictureBoxLeftImg.TranslatePointToImageCoordinates(new Point(cropLeftRect.rect.Left, cropLeftRect.rect.Top));
            Pnt2 = pictureBoxLeftImg.TranslatePointToImageCoordinates(new Point(cropLeftRect.rect.Right, cropLeftRect.rect.Bottom));

            // Crop values to fit in image
            Pnt1.X = Math.Max(Pnt1.X, 0);
            Pnt2.X = Math.Max(Pnt2.X, 0);
            Pnt1.Y = Math.Max(Pnt1.Y, 0);
            Pnt2.Y = Math.Max(Pnt2.Y, 0);

            Pnt1.X = Math.Min(Pnt1.X, LeftImage.Width);
            Pnt2.X = Math.Min(Pnt2.X, LeftImage.Width);
            Pnt1.Y = Math.Min(Pnt1.Y, LeftImage.Height);
            Pnt2.Y = Math.Min(Pnt2.Y, LeftImage.Height);
        }

        private void CreateMesh(object sender, DoWorkEventArgs e)
        {
            isWorking = true;
            Point Pnt1, Pnt2;
            GetCroppedImageCoords(out Pnt1, out Pnt2);

            ///*** Crop Left image ***/
            //LeftImage.ROI = new Rectangle(Pnt1.X, Pnt1.Y, Pnt2.X - Pnt1.X, Pnt2.Y - Pnt1.Y);
            //// Create Destination Image
            //Image<Gray, Byte> LeftImgROI = new Image<Gray, byte>(pictureBoxLeftImg.Width, pictureBoxLeftImg.Height);
            //LeftImgROI = LeftImage.Copy();   /*Copy subimage*/
            //// Restore original image
            //LeftImage.ROI = Rectangle.Empty;

            ///*** Crop Right image ***/
            //RightImage.ROI = new Rectangle(Pnt1.X, Pnt1.Y, Pnt2.X - Pnt1.X, Pnt2.Y - Pnt1.Y);
            //// Create Destination Image
            //Image<Gray, Byte> RightImgROI = new Image<Gray, byte>(pictureBoxLeftImg.Width, pictureBoxLeftImg.Height);
            //RightImgROI = RightImage.Copy();   /*Copy subimage*/
            //// Restore original image
            //RightImage.ROI = Rectangle.Empty;
            Size TemplateSize = new Size(TemplateSz, TemplateSz);
            Size SearchSize = new Size(SearchAreaX + TemplateSz, SearchAreaY + TemplateSz);

            //**TODO Need to add search area here
            int sizeX = 1 + ((Pnt2.X - Pnt1.X - SearchAreaX - TemplateSz) / Step);
            int sizeY = 1 + ((Pnt2.Y - Pnt1.Y - SearchAreaY - TemplateSz) / Step);
            if (sizeX <= 0 || sizeY <= 0)
            {
                //this.Invoke(new Action(() => ));
                MessageBox.Show("Search area is too big for the selected crop section","Error");
                // Restore original image
                LeftImage.ROI = Rectangle.Empty;
                e.Cancel = true;
                return;
            }
          
            Disparity = new int[sizeX, sizeY];
            meshSize = new Size(sizeX, sizeY);
            // For each pixel we want to find on right image
            int _X = 0, _Y = 0;
            for (int j = Pnt1.Y + TemplateSz/2 + SearchAreaY/2; j <= Pnt2.Y - TemplateSz/2 - SearchAreaY/2; j += Step, _X = 0, _Y++)
            {
                for (int i = Pnt1.X + TemplateSz / 2 + SearchAreaX/2; i <= Pnt2.X - TemplateSz / 2 - SearchAreaX/2; i += Step, _X++)
                {
                    if (backgroundWorker.CancellationPending)
                    {
                        // Restore original image
                        RightImage.ROI = LeftImage.ROI = Rectangle.Empty;
                        e.Cancel = true;
                        return;
                    }
                        
                    // Search for max correlation inside the search area
                    //Debug.Print(string.Format("i:{0}, j:{1}\n", i, j));

                    // Select ROI in left Image (the template to search), centered on i,j
                    Rectangle LeftROI = new Rectangle(new Point(i - TemplateSz / 2, j - TemplateSz / 2), TemplateSize);
                    LeftImage.ROI = LeftROI;

                    // Select ROI in right image (the search area), centered on i,j
                    Rectangle SearchRect = new Rectangle(new Point(i - SearchAreaX / 2 - TemplateSz/2,
                                                                   j - SearchAreaY / 2 - TemplateSz/2), SearchSize);
                    RightImage.ROI = SearchRect;

                    // Do template matching
                    Image<Gray, float> resultImage = RightImage.MatchTemplate(LeftImage, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCORR_NORMED);
                    double[] minVals, maxVals;
                    Point[] minLoc, maxLoc;
                    resultImage.MinMax(out minVals, out maxVals, out minLoc, out maxLoc);

                    // Should return the top left corner of the template match in maxLoc[0], so convert to center
                    maxLoc[0].X += SearchRect.Left + TemplateSz / 2;
                    maxLoc[0].Y += SearchRect.Top + TemplateSz / 2;
                    Disparity[_X, _Y] = maxLoc[0].X - i; // Only taking deltaX
                    if (checkDebugMode.Checked == true)
                    {
                        // in case that it was pressed in the middle of the run
                        searchAreaRect.visible = true;
                        templateLeftRect.visible = true;
                        templateRightRect.visible = true;

                        // Show current template on left img, and on right img: search area, and found template.
                        RightImage.ROI = LeftImage.ROI = Rectangle.Empty;
                        Point SearchTL = pictureBoxRightImg.TranslateImageToPointCoordinates(new Point(SearchRect.Left, SearchRect.Top));
                        Point SearchBR = pictureBoxRightImg.TranslateImageToPointCoordinates(new Point(SearchRect.Right, SearchRect.Bottom));
                        
                        searchAreaRect.rect.Y = SearchTL.Y;
                        searchAreaRect.rect.X = SearchTL.X;
                        searchAreaRect.rect.Width= SearchBR.X - SearchTL.X;
                        searchAreaRect.rect.Height = SearchBR.Y - SearchTL.Y;

                        Point LeftROI_TL = pictureBoxRightImg.TranslateImageToPointCoordinates(new Point(LeftROI.Left, LeftROI.Top));
                        Point LeftROI_BR = pictureBoxRightImg.TranslateImageToPointCoordinates(new Point(LeftROI.Right, LeftROI.Bottom));

                        templateLeftRect.rect.Y = LeftROI_TL.Y;
                        templateLeftRect.rect.X = LeftROI_TL.X;
                        templateLeftRect.rect.Width = LeftROI_BR.X - LeftROI_TL.X;
                        templateLeftRect.rect.Height = LeftROI_BR.Y - LeftROI_TL.Y;

                        Rectangle RightROI = new Rectangle(new Point(maxLoc[0].X - TemplateSz / 2, maxLoc[0].Y - TemplateSz / 2), TemplateSize);
                        Point RightROI_TL = pictureBoxRightImg.TranslateImageToPointCoordinates(new Point(RightROI.Left, RightROI.Top));
                        Point RightROI_BR = pictureBoxRightImg.TranslateImageToPointCoordinates(new Point(RightROI.Right, RightROI.Bottom));

                        templateRightRect.rect.Y = RightROI_TL.Y;
                        templateRightRect.rect.X = RightROI_TL.X;
                        templateRightRect.rect.Width = RightROI_BR.X - RightROI_TL.X;
                        templateRightRect.rect.Height = RightROI_BR.Y - RightROI_TL.Y;

                        pictureBoxLeftImg.Invalidate();
                        pictureBoxRightImg.Invalidate();
                        textBoxDisparity.Invoke(new Action(() => textBoxDisparity.Text = Disparity[_X, _Y].ToString()));
                        // Wait for button press
                        buttonCreateMesh.Invoke(new Action(() => buttonDebugNext.Enabled = true));
                        waitHandle.WaitOne();
                        buttonCreateMesh.Invoke(new Action(() => buttonDebugNext.Enabled = false));
                        
                    }
 
                }
                backgroundWorker.ReportProgress((100*(_Y+1)) / sizeY);
            }

            // Restore original image
            RightImage.ROI = LeftImage.ROI = Rectangle.Empty;

            buttonCreateMesh.Invoke(new Action(() => buttonCreateMesh.Text = "Create 3D Mesh"));

            
        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            labelPercentage.Text = e.ProgressPercentage.ToString() + " %";
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isWorking = false;
            if (e.Cancelled)
            {
                ResetProgressStatus();
                buttonCreateMesh.Text = "Create 3D Mesh";
                if (searchAreaRect.visible == true) // was in debug
                {
                    searchAreaRect.visible = false;
                    templateLeftRect.visible = false;
                    templateRightRect.visible = false;
                    pictureBoxLeftImg.Invalidate();
                    pictureBoxRightImg.Invalidate();
                }
            }
            else if (e.Error == null)
            {
                ShowMeshDlg();
            }
        }

        private void buttonDebugNext_Click(object sender, EventArgs e)
        {
            waitHandle.Set();
        }
    }
}
