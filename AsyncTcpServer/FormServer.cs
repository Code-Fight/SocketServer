using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Comm;

namespace AsyncTcpServer
{
    public partial class FormServer : Form
    {


       ServerBussiness Server=new ServerBussiness();
        public FormServer()
        {
            InitializeComponent();
        }


        private void FormServer_Load(object sender, EventArgs e)
        {
            Server.DgLog += TipAndLog;
            Server.DgAddClient += AddClient;
            Server.DgRemoveClient += RemoveClient;
            Server.Start();
        }

        

        private void TipAndLog(string msg)
        {
            list_log.Invoke(new Action(delegate
            {
                if (list_log.Items.Count>200)
                {
                    list_log.Items.Clear();
                }
                list_log.Items.Insert(0,msg);
            }));
        }

        private void AddClient(string msg)
        {
            list_clientList.Invoke(new Action(delegate
            {
                if (msg.Length>0)
                {
                    list_clientList.Items.Add(msg);
                }
                
            }));
        }

        private void RemoveClient(string msg)
        {
            list_clientList.Invoke(new Action(delegate
            {
                list_clientList.Items.Remove(msg);
            }));
        }

        
        private void FormServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Server.Close();
        }
    }
}
