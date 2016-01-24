using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Comm;
using Newtonsoft.Json;

namespace AsyncTcpClient
{

    public class ClientBussiness
    {
        SocketHelper.TcpClients client;
        string ip = string.Empty;
        string port = string.Empty;
        private SocketInfo info = new SocketInfo();

        public delegate void DGLog(string msg);
        public DGLog DgLog = null;

        public delegate void DGMessage(string msg);
        public DGMessage DgMessage = null;

        private bool IsConnect = false;
        public ClientBussiness()
        {
            //客户端如何处理异常等信息参照服务端
            SocketHelper.pushSockets = new SocketHelper.PushSockets(Rec);//注册推送器
            client = new SocketHelper.TcpClients();
            info.AimName = ConfigurationManager.AppSettings["S_A_Name"];
            info.ClientName = ConfigurationManager.AppSettings["C_Name"];
            ip = ConfigurationManager.AppSettings["ServicesIP"];
            port = ConfigurationManager.AppSettings["ServicesPort"];
        }

        public void Start()
        {
            int connectTime = 0;
            try
            {
                connectTime = int.Parse(ConfigurationManager.AppSettings["ServicesPort"]);
            }
            catch (Exception ex)
            {
                connectTime = 5000;
            }
            Thread tStart=new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    if (!IsConnect)
                    {
                        try
                        {
                            DgLog("开始连接服务器...");
                            client.InitSocket(ip, int.Parse(port));
                            client.Start();
                        }
                        catch (Exception ex)
                        {
                            DgLog(string.Format("连接失败!原因：{0},将在{1}毫秒后重试...", ex.Message,connectTime));
                        }
                    }
                    Thread.Sleep(connectTime);
                }
                
            })){IsBackground = true};
            tStart.Start();
        }

        public void Stop()
        {
            try
            {
                client.Stop();
            }
            catch (Exception ex)
            {
                
            }
            
        }

        public void Send(string msg)
        {
            info.DataInfo = msg;
            info.Order=SocketOrder.RegSuccess;
            client.SendData(Serializer(info));
        }

        private void Rec(SocketHelper.Sockets sks)
        {
            if (sks.ex != null)
            {
                //在这里判断ErrorCode  可以自由扩展
                switch (sks.ErrorCode)
                {
                    case SocketHelper.Sockets.ErrorCodes.objectNull:
                        break;
                    case SocketHelper.Sockets.ErrorCodes.ConnectError:
                        break;
                    case SocketHelper.Sockets.ErrorCodes.ConnectSuccess:
                        DgLog("连接成功...");
                        IsConnect = true;
                        info.Order=SocketOrder.Login;
                        client.SendData(Serializer(info));
                        break;
                    case SocketHelper.Sockets.ErrorCodes.TrySendData:
                        break;
                    default:
                        break;
                }
                if (sks.ErrorCode != SocketHelper.Sockets.ErrorCodes.ConnectSuccess)
                {
                    DgLog(string.Format("{0}", sks.ex));
                    IsConnect = false;
                }
                

            }
            else
            {
                byte[] buffer = new byte[sks.Offset];
                Array.Copy(sks.RecBuffer, buffer, sks.Offset);
                string str = Encoding.UTF8.GetString(buffer);
                if (str == "ServerOff")
                {
                    DgLog("服务端主动关闭,即将尝试重新连接...");
                    IsConnect = false;
                }
                else
                {
                    DgMessage(string.Format("服务端{0}发来消息：{1}", sks.Ip, str));
                    //txtThis.Text += "\r\n";
                    //txtThis.Text += string.Format("服务端{0}发来消息：{1}", sks.Ip, str);
                }
            }

        }


        #region 序列化

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string Serializer(SocketInfo info)
        {
            return JsonConvert.SerializeObject(info);
        }

        public static SocketInfo Derializer(string info)
        {
            return JsonConvert.DeserializeObject<SocketInfo>(info);

        }

        #endregion
    }
}
