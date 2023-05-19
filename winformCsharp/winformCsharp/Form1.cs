using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace winformCsharp
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// The .net wrapper around WinSock sockets.
        /// </summary>
        TcpClient _client;

        /// <summary>
        /// Buffer to store incoming messages from the server.
        /// </summary>
        byte[] _buffer = new byte[4096];
        int last_index = 0;
        bool name_send = false;
        public Form1()
        {
            InitializeComponent();
            _client = new TcpClient();
            chatBox.Hide();

           
            // listbox color 
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.DrawItem += listBox1_DrawItem;
        }
       
        // receiving and storing messages
        private void Server_MessageReceived(IAsyncResult ar)
        {
           
            if (ar.IsCompleted)
            {
                // End the stream read
                var bytesIn = _client.GetStream().EndRead(ar);
                if (bytesIn > 0)
                {
                    // Create a string from the received data. For this server 
                    // our data is in the form of a simple string, but it could be
                    // binary data or a JSON object. Payload is your choice.
                    var tmp = new byte[bytesIn];
                    Array.Copy(_buffer, 0, tmp, 0, bytesIn);
                    var str = Encoding.ASCII.GetString(tmp);

                    // Any actions that involve interacting with the UI must be done
                    // on the main thread. This method is being called on a worker
                    // thread so using the form's BeginInvoke() method is vital to
                    // ensure that the action is performed on the main thread.
                    BeginInvoke((Action)(() =>
                    {
                        listBox1.Items.Add(str);                        
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        
                    }));
                }

                // Clear the buffer and start listening again
                Array.Clear(_buffer, 0, _buffer.Length);
                _client.GetStream().BeginRead(_buffer,
                                                0,
                                                _buffer.Length,
                                                Server_MessageReceived,
                                                null);
            }
        }

        // text sent
        // private void send_Click(object sender, EventArgs e)
        private void send_Click(object sender, EventArgs e)
        {
            if (!name_send)
            {
                name_send = true;
                string nameOfClient = nameBox.Text;
                string heshSign = "#";
                string sendingName = heshSign + nameOfClient;
                var name = Encoding.ASCII.GetBytes(sendingName);
                _client.GetStream().Write(name, 0, name.Length);
                
            }
            else
            {
                // Encode the message and send it out to the server.
                var msg = Encoding.ASCII.GetBytes(inputBox.Text);
                _client.GetStream().Write(msg, 0, msg.Length);
                last_index = listBox1.Items.Count;
                // Clear the text box and set it's focus
                inputBox.Text = "";
                inputBox.Focus();
            }
            
           

        }

      

       
        // ip address done 
        private void ipAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Enter)
            {
                port.Focus();
            }
        }
        // port done 
        private void port_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Enter)
            {

                nameBox.Focus();
            }
        }
        private void nameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Enter)
            {


                signIn.PerformClick();
            }
        }

        // going to sign in and receive text
        private void signIn_Click(object sender, EventArgs e)
        {
            string ip_address = ipAddress.Text;
            int port_address = Int32.Parse(port.Text);
            _client.Connect(ip_address, port_address);
            // send name to the server
           
            
            Array.Clear(_buffer, 0, _buffer.Length);
            _client.GetStream().BeginRead(_buffer,
                                            0,
                                            _buffer.Length,
                                            Server_MessageReceived,
                                            null);
        

        // Connect to the remote server. The IP address and port # could be
        // picked up from a settings file.
        chatBox.Show();
            inputBox.Focus();
            send.PerformClick();
        }

  

      
        // input box enter 
        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Enter)
            {
                send.PerformClick();
            }
        }


        // mouse hover effect
        private void send_MouseEnter(object sender, EventArgs e)
        {
            this.send.BackColor = System.Drawing.Color.White;
            this.send.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(199)))), ((int)(((byte)(157)))));

        }
        private void send_MouseLeave(object sender, EventArgs e)
        {
            this.send.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(199)))), ((int)(((byte)(157)))));
            this.send.ForeColor = System.Drawing.Color.White;
        }
        private void signIn_MouseEnter_1(object sender, EventArgs e)
        {
            signIn.BackColor = System.Drawing.Color.White;
            signIn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(199)))), ((int)(((byte)(157)))));
        }
        private void signIn_MouseLeave_1(object sender, EventArgs e)
        {
            signIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(199)))), ((int)(((byte)(157)))));
            signIn.ForeColor = System.Drawing.Color.White;
        }


        // listbox color thing 
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
                e.DrawBackground();
                Brush myBrush = Brushes.Black;
            ListBox lb = (ListBox)sender;
           
            if (e.Index == 0)
            {
                myBrush = Brushes.Red;
            }
            else if (e.Index==last_index)
            {
                myBrush = Brushes.Purple;
            }
              
                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                    e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);               
                e.DrawFocusRectangle();            
        }

       
    }
    
}
