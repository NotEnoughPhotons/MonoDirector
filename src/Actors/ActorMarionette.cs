using System;
using Il2CppSLZ.Marrow;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorMarionette : MonoBehaviour
    {
        public ActorMarionette(System.IntPtr ptr) : base(ptr) { }

        public BaseController LeftProxyController => _leftProxyController;
        public BaseController RightProxyController => _rightProxyController;
        
        private BaseController _leftProxyController;
        private BaseController _rightProxyController;

        private RigManager _actorRig;

        private void Awake()
        {
            // _leftProxyController.contRig = _actorRig.ControllerRig;
            // _rightProxyController.contRig = _actorRig.ControllerRig;
        }
    }
}