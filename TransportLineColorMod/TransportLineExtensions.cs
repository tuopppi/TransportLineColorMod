using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportLineColorMod
{
    public static class TransportLineExtensions
    {
        public static bool FlagSet(this TransportLine transportLine, TransportLine.Flags flag)
        {
            return (transportLine.m_flags & flag) == flag;
        }
    }
}
