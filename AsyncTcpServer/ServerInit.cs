using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace AsyncTcpServer
{

    public class ServerInit
    {
        private TcpClient client;
        private BinaryReader br;
        private BinaryWriter bw;
        BackgroundWorker connectWork = new BackgroundWorker();
        private int serverPort = 0;
        private TcpListener myListener;

        #region 输出log

        public delegate void DGlog(string msg);

        public DGlog dgLog = null;

        #endregion

        #region 增加客户端到列表

        public delegate void DgAddClient(ServerInfo msg);

        public DgAddClient dgAddClient = null;

        #endregion

        #region 移除客户端

        public delegate void DgRomoveCilent(ServerInfo msg);

        public DgRomoveCilent dgRomoveCilent = null;

        #endregion

        /// <summary>
        /// 保存连接的所有用户
        /// </summary>
        private List<ServerInfo> InfoList = new List<ServerInfo>();
        /// <summary>
        /// 是否正常退出所有接收线程
        /// </summary>
        bool isExit = false;
        public ServerInit()
        {
            try
            {
                serverPort = int.Parse(ConfigurationSettings.AppSettings["port"]);
            }
            catch (Exception e)
            {
                //Log.WriteError(e.Message);
                throw;
            }
        }

        public void Start()
        {
            myListener = new TcpListener(IPAddress.Any, serverPort);
            myListener.Start();
            dgLog("服务器启动成功..");
            Thread myThread = new Thread(ListenClientConnect){IsBackground = true};
            myThread.Start();
        }

        public void Close()
        {
            //ServerInfo _info=new ServerInfo(null);
            //_info.Content=new ServerContentInfo(){Order = SocketOrder.ServerLogout};
            //AsyncSendToAllClient(_info);
            
        }
        /// <summary>
        /// 监听客户端请求
        /// </summary>
        private void ListenClientConnect()
        {
            TcpClient newClient = null;
            while (true)
            {
                ListenClientDelegate d = new ListenClientDelegate(ListenClient);
                IAsyncResult result = d.BeginInvoke(out newClient, null, null);
                //使用轮询方式来判断异步操作是否完成
                while (result.IsCompleted == false)
                {
                    if (isExit)
                        break;
                    Thread.Sleep(100);
                }
                //获取Begin 方法的返回值和所有输入/输出参数
                d.EndInvoke(out newClient, result);
                if (newClient != null)
                {
                    //每接受一个客户端连接，就创建一个对应的线程循环接收该客户端发来的信息
                    ServerInfo info = new ServerInfo(newClient);
                    Thread threadReceive = new Thread(ReceiveData){IsBackground = true};
                    threadReceive.Start(info);
                    dgAddClient(info);
                    dgLog(string.Format("[{0}]已连接", newClient.Client.RemoteEndPoint));
                    //dgLog(string.Format("当前连接客户端数：{0}", InfoList.Count));
                }
                else
                {
                    break;
                }
            }
        }

        private void ReceiveData(object userState)
        {
            ServerInfo info = (ServerInfo)userState;
            TcpClient client = info.client;
            while (!isExit)
            {
                string receiveString = null;
                ReceiveMessageDelegate d = new ReceiveMessageDelegate(ReceiveMessage);
                IAsyncResult result = d.BeginInvoke(info, out receiveString, null, null);
                //使用轮询方式来判断异步操作是否完成
                while (!result.IsCompleted)
                {
                    if (isExit)
                        break;
                    Thread.Sleep(100);
                }
                //获取Begin方法的返回值和所有输入/输出参数
                d.EndInvoke(out receiveString, result);
                if (receiveString == null)
                {
                    if (!isExit)
                    {
                        dgLog(string.Format("与{0} {1}失去联系，已终止接收该用户信息", info.Content.ClientName, client.Client.RemoteEndPoint));
                        dgRomoveCilent(info);
                    }
                    break;
                }
                //dgLog(string.Format("来自[{0}]:{1}", info.Content.ClientName, info.Content.DataInfo));
                string[] splitString = receiveString.Split(',');
                switch (info.Content.Order)
                {
                    case SocketOrder.Login:
                        dgLog(string.Format("客户端{0}上线:{1}", info.Content.ClientName, info.client.Client.RemoteEndPoint));
                        dgAddClient(info);
                        InfoList.Add(info);
                        break;
                    case SocketOrder.Logout:
                        dgLog(string.Format("客户端{0}下线:{1}", info.Content.ClientName, info.client.Client.RemoteEndPoint));
                        dgRomoveCilent(info);
                        InfoList.Remove(info);
                        break;

                    case SocketOrder.StartReg:
                        AsyncSendToClient(info);
                        break;
                    case SocketOrder.RegSuccess:
                        AsyncSendToClient(info);
                        break;
                    case SocketOrder.StartCckq:
                        AsyncSendToClient(info);
                        break;
                    case SocketOrder.CckqSuccess:
                        AsyncSendToClient(info);
                        break;

                    case SocketOrder.StartTckq:
                        AsyncSendToClient(info);
                        break;

                    case SocketOrder.TckqSuccess:
                        AsyncSendToClient(info);
                        break;

                    case SocketOrder.StartDown:
                        AsyncSendToClient(info);
                        break;
                    case SocketOrder.DownSuccess:
                        AsyncSendToClient(info);
                        break;
                    default:
                        AsyncSendToClient(info);
                        dgLog("未知命令[已转发]：" + info.Content.Order);
                        break;
                }
            }
        }

        /// <summary>
        /// 异步发送信息给所有客户
        /// </summary>
        /// <param name="info"></param>
        /// <param name="message"></param>
        private void AsyncSendToAllClient(ServerInfo info)
        {
            if (info==null)
            {
                return;
            }
            foreach (var serverInfo in InfoList)
            {
                serverInfo.Content.Order = info.Content.Order;
                serverInfo.Content.DataInfo = info.Content.DataInfo;
                AsyncSendToClient(serverInfo);
            }
        }

        /// <summary>
        /// 异步发送message给user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        private void AsyncSendToClient(ServerInfo info)
        {

            foreach (ServerInfo target in InfoList)
            {
                if (target.Content.ClientName == info.Content.AimName)
                {
                    SendToClientDelegate d = new SendToClientDelegate(SendToClient);
                    //ServerInfo s=new ServerInfo(target.client);
                    //s.Content = info.Content;
                    target.Content.DataInfo = info.Content.DataInfo;
                    target.Content.Order = info.Content.Order;
                    IAsyncResult result = d.BeginInvoke(target, null, null);
                    while (result.IsCompleted == false)
                    {
                        if (isExit)
                            break;
                        Thread.Sleep(100);
                    }
                    d.EndInvoke(result);
                    break;
                }
            }





        }
        private delegate void SendToClientDelegate(ServerInfo user);
        /// <summary>
        /// 发送message给user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        private void SendToClient(ServerInfo user)
        {
            try
            {
                //将字符串写入网络流，此方法会自动附加字符串长度前缀
                user.bw.Write(JsonConvert.SerializeObject(user.Content));
                user.bw.Flush();
                dgLog(string.Format("向[{0}]发送：{1}", user.Content.ClientName, "2&" + (int)user.Content.Order + "&" + user.Content.DataInfo));
            }
            catch
            {
                dgLog(string.Format("向[{0}]发送信息失败", user.Content.AimName));
            }
        }

        delegate void ReceiveMessageDelegate(ServerInfo user, out string receiveMessage);

        /// <summary>
        /// 接收客户端发来的信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="receiveMessage"></param>
        private void ReceiveMessage(ServerInfo info, out string receiveMessage)
        {
            try
            {
                receiveMessage = info.br.ReadString();
                info.Content = Derializer(receiveMessage);
            }
            catch (Exception ex)
            {
                //AddItemToListBox(ex.Message);
                receiveMessage = null;
            }
        }

        private delegate void ListenClientDelegate(out TcpClient client);
        /// <summary>
        /// 接受挂起的客户端连接请求
        /// </summary>
        /// <param name="newClient"></param>
        private void ListenClient(out TcpClient newClient)
        {
            try
            {
                newClient = myListener.AcceptTcpClient();
            }
            catch
            {
                newClient = null;
            }
        }

        public static ServerContentInfo Derializer(string info)
        {

            return JsonConvert.DeserializeObject<ServerContentInfo>(info); ;

        }
    }

}
