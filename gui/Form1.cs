using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ds3231_check
{
    public partial class Form1 : Form
    {
        string rxString;
        DateTime realTime;
        DateTime clockTime;
        double diff = 0;
        double diffamount = 0;
        int amountgot = 0;
        public Form1()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                portBox.Items.Add(port);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                button1.Text = "Connect";
                button2.Enabled = false;
                amountgot = 0;
            }
            else
            {
                try
                {
                    serialPort1.PortName = portBox.Text;
                    serialPort1.Open();
                    diffamount = 0;
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show("Error whilst trying to connect to " + portBox.Text + ": Unauthorized Access");
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show("Error whilst trying to connect to " + portBox.Text + ": Port does not exist");
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("Error whilst trying to connect to " + portBox.Text + ": Port is in use");
                }
                if (serialPort1.IsOpen)
                {
                    button1.Text = "Disconnect";
                    button2.Enabled = true;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            realTime = DateTime.Now;
            if (amountgot > 1)
            {
                clockTime = Convert.ToDateTime(rxString);
                textBox2.Text = clockTime.ToString("yyyy/MM/dd HH:mm:ss");
                diff = (diff * diffamount + clockTime.Subtract(realTime).TotalSeconds) / (diffamount + 1);
                diffamount++;
                textBox3.Text = diff.ToString();
            }
            else
            {
                textBox2.Text = "Waiting for input";
                textBox3.Text = "Waiting for input";
            }
            textBox1.Text = realTime.ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            amountgot++;
            rxString = serialPort1.ReadExisting();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Enabled)
            {
                int now = DateTime.Now.Second;
                while (now == DateTime.Now.Second) { }
                DateTime newnow = DateTime.Now.AddSeconds(1);
                byte year = (byte)(newnow.Year - 2000);
                byte month = (byte)(newnow.Month);
                byte day = (byte)(newnow.Day);

                byte hour = (byte)(newnow.Hour);
                byte minute = (byte)(newnow.Minute);
                byte second = (byte)(newnow.Second);
                byte[] date = new byte[] { year, month, day, hour, minute, second };
                serialPort1.Write(date, 0, 6);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/jkapi/ds3231_check");
        }
    }
}
