using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Spectrum : MonoBehaviour
    {
        public Spectrum(System.IntPtr ptr) : base(ptr) { }

        /* https://answers.unity.com/questions/139323/any-way-of-quotautomaticquot-lip-syncing.html */
        public float volume = 100;
        public float freqLow = 300f;
        public float freqHigh = 350f;

        public AudioSource source;

        public Il2CppStructArray<float> freqData;
        private int samples = 256;
        private int fMax = 41000; // Sample rate

        private int sizeFilter = 5;
        private float[] filter;
        private float filterSum;
        private int posFilter = 0;
        private int qSamples = 0;

        private void Awake()
        {
            freqData = new Il2CppStructArray<float>(256);
            source = GetComponent<AudioSource>();
        }

        public float BandVol(float fLow, float fHigh)
        {
            fLow = Mathf.Clamp(fLow, 20, fMax);
            fHigh = Mathf.Clamp(fHigh, fLow, fMax);

            source.GetSpectrumData(freqData, 0, FFTWindow.BlackmanHarris);
            int n1 = (int)Mathf.Floor(fLow * samples / fMax);
            int n2 = (int)Mathf.Floor(fHigh * samples / fMax);
            float sum = 0;

            for (int i = n1; i < n2; i++)
            {
                sum += freqData[i];
            }

            return sum / (n2 - n1 + 1);
        }

        public float MovingAverage(float sample)
        {
            if (qSamples == 0)
            {
                filter = new float[sizeFilter];
            }

            filterSum += sample - filter[posFilter];
            filter[posFilter++] = sample;
            if (posFilter > qSamples)
            {
                qSamples = posFilter;
            }
            posFilter = posFilter % sizeFilter;
            return filterSum / qSamples;
        }
    }
}
