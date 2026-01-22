using Il2CppInterop.Runtime.InteropTypes.Arrays;

using NEP.MonoDirector.Audio;

using UnityEngine;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Audio;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;
using Random = UnityEngine.Random;

namespace NEP.MonoDirector.Actors
{
    public class ActorBody
    {
        public struct ActorBodyPart
        {
            public void Attach(Actor actor, HumanBodyBones bone)
            {
                part.transform.parent = actor.ClonedAvatar.animator.GetBoneTransform(bone);
                part.transform.localPosition = part.transform.parent.localPosition;
                part.transform.rotation = part.transform.parent.rotation;
            }

            public GameObject part;
            public MeshCollider meshCollider;
        }

        public ActorBody(Actor actor, PhysicsRig physicsRig)
        {
            this.actor = actor;
            this.physicsRig = physicsRig;

            SetupCollisions();
            SetupTriggerHull();
            SetupAudio();
        }

        private List<AudioClip> footstepWalkAudio;
        private List<AudioClip> footstepJogAudio;
        private List<AudioClip> landingAudio;
        private List<AudioClip> grabAudio;
        private List<AudioClip> gripReleaseAudio;

        private Actor actor;
        private PhysicsRig physicsRig;

        private GameObject head;
        private GameObject chest;
        private GameObject spine;
        private GameObject hips;
        private GameObject leftHand;
        private GameObject rightHand;
        private GameObject leftFoot;
        private GameObject rightFoot;

        private MeshCollider headCollider;
        private MeshCollider chestCollider;
        private MeshCollider spineCollider;
        private MeshCollider hipCollider;
        private BoxCollider leftHandCollider;
        private BoxCollider rightHandCollider;

        private void SetupTriggerHull()
        {
            GameObject triggerHullObject = new GameObject("Actor Trigger Hull");
            BoxCollider triggerHull = triggerHullObject.AddComponent<BoxCollider>();
            triggerHull.isTrigger = true;
            float avatarHeight = actor.ClonedAvatar.height;
            float avatarWidth = actor.ClonedAvatar._waistEllipseX + actor.ClonedAvatar._waistEllipseZ;
            triggerHull.size = new Vector3(avatarWidth, avatarHeight, avatarWidth);
            triggerHullObject.transform.SetParent(actor.ClonedAvatar.transform);
            triggerHullObject.transform.localPosition = Vector3.zero;
            triggerHullObject.transform.localRotation = Quaternion.identity;
        }

