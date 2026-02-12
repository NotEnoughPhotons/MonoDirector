using System.Text;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using UnityEngine;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;
using MarrowSeat = Il2CppSLZ.Marrow.Seat;
using Il2CppSLZ.Marrow.Interaction;
using NEP.MonoDirector.Proxy;
using Il2CppSLZ.Marrow.Warehouse;
using NEP.MonoDirector.UI;

namespace NEP.MonoDirector.Actors
{
    public class Actor : Trackable, IBinaryData
    {
        public Actor() : base()
        {
#if DEBUG
            m_previousFrameDebugger = new Transform[55];
            m_nextFrameDebugger = new Transform[55];

            GameObject baseCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            baseCube.GetComponent<BoxCollider>().enabled = false;
            baseCube.transform.localScale = Vector3.one * 0.03F;

            MeshRenderer renderer = baseCube.GetComponent<MeshRenderer>();
            // renderer.material = new Material(Shader.Find(Jevil.Const.UrpLitName));

            GameObject empty = new GameObject("MONODIRECTOR DEBUG VIZ");
            baseCube.transform.parent = empty.transform;
            
            for (int i = 0; i < 55; i++)
            {
                m_previousFrameDebugger[i] = GameObject.Instantiate(empty).transform;
                m_nextFrameDebugger[i] = GameObject.Instantiate(empty).transform;
            }

            GameObject.Destroy(baseCube);
#endif
        }
        
        public Actor(MarrowAvatar avatar) : this()
        {
            avatarCrate = Constants.RigManager.AvatarCrate.Crate;
            
            m_playerAvatar = avatar;

            m_avatarBones = GetAvatarBones(m_playerAvatar);
            m_avatarFrames = new List<FrameGroup>();

            GameObject micObject = new GameObject("Actor Microphone");
            m_microphone = micObject.AddComponent<ActorSpeech>();

            m_tempFrames = new ObjectFrame[m_avatarBones.Length];

            m_ownedProps = new List<Prop>();
        }

        // For a traditional rig, this should be all the "head" bones
        public readonly List<int> HeadBones = new List<int>()
        {
            (int)HumanBodyBones.Head,
            (int)HumanBodyBones.Jaw,
            (int)HumanBodyBones.LeftEye,
            (int)HumanBodyBones.RightEye,
        };


        private AvatarCrate avatarCrate;
        public AvatarCrate AvatarCrate => avatarCrate;
        
        public MarrowAvatar PlayerAvatar { get => m_playerAvatar; }
        public MarrowAvatar ClonedAvatar { get => m_clonedAvatar; }
        public Transform[] AvatarBones { get => m_avatarBones; }

        public MarrowEntity MarrowEntity { get => m_marrowEntity; }

        public IReadOnlyList<FrameGroup> Frames => m_avatarFrames.AsReadOnly();
        public IReadOnlyList<Prop> OwnedProps => m_ownedProps.AsReadOnly();

        public ActorBody ActorBody { get => m_body; }
        public ActorSpeech Microphone { get => m_microphone; }
        public Texture2D AvatarPortrait { get => m_avatarPortrait; }

        public bool Seated { get => m_activeSeat != null; }

        protected List<FrameGroup> m_avatarFrames;

        private ActorBody m_body;
        private ActorProxy m_proxy;
        private ActorSpeech m_microphone;
        private Texture2D m_avatarPortrait;

        private List<Prop> m_ownedProps;

        private MarrowEntity m_marrowEntity;

        private MarrowSeat m_activeSeat;

        private MarrowAvatar m_playerAvatar;
        private MarrowAvatar m_clonedAvatar;

        private ObjectFrame[] m_tempFrames;

        private Transform[] m_avatarBones;
        private Transform[] m_clonedRigBones;

        private FrameGroup m_previousFrame;
        private FrameGroup m_nextFrame;

        private Transform m_lastPelvisParent;
        private int m_headIndex;
        
