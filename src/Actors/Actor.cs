using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using Avatar = Il2CppSLZ.VRMK.Avatar;
using Directory = Il2CppSystem.IO.Directory;
using Keyframe = NEP.MonoDirector.Data.Keyframe;

namespace NEP.MonoDirector.Actors
{
    public class Actor : Trackable, IBinaryData
    {
        public Actor() : base()
        {
            
        }
        
        public Actor(Il2CppSLZ.VRMK.Avatar avatar) : this()
        {
            RigManager rigManager = BoneLib.Player.RigManager;
            _avatarBarcode = rigManager.AvatarCrate.Barcode;
            
            _playerAvatar = avatar;

            _avatarBones = GetAvatarBones(_playerAvatar);
            _avatarFrames = new List<FrameGroup>();

            _controllerFrames = new List<KeyframeGroup<ControllerKeyframe>>();

            GameObject micObject = new GameObject("Actor Microphone");
            _microphone = micObject.AddComponent<ActorSpeech>();

            _tempFrames = new ObjectFrame[_avatarBones.Length];
            _tempControllerFrames = new ControllerKeyframe[2];
            OwnedProps = new List<Prop>();

            _controllers = new BaseController[2]
            {
                rigManager.ControllerRig.leftController,
                rigManager.ControllerRig.rightController
            };
        }

        // For a traditional rig, this should be all the "head" bones
        public readonly List<int> HeadBones = new List<int>()
        {
            (int)HumanBodyBones.Head,
            (int)HumanBodyBones.Jaw,
            (int)HumanBodyBones.LeftEye,
            (int)HumanBodyBones.RightEye,
        };

        private Barcode _avatarBarcode;
        public Barcode AvatarBarcode => _avatarBarcode;
        
        public Avatar PlayerAvatar => _playerAvatar;
        public Avatar ClonedAvatar => _clonedAvatar;
        public Transform[] AvatarBones => _avatarBones;

        public List<Prop> OwnedProps { get; private set; }
        public IReadOnlyList<FrameGroup> Frames => _avatarFrames.AsReadOnly();

        protected List<FrameGroup> _avatarFrames;
        protected List<KeyframeGroup<ControllerKeyframe>> _controllerFrames;

        private ActorBody _body;
        private ActorSpeech _microphone;
        private Texture2D _avatarPortrait;

        private Avatar _playerAvatar;
        private Avatar _clonedAvatar;

        private ObjectFrame[] _tempFrames;
        private ControllerKeyframe[] _tempControllerFrames;

        private Transform[] _avatarBones;
        private Transform[] _clonedRigBones;
        private BaseController[] _controllers;
        private BaseController[] _clonedControllers;

        private RigManager _clonedRig;
        
        private KeyframeGroup<ControllerKeyframe> new_previousFrame;
        private KeyframeGroup<ControllerKeyframe> new_nextFrame;
        
        private FrameGroup _previousFrame;
        private FrameGroup _nextFrame;
        
        private int _headIndex;

        public override void OnSceneBegin()
        {
            
        }

        public override void Act()
        {
            new_previousFrame = new KeyframeGroup<ControllerKeyframe>();
            new_nextFrame = new KeyframeGroup<ControllerKeyframe>();

            for(int i = 0; i < _controllerFrames.Count; i++)
            {
                var frame = _controllerFrames[i];

                new_previousFrame = new_nextFrame;
                new_nextFrame = frame;

                if (frame.Time > Director.Instance.Playhead.PlaybackTime)
                {
                    break;
                }
            }

            float gap = new_nextFrame.Time - new_previousFrame.Time;
            float head = Director.Instance.Playhead.PlaybackTime - new_previousFrame.Time;

            float delta = head / gap;

            ControllerKeyframe[] previousTransformFrames = new_previousFrame.Frames;
            ControllerKeyframe[] nextTransformFrames = new_nextFrame.Frames;

            for (int i = 0; i < 2; i++)
            {
                if (previousTransformFrames == null)
                {
                    continue;
                }

                Vector3 previousPosition = previousTransformFrames[i].position;
                Vector3 nextPosition = nextTransformFrames[i].position;

                Quaternion previousRotation = previousTransformFrames[i].rotation;
                Quaternion nextRotation = nextTransformFrames[i].rotation;

                var controller = _clonedControllers[i];

                if(controller == null)
                {
                    continue;
                }

                controller.transform.position = Vector3.Lerp(previousPosition, nextPosition, delta);
                controller.transform.rotation = Quaternion.Slerp(previousRotation, nextRotation, delta);
            }
            
            for(int i = 0; i < actionFrames.Count; i++)
            {
                var actionFrame = actionFrames[i];

                if(Director.Instance.Playhead.PlaybackTime < actionFrame.timestamp)
                {
                    continue;
                }
                else
                {
                    actionFrame.Run();
                }
            }
        }

