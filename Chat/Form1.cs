using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat
{
    public partial class Form1 : Form
    {
        bool Connected = false;
        IPEndPoint endPoint = null;
        public Form1()
        {
            InitializeComponent();
            endPoint = new IPEndPoint(IPAddress.Parse(txtBox_IP.Text),Convert.ToInt32(txtBox_remotePort.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread threadSend = new Thread(new ThreadStart(SendMessage));
            threadSend.IsBackground = true;
            threadSend.Start();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!this.Connected)
            {
                Thread threadSend = new Thread(new ThreadStart(MessageRecieve));
                threadSend.Start();
                this.Connected = true;
            }
        }
        private void SendMessage()
        {
            if (txtBox_message.Text != "")
            {
                UdpClient client = new UdpClient();
                string message = "[" + txtBox_Name.Text + "] " + txtBox_message.Text;
                byte[] bytes = Encoding.ASCII.GetBytes(message);
                try
                {
                    IPEndPoint remotePoint = new IPEndPoint(IPAddress.Parse(txtBox_IP.Text), Int32.Parse(txtBox_remotePort.Text));
                    client.Send(bytes, bytes.Length, remotePoint);
                    SetTextBox(message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exeption: " + ex.Message);
                }
            }
        }
        private void MessageRecieve()
        {
            UdpClient uClient = new UdpClient(Convert.ToInt32(txtBox_localPort.Text));
            var address = IPAddress.Parse(txtBox_IP.Text);
            uClient.JoinMulticastGroup(address, 10);
            while (true)
            {
                IPEndPoint end_point = null;
                byte[] bytes = uClient.Receive(ref end_point);
                SetTextBox(Encoding.ASCII.GetString(bytes));
            }
        }
        public void SetTextBox(String text)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke(new Action<String>(SetTextBox), text);
            }
            else
                listBox1.Items.Add(text);
        }

    }
}
