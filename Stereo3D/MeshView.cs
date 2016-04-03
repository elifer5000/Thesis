using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Stereo3D
{
    public partial class MeshView : Form
    {
        bool glLoaded = false;
        int TexID; // Texture ID
        bool showTexture = true;
        bool PlusFrame = false;
        // Store width and height of terrain
        Size size;
        int[,] heightTable;
        Bitmap textureBmp;
        // Current rendering state
        BeginMode terrainRenderStyle = BeginMode.Quads;

        int RenderSteps = 1;   // Grid size
        // For display list
        const int num_lists = 4;
        int[] lists = new int[num_lists];
        int MinY = 0;
        int MaxY = 0;

        // Camera
        Vector3 cameraPos = new Vector3(0f, 150f, 150f);
        Quaternion cameraRot = Quaternion.Identity;
        Vector3 MouseVecStart;
        Vector3 MouseVecCur;
        bool IsRotating = false;

        public MeshView()
        {
            InitializeComponent();
        }

        public void SetupMeshView(int[,] _heightTable, Size _size, Bitmap bmp, int steps)
        {
            heightTable = _heightTable;
            size = _size;
            size.Height *= steps;
            size.Width *= steps;
            textureBmp = bmp;
            RenderSteps = steps;

            IEnumerable<int> AllVals = _heightTable.Cast<int>();
            MinY = AllVals.Min();
            MaxY = AllVals.Max();
            //MaxY = -9999999;
            //for (int i = 0; i < _size.Width; i++)
            //    for (int j = 0; j < _size.Height; j++)
            //        if (heightTable[i, j] > MaxY)
            //            MaxY = heightTable[i, j];

            cameraPos.Y = 1.33f * MaxY;
            cameraPos.Z = 1.33f * size.Height + 5;
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            glLoaded = true;
            // Set up opengl scene
            GL.ClearColor(Color.SteelBlue);	// Set color
            GL.PointSize(3f);				// Set size of points
            GL.Enable(EnableCap.DepthTest);	// Enable depth test capabilities
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            //GL.Enable(EnableCap.Texture2D);
            
            SetupViewport();
            
            LoadTexture();

            SetupViewport();
            // Create the display lists
            CreateDisplayLists();
            terrainRenderStyle = BeginMode.Quads; // Starting mode

            
        }

        private void CreateDisplayLists()
        {
            int first_list = GL.GenLists(num_lists);
            for (int i = 0; i < num_lists; i++)
            {
                lists[i] = first_list + i;
                GL.NewList(lists[i], ListMode.Compile);
                bool AllWhite = false;
                switch (i)
                {
                    case 0:
                        terrainRenderStyle = BeginMode.Points;
                        break;
                    case 1:
                        terrainRenderStyle = BeginMode.Lines;
                        break;
                    case 2:
                        terrainRenderStyle = BeginMode.Quads;
                        break;
                    case 3:
                        terrainRenderStyle = BeginMode.Lines;
                        AllWhite = true;
                        break;
                    default:
                        break;
                }
                RenderHeightmap(AllWhite);
                GL.EndList();
            }
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (!glLoaded)
                return;
            
            glControl.MakeCurrent();
            Render();
        }

        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 lookat = Matrix4.LookAt(cameraPos.X, cameraPos.Y, cameraPos.Z, 0f, 0f, 0f, 0f, 1f, 0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
            Vector3 rotAxis;
            float rotAngle;
            cameraRot.ToAxisAngle(out rotAxis, out rotAngle);
            GL.Rotate(MathHelper.RadiansToDegrees(rotAngle), rotAxis);
            // Render with display lists
            switch (terrainRenderStyle)
            {
                case BeginMode.Points:
                    GL.CallList(lists[0]);
                    break;
                case BeginMode.Lines:
                    GL.CallList(lists[1]);
                    break;
                case BeginMode.Quads:
                    if (showTexture)
                        GL.Enable(EnableCap.Texture2D);
                    GL.CallList(lists[2]);
                    GL.Disable(EnableCap.Texture2D);
                    break;
                default:
                    break;
            }
            if (PlusFrame && terrainRenderStyle != BeginMode.Lines)
            {
                //GL.Color3(Color.White);
                GL.CallList(lists[3]);
            }

            // Test mouse coordinates
            //GL.Begin(BeginMode.Points);
            //GL.PointSize(5);
            //GL.Color3(Color.Red);
            //GL.Vertex3(MouseVec.X, MouseVec.Y, MouseVec.Z);
            //GL.Vertex3(MouseVec.X + 10, MouseVec.Y + 10, MouseVec.Z);
            //GL.End();

            glControl.SwapBuffers();
        }

        private void RenderHeightmap(bool forWireFrame)
        {
            GL.PushMatrix();
            // Center terrain in the middle of the scene
            GL.Translate(-size.Width / 2, 0.0, -size.Height / 2);

            // Set the selected BeginMode
            GL.Begin(terrainRenderStyle);
            int i = 0, j = 0;
            float deltaY = MaxY - MinY;
            float offset = forWireFrame ? 0.3f : 0.0f;    // to avoid z-fighting
            for (int bx = 0; bx < (size.Width - RenderSteps); bx += RenderSteps, i++, j = 0)
            {
                for (int bz = 0; bz < (size.Height - RenderSteps); bz += RenderSteps, j++)
                {
                    // Paint the current primitive based on the heightmap's color value
                    float color = (heightTable[i, j] - MinY) / deltaY;
                    if (forWireFrame)
                        GL.Color3(Color.White);
                    else
                        GL.Color3(color, color, color);

                    // If user wants to display points
                    if (terrainRenderStyle == BeginMode.Points)
                    {
                        GL.Vertex3(bx, heightTable[i, j], bz);

                        //diagVertices++;
                    }

                    // If user wants to display lines
                    else if (terrainRenderStyle == BeginMode.Lines)
                    {
                        // 0, 0 -> 1, 0
                        GL.Vertex3(bx, heightTable[i, j] + offset, bz);
                        GL.Vertex3(bx + RenderSteps, heightTable[i + 1, j] + offset, bz);
                        // 1, 0 -> 1, 1 
                        GL.Vertex3(bx + RenderSteps, heightTable[i + 1, j] + offset, bz);
                        GL.Vertex3(bx + RenderSteps, heightTable[i + 1, j + 1] + offset, bz + RenderSteps);
                        // 1, 1 -> 0, 1
                        GL.Vertex3(bx + RenderSteps, heightTable[i + 1, j + 1] + offset, bz + RenderSteps);
                        GL.Vertex3(bx, heightTable[i, j + 1] + offset, bz + RenderSteps);
                        // 0, 1 -> 0, 0
                        GL.Vertex3(bx, heightTable[i, j] + offset, bz);
                        GL.Vertex3(bx, heightTable[i, j + 1] + offset, bz + RenderSteps);

                        //diagVertices += 8;
                    }

                    // If user wants to display quads
                    else if (terrainRenderStyle == BeginMode.Quads)
                    {
                        // 0,0
                        float s = (float)(bx) / (float)size.Width;
                        float t = (float)(bz) / (float)size.Height;
                        GL.TexCoord2(s, t);
                        GL.Vertex3(bx, heightTable[i, j], bz);

                        // 1,0
                        s = (float)(bx + RenderSteps) / (float)size.Width;
                        t = (float)(bz) / (float)size.Height;
                        GL.TexCoord2(s, t);
                        GL.Vertex3(bx + RenderSteps, heightTable[i + 1, j], bz);

                        // 1,1
                        s = (float)(bx + RenderSteps) / (float)size.Width;
                        t = (float)(bz + RenderSteps) / (float)size.Height;
                        GL.TexCoord2(s, t);
                        GL.Vertex3(bx + RenderSteps, heightTable[i + 1, j + 1], bz + RenderSteps);

                        // 0,1
                        s = (float)(bx) / (float)size.Width;
                        t = (float)(bz + RenderSteps) / (float)size.Height;
                        GL.TexCoord2(s, t);
                        GL.Vertex3(bx, heightTable[i, j + 1], bz + RenderSteps);

                        //diagVertices += 4;
                    }
                }
            }

            GL.End();
            GL.PopMatrix();
        }

        void LoadTexture()
        {
            TexID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TexID);

            BitmapData bmp_data = textureBmp.LockBits(new Rectangle(0, 0, textureBmp.Width, textureBmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            textureBmp.UnlockBits(bmp_data);

            // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            if (!glLoaded)
                return;
            glControl.MakeCurrent();
            SetupViewport();
        }

        private void SetupViewport()
        {
            if (glControl.ClientSize.Height == 0)
                glControl.ClientSize = new System.Drawing.Size(glControl.ClientSize.Width, 1);

            GL.Viewport(0, 0, glControl.ClientSize.Width, glControl.ClientSize.Height);
            //System.Diagnostics.Debug.Print(glControl.Width.ToString() + " " + glControl.Height.ToString());
            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1f, 5000f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }

        private void MeshView_FormClosed(object sender, FormClosedEventArgs e)
        {
            GL.DeleteLists(lists[0], num_lists);
            GL.DeleteTextures(1, ref TexID);
        }

        private void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // Exit the application
                case Keys.Escape:
                    this.Close();
                    break;
                // Switch rendering to Points
                case Keys.D1:
                    terrainRenderStyle = BeginMode.Points;
                    break;
                // Switch rendering to Lines
                case Keys.D2:
                    terrainRenderStyle = BeginMode.Lines;
                    break;
                // Switch rendering to Quads
                case Keys.D3:
                    terrainRenderStyle = BeginMode.Quads;
                    break;
                // Switch rendering to Wireframe superimposed
                case Keys.D4:
                    PlusFrame = !PlusFrame;
                    break;
                case Keys.T:
                    showTexture = !showTexture;
                    break;
                //// Increase terrain quality
                //case Key.Plus:
                //    RenderSteps -= 1; // Terrain quality gets better by descreasing the steps
                //    GL.DeleteLists(lists[0], num_lists);
                //    CreateDisplayLists();
                //    break;
                //// Decrease terrain quality
                //case Key.Minus:
                //    RenderSteps += 1; // Terrain quality gets lower by increasing the steps
                //    GL.DeleteLists(lists[0], num_lists);
                //    CreateDisplayLists();
                //    break;
            }
            glControl.Invalidate();
        }

        private void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsRotating = true;
                MouseVecStart = convertScreenToWorldCoords(e.X, e.Y);
                MouseVecStart.Normalize();
            }
        }

        private void glControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (IsRotating)
            {
                MouseVecCur = convertScreenToWorldCoords(e.X, e.Y);
                MouseVecCur.Normalize();

                Vector3 VecAxis = Vector3.Cross(MouseVecStart, MouseVecCur);
                VecAxis.Normalize();

                float angle = (float)Math.Acos(Math.Max(-1.0f, Math.Min(1.0f, Vector3.Dot(MouseVecStart, MouseVecCur))));
                if (Math.Abs(angle) > 0)
                {
                    Quaternion Quat = new Quaternion(VecAxis, 2.0f * angle);
                    Quat.Normalize();
                    cameraRot = Quaternion.Multiply(cameraRot, Quat);
                    cameraRot.Normalize();
                    MouseVecStart = MouseVecCur;
                    //System.Diagnostics.Debug.Print(string.Format("{0}, {1}, {2}, {3}\n", MouseVec.X, MouseVec.Y, MouseVec.Z, MouseVec.W));
                    glControl.Invalidate();
                }
            }
        }

        private void glControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsRotating = false;
            }
        }

        public static Vector3 convertScreenToWorldCoords(int x, int y)
        {
            int[] viewport = new int[4];
            Matrix4 modelViewMatrix, projectionMatrix;
            GL.GetFloat(GetPName.ModelviewMatrix, out modelViewMatrix);
            GL.GetFloat(GetPName.ProjectionMatrix, out projectionMatrix);
            GL.GetInteger(GetPName.Viewport, viewport);
            Vector2 mouse;
            mouse.X = x;
            mouse.Y = y;
            Vector4 vector = UnProject(ref projectionMatrix, modelViewMatrix, new Size(viewport[2], viewport[3]), mouse);
            //Point coords = new Point((int)vector.X, (int)vector.Y);
            return new Vector3(vector);
        }

        // Already return in homogenous coords
        public static Vector4 UnProject(ref Matrix4 projection, Matrix4 view, Size viewport, Vector2 mouse)
        {
            Vector4 vec;

            vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
            vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
            vec.Z = 0;
            vec.W = 1.0f;

            Matrix4 viewInv = Matrix4.Invert(view);
            Matrix4 projInv = Matrix4.Invert(projection);

            Vector4.Transform(ref vec, ref projInv, out vec);
            Vector4.Transform(ref vec, ref viewInv, out vec);


            if (vec.W > float.Epsilon || vec.W < float.Epsilon)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }


            return vec;
        }

        public static Vector4 Project(OpenTK.Vector4 objPos, ref Matrix4 projection, Matrix4 view, Size viewport)
        {
            Vector4 vec = objPos;

            vec = Vector4.Transform(vec, Matrix4.Mult(projection, view));

            vec.X = (vec.X + 1) * (viewport.Width / 2);
            vec.Y = (vec.Y + 1) * (viewport.Height / 2);

            return vec;
        }
    }
}
