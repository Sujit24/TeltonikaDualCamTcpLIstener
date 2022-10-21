using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeltonikaDualCamTcpNetBase.BaseClasses;

namespace TeltonikaDualCamTcpNetBase.VideoProcessor
{
    public class TcpState
    {
        public byte[] DataBytes { get; set; }
        public int PacketCount { get; set; }
        public string IMEI { get; set; }
        public FileType FileType { get; set; }

    }
}
