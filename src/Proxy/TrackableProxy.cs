using System;
using UnityEngine;

namespace NEP.MonoDirector.Proxy
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class TrackableProxy(IntPtr ptr) : MonoBehaviour(ptr)
    {
    }
}
