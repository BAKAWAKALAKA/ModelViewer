using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Windows;

using Microsoft.Win32;
using OpenTK.Input;

namespace ModelViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Vector2 oldPoint;
        bool flag;
        static GLCameraClass camera;
        public Model model;
        public string fileName;
        bool gridflag; 


        bool loaded;

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            loaded = true;
            camera = new GLCameraClass(new Vector3(56, 56, 56), new Vector3(0, 1, 0), Vector3.Zero);
        }

        private void glControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (!loaded) return;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if(model!=null) model.Render();

            BaseGrid(90, 5);

            glControl.SwapBuffers();
            glControl.Invalidate();
        }

        private void BaseGrid(int max, int offset)
        {
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

            int i = 0;
            while (i < max)
            {
                GL.Color3(System.Drawing.Color.White);
                GL.Begin(BeginMode.LineLoop);
                GL.Vertex3(i, 0, 0);
                GL.Vertex3(i, 0, max);
                GL.End();

                GL.Begin(BeginMode.LineLoop);
                GL.Vertex3(i, 0, 0);
                GL.Vertex3(i, max, 0);
                GL.End();


                GL.Begin(BeginMode.LineLoop);
                GL.Vertex3(0, i, 0);
                GL.Vertex3(0, i, max);
                GL.End();


                GL.Begin(BeginMode.LineLoop);
                GL.Vertex3(0, i, 0);
                GL.Vertex3(max, i, 0);
                GL.End();


                GL.Begin(BeginMode.LineLoop);
                GL.Vertex3(0, 0, i);
                GL.Vertex3(max, 0, i);
                GL.End();


                GL.Begin(BeginMode.LineLoop);
                GL.Vertex3(0, 0, i);
                GL.Vertex3(0, max, i);
                GL.End();

                GL.Color3(System.Drawing.Color.Red); // здесь должна будет быть функция отрисовующая числа
                GL.Begin(BeginMode.Points);
                GL.Vertex3(i * offset, 0, 0);
                GL.Vertex3(0, i * offset, 0);
                GL.Vertex3(0, 0, i * offset);
                GL.End();

                i += offset;
            }
        }


// кнопки управления меню

        private void On_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == true)
            {
                fileName = file.FileName;
            }
            if (fileName != null && fileName != "")
            {
                SeiReader sei = new SeiReader(fileName);
                model = sei.ReadAll();
            }
            DataContext = model;
        }

// кнопки управления меню////




// кнопки управления камерой
        private void Button_Click_TopView(object sender, RoutedEventArgs e)
        {
            camera = new GLCameraClass(new Vector3(0, 90, 1), new Vector3(0, 1, 0), Vector3.Zero); // неожидо верх
        }

        private void Button_Click_HomeView(object sender, RoutedEventArgs e)
        {
            camera = new GLCameraClass(new Vector3(56, 56, 56), new Vector3(0, 1, 0), Vector3.Zero);
        }

        private void Button_Click_LeftView(object sender, RoutedEventArgs e)
        {
            camera = new GLCameraClass(new Vector3(0, 1, 90), new Vector3(0, 1, 0), Vector3.Zero); // неожидо лево
        }

        private void glMouse_Wheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                camera.ScaleCamera(2);

            }
            if (e.Delta < 0)
            {
                camera.ScaleCamera(0.5f);
            }
        }

        //наверно стоит добавить функцию отключения сетки
// кнопки управления камерой////


  
// кнопки управления состоянием модели
        private void On_NextState_Click(object sender, RoutedEventArgs e)
        {
            model.NextState();
        }

        private void On_PreviusState_Click(object sender, RoutedEventArgs e)
        {
            model.PreviesState();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (model != null) model.SetQulity((int)e.NewValue);
        }

// кнопки управления состоянием модели///




// кнопки управления вращением моедли

        private void glControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            flag = false;
            if (oldPoint.X != e.X && oldPoint.Y != e.Y)
            {
                Vector2 vec = (oldPoint - new Vector2(e.X, e.Y)).Normalized();
            }
        }

        private void glMouse_Move(object sender, System.Windows.Forms.MouseEventArgs e)
        {// todo пока не корректно нужно исправить повороты
            if (flag && model!=null )
                if (oldPoint.X != e.X && oldPoint.Y != e.Y) 
            {
                Vector2 vec = (oldPoint - new Vector2(e.X, e.Y)).Normalized();
                if (Math.Abs(vec.X) < Math.Abs(vec.Y)) model.Rotate(Orientation.baseX, vec.Y);
                else model.Rotate(Orientation.baseY, vec.X);

                oldPoint = new Vector2(e.X, e.Y);
            }
        }

        private void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button.ToString() == MouseButton.Left.ToString())
            {
                oldPoint = new Vector2(e.X, e.Y);
                flag = true;

            }
        }

// кнопки управления вращением моедли////////////
    }
}
