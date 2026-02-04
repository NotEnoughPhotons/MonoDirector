using NEP.MonoDirector.Data;

using UnityEngine;

using Il2CppSLZ.Marrow.Pool;
using Il2CppTMPro;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SoundHolder(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public static Dictionary<string, AudioClip> LoadedClips;

        private Poolee poolee;

        private AudioClip sound;

        private TextMeshPro nameText;

        private void Start()
        {
            poolee = GetComponent<Poolee>();
            nameText = transform.Find("DisplayName").GetComponent<TextMeshPro>();
            AssignSound(WarehouseLoader.soundTable[poolee.SpawnableCrate.Description]);
        }

        private void OnEnable()
        {
            nameText.text = sound.name;
        }

        private void OnDisable()
        {
            poolee.Despawn();
        }

        public void AssignSound(AudioClip sound)
        {
            this.sound = sound;
            nameText.text = sound.name;
        }

        public AudioClip GetSound()
        {
            return sound;
        }
    }
}