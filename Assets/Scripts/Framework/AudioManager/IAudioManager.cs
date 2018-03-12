using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaomaoFramework
{
    public interface IAudioManager
    {
        float VolumeEffect { get; }
        bool EnableEffect { get; }
    }
}
