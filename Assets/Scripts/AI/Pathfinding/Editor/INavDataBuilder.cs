using UnityEngine;
using System.Collections;

namespace NavData2d.Editor
{
    public interface INavDataBuilder
    {
        NavData2dBuildContainer GlobalBuildContainer { get; set; }
        void TriggerRepaint();
    }
}