        /// <summary>
        /// Records the actor's bones, positons, and rotations for this frame.
        /// </summary>
        /// <param name="index">The frame to record the bones.</param>
        public override void RecordFrame()
        {
            KeyframeGroup<ControllerKeyframe> frames = new KeyframeGroup<ControllerKeyframe>();
            CaptureControllerFrames(_controllers);
            frames.SetFrames(_tempControllerFrames, Director.Instance.Playhead.RecordingTime);
            _controllerFrames.Add(frames);
        }

        public void CloneAvatar()
        {
            _clonedControllers = new BaseController[2];
            ActorRig.CreateRig((Action<RigManager>)((rig) =>
            {
                rig.gameObject.AddComponent<ActorMarionette>();
                rig.SwapAvatar(_playerAvatar);
                _clonedRig = rig;
                _clonedControllers[0] = rig.ControllerRig.leftController;
                _clonedControllers[1] = rig.ControllerRig.rightController;
            }));
        }

        public void SwitchToActor(Actor actor)
        {
            _clonedAvatar.gameObject.SetActive(false);
            actor._clonedAvatar.gameObject.SetActive(true);
        }

        public override void Delete()
        {
            
        }

        public void DeleteProp(Prop prop)
        {
            Events.OnPropRemoved?.Invoke(prop);
            OwnedProps.Remove(prop);
            GameObject.Destroy(prop);
        }

        public void DeleteOwnedProps()
        {
            foreach (var prop in OwnedProps)
            {
                DeleteProp(prop);
            }
            
            OwnedProps.Clear();
        }

        private void ShowHairMeshes(Avatar avatar)
        {
            if(avatar == null)
            {
                Main.Logger.Error("ShowHairMeshes: Avatar doesn't exist!");
            }

            if(avatar.hairMeshes.Count == 0 || avatar.hairMeshes == null)
            {
                Main.Logger.Error("ShowHairMeshes: No hair meshes to clone.");
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

        private void CaptureControllerFrames(BaseController[] controllers)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                BaseController controller = controllers[i];
                
                if (!controller)
                {
                    continue;
                }

                ControllerKeyframe frame = new(controller);
                _tempControllerFrames[i] = frame;
            }
        }
        
        private void CaptureBoneFrames(Transform[] boneList)
        {
            for (int i = 0; i < boneList.Length; i++)
            {
                if (boneList[i] == null)
                {
                    _tempFrames[i] = new ObjectFrame(default, default);
                    continue;
                }

                Vector3 bonePosition = boneList[i].position;
                Quaternion boneRotation = boneList[i].rotation;

                ObjectFrame frame = new ObjectFrame(bonePosition, boneRotation);
                _tempFrames[i] = frame;
            }

            // Undo the head offset... afterward because no branching :P
            // This undoes it for every bone we count under it too!
            foreach (int headBone in HeadBones)
                _tempFrames[headBone].position += Patches.PlayerAvatarArtPatches.UpdateAvatarHead.calculatedHeadOffset;
        }

        private Transform[] GetAvatarBones(Avatar avatar)
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
            
            byte[] encodedBarcode = Encoding.UTF8.GetBytes(_avatarBarcode.ID);
            bytes.AddRange(BitConverter.GetBytes(encodedBarcode.Length));
            bytes.AddRange(encodedBarcode);
            bytes.AddRange(BitConverter.GetBytes(Director.Instance.Playhead.TakeTime));
            bytes.AddRange(BitConverter.GetBytes(_avatarFrames.Count));

            foreach (FrameGroup group in _avatarFrames)
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

                // avatarBarcode = Encoding.UTF8.GetString(strBytes);
                
#if DEBUG
                Main.Logger.Msg($"[ACTOR]: Barcode: {_avatarBarcode}");
#endif
                
                // Then the take
                byte[] takeBytes = new byte[sizeof(float)];
                stream.Read(takeBytes, 0, takeBytes.Length);

                float takeTime = BitConverter.ToSingle(takeBytes, 0);

                // Force the take time to be correct
                // This means if an actor takes a long time on disk
                // We then match their take time and not ours
                //if (Director.Instance.Playhead.TakeTime < takeTime)
                    //Director.Instance.Playhead.TakeTime = takeTime;
                
                // Then deserialize the frames
                byte[] frameNumBytes = new byte[sizeof(int)];
                stream.Read(frameNumBytes, 0, frameNumBytes.Length);

                int numFrames = BitConverter.ToInt32(frameNumBytes, 0);

#if DEBUG
                Main.Logger.Msg($"[ACTOR]: NumFrames: {numFrames}");
#endif
                
                FrameGroup[] frameGroups = new FrameGroup[numFrames];

                for (int f = 0; f < numFrames; f++)
                {
                    frameGroups[f] = new FrameGroup();
                    frameGroups[f].FromBinary(stream);
                }

                _avatarFrames = new List<FrameGroup>(frameGroups);
            }
        }

        // TADB - Tracked Actor Data Block
        public uint GetBinaryID() => 0x42444154;
    }
}
