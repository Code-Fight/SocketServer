using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Runtime.Serialization.Json;
using Comm;
using Newtonsoft.Json;

namespace AsyncTcpClient
{
    public partial class FormClient : Form
    {
        ClientBussiness client =new ClientBussiness();
        
        public FormClient()
        {
            InitializeComponent();
        }



        private void FormClient_Load(object sender, EventArgs e)
        {
            txt_UserName.Text = ConfigurationManager.AppSettings["C_Name"];
            client.DgLog += WriteMessageLog;
            client.DgMessage += RevMsg;
            client.Start();
            
        }




        
        private void btn_SendeMessage_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择命令");
                return;
            }
            string msg = "2&";
            msg += comboBox1.SelectedItem.ToString()+"&";
            msg += rtf_SendMessage.Text;
            client.Send(msg);
            
            //msg.DataInfo = rtf_SendMessage.Text.Trim();
            //if (lst_OnlineUser.SelectedIndex != -1)
            //{
            //    AsyncSendMessage(msg);
            //    rtf_SendMessage.Clear();
            //}
            //else
            //    MessageBox.Show("请先在[当前在线]中选择一个对话者");
        }


       

        private void FormClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Stop();
        }

       


        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="msg"></param>
        private void WriteMessageLog(string msg)
        {
            statuslist.Invoke(new Action(delegate
            {
                statuslist.Items.Add(msg );
            }));
        }



        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="msg"></param>
        private void RevMsg(string msg)
        {
            infolist.Invoke(new Action(delegate
            {
                infolist.Items.Add(msg );
            }));
        }
        


       
    }


}
