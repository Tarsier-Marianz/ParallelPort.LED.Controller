using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ParallelPort.LED.Controller {
    public partial class ParallelForm : Form {
        private string LED_NAME_SUFFIX = "ledControl";
        private int PORT_ADDRESS = 888;
        private int data = 0;
        private int tagInt = 0;
        private int []danceData = new int[]{24,36,66,129,0,129,195,231,255};
        private bool isDance = false;
        public ParallelForm() {
            InitializeComponent();
            InitPorts();
            InitRegAddresses();
        }

        private void InitRegAddresses() {
            cboxRegAdd.Items.Add("0x3BCh"); // 956 Decimal 
            cboxRegAdd.Items.Add("0x378h"); // 888 Decimal
            cboxRegAdd.Items.Add("0x278h"); // 632 Decimal
            cboxRegAdd.SelectedIndex = 1;
        }

        private void InitPorts() {
            List<string> ports = PortControl.GetParallelPorts();
            if(ports != null) {
                cboxPorts.Items.Clear();
                foreach(string p in ports) {
                    cboxPorts.Items.Add(p);
                }
                cboxPorts.SelectedIndex = 0;
            }
        }

        private void leds_Click(object sender, EventArgs e) {
            LEDControl led = sender as LEDControl;
            led.On = !led.On;
            if (led.Name.Equals(LED_NAME_SUFFIX + 9)) {
                led.IsBlink = isDance = led.On;
                ToggleOnOff(false);
                timerDance.Enabled = led.On;
                //data = 0;
            }else if (led.Name.Equals(LED_NAME_SUFFIX + 10)) {
                data = (led.On) ? 255 : 0;
                ToggleOnOff(led.On);
            } else {
                tagInt = Convert.ToInt32(led.Tag);
                int power = (int)Math.Pow(2, tagInt);
                if (led.On) {
                    data += power;
                } else {
                    data -= power;
                }
            }
            PortControl.Output(PORT_ADDRESS, data);
            ShowData(data);
        }

        private void cboxRegAdd_SelectedIndexChanged(object sender, EventArgs e) {
            if (cboxRegAdd.SelectedIndex == 0) {
                PORT_ADDRESS = 956;
            }else if (cboxRegAdd.SelectedIndex == 1) {
                PORT_ADDRESS = 888;
            } else if (cboxRegAdd.SelectedIndex == 2) {
                PORT_ADDRESS = 632;
            } else { }
        }

        private void ToggleOnOff(bool state) {
            foreach (Control c in this.Controls) {
                if (c is LEDControl) {
                    LEDControl led = c as LEDControl;
                    if (isDance) {
                        ledControl9.IsBlink = timerDance.Enabled = false;
                    }
                    if (!led.Tag.Equals("10")) {
                        led.On = state;
                    }
                }
            }
        }

        private void ShowData(int data) {
            txtDec.Text = Convert.ToString(data, 10).PadLeft(3, '0');
            txtHex.Text = Convert.ToString(data, 16).PadLeft(2, '0').ToUpper();
            txtBin.Text = Convert.ToString(data, 2).PadLeft(8, '0');
        }

        private int danceIndex = 0;
        private void timerDance_Tick(object sender, EventArgs e) {
            if (danceIndex < 9) {
                data = danceData[danceIndex];
                PortControl.Output(PORT_ADDRESS, data);
                danceIndex += 1;
            } else {
                data = 0;
                danceIndex = 0;
            }
            ShowData(data);
        }

    }
}