        // Debug build stuff
        #if DEBUG
        private Transform[] m_previousFrameDebugger;
        private Transform[] m_nextFrameDebugger;
        #endif

        public override void OnSceneBegin()
        {
            base.OnSceneBegin();

            for (int i = 0; i < 55; i++)
            {
                var bone = m_clonedRigBones[i];

                if (bone == null)
                {
                    continue;
                }
                
                bone.position = m_avatarFrames[0].TransformFrames[i].position;
                bone.rotation = m_avatarFrames[0].TransformFrames[i].rotation;
            }
        }

        public override void Act()
        {
            m_previousFrame = new FrameGroup();
            m_nextFrame = new FrameGroup();

            for(int i = 0; i < m_avatarFrames.Count; i++)
            {
                var frame = m_avatarFrames[i];

                m_previousFrame = m_nextFrame;
                m_nextFrame = frame;

                if (frame.FrameTime > Playback.Instance.PlaybackTime)
                {
                    break;
                }
            }

            float gap = m_nextFrame.FrameTime - m_previousFrame.FrameTime;
            float head = Playback.Instance.PlaybackTime - m_previousFrame.FrameTime;

            float delta = head / gap;

            ObjectFrame[] previousTransformFrames = m_previousFrame.TransformFrames;
            ObjectFrame[] nextTransformFrames = m_nextFrame.TransformFrames;

            for (int i = 0; i < 55; i++)
            {
                if (i == (int)HumanBodyBones.Jaw)
                {
                    continue;
                }

                if (previousTransformFrames == null)
                {
                    continue;
                }

                Vector3 previousPosition = previousTransformFrames[i].position;
                Vector3 nextPosition = nextTransformFrames[i].position;

                Quaternion previousRotation = previousTransformFrames[i].rotation;
                Quaternion nextRotation = nextTransformFrames[i].rotation;

                var bone = m_clonedRigBones[i];

                if(bone == null)
                {
                    continue;
                }

                bone.position = Vector3.Lerp(previousPosition, nextPosition, delta);
                bone.rotation = Quaternion.Slerp(previousRotation, nextRotation, delta);
#if DEBUG
                m_previousFrameDebugger[i].position = previousPosition;
                m_previousFrameDebugger[i].rotation = previousRotation;
                
                m_nextFrameDebugger[i].position = nextPosition;
                m_nextFrameDebugger[i].rotation = nextRotation;
#endif
            }
            
            for(int i = 0; i < actionFrames.Count; i++)
            {
                var actionFrame = actionFrames[i];

                if(Playback.Instance.PlaybackTime < actionFrame.timestamp)
                {
                    continue;
                }
                else
                {
                    actionFrame.Run();
                }
            }

            m_microphone?.Playback();
            m_microphone?.UpdateJaw();
        }

        /// <summary>
        /// Records the actor's bones, positons, and rotations for this frame.
        /// </summary>
        /// <param name="index">The frame to record the bones.</param>
        public override void RecordFrame()
        {
            FrameGroup frameGroup = new FrameGroup();
            CaptureBoneFrames(m_avatarBones);
            frameGroup.SetFrames(m_tempFrames, Recorder.Instance.RecordingTime);
            m_avatarFrames.Add(frameGroup);
        }

