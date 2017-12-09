using HelixToolkit.Wpf.SharpDX;
using Nart.Model_Object;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nart
{
    public class Projectata : ObservableObject
    {
        private string _name = "蔡慧君";
        private string _id = "123456";
        private string _institution = "成大";
        private bool _canSelectPoints = false;
        private string _selectPointState = "OFF";
        private  ObservableCollection<BallModel> _ballCollection=  new ObservableCollection<BallModel>();

        public Projectata()
        {

            //Random crandom = new Random();
            //for (int i = 0; i < 5; i++)
            //{
            //    BallModel ball = new BallModel();
            //    ball.BallName = i.ToString() /*+ "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"*/;
            //    ball.BallInfo = "!!!!!!!!!!!!!!!!!!!!!!!!!!";

            //    var b1 = new MeshBuilder();

            //    int a =crandom.Next() % 100;
            //    int b = crandom.Next() % 100;
            //    int c = crandom.Next() % 100;
            //    ball.ModelCenter = new Vector3(a, b, c);
            //    b1.AddSphere(new Vector3(a, b, c), 5);
            //    ball.Geometry = b1.ToMeshGeometry3D();
            //    ball.Material =PhongMaterials.White;
            //    ball.Transform = new System.Windows.Media.Media3D.MatrixTransform3D();
                

            //    //MultiAngleViewModel.NormalModelCollection.Add(ball);

            //    BallCollection.Add(ball);
            //}
        }


        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetValue(ref _name, value);
            }
        }
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetValue(ref _id, value);
            }
        }
        public string Institution
        {
            get
            {
                return _institution;
            }
            set
            {
                SetValue(ref _institution, value);
            }
        }
        public bool CanSelectPoints
        {
            get
            {
                return _canSelectPoints;
            }
            set
            {
                SetValue(ref _canSelectPoints, value);
                if (_canSelectPoints)
                {
                    SelectPointState = "ON";
                }
                else
                {
                    SelectPointState = "OFF";
                }
            }
        }
        public string SelectPointState
        {
            get
            {
                return _selectPointState;
            }
            set
            {
                SetValue(ref _selectPointState, value);
            }
        }
        public  ObservableCollection<BallModel> BallCollection
        {
            get
            {
                return _ballCollection;
            }
            set
            {
                SetValue(ref _ballCollection, value);
            }
        } 

    }
}