        private void SetupCollisions()
        {
            Transform actorHead = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.Head);
            Transform actorChest = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.UpperChest);
            Transform actorSpine = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.Spine);
            Transform actorHips = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.Hips);
            Transform actorLeftHand = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.LeftHand);
            Transform actorRightHand = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.RightHand);
            Transform actorLeftFoot = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform actorRightFoot = actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.RightFoot);

            head = new GameObject("Head");
            chest = new GameObject("Chest");
            spine = new GameObject("Spine");
            hips = new GameObject("Hips");
            leftHand = new GameObject("LeftHand");
            rightHand = new GameObject("RightHand");
            leftFoot = new GameObject("LeftFoot");
            rightFoot = new GameObject("RightFoot");

            headCollider = head.AddComponent<MeshCollider>();
            chestCollider = chest.AddComponent<MeshCollider>();
            spineCollider = spine.AddComponent<MeshCollider>();
            hipCollider = hips.AddComponent<MeshCollider>();
            leftHandCollider = leftHand.AddComponent<BoxCollider>();
            rightHandCollider = rightHand.AddComponent<BoxCollider>();

            ImpactProperties headVFX = head.AddComponent<ImpactProperties>();
            ImpactProperties chestVFX = chest.AddComponent<ImpactProperties>();
            ImpactProperties spineVFX = spine.AddComponent<ImpactProperties>();
            ImpactProperties hipVFX = hips.AddComponent<ImpactProperties>();
            ImpactProperties leftHandVFX = leftHand.AddComponent<ImpactProperties>();
            ImpactProperties rightHandVFX = rightHand.AddComponent<ImpactProperties>();

            headCollider.sharedMesh = physicsRig.torso.cHead.sharedMesh;
            chestCollider.sharedMesh = physicsRig.torso.cChest.sharedMesh;
            spineCollider.sharedMesh = physicsRig.torso.cSpine.sharedMesh;
            hipCollider.sharedMesh = physicsRig.torso.cPelvis.sharedMesh;

            leftHandCollider.size = physicsRig.leftHand.physHand.fingersCol.size;
            rightHandCollider.size = physicsRig.rightHand.physHand.fingersCol.size;

            head.transform.parent = actorHead;
            chest.transform.parent = actorChest;
            spine.transform.parent = actorSpine;
            hips.transform.parent = actorHips;
            leftHand.transform.parent = actorLeftHand;
            rightHand.transform.parent = actorRightHand;
            leftFoot.transform.parent = actorLeftFoot;
            rightFoot.transform.parent = actorRightFoot;

            head.transform.localPosition = Vector3.zero;
            chest.transform.localPosition = Vector3.zero;
            spine.transform.localPosition = Vector3.zero;
            hips.transform.localPosition = Vector3.zero;
            leftHand.transform.localPosition = Vector3.zero;
            rightHand.transform.localPosition = Vector3.zero;

            head.transform.rotation = Quaternion.identity;
            chest.transform.rotation = Quaternion.identity;
            spine.transform.rotation = Quaternion.identity;
            hips.transform.rotation = Quaternion.identity;
            leftHand.transform.rotation = Quaternion.identity;
            rightHand.transform.rotation = Quaternion.identity;

            // ImpactPropertiesManager vfxManager = physicsRig.GetComponent<ImpactPropertiesManager>();
            // 
            // headVFX.surfaceData = vfxManager.surfaceData;
            // chestVFX.surfaceData = vfxManager.surfaceData;
            // spineVFX.surfaceData = vfxManager.surfaceData;
            // hipVFX.surfaceData = vfxManager.surfaceData;
            // leftHandVFX.surfaceData = vfxManager.surfaceData;
            // rightHandVFX.surfaceData = vfxManager.surfaceData;
        }

        private void SetupAudio()
        {
            footstepWalkAudio = new List<AudioClip>();
            footstepJogAudio = new List<AudioClip>();
            landingAudio = new List<AudioClip>();

            MarrowAvatar avatar = actor.PlayerAvatar;


            Il2CppReferenceArray<AudioClip> avatarWalkingClips = avatar.footstepsWalk?.audioClips;
            Il2CppReferenceArray<AudioClip> avatarJoggingClips = avatar.footstepsJog?.audioClips;

            FootstepSFX sfx = GameObject.FindObjectOfType<FootstepSFX>();

            AudioClip[] targetWalkClips = avatarWalkingClips != null && avatarWalkingClips.Length > 0 ? avatarWalkingClips : sfx.walkConcrete;
            AudioClip[] targetJogClips = avatarJoggingClips != null && avatarJoggingClips.Length > 0 ? avatarJoggingClips : sfx.runConcrete;

            footstepWalkAudio.AddRange(targetWalkClips);
            footstepJogAudio.AddRange(targetJogClips);

            //landingAudio.AddRange(avatar.highFallOntoFeet.audioClips);
        }

        public void AllowCollisions(bool allow)
        {
            headCollider.enabled = allow;
            chestCollider.enabled = allow;
            spineCollider.enabled = allow;
            hipCollider.enabled = allow;
            leftHandCollider.enabled = allow;
            rightHandCollider.enabled = allow;
        }

        public void OnHandGrab()
        {

        }

        public void OnFootstep(float velocitySqr = 0f)
        {
            List<AudioClip> clipList = velocitySqr < 3f ? footstepWalkAudio : footstepJogAudio;
            AudioManager.Instance.PlayAtPosition(clipList[Random.Range(0, clipList.Count)], leftFoot.transform.position);
        }

        public void Delete()
        {
            GameObject.Destroy(head);
            GameObject.Destroy(chest);
            GameObject.Destroy(spine);
            GameObject.Destroy(hips);
            GameObject.Destroy(leftHand);
            GameObject.Destroy(rightHand);
        }
    }
}
