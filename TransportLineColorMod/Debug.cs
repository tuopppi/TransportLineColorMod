using ColossalFramework.Plugins;

namespace TransportLineColorMod
{
    class Debug
    {
        public static void Message(string message)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, message);
        }
    }
}