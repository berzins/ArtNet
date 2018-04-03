using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtNet.ArtPacket;

namespace ArtNet {
    /// <summary>
    /// use this interface to subsribe necesary ArtPackets at ArtDispatcher class
    /// </summary>
    public interface ArtListener {
        /// <summary>
        /// run on art listtener when subscribed packet has arived.
        /// </summary>
        void Action(Packet p);
    }
}
