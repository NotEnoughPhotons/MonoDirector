using MelonLoader;

namespace NEP.MonoDirector.Core
{
    public static class Logging
    {
        private static MelonLogger.Instance m_LoggerInstance;

        internal static void Initialize()
        {
            m_LoggerInstance = new MelonLogger.Instance("MonoDirector", MelonLoader.Logging.ColorARGB.Magenta);
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
    }
}
