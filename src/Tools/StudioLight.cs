using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class StudioLight(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private Color[] colors = new Color[]
        {
            Color.white,
            Color.red,
            new Color(1, 0.6470588f, 0, 1f),
            Color.yellow,
            Color.green,
            Color.blue,
            new Color(0.2941177f, 0, 0.509804f, 1f),
            new Color(0.9333333f, 0.509804f, 0.9333333f, 1f)
        };

        private float[] intensities = new float[]
        {
            1f,
            2f,
            3f,
            4f,
            5f
        };

        private Light spotlight;
        private Light pointLight;

        private CylinderGrip grip;

        private int colorIndex;
        private int intensityIndex;

        private void Awake()
        {
            spotlight = transform.Find("Spotlight").GetComponent<Light>();
            pointLight = transform.Find("Point Light").GetComponent<Light>();

            grip = transform.Find("Grip").GetComponent<CylinderGrip>();

            grip.attachedUpdateDelegate += new Action<Hand>((hand) => HandAttachedUpdate(hand));
        }

        private void HandAttachedUpdate(Hand hand)
        {
            if (hand.GetIndexButtonDown())
            {
                if (colorIndex > colors.Length)
                {
                    colorIndex = 0;
                }
                else
                {
                    colorIndex++;
                }

                spotlight.color = colors[colorIndex];
                pointLight.color = colors[colorIndex];
            }

            if (hand.Controller.GetMenuTap())
            {
                if (intensityIndex > colors.Length)
                {
                    intensityIndex = 0;
                }
                else
                {
                    intensityIndex++;
                }

                spotlight.intensity = intensities[intensityIndex];
                pointLight.intensity = intensities[intensityIndex];
            }
        }
    }
}
