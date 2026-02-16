namespace NEP.MonoDirector
{
    public static class Settings
    {
        public static class Camera
        {
            public static float cameraSpeedSlow;
            public static float cameraSpeedFast;
            public static float cameraSmoothRotationDelta;
            public static bool useHeadCamera;
            public static bool handheldLockXAxis;
            public static bool handheldLockYAxis;
            public static bool handheldLockZAxis;
            public static bool handheldKinematicOnRelease;
        }

        public static class World
        {
            public static int delay = 2;
            public static bool useMicrophone = false;
            public static bool micPlayback = false;
            public static float playbackRate = 1f;
            public static float fps = 60f;
            public static bool ignoreSlomo = false;
            public static bool temporalScaling = false;
            public static bool recordActors = true;
        }

        public static class Debug
        {
            public static bool debugEnabled = true;
            public static bool useKeys = true;
        }
    }
}
