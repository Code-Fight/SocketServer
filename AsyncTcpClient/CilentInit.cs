using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Newtonsoft.Json;

namespace AsyncTcpClient
{

    public class CilentInit
    {
        //是否正常链接
        private bool IsConnect = false;
        //是否正常退出
        private bool isExit = false;
        private TcpClient client;
        private BinaryReader br;
        private BinaryWriter bw;
        BackgroundWorker connectWork = new BackgroundWorker();
        CilentInfo SendMsg = new CilentInfo();
        CilentInfo RevMsg = new CilentInfo();
        private string ServerIP;
        private int ServerPort = 0;
        
        public delegate void DGlog(string msg);
        public DGlog dgLog = null;

        public delegate void DGthrowRevMsg(string msg);
        public DGthrowRevMsg DgThrowRevMsg = null;
        private Thread tConnect = null;
        private Thread tStart = null;
        /// <summary>
        /// 发送信息状态的数据结构
        /// </summary>
        private struct SendMessageStates
        {
            public SendMessageDelegate d;
            public IAsyncResult result;
        }

        public CilentInit()
        {
            try
            {
                SendMsg.AimName = ConfigurationSettings.AppSettings["S_A_Name"];
                SendMsg.ClientName = ConfigurationSettings.AppSettings["C_Name"];
                ServerIP = ConfigurationSettings.AppSettings["ServicesIP"];
                ServerPort =int.Parse(ConfigurationSettings.AppSettings["ServicesPort"]);

            }
            catch (Exception e)
            {
                //Log.WriteError(e.Message);
                throw;
            }
        }

        
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            //connectWork.DoWork += connectWork_DoWork;
            //connectWork.RunWorkerCompleted += connectWork_RunWorkerCompleted;
            //dgLog("开始连接.");
            //connectWork.RunWorkerAsync();
            //ConnectServer();
            tStart = new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    if (!IsConnect)
                    {
                        dgLog("开始链接...");
                        ConnectStart();
                        Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["ConnectTime"]));
                    }
                    
                }
                
            })) { IsBackground = true };
            tStart.Start();
        }


        private void ConnectStart()
        {
            client = new TcpClient();
            IAsyncResult result = client.BeginConnect(ServerIP, ServerPort, null, null);
            while (!result.IsCompleted)
            {
                Thread.Sleep(100);
            }
            try
            {
                client.EndConnect(result);
                ConnectComplete("success");
                IsConnect = true;

            }
            catch (Exception ex)
            {
                ConnectComplete(ex.Message);
                IsConnect = false;
            }
        }

        private void ConnectComplete(string msg)
        {
            if (msg.Trim() == "success")
            {
                dgLog("连接成功");
                //获取网络流
                NetworkStream networkStream = client.GetStream();
                //将网络流作为二进制读写对象
                br = new BinaryReader(networkStream);
                bw = new BinaryWriter(networkStream);
                SendMsg.Order = SocketOrder.Login;
                SendMsg.DataInfo = "登录";
                AsyncSendMessage(SendMsg);
                Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
                threadReceive.IsBackground = true;
                threadReceive.Start();
            }
            else
            {
                dgLog("连接失败:" + msg);
                dgLog(string.Format("{0}毫秒后，开始重新连接...", ConfigurationManager.AppSettings["ConnectTime"]));

            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMessage(string msg)
        {
            try
            {
                string[] orderStrings = msg.Split('&');
                SendMsg.Order = (SocketOrder)int.Parse(orderStrings[1]);
                SendMsg.DataInfo = orderStrings[2];
                AsyncSendMessage(SendMsg);
                
            }
            catch (Exception ex)
            {
                dgLog("发送失败，未能识别命令：" + msg);
                throw;
            }
            
        }

        /// <summary>
        /// 客户端要下线时 调用
        /// </summary>
        public void Colse()
        {
            if (client != null&&IsConnect)
            {
                SendMsg.Order = SocketOrder.Logout;
                SendMsg.DataInfo = "退出";
                AsyncSendMessage(SendMsg);
                client.Close();
            }
            if (br != null)
                br.Close();
            if (bw != null)
                bw.Close();
        }

        /// <summary>
        /// 链接检测线程
        /// </summary>
        private void ConnectCheck()
        {
            if (tConnect != null) //启动链接监视线程
            {
                tConnect.Abort();
            }
            tConnect = new Thread(new ThreadStart(delegate
            {

                Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["ConnectTime"]));
                if (!IsConnect)
                {

                    Start();
                }

            })) {IsBackground = true};
            tConnect.Start();
        }


        delegate void ReceiveMessageDelegate(out string receiveMessage);

        /// <summary>
        /// 读取服务器发过来的信息
        /// </summary>
        /// <param name="receiveMessage"></param>
        private void receiveMessage(out string receiveMessage)
        {
            receiveMessage = null;
            try
            {
                receiveMessage = br.ReadString();
                RevMsg = Derializer(receiveMessage);
            }
            catch (Exception ex)
            {
                dgLog(ex.Message);
            }
        }

        /// <summary>
        /// 向 rtf 中添加聊天记录
        /// </summary>
        /// <param name="message"></param>
        private void AddTalkMessage(string message)
        {
            if (DgThrowRevMsg != null)
            {
                DgThrowRevMsg(message);
            }
            
        }
        

        /// <summary>
        /// 处理接收的服务器收据
        /// </summary>
        private void ReceiveData()
        {
            string receiveString = null;
            while (!isExit)
            {
                ReceiveMessageDelegate d = new ReceiveMessageDelegate(receiveMessage);
                IAsyncResult result = d.BeginInvoke(out receiveString, null, null);
                //使用轮询方式来盘点异步操作是否完成
                while (!result.IsCompleted)
                {
                    if (isExit)
                        break;
                    Thread.Sleep(100);
                }
                //获取Begin方法的返回值所有输入/输出参数
                d.EndInvoke(out receiveString, result);
                if (receiveString == null)
                {
                    if (!isExit)
                    {
                        dgLog("与服务器失去联系,即将开始重新链接...");
                        IsConnect = false;
                    }

                    break;
                }
                //string[] splitString = receiveString.Split(',');
                //string command = splitString[0].ToLower();
                try
                {
                    RevMsg = Derializer(receiveString);
                    if (RevMsg != null)
                    {

                        string str = "2&";
                        str += (int) RevMsg.Order + "&";
                        str += RevMsg.DataInfo;
                        AddTalkMessage(str);
                    }
                    else
                    {
                        AddTalkMessage("消息解析失败:"+receiveString);
                    }
                }
                catch (Exception ex)
                {
                    AddTalkMessage("消息解析失败:"+receiveString);
                }

            }
            
           
        }

        private void ReceiveDataComplete()
        {
            
        }

        /// <summary>
        /// 异步向服务器发送数据
        /// </summary>
        /// <param name="message"></param>
        private void AsyncSendMessage(CilentInfo message)
        {
            SendMessageDelegate d = new SendMessageDelegate(SendMessage);
            IAsyncResult result = d.BeginInvoke(message, null, null);
            while (!result.IsCompleted)
            {
                if (isExit)
                    return;
                Thread.Sleep(50);
            }
            SendMessageStates states = new SendMessageStates();
            states.d = d;
            states.result = result;
            Thread t = new Thread(FinishAsyncSendMessage);
            t.IsBackground = true;
            t.Start(states);
        }

        /// <summary>
        /// 处理接收的服务端数据
        /// </summary>
        /// <param name="obj"></param>
        private void FinishAsyncSendMessage(object obj)
        {
            SendMessageStates states = (SendMessageStates)obj;
            states.d.EndInvoke(states.result);
        }

        delegate void SendMessageDelegate(CilentInfo message);

        /// <summary>
        /// 向服务端发送数据
        /// </summary>
        /// <param name="message"></param>
        private void SendMessage(CilentInfo message)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                bw.Write(Serializer(message));
                bw.Flush();
            }
            catch
            {
                dgLog("发送失败");
            }
        }


        /// <summary>
        /// 异步方式与服务器进行连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void connectWork_DoWork(object sender, DoWorkEventArgs e)
        {
            client = new TcpClient();
            IAsyncResult result = client.BeginConnect(ServerIP, ServerPort, null, null);
            while (!result.IsCompleted)
            {
                Thread.Sleep(100);
            }
            try
            {
                client.EndConnect(result);
                e.Result = "success";
                IsConnect = true;
                
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
                return;
            }
           
            
        }

        /// <summary>
        /// 异步方式与服务器完成连接操作后的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void connectWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            ConnectCheck();
            
        }


        private void CheckConnect()
        {
            
        }

        #region 序列化

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string Serializer(CilentInfo info)
        {
            return JsonConvert.SerializeObject(info);
        }

        public static CilentInfo Derializer(string info)
        {
            return JsonConvert.DeserializeObject<CilentInfo>(info);

        }

        #endregion


    }
}
