using System;
using System.Collections.Generic;
using OpenTK;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelViewer
{
    /// <summary>
    /// base Class for SeiReader
    /// </summary>
    class Sei
    {

    }

    /// <summary>
    /// class for accses to sei file and read model from it
    /// </summary>
    class SeiReader:Sei
    {
        string fileName;
        List<Vector3> points;
        List<int> relationship;

        /// <summary>
        /// jast fill fileName
        /// </summary>
        /// <param name="name"></param>
        public SeiReader(string name)
        {
            if (name != null && name != "")
                fileName = name;
        }

        // todo может лучше сделать стратегию?
        /// <summary>
        /// Read file of fileName and parse it.
        /// </summary>
        /// <returns> model in file </returns>
        public Model ReadAll()
        {
            int pointsCount, structCount, stateCount;
            points = new List<Vector3>();
            relationship = new List<int>();

            Vector3 vec;

            StreamReader str = new StreamReader(fileName);

            // читаем первую строку: количество точек, подструктур, состояний
            string s = str.ReadLine();
            List<string> line = new List<string>((s).Split(' '));

            pointsCount = int.Parse(line[0], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            structCount = int.Parse(line[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            stateCount = int.Parse(line[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
           
            // читаем  точки исходя из указанного количества точек
            for (int i = 0; i < pointsCount; i++)
            {
                s = str.ReadLine();

                if (s != " " && s != "" && s != null)
                {
                    line = new List<string>((s).Split(' '));
                    line.RemoveAll(x => x == "");

                    if (line.Count > 3)
                    {
                        vec = new Vector3(float.Parse(line[1],
                                                    System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Float,
                                                     System.Globalization.CultureInfo.InvariantCulture
                                                           ),
                                           float.Parse(line[2],
                                                    System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Float,
                                                     System.Globalization.CultureInfo.InvariantCulture
                                                           ),
                                           float.Parse(line[3],
                                                    System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Float,
                                                     System.Globalization.CultureInfo.InvariantCulture
                                                           )
                                          );
                
                        points.Add(vec);
                    }
                }
            }
    
            // вычисляем центральный вектор для смещения и смещаем координаты
            Vector3 centerVec = new Vector3();
            foreach (Vector3 v in points) { centerVec += v; }
            centerVec = Vector3.Divide(centerVec, points.Count);
            for (int i = 0; i < points.Count; i++)
            {
               points[i] -= (centerVec);
              
            }

            // читаем подструктуры
            for (int i = 0; i < structCount; i++)
            {
                string st = str.ReadLine();

                line = new List<string>((st).Split(' '));
                line.RemoveAll(x => x == "");
                
                for (int j = 0; j < line.Count; j++)
                {
                    s = line[j];
                    relationship.Add(int.Parse(s));
                }
            }

            // пропускаем собственные числа
            for (int i = 0; i < stateCount; i++)
            {
                str.ReadLine();
            }

            // читаем состояния
            List<List<Vector3>> StatesList = new List<List<Vector3>>();
            // первый элемент - список нулевого состояния поэтому заполняется нулевыми векторами
            List<Vector3> lok = new List<Vector3>();
            for (int i = 0; i < points.Count; i++) { lok.Add(new Vector3()); }
            StatesList.Add(lok);
            // остольные состояния
            int col = 0;
            for (int i = 0; i < stateCount; i++)
            {
                s = str.ReadLine();
                line = new List<string>((s).Split(' '));
                line.RemoveAll(x => x == "");

                List<Vector3> li = new List<Vector3>();
                col = 0;
                for (int j = 0; j < points.Count; j++)
                {
                    Vector3 vop = new Vector3(float.Parse(line[col],
                                                    System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Float,
                                                    System.Globalization.CultureInfo.InvariantCulture),
                                                    float.Parse(line[++col],
                                                    System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Float,
                                                    System.Globalization.CultureInfo.InvariantCulture),
                                                    float.Parse(line[++col],
                                                    System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Float,
                                                    System.Globalization.CultureInfo.InvariantCulture))
                                                    ;
                    li.Add(100000000000 * vop);
                    ++col;
                    col++;
                }
                StatesList.Add(li);
            }

            // стрим больше не нужен
            str.Close();

            for(int i = 0; i < points.Count; i++)
            {
                points[i] = points[i] * 1000;
            }


            // Создаем для заполнения

            return new Model(points, StatesList, relationship);
        }


    }

}



                