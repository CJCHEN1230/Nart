using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nart
{
    public class OSPModel : MeshGeometryModel3D
    {

        public OSPModel()
        {
            //MeshGeometry3D.Colors
            MeshGeometry3D test = new MeshGeometry3D();           
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Colors];
        }
    }
}
