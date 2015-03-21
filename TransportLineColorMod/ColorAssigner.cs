using ColossalFramework;
using ColossalFramework.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TransportLineColorMod
{
    public class ColorAssigner
    {
        public const string CUSTOM_COLORS_FILE_NAME = "colors.txt";

        // Generated using http://phrogz.net/css/distinct-colors.html
        // Hue 0°-360° (22°), Saturation 100%, Value 60-100 (23%)
        public const string DEFAULT_COLORS = "#d40000, #bed400, #00d42a, #0094d4, #5500d4, #d4006a, #990000, #8a9900, #00991f, #006b99, #3d0099, #99004d, #d44e00, #71d400, #00d478, #0047d4, #a200d4, #d4001c, #993800, #529900, #009957, #003399, #750099, #990014, #d49b00, #23d400, #00d4c6, #0700d4, #d400b7, #997000, #199900, #00998f, #050099, #990085";

        private readonly TransportManager m_transportManager;
        private readonly string m_colorMapConfigPath;
        private List<UnityEngine.Color> m_colors;

        /// <summary>
        /// ColorAssigner will listen <see cref="TransportLineObserver.NewTransportLine" /> event and 
        /// assign new color for each created TransportLine
        /// </summary>
        /// <param name="observer"></param>
        public ColorAssigner(TransportLineObserver observer)
        {
            m_colorMapConfigPath = new[] { 
                DataLocation.modsPath, 
                TransportLineColorMod.MOD_NAME, 
                CUSTOM_COLORS_FILE_NAME 
            }.Aggregate(Path.Combine);

            if (!TryReadCustomColorMap(m_colorMapConfigPath))
            {
                Debug.DebugMessage("Using default colors");
                m_colors = ParseColorMap(DEFAULT_COLORS);
            }

            m_transportManager = Singleton<TransportManager>.instance;
            observer.NewTransportLine += setLineColor;
        }

        /// <summary>
        /// Reads and parses custom color file.
        /// Returns true if file was exists and was successfully parsed and contains at least one valid value.
        /// Returns false if file does not exist or was unable to read it.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool TryReadCustomColorMap(string path)
        {
            if (FileUtils.Exists(path))
            {
                try
                {
                    using (var reader = File.OpenText(path))
                    {
                        m_colors = ParseColorMap(reader.ReadToEnd());
                        return m_colors.Count > 0;
                    }
                }
                catch (Exception e)
                {
                    // Custom color map file was empty, had invalid syntax, etc...
                    Debug.Error("Tried to read custom color map but encountered following error: {0}", e.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
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

                Debug.DebugMessage("Color assigned for lineID {0}", lineID);
            }
        }

        /// <summary>
        /// Returns list of valid colors parsed from passed string. If no valid values were found an empty list is returned.
        /// </summary>
        /// <param name="hexValueList">Comma separated list of hex color values. example <see cref="DEFAULT_COLORS"/></param>
        public static List<UnityEngine.Color> ParseColorMap(string hexValueList)
        {
            Debug.DebugMessage("Parsing color map: {0}", hexValueList);

            var rgb_colors = new List<UnityEngine.Color>();
            var invalidValues = new List<string>();

            string newLineCharsRemoved = hexValueList.Replace('\n', ' ').Replace('\r', ' ');
            foreach (var item in newLineCharsRemoved.Split(','))
            {
                uint colorNum;
                bool isHexNumber = UInt32.TryParse(item.Trim('#', ' '), System.Globalization.NumberStyles.HexNumber, null, out colorNum);
                if (isHexNumber && colorNum <= 0xffffff)
                {
                    // convert numeric value to RGB color
                    rgb_colors.Add(new UnityEngine.Color(
                        ((colorNum & 0xff0000) >> 0x10) / 255f,
                        ((colorNum & 0x00ff00) >> 0x08) / 255f,
                        ((colorNum & 0x0000ff) >> 0x00) / 255f));
                }
                else
                {
                    invalidValues.Add(item);
                }
            }

            if (invalidValues.Count > 0)
            {
                var emptyStringsReplaced = invalidValues.Select(v => v.IsNullOrWhiteSpace() ? "<empty string>" : v).ToArray();
                Debug.Warning("Color list contains invalid value(s): {0}", String.Join(",", emptyStringsReplaced));
            }

            return rgb_colors;
        }
    }
}