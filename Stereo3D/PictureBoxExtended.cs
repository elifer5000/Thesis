using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV.UI;

namespace Stereo3D
{
	/// <summary>
	/// This class extends the PictureBox control with the following:
	/// <list type="table">
	/// <item><term>GetRelativeMousePosition()</term></item>
	/// </list>
	/// </summary>
	public class PictureBoxExtended : ImageBox
	{

		#region Events

		/// <summary>
		/// Handler for when the mouse moves over the image part of the picture box
		/// </summary>
		public delegate void MouseMoveOverImageHandler(object sender, MouseEventArgs e);

		/// <summary>
		/// Occurs when the mouse have moved over the image part of a picture box
		/// </summary>
		public event MouseMoveOverImageHandler MouseMoveOverImage;

		#endregion

		#region Properties
		/// <summary>
		/// Gets the mouse position relative to the <see cref="PictureBox.Image">Image</see> top left corner
		/// </summary>
		/// <value>The location of the mouse translated onto the <see cref="PictureBox.Image">Image</see> .</value>
		public Point MousePositionOnImage
		{
			get { Point local = PointToClient(MousePosition); return TranslatePointToImageCoordinates(local);}
		}
		#endregion
		#region Methods
		#region Public Methods
		/// <summary>
		/// Translates a point to coordinates relative to the <see cref="PictureBox.Image">Image</see>.
		/// The supplied point is taken relativce to the control's upper left corner
		/// </summary>
		/// <param name="controlCoordinates">The point to translate, relative to the control's upper left corner.</param>
		/// <returns>A new point representing where over the <see cref="PictureBox.Image">Image</see> the supplied point is.</returns>
		public Point TranslatePointToImageCoordinates(Point controlCoordinates)
		{
			switch (SizeMode)
			{
				case PictureBoxSizeMode.Normal:
					return TranslateNormalMousePosition(controlCoordinates);
				case PictureBoxSizeMode.AutoSize:
					return TranslateAutoSizeMousePosition(controlCoordinates);
				case PictureBoxSizeMode.CenterImage:
					return TranslateCenterImageMousePosition(controlCoordinates);
				case PictureBoxSizeMode.StretchImage:
					return TranslateStretchImageMousePosition(controlCoordinates);
				case PictureBoxSizeMode.Zoom:
					return TranslateZoomMousePosition(controlCoordinates);
			}
			throw new NotImplementedException("PictureBox.SizeMode was not in a valid state");
		}

        public Point TranslateImageToPointCoordinates(Point imageCoordinates)
        {
            switch (SizeMode)
            {
                //case PictureBoxSizeMode.Normal:
                //    return TranslateNormalMousePosition(controlCoordinates);
                //case PictureBoxSizeMode.AutoSize:
                //    return TranslateAutoSizeMousePosition(controlCoordinates);
                //case PictureBoxSizeMode.CenterImage:
                //    return TranslateCenterImageMousePosition(controlCoordinates);
                //case PictureBoxSizeMode.StretchImage:
                //    return TranslateStretchImageMousePosition(controlCoordinates);
                case PictureBoxSizeMode.Zoom:
                    return TranslateZoomImagePosition(imageCoordinates);
            }
            throw new NotImplementedException("PictureBox.SizeMode was not in a valid state");
        }
		#endregion
		#region Protected Methods
		/// <summary>
		/// Gets the mouse position over the image when the <see cref="PictureBox">PictureBox's</see> <see cref="PictureBox.SizeMode">SizeMode</see> is set to AutoSize
		/// </summary>
		/// <param name="coordinates">Point to translate</param>
		/// <returns>A point relative to the top left corner of the <see cref="PictureBox.Image">Image</see></returns>
		/// <remarks>
		/// In AutoSize mode, the <see cref="PictureBox">PictureBox</see> is automagically resized* to the size of the <see cref="PictureBox.Image">Image.</see>
		/// Thus, the image is at the top left corner of the control, and no translation takes place.
		/// * This is not necessary true.  The <see cref="PictureBox">PictureBox</see> may NOT be resized depending on how it is docked in it's parent.
		/// However, even in these cases no translation is needed, as the image is rendered the same as if it was in Normal mode
		/// </remarks>
		protected Point TranslateAutoSizeMousePosition(Point coordinates)
		{
			//TODO: When we implement scrolling, we will have to make sure we test that properly. As of now, not sure how the rendering will take place
			return coordinates;
		}

