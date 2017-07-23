using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Nart
{
    class ModelData2
    {

        public Element3DCollection ModelGeometry { get; private set; }


        public void LoadSTL(string path)
        {
            StLReader r = new HelixToolkit.Wpf.StLReader();
            Model3DGroup model3dgroup = r.Read(path);






            Console.WriteLine("\n\n模型幾個:" + model3dgroup.Children.Count);
            for (int i = 0; i < model3dgroup.Children.Count; i++)
            {

                System.Windows.Media.Media3D.GeometryModel3D temp = model3dgroup.Children[i] as System.Windows.Media.Media3D.GeometryModel3D;

                System.Windows.Media.Media3D.MeshGeometry3D wpfmesh = temp.Geometry as System.Windows.Media.Media3D.MeshGeometry3D;

                

                var s = new MeshGeometryModel3D();
                
                PhongMaterial pm = new HelixToolkit.Wpf.SharpDX.PhongMaterial();
                pm.AmbientColor = new Color4(Convert.ToSingle(0.349), Convert.ToSingle(0.349), Convert.ToSingle(0.349), 1);
                pm.DiffuseColor = new Color4(Convert.ToSingle(0), Convert.ToSingle(0.5019), Convert.ToSingle(0), 1);
                pm.SpecularColor = new Color4(Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), 1);
                pm.SpecularShininess = 12.8F;
                pm.ReflectiveColor = new Color4(Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), Convert.ToSingle(0.1607), 1);



                HelixToolkit.Wpf.SharpDX.MeshGeometry3D meshgeometry = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();

                meshgeometry.Normals = new Vector3Collection();
                Console.WriteLine("\n\n個數:" + wpfmesh.Normals.Count);
                for (int j = 0; j < wpfmesh.Normals.Count; j++)
                {
                    meshgeometry.Normals.Add(new Vector3(Convert.ToSingle(wpfmesh.Normals[j].X)
                        , Convert.ToSingle(wpfmesh.Normals[j].Y)
                        , Convert.ToSingle(wpfmesh.Normals[j].Z)));
                }



                meshgeometry.Positions = new Vector3Collection();
                Console.WriteLine("\n\n個數:" + wpfmesh.Positions.Count);
                for (int j = 0; j < wpfmesh.Positions.Count; j++)
                {
                    meshgeometry.Positions.Add(new Vector3(Convert.ToSingle(wpfmesh.Positions[j].X)
                        , Convert.ToSingle(wpfmesh.Positions[j].Y)
                        , Convert.ToSingle(wpfmesh.Positions[j].Z)));
                }


                meshgeometry.Indices = new IntCollection();
                Console.WriteLine("\n\n個數:" + wpfmesh.TriangleIndices.Count);
                for (int j = 0; j < wpfmesh.TriangleIndices.Count; j++)
                {
                    meshgeometry.Indices.Add(wpfmesh.TriangleIndices[j]);
                }


                //HelixToolkit.Wpf.SharpDX.Geometry test = new HelixToolkit.Wpf.SharpDX.Geometry();

                s.Material = pm;
                s.Geometry = meshgeometry;

               //s.Geometry.UpdateBounds();

                //Console.WriteLine("\nx:" + -(s.Bounds.Minimum.X + s.Bounds.Maximum.X) / 2.0 + "\ny:" + -(s.Bounds.Minimum.Y + s.Bounds.Maximum.Y) / 2.0 + "\nz:" + -(s.Bounds.Minimum.Z + s.Bounds.Maximum.Z) / 2.0);
                //float a = s.BoundsSphere.Center.X;
                //float b = s.BoundsSphere.Center.Y;
                //float c = s.BoundsSphere.Center.Z;





                //this.ModelTransform = new TranslateTransform3D(-(s.Geometry.BoundingSphere.Center.X), -(s.Geometry.BoundingSphere.Center.Y), -(s.Geometry.BoundingSphere.Center.Z));

                //this.ModelTransform = new TranslateTransform3D(5000, 5000, 5000);

                //s.Transform = new TranslateTransform3D(-(s.Geometry.BoundingSphere.Center.X), -(s.Geometry.BoundingSphere.Center.Y), -(s.Geometry.BoundingSphere.Center.Z));

                s.Transform = new TranslateTransform3D(0, 140, 180);


                this.ModelGeometry.Add(s);







                
            }
            
        }

    }
}
