using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace EmguTest
{
    public partial class MeshViewer : Form
    {
        int keyX = 0;
        bool glLoaded = false;

        public MeshViewer()
        {
            InitializeComponent();
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            glLoaded = true;
            GL.ClearColor(Color.SkyBlue);
            SetupViewport();
            //Application.Idle += new EventHandler(Application_Idle);
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (!glLoaded)
                return;

            Render();
        }

        private void SetupViewport()
        {
            int w = glControl.Width;
            int h = glControl.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1);
            GL.Viewport(0, 0, w, h);
        }

        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Translate(keyX, 0, 0);

            if (glControl.Focused)
                GL.Color3(Color.Yellow);
            else
                GL.Color3(Color.Blue);
            //GL.Rotate(rotation, OpenTK.Vector3.UnitZ);
            GL.Begin(BeginMode.Triangles);
            GL.Vertex2(10, 20);
            GL.Vertex2(100, 20);
            GL.Vertex2(100, 50);
            GL.End();
            glControl.SwapBuffers();
        }

        private void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (!glLoaded)
                return;

            if (e.KeyCode == Keys.Space)
            {
                keyX++;
                glControl.Invalidate();
            }
        }

        private void glControl_Enter(object sender, EventArgs e)
        {
            if (!glLoaded)
                return;
            glControl.Invalidate();
        }

        private void glControl_Leave(object sender, EventArgs e)
        {
            if (!glLoaded)
                return;
            glControl.Invalidate();
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            if (!glLoaded)
                return;
            SetupViewport();
            glControl.Invalidate();
        }
    }
}
