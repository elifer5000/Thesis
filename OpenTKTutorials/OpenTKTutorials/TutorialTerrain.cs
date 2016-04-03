

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Platform;

namespace OpenTKTutorials
{
    [StructLayout(LayoutKind.Sequential)]
    struct Vertex
    { // mimic InterleavedArrayFormat.T2fN3fV3f
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Position;
    }

    /// <summary>
    /// Terrain Example created by flopoloco
    /// Credits: [] Nehe for the knowledge
    /// 		 [] OpenTK Team for the library
    /// 
    /// How this application works:
    /// 1. Load a grayscale heightmap image
    /// 2. Store infomation of image useful for rendering the 3D terrain (a 2D height array, and dimensions of image)
    /// 3. Dynamically render the terrain / Interact with application
    /// </summary>
    public class TutorialTerrain : GameWindow
    {
        int TexID; // Texture ID
        // Store width and height of terrain
        Size size;
        // Array used for height information (Pixel color of the image is the height value)
        byte[,] heightTable;

        // Current rendering state
        BeginMode terrainRenderStyle = BeginMode.Quads;

        // Terrain rendering detail (the less the better/slower)
        int renderSteps = 30;
       
        // For display list
        const int num_lists = 3;
        int[] lists = new int[num_lists];

        // For vertex buffer object
        struct Vbo { public int VboID, EboID, NumElements; }
        Vbo[] vbo = new Vbo[2];
        Vertex[] vertices;
        int[] indices;

        public int RenderSteps
        {
            get { return renderSteps; }
            set
            {
                if (value < 1) renderSteps = 1;
                else if (value > 5000) renderSteps = 5000;
                else renderSteps = value;
            }
        }

        // Camera
        float cameraPositionX = 0f, cameraPositionY = 300.0f, cameraPositionZ = 500f;

        // Help box strings
        string tutTxt = "Page Up/Down\t: Camera height\n" +
                        "Arrow keys\t: Camera position\n" +
                        "1\t: Points rendering\n" +
                        "2\t: Lines rendering\n" +
                        "3\t: Quads rendering\n" +
                        "+\t: Increase terrain quality\n" +
                        "-\t: Decrease terrain quality\n" +
                        "ESC\t: Quit applicaton\n";

        // Update statistics information every 1 second
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        // Count the number of opengl vertex calls for a render loop
        int diagVertices = 0;




        public TutorialTerrain()
            : base(640, 480)
        {
            // Gettin input with "KeyUp" events is useful when you want to input single key presses
            // (not constantly repeated)
            this.Keyboard.KeyUp += new EventHandler<KeyboardKeyEventArgs>(TutorialTerrain_KeyUp);
            this.Mouse.ButtonDown += new EventHandler<MouseButtonEventArgs>(TutorialTerrain_ButtonDown);
            stopwatch.Start();
        }




