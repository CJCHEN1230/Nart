using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nart
{
    public enum  ModelType : byte
    {
        None = 0,

        Head,

        MovedMaxilla ,
        MovedMandible ,

        TargetMaxilla,
        TargetMandible,
        
        Maxilla,
        Mandible,

        TargetMaxillaTriangle,
        TargetMandibleTriangle,

        MovedMaxillaTriangle,
        MovedMandibleTriangle,
    }
}
