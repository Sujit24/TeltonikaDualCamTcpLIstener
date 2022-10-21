using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeltonikaDualCamTcpNetBase.BaseClasses;
using TeltonikaDualCamTcpNetBase.Codec;

namespace TeltonikaDualCamTcpNetBase.VideoProcessor
{
    public class TeltonikaVideoProcessor:IVideoProcessor
    {
        private Queue<TcpState> _tcpStateQueue=new Queue<TcpState>();
        private object _tcpStateQueueLock = new object();
        private bool _stopped;
        private byte[] _rawFileData;
        public bool Stopped { get { return _stopped; } set { _stopped = value; } }

        public void Add(TcpState tcpState)
        {
            Utils.LogToFile(3, "[INFO]", "Calling Add()");
            lock (_tcpStateQueueLock)
            {
                _tcpStateQueue.Enqueue(tcpState);
            }
        }
        public void RunVideoProcessing()
        {
            Utils.LogToFile(3, "[INFO]", "Calling RunVideoProcessing()");
            Stopped = false;
            while (!Stopped)
            {
                try
                {
                    List<TcpState> tcpStates = new List<TcpState>();
                    lock (_tcpStateQueueLock)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (_tcpStateQueue.Count > 0)
                            {
                                tcpStates.Add(_tcpStateQueue.Dequeue());
                            }
                            else
                                break;
                        }                       
                    }

                    if (tcpStates.Count>0)
                    {
                        foreach (var tcp in tcpStates)
                        {
                            try
                            {
                                List<FileDataTransferCommand> fileDataTransferCommands = DecodeDataTransferCommand(tcp.DataBytes,tcp.PacketCount);
                                Utils.LogToFile(3, "[INFO]", string.Format("Total Media File Length:{0} Bytes", _rawFileData.Length));
                                string filePath = CreateMediaFile(tcp.FileType.ToString(), tcp.IMEI);
                                if (tcp.FileType.ToString().Contains("mp4"))
                                {
                                    filePath=ConvertToMp4File(filePath,tcp.IMEI);
                                  
                                }

                            }
                            catch (Exception ex)
                            {
                                Utils.LogToFile(1, "[EXCEPTION]", string.Format("Exception In RunVideoProcessing():{0}", ex.Message.ToString()));
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Utils.LogToFile(1, "[EXCEPTION]", string.Format("Exception In RunVideoProcessing():{0}", ex.Message.ToString()));
                }            
            }
        }

        private List<FileDataTransferCommand> DecodeDataTransferCommand(byte[]rawdata,int packetCount)
        {
            Utils.LogToFile(3, "[INFO]", "Calling DecodeDataTransferCommand()");
            List<FileDataTransferCommand> fileDataTransferCommands = new List<FileDataTransferCommand>();
            _rawFileData = new byte[0];
            ReverseBinaryReader reverseBinaryReader = new ReverseBinaryReader(new MemoryStream(rawdata));
            Utils.LogToFile(3, "[INFO]", "Processing Media File");
            for (int i=1; i<=packetCount; i++)
            {
                try
                {
                    short commandId = reverseBinaryReader.ReadInt16();
                    short dataLength = reverseBinaryReader.ReadInt16();
                    byte[] fileData = reverseBinaryReader.ReadBytes(Convert.ToInt32(dataLength - 2));
                    _rawFileData = _rawFileData.Concat(fileData).ToArray();
                    short crc = reverseBinaryReader.ReadInt16();
                    fileDataTransferCommands.Add(new FileDataTransferCommand { CommandId=commandId,DataLength=dataLength,FileData=fileData,DataCrc=crc});
                }
                catch (Exception ex)
                {
                    Utils.LogToFile(1, "[EXCEPTION]",string.Format("Exception In DecodeDataTransferCommand():{0}", ex.Message.ToString()));
                    throw;
                }
            }

            return fileDataTransferCommands;
        }

        private string CreateMediaFile(string fileType,string imei)
        {
            Utils.LogToFile(3, "[INFO]", "Calling CreateMediaFile()");
            string saveFilePath=String.Empty;
            if (_rawFileData.Length!=0)
            {
                try
                {
                    string filePath = ConfigurationManager.AppSettings["SaveFilePath"];
                    saveFilePath= $"{filePath}\\{imei}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.{fileType}";
                    if (!File.Exists(saveFilePath))
                    {
                        Utils.LogToFile(3, "[INFO]", "Creating The Media File");
                        File.WriteAllBytes(saveFilePath, _rawFileData);
                        Utils.LogToFile(3, "[INFO]", "Media File Created Successfully");
                        Utils.LogToFile(3, "[Info]", string.Format("Write File Path:{0}", saveFilePath));
                    }
                }
                catch (Exception ex)
                {
                    Utils.LogToFile(1, "[EXCEPTION]", string.Format("Exception In CreateMediaFile():{0}", ex.Message.ToString()));
                }
            }
            return saveFilePath;
        }


       private string ConvertToMp4File(string filePath,string imei)
        {
            Utils.LogToFile(3, "[INFO]", "Calling ConvertToMp4File()");
            string saveFilePath = "";
            try
            {
                string ffmpegFileLocation = ConfigurationManager.AppSettings["ffmgeplocation"];
                saveFilePath = $"{ConfigurationManager.AppSettings["SaveFilePath"]}\\Decompressed_{imei}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.mp4";
                Process proc = new Process();  
                proc.StartInfo.FileName=ffmpegFileLocation;
                proc.StartInfo.Arguments= $"-r {20} -i {filePath} -ss 00:00:0.9 -c:a copy -c:v libx264 {saveFilePath}";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                Utils.LogToFile(3, "[INFO]", "Decompressing Video File");
                if (!proc.Start())
                {
                    Utils.LogToFile(3, "[INFO]", "Can't Convert To MP4 File");
                    return "";
                }
                Utils.LogToFile(3, "[INFO]", "Video File Decompressed Successfully");
                Utils.LogToFile(3, "[Info]", string.Format("Decompressed Video File Path:{0}", saveFilePath));
                
                proc.Close();
             
            }
            catch (Exception ex)
            {
                Utils.LogToFile(1, "[EXCEPTION]", string.Format("Exception In ConvertToMp4File():{0}", ex.Message.ToString()));
            }
            return saveFilePath;
        }

    }
}
