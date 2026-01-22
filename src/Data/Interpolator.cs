using NEP.MonoDirector.Core;
using UnityEngine;

namespace NEP.MonoDirector.Data
{
    public static class Interpolator
    {
        public static float GetPlaybackTime()
        {
            return Playback.Instance.PlaybackTime;
        }

        public static float GetFrameDelta(float nextFrameTime, float previousFrameTime)
        {
            float gap = nextFrameTime - previousFrameTime;
            float head = GetPlaybackTime() - previousFrameTime;

            return head / gap;
        }

        public static Vector3 InterpolatePosition(List<ObjectFrame> frames)
        {
            ObjectFrame previousFrame = new ObjectFrame();
            ObjectFrame nextFrame = new ObjectFrame();

            foreach (var frame in frames)
            {
                previousFrame = nextFrame;
                nextFrame = frame;

                if (frame.frameTime > GetPlaybackTime())
                {
                    break;
                }
            }

            float delta = GetFrameDelta(nextFrame.frameTime, previousFrame.frameTime);

            return Vector3.Lerp(previousFrame.position, nextFrame.position, delta);
        }

        public static Quaternion InterpolateRotation(List<ObjectFrame> frames)
        {
            ObjectFrame previousFrame = new ObjectFrame();
            ObjectFrame nextFrame = new ObjectFrame();

            foreach (var frame in frames)
            {
                previousFrame = nextFrame;
                nextFrame = frame;

                if (frame.frameTime > GetPlaybackTime())
                {
                    break;
                }
            }

            float delta = GetFrameDelta(nextFrame.frameTime, previousFrame.frameTime);

            return Quaternion.Slerp(previousFrame.rotation, nextFrame.rotation, delta);
        }

        public static Vector3 InterpolatePosition(FrameGroup[] frames, int frameIndex)
        {
            FrameGroup previousFrame = new FrameGroup();
            FrameGroup nextFrame = new FrameGroup();

            foreach (var frame in frames)
            {
                previousFrame = nextFrame;
                nextFrame = frame;

                if (frame.FrameTime > GetPlaybackTime())
                {
                    break;
                }
            }

            float delta = GetFrameDelta(nextFrame.FrameTime, previousFrame.FrameTime);

            return Vector3.Lerp(previousFrame.TransformFrames[frameIndex].position, nextFrame.TransformFrames[frameIndex].position, delta);
        }

        public static Quaternion InterpolateRotation(FrameGroup[] frames, int frameIndex)
        {
            FrameGroup previousFrame = new FrameGroup();
            FrameGroup nextFrame = new FrameGroup();

            foreach (var frame in frames)
            {
                previousFrame = nextFrame;
                nextFrame = frame;

                if (frame.FrameTime > GetPlaybackTime())
                {
                    break;
                }
            }

            float delta = GetFrameDelta(nextFrame.FrameTime, previousFrame.FrameTime);

            return Quaternion.Slerp(previousFrame.TransformFrames[frameIndex].rotation, nextFrame.TransformFrames[frameIndex].rotation, delta);
        }

        public static Vector3 InterpolateVelocity(List<ObjectFrame> frames)
        {
            ObjectFrame previousFrame = new ObjectFrame();
            ObjectFrame nextFrame = new ObjectFrame();

            foreach (var frame in frames)
            {
                previousFrame = nextFrame;
                nextFrame = frame;

                if (frame.frameTime > GetPlaybackTime())
                {
                    break;
                }
            }

            float delta = GetFrameDelta(nextFrame.frameTime, previousFrame.frameTime);

            return Vector3.Lerp(previousFrame.rigidbodyVelocity, nextFrame.rigidbodyVelocity, delta);
        }

        public static void InterpolateTransform(Transform transform, ObjectFrame previous, ObjectFrame next)
        {
            Vector3 previousPosition;
            Vector3 nextPosition;

            Quaternion previousRotation;
            Quaternion nextRotation;

            GetSteppedPosition(previous, next, out previousPosition, out nextPosition);
            GetSteppedRotation(previous, next, out previousRotation, out nextRotation);

            float delta = GetFrameDelta(next.frameTime, previous.frameTime);

            transform.position = Vector3.Lerp(previousPosition, nextPosition, delta);
            transform.rotation = Quaternion.Slerp(previousRotation, nextRotation, delta);
        }

        public static void GetSteppedPosition(ObjectFrame previousFrame, ObjectFrame nextFrame, out Vector3 previous, out Vector3 next)
        {
            previous = previousFrame.position;
            next = nextFrame.position;
        }

        public static void GetSteppedRotation(ObjectFrame previousFrame, ObjectFrame nextFrame, out Quaternion previous, out Quaternion next)
        {
            previous = previousFrame.rotation;
            next = nextFrame.rotation;
        }
    }
}
