using ColossalFramework.Plugins;
using System;

namespace TransportLineColorMod
{
    public static class Debug
    {
        private const string MSG_PREFIX = "[" + TransportLineColorMod.MOD_NAME + "] ";

        public static void Message(object message, params object[] args)
        {
            #if (DEBUG)
            DebugOutputPanel.AddMessage(
                PluginManager.MessageType.Message,
                String.Format(MSG_PREFIX + message.ToString(), args)
            );
            #endif
        }
    }
}