        public void CloneAvatar()
        {
            GameObject clonedAvatarObject = GameObject.Instantiate(m_playerAvatar.gameObject);
            m_clonedAvatar = clonedAvatarObject.GetComponent<MarrowAvatar>();

            m_clonedAvatar.gameObject.SetActive(true);

            m_body = new ActorBody(this, Constants.RigManager.physicsRig);

            // stops position overrides, if there are any
            m_clonedAvatar.GetComponent<Animator>().enabled = false;

            m_clonedRigBones = GetAvatarBones(m_clonedAvatar);

            GameObject.Destroy(m_clonedAvatar.GetComponent<LODGroup>());

            actorName = Constants.RigManager.AvatarCrate.Crate.Title;
            m_clonedAvatar.name = actorName;
            ShowHairMeshes(m_clonedAvatar);

            m_microphone.SetAvatar(m_clonedAvatar);

            m_clonedAvatar.gameObject.SetActive(true);

            // avatarPortrait = AvatarPhotoBuilder.avatarPortraits[actorName];

            m_proxy = clonedAvatarObject.AddComponent<ActorProxy>();
            m_proxy.SetActor(this);

            m_marrowEntity = clonedAvatarObject.AddComponent<MarrowEntity>();
            m_marrowEntity.Validate();

            Events.OnActorCasted?.Invoke(this);
        }

        public void SwitchToActor(Actor actor)
        {
            m_clonedAvatar.gameObject.SetActive(false);
            actor.m_clonedAvatar.gameObject.SetActive(true);
        }

        public void OwnProp(Prop prop)
        {
            m_ownedProps.Add(prop);
        }

        public void DisownProp(Prop prop)
        {
            m_ownedProps.Remove(prop);
        }

        public override void Delete()
        {
            Events.OnActorUncasted?.Invoke(this);
            
            foreach (var ownedProp in m_ownedProps)
            {
                PropMarkerManager.RemoveMarkerFromProp(ownedProp);
                Director.WorldProps.Remove(ownedProp);
                ownedProp.DeleteAllFrames();
            }

            m_ownedProps.Clear();
            m_body.Delete();
            GameObject.Destroy(m_clonedAvatar.gameObject);
            GameObject.Destroy(m_microphone.gameObject);
            m_microphone = null;
            m_avatarFrames.Clear();
        }

        public void ParentToSeat(MarrowSeat seat)
        {
            m_activeSeat = seat;

            Transform pelvis = m_clonedAvatar.animator.GetBoneTransform(HumanBodyBones.Hips);

            m_lastPelvisParent = pelvis.GetParent();

            Vector3 seatOffset = new Vector3(seat._buttOffset.x, Mathf.Abs(seat._buttOffset.y) * m_clonedAvatar.heightPercent, seat._buttOffset.z);

            pelvis.SetParent(seat.transform);

            pelvis.position = seat.buttTargetInWorld;
            pelvis.localPosition = seatOffset;
        }

        public void UnparentSeat()
        {
            m_activeSeat = null;
            Transform pelvis = m_clonedAvatar.animator.GetBoneTransform(HumanBodyBones.Hips);
            pelvis.SetParent(m_lastPelvisParent);
        }

        private void ShowHairMeshes(MarrowAvatar avatar)
        {
            if(avatar == null)
            {
                Logging.Error("ShowHairMeshes: Avatar doesn't exist!");
            }

            if(avatar.hairMeshes.Count == 0 || avatar.hairMeshes == null)
            {
                Logging.Warn("ShowHairMeshes: No hair meshes to clone.");
            }

            foreach (var mesh in avatar.hairMeshes)
            {
                if(mesh == null)
                {
                    continue;
                }

                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            }
        }

        private void CaptureBoneFrames(Transform[] boneList)
        {
            for (int i = 0; i < boneList.Length; i++)
            {
                if (boneList[i] == null)
                {
                    m_tempFrames[i] = new ObjectFrame(default, default);
                    continue;
                }

                Vector3 bonePosition = boneList[i].position;
                Quaternion boneRotation = boneList[i].rotation;

                ObjectFrame frame = new ObjectFrame(bonePosition, boneRotation);
                m_tempFrames[i] = frame;
            }

            // Undo the head offset... afterward because no branching :P
            // This undoes it for every bone we count under it too!
            foreach (int headBone in HeadBones)
                m_tempFrames[headBone].position += Patches.PlayerAvatarArtPatches.UpdateAvatarHead.calculatedHeadOffset;
        }