		/// <summary>
		/// Gets the mouse position over the image when the <see cref="PictureBox">PictureBox's</see> <see cref="PictureBox.SizeMode">SizeMode</see> is set to Zoom
		/// </summary>
		/// <param name="coordinates">Point to translate</param>
		/// <returns>A point relative to the top left corner of the <see cref="PictureBox.Image">Image</see>
		/// If the Image is null, no translation is performed
		/// </returns>
		protected Point TranslateZoomMousePosition(Point coordinates)
		{
			//	test to make sure our image is not null
			if (Image == null) return coordinates;
			//	Make sure our control width and height are not 0 and our image width and height are not 0
			if (Width == 0 || Height == 0|| Image.Size.Width ==0||Image.Size.Height==0) return coordinates;
			//	This is the one that gets a little tricky.  Essentially, need to check the aspect ratio of the image to the aspect ratio of the control
			// to determine how it is being rendered
			float imageAspect = (float)Image.Size.Width / Image.Size.Height;
			float controlAspect = (float)Width / Height;
			float newX = coordinates.X;
			float newY = coordinates.Y;
			if(imageAspect>controlAspect)
			{
				//	This means that we are limited by width, meaning the image fills up the entire control from left to right
				float ratioWidth = (float)Image.Size.Width / Width;
				newX *= ratioWidth;
				float scale = (float)Width / Image.Size.Width;
				float displayHeight = scale * Image.Size.Height;
				float diffHeight = Height - displayHeight;
				diffHeight /= 2;
				newY -= diffHeight;
				newY /= scale;
			}
			else
			{
				//	This means that we are limited by height, meaning the image fills up the entire control from top to bottom
				float ratioHeight = (float)Image.Size.Height / Height;
				newY *= ratioHeight;
				float scale = (float)Height / Image.Size.Height;
				float displayWidth = scale * Image.Size.Width;
				float diffWidth = Width - displayWidth;
				diffWidth /= 2;
				newX -= diffWidth;
				newX /= scale;
			}
			return new Point((int)newX, (int)newY);
		}

        protected Point TranslateZoomImagePosition(Point coordinates)
		{
			//	test to make sure our image is not null
			if (Image == null) return coordinates;
			//	Make sure our control width and height are not 0 and our image width and height are not 0
			if (Width == 0 || Height == 0|| Image.Size.Width ==0||Image.Size.Height==0) return coordinates;
			//	This is the one that gets a little tricky.  Essentially, need to check the aspect ratio of the image to the aspect ratio of the control
			// to determine how it is being rendered
			float imageAspect = (float)Image.Size.Width / Image.Size.Height;
			float controlAspect = (float)Width / Height;
			float newX = coordinates.X;
			float newY = coordinates.Y;
			if (imageAspect>controlAspect)
			{
                //	This means that we are limited by width, meaning the image fills up the entire control from left to right
                float ratioWidth = (float)Image.Size.Width / Width;
                float scale = (float)Width / Image.Size.Width;
                float displayHeight = scale * Image.Size.Height;
                float diffHeight = Height - displayHeight;
                diffHeight /= 2;

                newY *= scale;
                newY += diffHeight;
				newX /= ratioWidth;
				
			}
			else
			{
				//	This means that we are limited by height, meaning the image fills up the entire control from top to bottom
				float ratioHeight = (float)Image.Size.Height / Height;
                float scale = (float)Height / Image.Size.Height;
                float displayWidth = scale * Image.Size.Width;
                float diffWidth = Width - displayWidth;
                diffWidth /= 2;

                newX *= scale;
                newX += diffWidth;
				newY /= ratioHeight;
				
			}
			return new Point((int)newX, (int)newY);
		}
        
