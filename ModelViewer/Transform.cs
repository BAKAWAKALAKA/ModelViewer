using System;
using System.Collections.Generic;
using OpenTK;

namespace ModelViewer
{
    public enum Orientation{ baseY, baseX };
    /// <summary>
    /// 
    /// </summary>
    public class Transform : Component
    {
        /// <summary>
        /// 
        /// </summary>
        public Matrix4 rotMat;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 baseX { private set; get; }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 baseY { private set; get; }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 baseZ { private set; get; }


        private Model objectModel;

        /// <summary>
        /// иницилизация объекта задает вектора базы позиции маштаба и указывает ссылку на ассоциированный объект
        /// </summary>
        /// <param name="obj"></param>
        public Transform(Model obj)
        {
            baseX = new Vector3(1,0,0);
            baseY = new Vector3(0,1,0);
            baseZ = new Vector3(0,0,1);
            rotMat = Matrix4.Identity;

            objectModel = obj;
        }


        /// <summary>
        /// Поворачивает объект на нужный угл в нужном направлении
        /// </summary>
        /// <param name="orientation"> baseX, baseY</param>
        /// <param name="angle"> угол</param>
        public void Rotate(Orientation orientation, float angle)
        {// нужно переделать параметр на что то типо right left
            Vector3 vector_ = new Vector3();
            if (orientation == Orientation.baseX) vector_ = baseX;
            else vector_ = baseY;

            rotMat = (Matrix4.CreateFromAxisAngle(vector_, angle));       
            baseZ = TransformVector( baseZ);
            if (vector_ == baseY)
            {
                baseX = TransformVector(baseX);
            }
            else
            {
                baseY = TransformVector(baseY);
            }
        }


        private Vector3 TransformVector(Vector3 vec)
        {// поворачивает вектор в соответствии с матрицей вразения
            // позиция важна
            Vector4 original = new Vector4(vec, 1);
            //вообще нужно rotMat*tratMat*scaleMat
            return new Vector3(Vector4.Transform(original, rotMat));
        }

    }
}
