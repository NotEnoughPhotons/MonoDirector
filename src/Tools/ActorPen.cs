using UnityEngine;
using MelonLoader;
using Il2CppTrees;
using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Core;
using Il2CppSLZ.Marrow.Interaction;
using NEP.MonoDirector.Proxy;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class ActorPen(IntPtr ptr) : ToolEntity(ptr)
    {
        private Transform m_rayPoint;
        private ActorProxy m_selectedProxy;

        protected override void Awake()
        {
            base.Awake();

            m_rayPoint = transform.Find("RayPoint");
        }

        protected override void OnPrimaryButtonDown()
        {
            Ray ray = new Ray(m_rayPoint.position, m_rayPoint.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, (int)BoneLib.GameLayers.DEFAULT, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.attachedRigidbody == null)
                {
                    return;
                }

                MarrowBody body = hit.collider.attachedRigidbody.GetComponent<MarrowBody>();
                MarrowEntity entity = body.Entity;
                ActorProxy proxy = entity.GetComponent<ActorProxy>();

                if (proxy == null)
                {
                    return;
                }

                m_selectedProxy = proxy;
                m_selectedProxy.OnSelected();
            }
            else
            {
                if (m_selectedProxy != null)
                {
                    m_selectedProxy.OnDeselected();
                    m_selectedProxy = null;
                }
            }
        }
    }
}
