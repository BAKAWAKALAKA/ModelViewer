using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;


namespace ModelViewer
{
    /// <summary>
    /// Component of model that describes view of  object model( all points and states);
    /// </summary>
    public class Quality:Component// считай тоже что и gameObject
    {
        private  Vector3[,] edges = { {new Vector3(1,-1,-1),new Vector3(1,0,-1),new Vector3(1,1,-1), new Vector3(0,1,-1),new Vector3(-1,1,-1), new Vector3(-1,0,-1), new Vector3(-1,-1,-1),new Vector3(0,-1,-1) },
                                     {new Vector3(1,-1,1), new Vector3(1,0,1), new Vector3(1,1,1),  new Vector3(0,1,1), new Vector3(-1,1,1),  new Vector3(-1,0,1), new Vector3(-1,-1,1), new Vector3(0,-1,1) },
                                     {new Vector3(1,-1,-1),new Vector3(1,0,-1),new Vector3(1,1,-1), new Vector3(1,1,0), new Vector3(1,1,1),   new Vector3(1,0,1),  new Vector3(1,-1,1),  new Vector3(1,-1,0) },
                                     {new Vector3(-1,-1,-1),new Vector3(-1,0,-1),new Vector3(-1,1,-1),new Vector3(-1,1,0),new Vector3(-1,1,1),new Vector3(-1,0,1), new Vector3(-1,-1,1), new Vector3(-1,-1,0) },
                                     {new Vector3(1,-1,-1),new Vector3(0,-1,-1),new Vector3(-1,-1,-1),new Vector3(-1,-1,0),new Vector3(-1,-1,1),new Vector3(0,-1,1),new Vector3(1,-1,1), new Vector3(1,-1,0) },
                                     { new Vector3(1,1,-1),new Vector3(0,1,-1),new Vector3(-1,1,-1), new Vector3(-1,1,0),new Vector3(-1,1,1),new Vector3(0,1,1),   new Vector3(1,1,1),   new Vector3(1,1,0) }
                                   };
        private  Vector3[] localCoord = {
            new Vector3(1,-1,-1),
            new Vector3(1,1,-1),

            new Vector3(-1,1,-1),
            new Vector3(-1,-1,-1),

            new Vector3(1,-1,1),
            new Vector3(1,1,1),

            new Vector3(-1,1,1),
            new Vector3(-1,-1,1),

            new Vector3(1,0,-1),
            new Vector3(0,1,-1),

            new Vector3(-1,0,-1),
            new Vector3(0,-1,-1),

            new Vector3(1,-1,0),
            new Vector3(1,1,0),

            new Vector3(-1,1,0),
            new Vector3(-1,-1,0),

            new Vector3(1,0,1),
            new Vector3(0,1,1),

            new Vector3(-1,0,1),
            new Vector3(0,-1,1)
        };

        // all index points on points in  glob_Coord
        private List<int> references;
        private List<Vector3> glob_Coord;

        private List<List<Vector3>> StatesList;

        public List<float> _viewVertex;
        public List<ushort> _viewVertexIndex;

