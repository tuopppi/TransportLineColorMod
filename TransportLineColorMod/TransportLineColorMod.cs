using ICities;

namespace TransportLineColorMod
{
    public class TransportLineColorMod : IUserMod
    {
        public const string MOD_NAME = "TransportLineColorMod";
        public const string MOD_DESCRIPTION = "Assigns random color for all created transport lines";

        public string Name
        {
            get { return MOD_NAME; }
        }

        public string Description
        {
            get { return MOD_DESCRIPTION; }
        }
    }
}