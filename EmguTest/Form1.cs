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


namespace EmguTest
{
    public partial class Form1 : Form
    {
        UserRect urect;
        UserRect urect2;
        Image<Gray, Byte> srcImage;
        public Form1()
        {
            InitializeComponent();
            urect = new UserRect(new Rectangle(30, 30, 100, 100));
            urect.fixedAR = true;
            //urect2 = new UserRect(new Rectangle(0, 0, 25, 25), Color.Yellow); // could be used for cropping
            urect.SetPictureBox(this.imageBox1);
            //urect2.SetPictureBox(this.imageBox1);
            imageBox1.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            imageBox1.SizeMode = PictureBoxSizeMode.Zoom;
            srcImage = new Image<Gray, byte>(@"E:\cadelias's Docs\Downloads\Tel Aviv 050.JPG");
            imageBox1.Image = srcImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                srcImage = new Image<Gray, byte>(Openfile.FileName);
                imageBox1.Image = srcImage;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (imageBox1.Size.IsEmpty)
                return;

            //urect.rect.Height = urect.rect.Width = TemplateSz;
            // Now I need to be able to do the following: If I want to set the rectangle to 50 px, what is the
            // needed size according to the current zoom level (in case i want to add zoom and change rectangle
            // size accordingly)
            Point Pnt1 = imageBox1.TranslatePointToImageCoordinates(new Point(urect.rect.Left, urect.rect.Top));
            Point Pnt2 = imageBox1.TranslatePointToImageCoordinates(new Point(urect.rect.Right, urect.rect.Bottom));

            srcImage.ROI = new Rectangle(Pnt1.X, Pnt1.Y, Pnt2.X-Pnt1.X, Pnt2.Y-Pnt1.Y);
            // new Rectangle ( x, y , width, hight) [x and y are the top left point of the ROI]

            /*Create Destination Image*/
            Image<Gray, Byte> imgROI = new Image<Gray, byte>(srcImage.Width, srcImage.Height);
            /*Copy subimage*/
            imgROI = srcImage.Copy();
            // Restore original image
            srcImage.ROI = Rectangle.Empty;

            // Do template matching
            Image<Gray, float> resultImage = srcImage.MatchTemplate(imgROI, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCORR_NORMED);
            double[] minVals, maxVals;
            Point[] minLoc, maxLoc;
            resultImage.MinMax(out minVals, out maxVals, out minLoc, out maxLoc);
            
            imageBox2.Image = imgROI;
            imageBox2.SizeMode = PictureBoxSizeMode.Zoom;
            //srcImage.Draw(new CircleF(maxLoc[0], 10), new Gray(200.0), 1);
            Rectangle rect = new Rectangle(maxLoc[0], new Size(20, 20));
            srcImage.Draw(rect, new Gray(200.0), 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double TemplateSz = Convert.ToDouble(textBox1.Text);    // pixels
            float ratioHeight = (float)imageBox1.Height /imageBox1.Image.Size.Height;  // units/px
            float ratioWidth = (float)imageBox1.Width / imageBox1.Image.Size.Width;  // units/px

            urect.rect.Height = (int)(ratioHeight * TemplateSz);
            urect.rect.Width = (int)(ratioWidth * TemplateSz);
            imageBox1.Invalidate();
        }
        
        void Application_Idle(object sender, EventArgs e)
        {
        }


        private void buttonShow3D_Click(object sender, EventArgs e)
        {
            Form Show3DForm = new MeshViewer();
            Show3DForm.Show();
        }
    }
}
