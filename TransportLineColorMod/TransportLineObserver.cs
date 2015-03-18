using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;

namespace TransportLineColorMod
{
    /// <summary>
    /// Notifies its listeners when new transport line is added
    /// </summary>
    public class TransportLineObserver : ThreadingExtensionBase
    {
        private readonly ColorAssigner m_colorAssigner;
        private TransportManager m_transportManager;
        private HashSet<ushort> m_registeredTransportLineIDs;

        /// <summary>
        /// Triggered when creation of new TransportLine is started
        /// </summary>
        public event Action<ushort, TransportLine> NewTransportLine;

        public TransportLineObserver() : base() {
            m_colorAssigner = new ColorAssigner(this);
        }

        public override void OnCreated(IThreading threading)
        {
            m_transportManager = Singleton<TransportManager>.instance;
            m_registeredTransportLineIDs = new HashSet<ushort>();

            base.OnCreated(threading);
        }

        /// <summary>
        /// Monitors TransportLine count and raises <see cref="NewTransportLine"/> event when new TransportLine is created.
        /// </summary>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            // Noticed new TransportLine
            if (SimulationTransportLineCount > RegisteredTransportLineCount)
            {
                ForEachTransportLine((lineID, transportLine) =>
                {
                    if ((transportLine.m_flags & TransportLine.Flags.Created) == TransportLine.Flags.Created)
                    {
                        m_registeredTransportLineIDs.Add(lineID);
                        Debug.Message("Added transport line {0} flags {1}", lineID, transportLine.m_flags);
                        NewTransportLine(lineID, transportLine);
                    }
                });
            }
            // TransportLine was removed
            else if (SimulationTransportLineCount < RegisteredTransportLineCount)
            {
                Debug.Message("Removed transport line");

                m_registeredTransportLineIDs.Clear();

                // Collect all lines that are still present
                ForEachTransportLine((lineID, transportLine) =>
                {
                    if (transportLine.Complete)
                    {
                        m_registeredTransportLineIDs.Add(lineID);
                    }
                });
            }
        }

        /// <summary>
        /// Iterate over all TransportLines calling action function for each line.
        /// </summary>
        /// <param name="action">(lineID:ushort * transportLine:TransportLine) : ()</param>
        private void ForEachTransportLine(Action<ushort, TransportLine> action) {
            var lines = m_transportManager.m_lines;
            for (ushort lineID = 0; lineID < lines.m_size; lineID++)
            {
                action(lineID, lines.m_buffer[lineID]);
            }
        }

        private int SimulationTransportLineCount
        {
            get { return m_transportManager.m_lineCount; }
        }

        private int RegisteredTransportLineCount
        {
            get { return m_registeredTransportLineIDs.Count; }
        }
    }
}