        public int quality { private set;  get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexes"></param>
        /// <param name="vertexesIndexes"></param>
        public Quality(List<Vector3> vertexes, List<int> vertexesIndexes, List<List<Vector3>> states)
        {
            glob_Coord = vertexes;
            references = vertexesIndexes;
            StatesList = states;

            _viewVertex = new List<float>();
            _viewVertexIndex = new List<ushort>();
            { } //ап
            SetQuality(1,0);
        }


        /// <summary>
        /// make list of points that make model with current quality
        /// </summary>
        /// <param name="size"> size of point model </param>
        /// <param name="qual">"quality" of model</param>
        /// <returns> list of view points of model with current quality</returns>
        public void SetQuality( int qual, int state)
        {
            this.quality = qual;
            //_viewVertex.Clear();
            List<Vector3> result = new List<Vector3>();
            Vector3 dV1, dV2, V;
            for (int obj = 0; obj < (references.Count / 20); obj++)
            {
                for (int edge = 0; edge <= 5; edge++)
                {
                    int objEdgeIndex = (qual + 1) * (qual + 1) * (edge + obj * 6);

                    dV1 = new Vector3(edges[edge, 2].X - edges[edge, 0].X,
                                       edges[edge, 2].Y - edges[edge, 0].Y,
                                       edges[edge, 2].Z - edges[edge, 0].Z);

                    dV2 = new Vector3(edges[edge, 6].X - edges[edge, 0].X,
                                       edges[edge, 6].Y - edges[edge, 0].Y,
                                       edges[edge, 6].Z - edges[edge, 0].Z);

                    for (int x = 0; x <= qual; x++)
                    {
                        for (int y = 0; y <= qual; y++)
                        {
                            V = new Vector3(edges[edge, 0].X + dV1.X * x / qual + dV2.X * y / qual,
                                             edges[edge, 0].Y + dV1.Y * x / qual + dV2.Y * y / qual,
                                             edges[edge, 0].Z + dV1.Z * x / qual + dV2.Z * y / qual);
                            //result.Add(V);
                            V = LocalToGlobal(obj, V, state);
                            _viewVertex.Add(V.X);
                            _viewVertex.Add(V.Y);
                            _viewVertex.Add(V.Z);
                        }
                    }
                }
            }
            GenerateIndex(qual);
        }

        
        private void GenerateIndex(int quil)
        {
            if (_viewVertexIndex!=null) _viewVertexIndex.Clear();

            ushort xIndex;
            ushort yIndex;

            for (int i = 0; i < _viewVertex.Count(); i += (quil + 1) * (quil + 1))
            {
                for (int x = 0; x < quil; x++)
                {
                    xIndex =(ushort)( x * (quil + 1) + i);
                    yIndex = (ushort) (xIndex + quil + 1);

                    for (int y = 0; y < quil; y++)
                    {

                        _viewVertexIndex.Add(xIndex);
                        _viewVertexIndex.Add(++xIndex); // тк нужно сместить указатель на первую точку в квадрате
                        _viewVertexIndex.Add(yIndex);
                        _viewVertexIndex.Add(++yIndex);

                    }
                }
            }

        }


        private Vector3 LocalToGlobal(int ref_num, Vector3 _base , int State) 
        { // шаг 1 и 2 

            // Vector3d _base = new Vector3d(1, 1, 1);
            float[] SF;
            // шаг три = формирование массива
            SF = new float[20];
            for (int i = 0; i < 8; i++)
            {
                SF[i] = 0.125f * (1 + _base.X * localCoord[i].X)
                                * (1 + _base.Y * localCoord[i].Y)
                                * (1 + _base.Z * localCoord[i].Z)
                                * (_base.X * localCoord[i].X + _base.Y * localCoord[i].Y + _base.Z * localCoord[i].Z - 2);
            }

            for (int i = 12; i < 16; i++)
            {
                SF[i] = 0.25f * (1 + _base.X * localCoord[i].X) * (1 + _base.Y * localCoord[i].Y) * (1 - (float) Math.Pow(Math.Abs(_base.Z), 2));
            }
            
            SF[9] = 0.25f * (1 - (float) Math.Pow(_base.X, 2)) * (1 + _base.Y * localCoord[9].Y) * (1 + _base.Z * localCoord[9].Z);
            SF[11] = 0.25f * (1 - (float)Math.Pow(_base.X, 2)) * (1 + _base.Y * localCoord[11].Y) * (1 + _base.Z * localCoord[11].Z);
            SF[17] = 0.25f * (1 - (float)Math.Pow(_base.X, 2)) * (1 + _base.Y * localCoord[17].Y) * (1 + _base.Z * localCoord[17].Z);
            SF[19] = 0.25f * (1 - (float)Math.Pow(_base.X, 2)) * (1 + _base.Y * localCoord[19].Y) * (1 + _base.Z * localCoord[19].Z);

            SF[8] = 0.25f * (1 - (float) Math.Pow(_base.Y, 2)) * (1 + _base.X * localCoord[8].X) * (1 + _base.Z * localCoord[8].Z);
            SF[10] = 0.25f * (1 - (float) Math.Pow(_base.Y, 2)) * (1 + _base.X * localCoord[10].X) * (1 + _base.Z * localCoord[10].Z);
            SF[16] = 0.25f * (1 - (float) Math.Pow(_base.Y, 2)) * (1 + _base.X * localCoord[16].X) * (1 + _base.Z * localCoord[16].Z);
            SF[18] = 0.25f * (1 - (float) Math.Pow(_base.Y, 2)) * (1 + _base.X * localCoord[18].X) * (1 + _base.Z * localCoord[18].Z);


            // шаг 4

            Vector3 XG = new Vector3();
            for (int i = 0; i < 20; i++)
            {
                XG.X = XG.X + SF[i] * (glob_Coord[references[ref_num * 20 + i]] + StatesList[State][references[ref_num * 20 + i]]).X;
                XG.Y = XG.Y + SF[i] * (glob_Coord[references[ref_num * 20 + i]] + StatesList[State][references[ref_num * 20 + i]]).Y;
                XG.Z = XG.Z + SF[i] * (glob_Coord[references[ref_num * 20 + i]] + StatesList[State][references[ref_num * 20 + i]]).Z;
            }

            return XG;

        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class Model
    {
        float[] colorData = new float[] {
            0.0f, 0.0f, 1.0f
        };

        Mesh mesh;
        Quality quality;
        Transform transform;

        public int curentState { private set; get; }
        public int states { private set; get; }
        public int vertexes { private set; get; }
        public int subgroups { private set; get; }

        public Model(List<Vector3> v, List<List<Vector3>> sv, List<int> i)
        {
            quality = new Quality(v, i, sv);
            mesh = new Mesh();
            transform = new Transform(this);

            states = sv.Count;
            curentState = 0;
            vertexes = v.Count;
            subgroups = i.Count / 20;

            mesh.Load(quality._viewVertex.ToArray(), quality._viewVertexIndex.ToArray(), colorData);
        }


        public void Rotate(Orientation orientation, float angle)
        {
            transform.Rotate(orientation, angle);
        }

        public void NextState() {
            if(curentState+1 < states)
            SetState(++curentState);
        }

        public void PreviesState()
        {
            if(curentState > 0)
            SetState(--curentState);
        }

        public void SetState(int index_)
        {
            if (curentState >= states) curentState = states - 1;
            if (curentState < 0) curentState = 0;
;
            SetQulity(quality.quality);
        }

        public void SetQulity(int _quility)
        {
            quality.SetQuality(_quility,curentState);
            mesh = new Mesh();
            mesh.Load(quality._viewVertex.ToArray(), quality._viewVertexIndex.ToArray(), colorData);
        }

        public void Render()
        {
            if (mesh != null) mesh.Render( ref transform.rotMat);
        }


    }

}
