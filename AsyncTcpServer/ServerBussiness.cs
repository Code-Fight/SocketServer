using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Comm;
using Newtonsoft.Json;

namespace AsyncTcpServer
{
    public class ServerBussiness
    {
        SocketHelper.TcpServer server;
        private List<ServerInfo> ClientList = new List<ServerInfo>(); 
        public delegate void DGAddClient(string msg);

        public DGAddClient DgAddClient = null;

        public delegate void DGRemoveClient(string msg);

        public DGRemoveClient DgRemoveClient = null;

        public delegate void DGLog(string msg);

        public DGLog DgLog = null; 

        public ServerBussiness()
        {
            SocketHelper.pushSockets = new SocketHelper.PushSockets(Rec);
        }

        public void Start()
        {
            try
            {
                SocketHelper.pushSockets = new SocketHelper.PushSockets(Rec);
                //防止二次开启引发异常
                if (server == null)
                {
                    server = new SocketHelper.TcpServer();
                    server.InitSocket(IPAddress.Any, int.Parse(ConfigurationManager.AppSettings["port"]));
                    server.Start();
                    WriteLog("服务端启动成功.!");
                }

            }
            catch (Exception ex)
            {
                WriteLog(string.Format("启动失败!原因：{0}", ex.Message));

            }
        }

        public void Close()
        {
            server.Stop();
        }

        private void Rec(SocketHelper.Sockets sks)
        {
            IPEndPoint _ip = new IPEndPoint(IPAddress.Any, 60000);
                if (sks.ex != null)
                {
                    //在此处理异常信息
                    WriteLog(string.Format("客户端出现异常:{0}.!", sks.ex.Message));
                    //ServerInfo _info = new ServerInfo() { Ip = sks.Ip };
                    ClientRemove(sks.Ip);
                    //labClientCount.Text = (cmbClient.Items.Count).ToString();

                }
                else
                {
                    if (sks.NewClientFlag)
                    {
                        WriteLog(string.Format("新客户端:{0}连接成功.!", sks.Ip));
                        //(sks.Ip.ToString());
                        //labClientCount.Text = (cmbClient.Items.Count).ToString();
                    }
                    else
                    {
                        byte[] buffer = new byte[sks.Offset];
                        Array.Copy(sks.RecBuffer, buffer, sks.Offset);
                        string str = string.Empty;
                        if (sks.Offset == 0)
                        {
                            str = sks.Ip.ToString()+" 客户端下线";
                            WriteLog(str);
                            ClientRemove(sks.Ip);
                            //labClientCount.Text = (cmbClient.Items.Count).ToString();
                        }
                        else
                        {
                            try
                            {
                                str = Encoding.UTF8.GetString(buffer);
                                SocketInfo _info = new SocketInfo();
                                _info = Derializer(str);
                                if (_info.Order==SocketOrder.Login)
                                {
                                    if (!CheckClientIsHaving(_info.ClientName,out _ip))
                                    {
                                        ClientAdd(new ServerInfo()
                                        {
                                            AimName = _info.AimName,
                                            ClientName = _info.ClientName,
                                            Ip = sks.Ip
                                        });
                                    }
                                    else
                                    {
                                        SendToClient(sks.Ip, "服务器存在相同名称客户端，请重命名.");
                                    }
                                    
                                }
                                else
                                {
                                    if (CheckClientIsHaving(_info.AimName, out _ip))
                                    {
                                        WriteLog(string.Format("{0}向{1}发送：{2}", _info.ClientName, _info.AimName, _info.DataInfo));
                                        SendToClient(_ip, _info.DataInfo);
                                    }
                                    else
                                    {
                                        WriteLog("没找到目标服务器,转发失败,已反馈到源服务器");
                                        SendToClient(sks.Ip, "发送失败,目标服务器可能已经离线..");
                                    }
                                    
                                    
                                }

                               
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                           
                        }

                    }
                }
            
        }

        private void SendToAll()
        {
            server.SendToAll("服务端消息群发:" + Guid.NewGuid().ToString());
        }

        private void SendToClient(IPEndPoint client, string msg)
        {
            server.SendToClient(client, msg);
        }


        private bool CheckClientIsHaving(string ClientName,out IPEndPoint Ip)
        {
            foreach (var serverInfo in ClientList.Where(serverInfo => serverInfo.ClientName==ClientName))
            {
                Ip = serverInfo.Ip;
                return true;
            }
            Ip = null;
            return false;
        }

        private void ClientAdd(ServerInfo _info)
        {
            ClientList.Add(_info);
            DgAddClient(_info.ClientName + " " + _info.Ip.ToString());
        }

        private void ClientRemove(IPEndPoint Ip)
        {

            if (Ip == null)
            {
                return;
            }
            foreach (var client in ClientList)
            {
                if (Equals(client.Ip, Ip))
                {
                    ClientList.Remove(client);
                    DgRemoveClient(client.ClientName+" "+Ip.ToString());
                    break;
                }
                
            }
        }

        private void WriteLog(string msg)
        {
            if (msg.Length>0)
            {
                DgLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg);
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
