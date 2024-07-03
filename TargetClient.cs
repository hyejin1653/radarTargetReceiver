using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Mda;
using Google.Protobuf;
using System.IO;

namespace Target_Receiver
{

    public class TargetClient
    {
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        private string LogFilePath;

        public async Task Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

           
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                LogFilePath = Path.Combine("/logs");
            }
            else
            {
                LogFilePath = Path.Combine("C:\\logs");
            }

            await Task.WhenAll(
                StartRadarTarget()
          );
        }

        private float SetFloat(string pValue)
        {
            return !string.IsNullOrEmpty(pValue.ToString()) ? Convert.ToSingle(pValue.ToString()) : -99999;
        }

        private void LogWriter(string oriData)
        {
            if (!Directory.Exists(LogFilePath))
            {
                Directory.CreateDirectory(LogFilePath);
            }

            using (FileStream fs = new FileStream(Path.Combine(LogFilePath, DateTime.Now.ToString("yyyyMMdd_HH") + ".log"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.Position = fs.Length;
                    string msg = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] " + oriData;
                    sw.Write(msg);
                }
            }
        }

        public async Task StartRadarTarget()
        {
            UdpClient client = new UdpClient();

            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, ConfigFile.Port);
            client.Client.Bind(localEP);

            

            while (!cancellationToken.IsCancellationRequested)
            {
                UdpReceiveResult data = await client.ReceiveAsync();
                byte[] buff = data.Buffer;

                string payload = Encoding.UTF8.GetString(buff);
                
                Console.Write(payload);
                this.LogWriter(payload);

                string[] datas = payload.Split(new string[] { ",","*" }, StringSplitOptions.RemoveEmptyEntries);

                VTS_RadarTaget_Signal radar = new VTS_RadarTaget_Signal();
                radar.Radarid = datas[1].ToString();
                radar.Status = Convert.ToInt32(datas[2]);
                radar.Latitude = Convert.ToDouble(datas[3]);
                radar.Longtitude = Convert.ToDouble(datas[5]);
                radar.Cog = Convert.ToSingle(datas[7]);
                radar.Sog = Convert.ToSingle(datas[8]);
                radar.Timestamp = datas[9].ToString();
                radar.RecvDatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
                radar.Sensorid = ConfigFile.Port.ToString();


                //Console.Write(radar);
                //Console.WriteLine(",");

                int size = radar.CalculateSize();
                byte[] buffer = new byte[size];

                CodedOutputStream outputStream = new CodedOutputStream(buffer);
                radar.WriteTo(outputStream);

                if (!string.IsNullOrEmpty(ConfigFile.KAFKA))
                {
                    KafkaProducer.Send($"VTS_RadarTaget_Signal", buffer);
                }

         

                

                

            }
        }
    }

}