        void TutorialTerrain_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                // Exit the application
                case Key.Escape:
                    this.Exit();
                    break;
                // Switch rendering to Points
                case Key.Number1:
                    this.terrainRenderStyle = BeginMode.Points;
                    break;
                // Switch rendering to Lines
                case Key.Number2:
                    this.terrainRenderStyle = BeginMode.Lines;
                    break;
                // Switch rendering to Quads
                case Key.Number3:
                    this.terrainRenderStyle = BeginMode.Quads;
                    break;
                // Increase terrain quality
                case Key.Plus:
                    RenderSteps -= 1; // Terrain quality gets better by descreasing the steps
                    GL.DeleteLists(lists[0], num_lists);
                    CreateDisplayLists();
                    break;
                // Decrease terrain quality
                case Key.Minus:
                    RenderSteps += 1; // Terrain quality gets lower by increasing the steps
                    GL.DeleteLists(lists[0], num_lists);
                    CreateDisplayLists();
                    break;
            }
        }




        void TutorialTerrain_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                System.Windows.Forms.MessageBox.Show(tutTxt, "Help", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }




        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Camera position
            if (Keyboard[Key.PageUp])
                cameraPositionY += 100.0f * (float)e.Time;

            if (Keyboard[Key.PageDown])
                cameraPositionY -= 100.0f * (float)e.Time;

            if (Keyboard[OpenTK.Input.Key.Left])
                cameraPositionX -= 100.0f * (float)e.Time;

            if (Keyboard[OpenTK.Input.Key.Right])
                cameraPositionX += 100.0f * (float)e.Time;

            if (Keyboard[OpenTK.Input.Key.Up])
                cameraPositionZ -= 100.0f * (float)e.Time;

            if (Keyboard[OpenTK.Input.Key.Down])
                cameraPositionZ += 100.0f * (float)e.Time;
        }




        protected override void OnLoad(EventArgs e)
        {
            // Set up opengl scene
            GL.ClearColor(Color.SteelBlue);	// Set color
            GL.PointSize(3f);				// Set size of points
            GL.Enable(EnableCap.DepthTest);	// Enable depth test capabilities
            GL.Enable(EnableCap.Texture2D);

            // Load heightmap
            LoadHeightMap(@"..\..\heightmap.png");
            TexID = LoadTexture(@"..\..\Tel Aviv 050.JPG");

            // Fill vertices array
            //vbo[0] = FillVertexAndElementsArray();


            // Create the display lists
            CreateDisplayLists();
        }

        private void CreateDisplayLists()
        {
            int first_list = GL.GenLists(num_lists);
            for (int i = 0; i < num_lists; i++)
            {
                lists[i] = first_list + i;
                GL.NewList(lists[i], ListMode.Compile);
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
                    default:
                        break;
                }
                RenderHeightmap();
                GL.EndList();
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.DeleteLists(lists[0], num_lists);
            GL.DeleteTextures(1, ref TexID);
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1f, 5000f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }




        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 lookat = Matrix4.LookAt(cameraPositionX, cameraPositionY, cameraPositionZ, 0f, 0f, 0f, 0f, 1f, 0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            // Render heightmap normally
            //GL.PushMatrix();
            //this.RenderHeightmap();
            //GL.PopMatrix();

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
                    GL.CallList(lists[2]);
                    break;
                default:
                    break;
            }

            // Render with vbo
            // Center terrain in the middle of the scene
            //GL.Translate(-size.Width / 2, 0.0, -size.Height / 2);
            //Draw(vbo[0]);


            // Update FPS and diagnostics on window title every 1 second
            if (stopwatch.ElapsedMilliseconds > 1000.0)
            {
                this.Title = "FPS: " + (1.0 / e.Time).ToString() + " | GLVertex3 calls: " + diagVertices.ToString() + " | Right-click for help";
                stopwatch.Reset();
                stopwatch.Start();
            }

            this.SwapBuffers();
        }




        /// <summary>
        /// Generates a heightmap out from an image file
        /// </summary>
        /// <param name="filepath">Path to image</param>
        private void LoadHeightMap(string filepath)
        {
            if (File.Exists(filepath))										// File exists
            {
                Bitmap bitmap = new Bitmap(filepath);						// Treat file as an image
                size = new Size(bitmap.Width, bitmap.Height);				// Store image dimensions
                heightTable = new byte[size.Width, size.Height];			// Initiate heightTable arrays

                // Loop through all pixels of the image and store their colors in the heightTable array
                for (int bx = 0; bx < size.Width; bx++)
                {
                    for (int by = 0; by < size.Height; by++)
                    {
                        // Since image is grayscale, we can use any color channel
                        heightTable[bx, by] = bitmap.GetPixel(bx, by).R;
                    }
                }
            }
        }

        /// <summary>
        /// Simple heightmap renderer
        /// </summary>
        private void RenderHeightmap()
        {
            // Reset the vertice diagnostic variable
            diagVertices = 0;

            // Center terrain in the middle of the scene
            GL.Translate(-size.Width / 2, 0.0, -size.Height / 2);

            // Set the selected BeginMode
            GL.Begin(terrainRenderStyle);

            for (int bx = 0; bx < (size.Width - RenderSteps); bx += RenderSteps)
            {
                for (int bz = 0; bz < (size.Height - RenderSteps); bz += RenderSteps)
                {
                    // Paint the current primitive based on the heightmap's color value
                    GL.Color3(heightTable[bx, bz], heightTable[bx, bz], heightTable[bx, bz]);

                    // If user wants to display points
                    if (terrainRenderStyle == BeginMode.Points)
                    {
                        GL.Vertex3(bx, heightTable[bx, bz], bz);

                        diagVertices++;
                    }

                    // If user wants to display lines
                    else if (terrainRenderStyle == BeginMode.Lines)
                    {
                        // 0, 0 -> 1, 0
                        GL.Vertex3(bx, heightTable[bx, bz], bz);
                        GL.Vertex3(bx + RenderSteps, heightTable[bx + RenderSteps, bz], bz);
                        // 1, 0 -> 1, 1 
                        GL.Vertex3(bx + RenderSteps, heightTable[bx + RenderSteps, bz], bz);
                        GL.Vertex3(bx + RenderSteps, heightTable[bx + RenderSteps, bz + RenderSteps], bz + RenderSteps);
                        // 1, 1 -> 0, 1
                        GL.Vertex3(bx + RenderSteps, heightTable[bx + RenderSteps, bz + RenderSteps], bz + RenderSteps);
                        GL.Vertex3(bx, heightTable[bx, bz + RenderSteps], bz + RenderSteps);
                        // 0, 1 -> 0, 0
                        GL.Vertex3(bx, heightTable[bx, bz], bz);
                        GL.Vertex3(bx, heightTable[bx, bz + RenderSteps], bz + RenderSteps);

                        diagVertices += 8;
                    }

                    // If user wants to display quads
                    else if (terrainRenderStyle == BeginMode.Quads)
                    {
                        // 0,0
                        float s = (float)(bx) / (float)size.Width;
                        float t = (float)(bz) / (float)size.Height;
                        GL.TexCoord2(s, t);
                        GL.Vertex3(bx, heightTable[bx, bz], bz);

                        // 1,0
                        s = (float)(bx + RenderSteps) / (float)size.Width;
                        t = (float)(bz) / (float)size.Height;
                        GL.TexCoord2(s, t);
                        GL.Vertex3(bx + RenderSteps, heightTable[bx + RenderSteps, bz], bz);

                        // 1,1
                        s = (float)(bx + RenderSteps) / (float)size.Width;
                        t = (float)(bz + RenderSteps) / (float)size.Height;
                        GL.TexCoord2(s, t);
                        GL.Vertex3(bx + RenderSteps, heightTable[bx + RenderSteps, bz + RenderSteps], bz + RenderSteps);

                        // 0,1
                        s = (float)(bx) / (float)size.Width;
                        t = (float)(bz + RenderSteps) / (float)size.Height;
                        GL.TexCoord2(s, t);
                        GL.Vertex3(bx, heightTable[bx, bz + RenderSteps], bz + RenderSteps);

                        diagVertices += 4;
                    }
                }
            }

            GL.End();
        }

        static int LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return id;
        }

        private Vbo FillVertexAndElementsArray()
        {
            Vbo handle = new Vbo();
            int NumVertPerRow = (int)(size.Width / RenderSteps);
            int NumVertPerCol = (int)(size.Height / RenderSteps);
            int NumVertices = (int)(NumVertPerRow * NumVertPerCol);
            vertices = new Vertex[NumVertices];
            // 3 vertices per triangle, 2 triangles per vertex created (except last row and col)
            int NumTriangles = (NumVertPerRow - 1) * (NumVertPerCol - 1) * 2 * 3;
            handle.NumElements = NumTriangles;
            indices = new int[NumTriangles];
            int i = 0;
            int j = 0;
            for (int bx = 0; bx < (size.Width - RenderSteps); bx += RenderSteps)
            {
                for (int bz = 0; bz < (size.Height - RenderSteps); bz += RenderSteps, i++)
                {
                    // Position
                    vertices[i].Position = new Vector3(bx, heightTable[bx, bz], bz);
                    // Texture coordinates (fraction of picture, whole thing will be [0..1]
                    float s = (float)(bx) / (float)size.Width;
                    float t = (float)(bz) / (float)size.Height;
                    vertices[i].TexCoord = new Vector2(s, t);
                    // Normal (average of 4 faces' normals that share this vertex, 2 for sides and itself for corners)
                    // TODO
                    //Vector3.Cross(
                    //vertices[i].Normal = new Vector3(bx, heightTable[bx, bz], bz);

                    // Create 2 triangles for each vertex
                    // Skip if it's last in col, or is the last col
                    if ((i + 1) % NumVertPerCol == 0 || i >= (NumVertices - NumVertPerCol))
                        continue;
                    indices[j++] = i;
                    indices[j++] = i + NumVertPerRow;
                    indices[j++] = i + NumVertPerRow + 1;

                    indices[j++] = i;
                    indices[j++] = i + NumVertPerRow + 1;
                    indices[j++] = i;
                }
            }

            int _size;
            GL.GenBuffers(1, out handle.VboID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle.VboID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * BlittableValueType.StrideOf(vertices)), vertices,
                          BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out _size);
            if (vertices.Length * BlittableValueType.StrideOf(vertices) != _size)
                throw new ApplicationException("Vertex data not uploaded correctly");

            GL.GenBuffers(1, out handle.EboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle.EboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices,
                          BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out _size);
            if (indices.Length * sizeof(int) != _size)
                throw new ApplicationException("Element data not uploaded correctly");

            return handle;
        }

        void Draw(Vbo handle)
        {
            // To draw a VBO:
            // 1) Ensure that the VertexArray client state is enabled.
            // 2) Bind the vertex and element buffer handles.
            // 3) Set up the data pointers (vertex, normal, color) according to your vertex format.
            // 4) Call DrawElements. (Note: the last parameter is an offset into the element buffer
            //    and will usually be IntPtr.Zero).

            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            //GL.EnableClientState(ArrayCap.NormalArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, handle.VboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle.EboID);

            GL.TexCoordPointer(2, TexCoordPointerType.Float, 8 * sizeof(float), (IntPtr)(0));
            //not using yet: //GL.NormalPointer(NormalPointerType.Float, 8 * sizeof(float), (IntPtr)(2 * sizeof(float)));
            GL.VertexPointer(3, VertexPointerType.Float, 8 * sizeof(float), (IntPtr)(5 * sizeof(float)));

            GL.DrawElements(BeginMode.Triangles, handle.NumElements, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }    
    }
}