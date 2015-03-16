using ICities;

namespace TransportLineColorMod
{
    public class TransportLineColorMod : IUserMod
    {
        public string Name
        {
            get { 
                return "Transport Line Color Mod"; 
            }
        }

        public string Description
        {
            get
            {
                return "Assigns random color for all created transport lines";
            }
        }
    }
}