        private Transform[] GetAvatarBones(MarrowAvatar avatar)
        {
            Transform[] bones = new Transform[(int)HumanBodyBones.LastBone];

            for (int i = 0; i < (int)HumanBodyBones.LastBone - 1; i++)
            {
                var currentBone = (HumanBodyBones)i;

                var boneTransform = avatar.animator.GetBoneTransform(currentBone);
                bones[i] = boneTransform;
            }

            return bones;
        }
        
        //
        // Enums
        //
        public enum VersionNumber : short
        {
            V1
        }
        
        //
        // IBinaryData
        //
        public byte[] ToBinary()
        {
            // TODO: Keep this up to date

            List<byte> bytes = new List<byte>();
            
            // The header contains the following data
            //
            // version: u16
            // barcode_size: i32
            // barcode : utf-8 string
            // take_time: float
            // num_frames : u32
            //
            // Below the header is the following
            //
            // num_frames FrameGroup blocks
            //
            bytes.AddRange(BitConverter.GetBytes((short)VersionNumber.V1));
            
            // byte[] encodedBarcode = Encoding.UTF8.GetBytes(avatarCrate);
            // bytes.AddRange(BitConverter.GetBytes(encodedBarcode.Length));
            // bytes.AddRange(encodedBarcode);
            // bytes.AddRange(BitConverter.GetBytes(Recorder.Instance.TakeTime));
            // bytes.AddRange(BitConverter.GetBytes(m_avatarFrames.Count));

            foreach (FrameGroup group in m_avatarFrames)
                bytes.AddRange(group.ToBinary());

            return bytes.ToArray();
        }

        public void FromBinary(Stream stream)
        {
            // Check the version number
            byte[] versionBytes = new byte[sizeof(short)];
            stream.Read(versionBytes, 0, versionBytes.Length);

            short version = BitConverter.ToInt16(versionBytes, 0);

            if (version != (short)VersionNumber.V1)
                throw new Exception($"Unsupported version type! Value was {version}");

            // Deserialize
            if (version == (short)VersionNumber.V1)
            {
                // How long is the string?
                byte[] strLenBytes = new byte[sizeof(int)];
                stream.Read(strLenBytes, 0, strLenBytes.Length);

                int strLen = BitConverter.ToInt32(strLenBytes, 0);

                byte[] strBytes = new byte[strLen];
                stream.Read(strBytes, 0, strBytes.Length);

                // TODO: get crate based on serialized barcode
                //avatarCrate = Encoding.UTF8.GetString(strBytes);
                
#if DEBUG
                Logging.Msg($"[ACTOR]: Barcode: {avatarCrate}");
#endif
                
                // Then the take
                byte[] takeBytes = new byte[sizeof(float)];
                stream.Read(takeBytes, 0, takeBytes.Length);

                float takeTime = BitConverter.ToSingle(takeBytes, 0);

                // Force the take time to be correct
                // This means if an actor takes a long time on disk
                // We then match their take time and not ours
                if (Recorder.Instance.TakeTime < takeTime)
                    Recorder.Instance.TakeTime = takeTime;
                
                // Then deserialize the frames
                byte[] frameNumBytes = new byte[sizeof(int)];
                stream.Read(frameNumBytes, 0, frameNumBytes.Length);

                int numFrames = BitConverter.ToInt32(frameNumBytes, 0);

#if DEBUG
                Logging.Msg($"[ACTOR]: NumFrames: {numFrames}");
#endif
                
                FrameGroup[] frameGroups = new FrameGroup[numFrames];

                for (int f = 0; f < numFrames; f++)
                {
                    frameGroups[f] = new FrameGroup();
                    frameGroups[f].FromBinary(stream);
                }

                m_avatarFrames = new List<FrameGroup>(frameGroups);
            }
        }

        // TADB - Tracked Actor Data Block
        public uint GetBinaryID() => 0x42444154;
    }
}
