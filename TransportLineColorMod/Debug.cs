using ColossalFramework.Plugins;
using System;

namespace TransportLineColorMod
{
    public static class Debug
    {
        private const string MSG_PREFIX = "[" + TransportLineColorMod.MOD_NAME + "] ";

        public static void DebugMessage(object message, params object[] args)
        {
            #if (DEBUG)
            Message(message, args);
            #endif
        }

        public static void Message(object message, params object[] args)
        {
            output(PluginManager.MessageType.Message, message, args);
        }

        public static void Warning(object message, params object[] args)
        {
            output(PluginManager.MessageType.Warning, message, args);
        }

        public static void Error(object message, params object[] args)
        {
            output(PluginManager.MessageType.Error, message, args);
        }

        private static void output(PluginManager.MessageType type, object message, params object[] args)
        {
            DebugOutputPanel.AddMessage(type, String.Format(MSG_PREFIX + message.ToString(), args));
        }
    }
}