using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;

namespace NartControl
{
    using System.ComponentModel;
    using Camera = HelixToolkit.Wpf.SharpDX.Camera;
    using OrthographicCamera = HelixToolkit.Wpf.SharpDX.OrthographicCamera;

    public class MultiAngleViewModel : INotifyPropertyChanged
    {
        

        
        public Element3DCollection ModelGeometry { get; private set; } = new Element3DCollection();
        public DefaultEffectsManager EffectsManager { get; private set; }
        public Transform3D ModelTransform { get; private set; } = new TranslateTransform3D(0, 0, 0);
        public DefaultRenderTechniquesManager RenderTechniquesManager { get; private set; }
        //public ModelContainer3DX ModelContainer { get; private set; } = new ModelContainer3DX();
        public Camera Camera1 { get; } = new OrthographicCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

        public Camera Camera2 { get; } = new OrthographicCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

        public Camera Camera3 { get; } = new OrthographicCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(5, 12, -5), UpDirection = new Vector3D(0, 1, 0) };        
        public MultiAngleViewModel()
        {
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