		/// <summary>
		/// Gets the mouse position over the image when the <see cref="PictureBox">PictureBox's</see> <see cref="PictureBox.SizeMode">SizeMode</see> is set to StretchImage
		/// </summary>
		/// <param name="coordinates">Point to translate</param>
		/// <returns>A point relative to the top left corner of the <see cref="PictureBox.Image">Image</see>
		/// If the Image is null, no translation is performed
		/// </returns>
		protected Point TranslateStretchImageMousePosition(Point coordinates)
		{
			//	test to make sure our image is not null
			if (Image == null) return coordinates;
			//	Make sure our control width and height are not 0
			if (Width == 0 || Height == 0) return coordinates;
			//	First, get the ratio (image to control) the height and width
			float ratioWidth = (float)Image.Size.Width/Width;
			float ratioHeight = (float)Image.Size.Height / Height;
			//	Scale the points by our ratio
			float newX = coordinates.X;
			float newY = coordinates.Y;
			newX *= ratioWidth;
			newY *= ratioHeight;
			return new Point((int)newX, (int)newY);
		}

		/// <summary>
		/// Gets the mouse position over the image when the <see cref="PictureBox">PictureBox's</see> <see cref="PictureBox.SizeMode">SizeMode</see> is set to Center
		/// </summary>
		/// <param name="coordinates">Point to translate</param>
		/// <returns>A point relative to the top left corner of the <see cref="PictureBox.Image">Image</see>
		/// If the Image is null, no translation is performed
		/// </returns>
		protected Point TranslateCenterImageMousePosition(Point coordinates)
		{
			//	Test to make sure our image is not null
			if(Image==null) return coordinates;
			//	First, get the top location (relative to the top left of the control) of the image itself
			// To do this, we know that the image is centered, so we get the difference in size (width and height) of the image to the control
			int diffWidth = Width - Image.Size.Width;
			int diffHeight = Height - Image.Size.Height;
			//	We now divide in half to accomadate each side of the image
			diffWidth /= 2;
			diffHeight /= 2;
			//	Finally, we subtract this numer from the original coordinates
			// In the case that the image is larger than the picture box, this still works
			coordinates.X -= diffWidth;
			coordinates.Y -= diffHeight;
			return coordinates;
		}

		/// <summary>
		/// Gets the mouse position over the image when the <see cref="PictureBox">PictureBox's</see> <see cref="PictureBox.SizeMode">SizeMode</see> is set to Normal
		/// </summary>
		/// <param name="coordinates">Point to translate</param>
		/// <returns>A point relative to the top left corner of the <see cref="PictureBox.Image">Image</see></returns>
		/// <remarks>
		/// In normal mode, the image is placed in the top left corner, and as such the point does not need to be translated.
		/// The resulting point is the same as the original point
		/// </remarks>
		protected Point TranslateNormalMousePosition(Point coordinates)
		{
			//	TODO: When we implement scrolling in this, we will need to test for scroll offset
			//	NOTE: As it stands now, this could be made static, but in the future we will be making this handle scaling
			return coordinates;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"></see> event.
		/// If the mouse is over the <see cref="PictureBox.Image">Image</see>, raises the <see cref="MouseMoveOverImage"></see> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (Image != null)
			{
				if (MouseMoveOverImage != null)
				{
					Point p = TranslatePointToImageCoordinates(e.Location);
					if (p.X >= 0 && p.X < Image.Size.Width && p.Y >= 0 && p.Y < Image.Size.Height)
					{
						MouseEventArgs ne = new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta);
						MouseMoveOverImage(this, ne);
					}
				}
			}
		}

		#endregion
		#endregion
	}
}
