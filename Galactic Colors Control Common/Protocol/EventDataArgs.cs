using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galactic_Colors_Control_Common.Protocol
{
    /// <summary>
    /// Hide EventData in EventArgs 
    /// for OnEvent Handler
    /// </summary>
    public class EventDataArgs : EventArgs
    {
        private EventData m_Data;
        public EventDataArgs(EventData _myData)
        {
            m_Data = _myData;
        }

        public EventData Data { get { return m_Data; } }
    }
}
