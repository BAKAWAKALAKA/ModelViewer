using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ModelViewer
{
    /// <summary>
    /// this class describes "camera" in openGL space
    /// </summary>
    class GLCameraClass //нужно добваить Transform
    {
        public Vector3 baseVector;
        public Vector3 camPos = new Vector3(45, 45, 45);
        public Vector4 camAngle  { get; private set; } // (0,0,1,angle) первые три цифры означают кака из них верх по которой происходит поворот камеры по правилу правой руки
        public Vector3 up;
        static   public Matrix4 modelView;
        static   public Matrix4 fliedView;

        /// <summary>
        ///  Constructor that create viewmatrix and filedmatrix set proiection mode and background color
        ///  Конструктор что создает viewmatrix и filedmatrix устанавливает режим проекции и устанавливает фон в серый
        /// </summary>
        /// <param name="position"> position of "camera"</param>
        /// <param name="upAxis"> where is up</param>
        /// <param name="target"> when you look </param>
        public GLCameraClass(Vector3 position, Vector3 upAxis, Vector3 target) // здесь надо задать матрицы
        {
            camPos = position;
            //camAngle = rotation;
            up = upAxis;
            baseVector = target;
            GL.ClearColor(Color4.Gray); //не совсем корректно
            GL.Enable(EnableCap.DepthTest);
            fliedView = Matrix4.CreatePerspectiveFieldOfView((float)(60 * Math.PI / 180), // угол ширины обзора?
                                                             1, // соотношение сторон
                                                             10, // растояние ближайшкй видимой точки
                                                             500);// растояния видимой дальней точки
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref fliedView);
            modelView =
                Matrix4.LookAt(camPos, // camera position
                               target,  //  куда смотрим
                               up);   // верх камеры
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelView);
        }


        public void ToCameraDefults() { } // возврат в начальное состояние

        /// <summary>
        ///   produces a rotation of angle degrees around the vector axis . 
        ///   вращение на angle градусов против часовой стрелки в направлении вектора axis
        /// </summary>
        /// <param name="angle"> angle in grad </param>
        /// <param name="axis"> orientation of rotation</param>
        public void RotateCamera(float angle,Vector3 axis)
        {
            camPos =camPos + new Vector3(axis.X * angle, axis.Y * angle, axis.Z * angle);

            GL.Rotate(angle,axis);

        }

        /// <summary>
        /// Scale camera(actuale world)
        /// увеличивает или уменьшает растояние до объекта не изменяя направление 
        /// </summary>
        /// <param name="direction"> how mach scale</param>
        public void ScaleCamera(float direction) 
        {
            GL.Scale((double)direction, (double)direction, (double)direction);
        }
    }

}
