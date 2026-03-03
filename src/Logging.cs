using MelonLoader;

namespace NEP.MonoDirector.Core
{
    public static class Logging
    {
        private static MelonLogger.Instance m_LoggerInstance;

        internal static void Initialize()
        {
            m_LoggerInstance = new MelonLogger.Instance("MonoDirector");
        }

        public static void Msg(string message)
        {
            m_LoggerInstance.Msg(message);
        }

        public static void Warn(string message)
        {
            m_LoggerInstance.Warning(message);
        }

        public static void Error(string message)
        {
            m_LoggerInstance.Error(message);
        }

        public static void MsgDebug(string message)
        {
#if DEBUG
            Msg(message);
#endif
        }

        public static void WarnDebug(string message)
        {
#if DEBUG
            Warn(message);
#endif
        }

        public static void ErrorDebug(string message)
        {
#if DEBUG
            Error(message);
#endif
        }
    }
}
