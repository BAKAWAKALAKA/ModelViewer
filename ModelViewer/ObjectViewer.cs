using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
//using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ModelViewer
{
   public  class Mesh
    {
        int attr_vs;
        int mat4id;
        int program;
        int vShader;
        string vsSourse = @"attribute vec3 coord; uniform  mat4 modelView; void main() { gl_Position = vec4(coord, 1.0); };";
        uint EBO;
        uint VBO;
        uint CBO;
        uint VAO;

        int indexCount;

        bool loaded;

        public bool Load(float[] Vertexs, ushort[] index, float[] colorData)
        {
            loaded = true;

            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            GL.DeleteBuffer(CBO);


            // начало отрисовки
            GL.GenBuffers(1, out EBO); // генерация буфера и присваивание айдишника EBO
            GL.GenBuffers(1, out VBO); // генерация буфера и присваивание айдишника VBO
            GL.GenBuffers(1, out CBO); // генерация буфера и присваивание айдишника CBO

            // устанавливаем в буфер индексов EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                (IntPtr)(index.Length * sizeof(ushort)),
                index,
                BufferUsageHint.StaticDraw);// динамик т к часто отрисоваться будут
            // устанавливаем в буфер вершин VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(Vertexs.Length * sizeof(float)),
                Vertexs,
                BufferUsageHint.StaticDraw);
            // устанавливаем буфер цвета CBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, CBO);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(colorData.Length * sizeof(float)),
                colorData,
                BufferUsageHint.StaticDraw);

           // //шейдеры
           // vShader = GL.CreateShader(ShaderType.VertexShader);
           // GL.ShaderSource(vShader,vsSourse);
           // GL.CompileShader(vShader);
           //// Console.WriteLine("{0}",GL.GetShaderInfoLog(vShader));
           // program = GL.CreateProgram();
           // GL.AttachShader(program, vShader);
           // GL.LinkProgram(program);
           // mat4id = GL.GetUniformLocation(program, "modelView");
           // attr_vs = GL.GetAttribLocation(program, "coord");


            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                (IntPtr)(index.Length * sizeof(ushort)),
                index,
                BufferUsageHint.StaticDraw);// динамик т к часто отрисоваться будут


            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.VertexPointer(3, VertexPointerType.Float, 0, IntPtr.Zero);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(Vertexs.Length * sizeof(float)),
                Vertexs,
                BufferUsageHint.StaticDraw);


            GL.BindBuffer(BufferTarget.ArrayBuffer, CBO);
            GL.ColorPointer(3, ColorPointerType.Float, 0, IntPtr.Zero);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            indexCount = index.Length;
            return loaded;

        }

        public void Render(ref Matrix4 matrix4)
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            //GL.UseProgram(program);
            //// GL.UniformMatrix4(mat4id, false, ref matrix4);
            //GL.EnableVertexAttribArray(attr_vs);
            //GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            //GL.VertexAttribPointer(attr_vs, 3, VertexAttribPointerType.Float, false, 0, 0);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(VAO);

            GL.DrawElements(BeginMode.TriangleStrip,
               indexCount,
               DrawElementsType.UnsignedShort, IntPtr.Zero);

            GL.BindVertexArray(0);
            GL.UseProgram(0);


            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.ColorArray);


            GL.Color3(System.Drawing.Color.Red);//x
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(50, 0, 0);
            GL.End();

            GL.Color3(System.Drawing.Color.Blue);//y
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 50, 0);
            GL.End();

            GL.Color3(System.Drawing.Color.Yellow);//z
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 50);
            GL.End();
        }

    }
}
