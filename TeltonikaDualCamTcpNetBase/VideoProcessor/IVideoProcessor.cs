using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeltonikaDualCamTcpNetBase.VideoProcessor
{
    public interface IVideoProcessor
    {
       void Add(TcpState tcpState);
       void RunVideoProcessing();
       bool Stopped { get; set; }
    }
}
