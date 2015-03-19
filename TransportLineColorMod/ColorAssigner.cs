using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TransportLineColorMod
{
    public class ColorAssigner
    {
        private readonly TransportManager m_transportManager;

        // Generated using http://phrogz.net/css/distinct-colors.html
        // Hue 0°-360° (13°), Saturation 90%-100% (8%), Value 80-100 (2%)
        // TODO: Add support for loading custom color maps?
        private const string COLOR_MAP = "#cc1414, #45cc14, #144fcc, #cc1430, #04a2d6, #0fdb04, #ad16db, #4e04e0, #eb3705, #175aeb, #eb0559, #e2f518, #fa7a19, #c519fa, #05c1ff, #cc0404, #1ecc14, #1427cc, #cc0422, #7804d6, #16db37, #d816db, #8616e0, #eb4517, #054deb, #eb1737, #53f518, #fa6f05, #f619fa, #1a62ff, #cc2f04, #14cc33, #2a14cc, #d18904, #d6047f, #16dbb7, #db16b4, #e0168c, #eb6805, #172deb, #eb0527, #11f505, #faab19, #fa19cd, #2205ff, #cc3c14, #14cc5b, #5214cc, #04d153, #db0404, #16d5db, #db165e, #d4e617, #eb7317, #3017eb, #f09d05, #18f56d, #fadc19, #fa052a, #5805ff, #cc6414, #14cc83, #7914cc, #1c04d1, #db1616, #167fdb, #db0453, #a8e617, #eba117, #5e17eb, #a9f005, #19bef5, #19fa3e, #fa193b, #8f05ff, #cc5b04, #14ccaa, #a114cc, #4804d1, #db4116, #0477db, #db0425, #7be617, #ebce17, #8305eb, #76f005, #9218f5, #05fa63, #ffa805, #ff0597, #cc8c14, #14c6cc, #c914cc, #c5d604, #db3304, #0448db, #db1634, #4ee617, #22eb17, #b917eb, #05f092, #f51999, #19fad1, #baff1a, #ff1a6e, #ccb414, #149ecc, #cc14a7, #97d604, #db6b16, #1654db, #e09305, #05e65b, #17eb3b, #e717eb, #188bf0, #fa1919, #0553fa, #88ff1a, #ff0561, #bdcc14, #1476cc, #cc1480, #6ad604, #db6204, #162adb, #16e064, #17e693, #17ebc4, #eb17c0, #0582f0, #fa0505, #192ffa, #48ff05, #95cc14, #046fcc, #cc1458, #3cd604, #db9616, #2d16db, #16aee0, #eb1717, #17e4eb, #eb058b, #2005f0, #fa3a05, #3319fa, #1affa3, #6dcc14, #0443cc, #cc044d, #04d682, #dbc116, #5816db, #1e04e0, #eb0505, #05b1eb, #eb1765, #5305f0, #fa4a19, #6419fa, #1af7ff";
        private readonly List<UnityEngine.Color> m_colors;

        /// <summary>
        /// ColorAssigner will listen <see cref="TransportLineObserver.NewTransportLine" /> event and 
        /// assign new color for each created TransportLine
        /// </summary>
        /// <param name="observer"></param>
        public ColorAssigner(TransportLineObserver observer)
        {
            m_colors = parseColorMap(COLOR_MAP);
            m_transportManager = Singleton<TransportManager>.instance;
            observer.NewTransportLine += setLineColor;
        }

        private void setLineColor(ushort lineID, TransportLine transportLine)
        {
            // Temporary flag seems to mean the line drawing tool (initial "dot")
            if (!transportLine.FlagSet(TransportLine.Flags.CustomColor) &&
                !transportLine.FlagSet(TransportLine.Flags.Temporary))
            {
                var results = m_transportManager.SetLineColor(lineID, m_colors[lineID % m_colors.Count]);
                    
                // Evaluate results; without looping results enumerator lines don't seem to change their color.
                while (results.MoveNext()) ;

                Debug.Message("Color assigned for lineID {0}", lineID);
            }
        }

        private List<UnityEngine.Color> parseColorMap(string colors)
        {
            var values = from hex in colors.Split(',')
                         select UInt32.Parse(hex.Trim('#', ' '), System.Globalization.NumberStyles.HexNumber) into hexValue
                         select new UnityEngine.Color(
                            ((hexValue & 0xff0000)>> 0x10) / 255f,
                            ((hexValue & 0xff00)  >> 8)    / 255f,
                            (hexValue & 0xff)              / 255f);

            return values.ToList();
        }
    }
}