using ColossalFramework;
using System;

namespace TransportLineColorMod
{
    /// <summary>
    /// ColorAssigner will listen <see cref="TransportLineObserver.NewTransportLine" /> event and 
    /// assign new color for each created TransportLine
    /// </summary>
    public class ColorAssigner
    {
        private readonly TransportManager m_transportManager;

        /// <summary>
        /// ColorAssigner will listen <see cref="TransportLineObserver.NewTransportLine" /> event and 
        /// assign new color for each created TransportLine
        /// </summary>
        /// <param name="observer"></param>
        public ColorAssigner(TransportLineObserver observer)
        {
            m_transportManager = Singleton<TransportManager>.instance;
            observer.NewTransportLine += setLineColor;
        }

        private void setLineColor(ushort lineID, TransportLine transportLine)
        {
            if (!hasCustomColor(transportLine))
            {
                var before = transportLine.m_flags.ToString();

                var results = m_transportManager.SetLineColor(lineID, generateLineColor(lineID));

                // Evaluate results; without looping results enumerator lines don't seem to change their color.
                while (results.MoveNext())
                {
                    if (results.Current)
                    {
                        Debug.Message("Custom color assigned for lineID {0}: {1} -> {2}", lineID, before, transportLine.m_flags);
                    }
                    else
                    {
                        Debug.Message("Failed to assign color for lineID {0}: {1} -> {2}", lineID, before, transportLine.m_flags);
                    }
                }
            }
        }

        private UnityEngine.Color generateLineColor(int lineID)
        {
            const float saturation = 0.95f;
            const float value = 0.85f;

            int hi = (lineID / 60) % 6;
            float f = (float)(lineID / 60f - Math.Floor(lineID / 60f));

            float p = value * (1 - saturation);
            float q = value * (1 - f * saturation);
            float t = value * (1 - (1 - f) * saturation);

            if (hi == 0)
                return new UnityEngine.Color(value, t, p);
            else if (hi == 1)
                return new UnityEngine.Color(q, value, p);
            else if (hi == 2)
                return new UnityEngine.Color(p, value, t);
            else if (hi == 3)
                return new UnityEngine.Color(p, q, value);
            else if (hi == 4)
                return new UnityEngine.Color(t, p, value);
            else
                return new UnityEngine.Color(value, p, q);
        }

        private bool hasCustomColor(TransportLine line)
        {
            return (line.m_flags & TransportLine.Flags.CustomColor) == TransportLine.Flags.CustomColor;
        }
    }
}