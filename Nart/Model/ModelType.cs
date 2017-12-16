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

        OriMaxilla ,
        OriMandible ,

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
