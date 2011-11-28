using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PIEHidDotNet;


namespace PIHidDotName_Csharp_Sample
{
    public partial class Form1 : Form, PIEDataHandler, PIEErrorHandler
    {
        PIEDevice[] devices;
        
        int[] cbotodevice=null; //for each item in the CboDevice list maps this index to the device index.  Max devices =100 
        byte[] wData = null; //write data buffer
        int selecteddevice=-1; //set to the index of CboDevice
        
        //for thread-safe way to call a Windows Forms control
        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void SetTextCallback(string text);
        Control c;
        //end thread-safe


        public Form1()
        {
            InitializeComponent();
            textBoxA1.ReadOnly = true;
            textBoxA25.ReadOnly = true;
            textBoxD11.ReadOnly = true;
            textBoxD125.ReadOnly = true;
            textBoxD21.ReadOnly = true;
            textBoxD225.ReadOnly = true;
        }

        private void BtnEnumerate_Click(object sender, EventArgs e)
        {
            CboDevices.Items.Clear();
            cbotodevice = new int[128]; //128=max # of devices
            //enumerate and setupinterfaces for all devices
            devices = PIEHidDotNet.PIEDevice.EnumeratePIE();
            if (devices.Length == 0)
            {
                toolStripStatusLabel1.Text = "No Devices Found";
            }
            else
            {
                //System.Media.SystemSounds.Beep.Play(); 
                int cbocount=0; //keeps track of how many valid devices were added to the CboDevice box
                for (int i = 0; i < devices.Length; i++)
                {
                    //information about device
                    //PID = devices[i].Pid);
                    //HID Usage = devices[i].HidUsage);
                    //HID Usage Page = devices[i].HidUsagePage);
                    //HID Version = devices[i].Version);
                    if (devices[i].HidUsagePage == 0xc)
                    {
                        switch (devices[i].Pid)
                        {
                            case 217:
                                CboDevices.Items.Add("ReDAC IO ("+devices[i].Pid+"), ID: " + i);
                                cbotodevice[cbocount] = i;
                                cbocount++;
                                break;
                            default:
                                CboDevices.Items.Add("Unknown Device (" + devices[i].Pid + "), ID: " + i);
                                cbotodevice[cbocount] = i;
                                cbocount++;
                                break;
                        }
                        devices[i].SetupInterface();
                    }
                }
            }
            if (CboDevices.Items.Count > 0)
            {
                CboDevices.SelectedIndex = 0;
                selecteddevice = cbotodevice[CboDevices.SelectedIndex];
                wData = new byte[devices[selecteddevice].WriteLength];//go ahead and setup for write
            }
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //closeinterfaces on all devices
            for (int i = 0; i < CboDevices.Items.Count; i++)
            {
                devices[cbotodevice[i]].CloseInterface();
            }
            System.Environment.Exit(0);
        }

        private void CboDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            selecteddevice = cbotodevice[CboDevices.SelectedIndex];
            wData = new byte[devices[selecteddevice].WriteLength];//size write array 
        }
        private void BtnCallback_Click(object sender, EventArgs e)
        {
            //setup callback if there are devices found for each device found

            if (CboDevices.SelectedIndex != -1)
            {
                for (int i = 0; i < CboDevices.Items.Count; i++)
                {
                    //use the cbotodevice array which contains the mapping of the devices in the CboDevices to the actual device IDs
                    devices[cbotodevice[i]].SetErrorCallback(this);
                    devices[cbotodevice[i]].SetDataCallback(this, DataCallbackFilterType.callOnChangedData);
                }
            }
        }
        //data callback    
        public void HandlePIEHidData(Byte[] data, PIEDevice sourceDevice)
        {
            //check the sourceDevice and make sure it is the same device as selected in CboDevice   
            if (sourceDevice == devices[selecteddevice])
            {
                byte[] rdata = null;
                while (0 == sourceDevice.ReadData(ref rdata)) //do this so don't ever miss any data
                {
                    //read the unit ID
                    c = this.LblUnitID;
                    this.SetText(rdata[sourceDevice.ReadLength - 1].ToString());
                    //write raw data to listbox1
                    String output = "Callback: " + this.devices[selecteddevice].Pid + ", ID: " + selecteddevice.ToString() + ", data=";
                    for (int i = 0; i < sourceDevice.ReadLength; i++)
                    {
                        output = output + rdata[i].ToString() + " ";
                    }
                    this.SetListBox(output);
                    //write Analog data to the individual textboxes
                    c = this.textBoxA2;
                    this.SetText(rdata[1].ToString());
                    c = this.textBoxA3;
                    this.SetText(rdata[2].ToString());
                    c = this.textBoxA4;
                    this.SetText(rdata[3].ToString());
                    c = this.textBoxA5;
                    this.SetText(rdata[4].ToString());
                    c = this.textBoxA6;
                    this.SetText(rdata[5].ToString());
                    c = this.textBoxA7;
                    this.SetText(rdata[6].ToString());
                    c = this.textBoxA8;
                    this.SetText(rdata[7].ToString());
                    c = this.textBoxA9;
                    this.SetText(rdata[8].ToString());
                    c = this.textBoxA10;
                    this.SetText(rdata[9].ToString());
                    c = this.textBoxA11;
                    this.SetText(rdata[10].ToString());
                    c = this.textBoxA12;
                    this.SetText(rdata[11].ToString());
                    c = this.textBoxA13;
                    this.SetText(rdata[12].ToString());
                    c = this.textBoxA14;
                    this.SetText(rdata[13].ToString());
                    c = this.textBoxA15;
                    this.SetText(rdata[14].ToString());
                    c = this.textBoxA16;
                    this.SetText(rdata[15].ToString());
                    c = this.textBoxA17;
                    this.SetText(rdata[16].ToString());
                    c = this.textBoxA18;
                    this.SetText(rdata[17].ToString());
                    c = this.textBoxA19;
                    this.SetText(rdata[18].ToString());
                    c = this.textBoxA20;
                    this.SetText(rdata[19].ToString());
                    c = this.textBoxA21;
                    this.SetText(rdata[20].ToString());
                    c = this.textBoxA22;
                    this.SetText(rdata[21].ToString());
                    c = this.textBoxA23;
                    this.SetText(rdata[22].ToString());
                    c = this.textBoxA24;
                    this.SetText(rdata[23].ToString()); //pin 24

                    //write digital 1 data to the individual textboxes
                    c = this.textBoxD12;
                    this.SetText((rdata[24] & 1).ToString());
                    c = this.textBoxD13;
                    this.SetText(((rdata[24] & 2) >> 1).ToString());
                    c = this.textBoxD14;
                    this.SetText(((rdata[24] & 4) >> 2).ToString());
                    c = this.textBoxD15;
                    this.SetText(((rdata[24] & 8) >> 3).ToString());
                    c = this.textBoxD16;
                    this.SetText(((rdata[24] & 16) >> 4).ToString());
                    c = this.textBoxD17;
                    this.SetText(((rdata[24] & 32) >> 5).ToString());
                    c = this.textBoxD18;
                    this.SetText(((rdata[24] & 64) >> 6).ToString());
                    c = this.textBoxD19;
                    this.SetText(((rdata[24] & 128) >> 7).ToString());
                    c = this.textBoxD110;
                    this.SetText((rdata[25] & 1).ToString());
                    c = this.textBoxD111;
                    this.SetText(((rdata[25] & 2) >> 1).ToString());
                    c = this.textBoxD112;
                    this.SetText(((rdata[25] & 4) >> 2).ToString());
                    c = this.textBoxD113;
                    this.SetText(((rdata[25] & 8) >> 3).ToString());
                    c = this.textBoxD114;
                    this.SetText(((rdata[25] & 16) >> 4).ToString());
                    c = this.textBoxD115;
                    this.SetText(((rdata[25] & 32) >> 5).ToString());
                    c = this.textBoxD116;
                    this.SetText(((rdata[25] & 64) >> 6).ToString());
                    c = this.textBoxD117;
                    this.SetText(((rdata[25] & 128) >> 7).ToString());
                    c = this.textBoxD118;
                    this.SetText((rdata[26] & 1).ToString());
                    c = this.textBoxD119;
                    this.SetText(((rdata[26] & 2) >> 1).ToString());
                    c = this.textBoxD120;
                    this.SetText(((rdata[26] & 4) >> 2).ToString());
                    c = this.textBoxD121;
                    this.SetText(((rdata[26] & 8) >> 3).ToString());
                    c = this.textBoxD122;
                    this.SetText(((rdata[26] & 16) >> 4).ToString());
                    c = this.textBoxD123;
                    this.SetText(((rdata[26] & 32) >> 5).ToString());
                    c = this.textBoxD124;
                    this.SetText(((rdata[26] & 64) >> 6).ToString()); //pin 24

                    //write digital 2 data to the individual textboxes
                    c = this.textBoxD22;
                    this.SetText((rdata[27] & 1).ToString());
                    c = this.textBoxD23;
                    this.SetText(((rdata[27] & 2) >> 1).ToString());
                    c = this.textBoxD24;
                    this.SetText(((rdata[27] & 4) >> 2).ToString());
                    c = this.textBoxD25;
                    this.SetText(((rdata[27] & 8) >> 3).ToString());
                    c = this.textBoxD26;
                    this.SetText(((rdata[27] & 16) >> 4).ToString());
                    c = this.textBoxD27;
                    this.SetText(((rdata[27] & 32) >> 5).ToString());
                    c = this.textBoxD28;
                    this.SetText(((rdata[27] & 64) >> 6).ToString());
                    c = this.textBoxD29;
                    this.SetText(((rdata[27] & 128) >> 7).ToString());
                    c = this.textBoxD210;
                    this.SetText((rdata[28] & 1).ToString());
                    c = this.textBoxD211;
                    this.SetText(((rdata[28] & 2) >> 1).ToString());
                    c = this.textBoxD212;
                    this.SetText(((rdata[28] & 4) >> 2).ToString());
                    c = this.textBoxD213;
                    this.SetText(((rdata[28] & 8) >> 3).ToString());
                    c = this.textBoxD214;
                    this.SetText(((rdata[28] & 16) >> 4).ToString());
                    c = this.textBoxD215;
                    this.SetText(((rdata[28] & 32) >> 5).ToString());
                    c = this.textBoxD216;
                    this.SetText(((rdata[28] & 64) >> 6).ToString());
                    c = this.textBoxD217;
                    this.SetText(((rdata[28] & 128) >> 7).ToString());
                    c = this.textBoxD218;
                    this.SetText((rdata[29] & 1).ToString());
                    c = this.textBoxD219;
                    this.SetText(((rdata[29] & 2) >> 1).ToString());
                    c = this.textBoxD220;
                    this.SetText(((rdata[29] & 4) >> 2).ToString());
                    c = this.textBoxD221;
                    this.SetText(((rdata[29] & 8) >> 3).ToString());
                    c = this.textBoxD222;
                    this.SetText(((rdata[29] & 16) >> 4).ToString());
                    c = this.textBoxD223;
                    this.SetText(((rdata[29] & 32) >> 5).ToString());
                    c = this.textBoxD224;
                    this.SetText(((rdata[29] & 64) >> 6).ToString()); //pin 24
                }
            }
        }
        //error callback
        public void HandlePIEHidError(Int32 error, PIEDevice sourceDevice)
        {
            this.SetToolStrip("Error: " + error.ToString());
            if (error == 307)
                sourceDevice.CloseInterface();
        }
        //for threadsafe setting of Windows Forms control
        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.c.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.c.Text = text;
            }
        }
        //for threadsafe setting of Windows Forms control
        private void SetListBox(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.listBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetListBox);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listBox1.Items.Add(text);
                this.listBox1.SelectedIndex = this.listBox1.Items.Count - 1;
            }
        }
        //for threadsafe setting of Windows Forms control
        private void SetToolStrip(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.statusStrip1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetToolStrip);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.toolStripStatusLabel1.Text = text;
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
           
        }
    
        private void BtnGreen_Click(object sender, EventArgs e)
        {
            //write to currently selected device - turn on the green LED
            if (CboDevices.SelectedIndex != -1)
            {
                
                for (int j = 0; j < devices[selecteddevice].WriteLength ; j++) 
                {
                    wData[j] = 0;
                }
                wData[1] = 134; 
                wData[2] = 16;
               
                int result = devices[selecteddevice].WriteData(wData);
                if (result != 0)
                {
                    toolStripStatusLabel1.Text = "Write Fail: " + result;
                }
                else
                {
                    toolStripStatusLabel1.Text = "Write Success - Green LED";
                }
            }
        }

        private void BtnBlink_Click(object sender, EventArgs e)
        {
            //write to currently selected device - turn on the Red LED
            if (CboDevices.SelectedIndex != -1) //do nothing if not enumerated
            {

                for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                {
                    wData[j] = 0;
                }
                wData[1] = 134;
                wData[2] = 32;

                int result = devices[selecteddevice].WriteData(wData);
                if (result != 0)
                {
                    toolStripStatusLabel1.Text = "Write Fail: " + result;
                }
                else
                {
                    toolStripStatusLabel1.Text = "Write Success-Red LED";
                }
            }
        }

        private void BtnFBlink_Click(object sender, EventArgs e)
        {
            //write to currently selected device - turn on the Red LED
            if (CboDevices.SelectedIndex != -1) //do nothing if not enumerated
            {

                for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                {
                    wData[j] = 0;
                }
                wData[1] = 134;
                wData[2] = 48;

                int result = devices[selecteddevice].WriteData(wData);
                if (result != 0)
                {
                    toolStripStatusLabel1.Text = "Write Fail: " + result;
                }
                else
                {
                    toolStripStatusLabel1.Text = "Write Success-Fast Blink Green LED";
                }
            }
        }

        private void BtnNoLEDS_Click(object sender, EventArgs e)
        {
            //write to currently selected device in CboDevice - turn off all LEDs
            if (CboDevices.SelectedIndex != -1)
            {

                for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
                {
                    wData[j] = 0;
                }
                wData[1] = 134;

                int result = devices[selecteddevice].WriteData(wData);
                if (result != 0)
                {
                    toolStripStatusLabel1.Text = "Write Fail: " + result;
                }
                else
                {
                    toolStripStatusLabel1.Text = "Write Success - No LEDs";
                }
            }
        }

        private void BtnUnitID_Click(object sender, EventArgs e)
        {
            //write Unit ID given in the TxtSetUnitID box
            for (int j = 0; j < devices[selecteddevice].WriteLength; j++) 
            {
                wData[j] = 0;
            }

            wData[1] = 137;
            wData[2] = 137;
            wData[7] =(byte)(Convert.ToInt16(TxtSetUnitID.Text)); //how to get to a numeric val
            wData[8] = 16;
            int result = devices[selecteddevice].WriteData(wData);
            if (result != 0)
            {
                toolStripStatusLabel1.Text = "Write Fail: " + result;
            }
            else
            {
                toolStripStatusLabel1.Text = "Write Success - Write Unit ID";
            }
        }
        
         

        private void BtnWriteDigital_Click(object sender, EventArgs e)
        {
            //write to digital outputs
            for (int j = 0; j < devices[selecteddevice].WriteLength; j++)
            {
                wData[j] = 0;
            }

            wData[1] = 147;

            if (checkBox2.Checked == true) wData[2] = (byte)(wData[2] | 1);
            else wData[2]= (byte)(wData[2] & (~1));
            if (checkBox3.Checked == true) wData[2] = (byte)(wData[2] | 2);
            else wData[2] = (byte)(wData[2] & (~2));
            if (checkBox4.Checked == true) wData[2] = (byte)(wData[2] | 4);
            else wData[2] = (byte)(wData[2] & (~4));
            if (checkBox5.Checked == true) wData[2] = (byte)(wData[2] | 8);
            else wData[2] = (byte)(wData[2] & (~8));
            if (checkBox6.Checked == true) wData[2] = (byte)(wData[2] | 16);
            else wData[2] = (byte)(wData[2] & (~16));
            if (checkBox7.Checked == true) wData[2] = (byte)(wData[2] | 32);
            else wData[2] = (byte)(wData[2] & (~32));
            if (checkBox8.Checked == true) wData[2] = (byte)(wData[2] | 64);
            else wData[2] = (byte)(wData[2] & (~64));
            if (checkBox9.Checked == true) wData[2] = (byte)(wData[2] | 128);
            else wData[2] = (byte)(wData[2] & (~128));

            if (checkBox10.Checked == true) wData[3] = (byte)(wData[3] | 1);
            else wData[3] = (byte)(wData[3] & (~1));
            if (checkBox11.Checked == true) wData[3] = (byte)(wData[3] | 2);
            else wData[3] = (byte)(wData[3] & (~2));
            if (checkBox12.Checked == true) wData[3] = (byte)(wData[3] | 4);
            else wData[3] = (byte)(wData[3] & (~4));
            if (checkBox13.Checked == true) wData[3] = (byte)(wData[3] | 8);
            else wData[3] = (byte)(wData[3] & (~8));
            if (checkBox14.Checked == true) wData[3] = (byte)(wData[3] | 16);
            else wData[3] = (byte)(wData[3] & (~16));
            if (checkBox15.Checked == true) wData[3] = (byte)(wData[3] | 32);
            else wData[3] = (byte)(wData[3] & (~32));
            if (checkBox16.Checked == true) wData[3] = (byte)(wData[3] | 64);
            else wData[3] = (byte)(wData[3] & (~64));
            if (checkBox17.Checked == true) wData[3] = (byte)(wData[3] | 128);
            else wData[3] = (byte)(wData[3] & (~128));

            if (checkBox18.Checked == true) wData[4] = (byte)(wData[4] | 1);
            else wData[4] = (byte)(wData[4] & (~1));
            if (checkBox19.Checked == true) wData[4] = (byte)(wData[4] | 2);
            else wData[4] = (byte)(wData[4] & (~2));
            if (checkBox20.Checked == true) wData[4] = (byte)(wData[4] | 4);
            else wData[4] = (byte)(wData[4] & (~4));
            if (checkBox21.Checked == true) wData[4] = (byte)(wData[4] | 8);
            else wData[4] = (byte)(wData[4] & (~8));
            if (checkBox22.Checked == true) wData[4] = (byte)(wData[4] | 16);
            else wData[4] = (byte)(wData[4] & (~16));
            if (checkBox23.Checked == true) wData[4] = (byte)(wData[4] | 32);
            else wData[4] = (byte)(wData[4] & (~32));
            if (checkBox24.Checked == true) wData[4] = (byte)(wData[4] | 64);
            else wData[4] = (byte)(wData[4] & (~64));
            if (checkBox25.Checked == true) wData[4] = (byte)(wData[4] | 128);
            else wData[4] = (byte)(wData[4] & (~128));

            int result = devices[selecteddevice].WriteData(wData);
            if (result != 0)
            {
                toolStripStatusLabel1.Text = "Write Fail: " + result;
            }
            else
            {
                toolStripStatusLabel1.Text = "Write Success - Write Digital Outputs";
            }
        }

        private void BtnSetKey_Click(object sender, EventArgs e)
        {
            //For users of the dongle feature 
            if (CboDevices.SelectedIndex != -1) //do nothing if not enumerated
            {
                //Programming switch must be up (red). Users of this feature
                //may want to add a program switch check.
                //This routine is done once per unit by the developer prior to sale.

                //Pick 4 numbers between 1 and 254.
                int K0 = 7;    //pick any number between 1 and 254, 0 and 255 not allowed
                int K1 = 58;   //pick any number between 1 and 254, 0 and 255 not allowed
                int K2 = 33;   //pick any number between 1 and 254, 0 and 255 not allowed
                int K3 = 243;  //pick any number between 1 and 254, 0 and 255 not allowed
                //Save these numbers, they are needed to check the key!

                //Write these to the device
                wData[0] = 0;
                wData[1] = 205; //set key command
                wData[2] = 0;
                wData[3] = 0;
                wData[4] = (byte)K0;
                wData[5] = (byte)K1;
                wData[6] = (byte)K2;
                wData[7] = (byte)K3;
                wData[8] = 0xDC;

                int result = devices[selecteddevice].WriteData(wData);
                if (result != 0)
                {
                    toolStripStatusLabel1.Text = "Write Fail: " + result;
                }
                else
                {
                    toolStripStatusLabel1.Text = "Write Success-Set Dongle Key";
                }
            }
        }

        private void BtnCheckKey_Click(object sender, EventArgs e)
        {
            //This is done within the developer's application to check for the correct
            //hardware.  The K0-K3 values must be the same as those entered in Set Key.
            if (CboDevices.SelectedIndex != -1)
            {
                LblPassFail.Text = "Pass/Fail";
                //check hardware

                //IMPORTANT turn off the callback if going so data isn't grabbed there, turn it back on later (not done here)
                devices[selecteddevice].SetDataCallback(this, DataCallbackFilterType.callNever);

                //IMPORTANT turn off the callback if going so data isn't grabbed there, turn it back on later (not done here)
                devices[selecteddevice].SetDataCallback(this, DataCallbackFilterType.callNever);

                //randomn numbers
                int N0 = 3;   //pick any number between 1 and 254
                int N1 = 1;   //pick any number between 1 and 254
                int N2 = 4;   //pick any number between 1 and 254
                int N3 = 1;   //pick any number between 1 and 254

                //this is the key from set key
                int K0 = 7;
                int K1 = 58;
                int K2 = 33;
                int K3 = 243;

                //hash and save these for comparison later
                int R0;
                int R1;
                int R2;
                int R3;
                PIEDevice.DongleCheck2(K0, K1, K2, K3, N0, N1, N2, N3, out R0, out R1, out R2, out R3);

                wData[0] = 0;
                wData[1] = 137;
                wData[2] = 137;
                wData[3] = 0;
                wData[4] = (byte)N0;
                wData[5] = (byte)N1;
                wData[6] = (byte)N2;
                wData[7] = (byte)N3;
                wData[8] = 121;

                devices[selecteddevice].ClearBuffer();
                int result = devices[selecteddevice].WriteData(wData);

                if (result != 0)
                {
                    toolStripStatusLabel1.Text = "Write Fail: " + result;
                }
                else
                {
                    toolStripStatusLabel1.Text = "Write Success-Check Dongle Key";
                }

                //after this write the next valid read will give 4 values which are used below for comparison

                byte[] data = null;
                int countout = 0;
                int ret = devices[selecteddevice].BlockingReadData(ref data, 100);

                while ((ret == 0 && data[3] != 0x79) || ret == 304)
                {
                    if (ret == 304)
                    {
                        // Didn't get any data for 100ms, increment the countout extra
                        countout += 100;
                    }
                    countout++;
                    if (countout > 500) //increase this if have to check more than once
                        break;
                    ret = devices[selecteddevice].BlockingReadData(ref data, 100);
                }

                if (ret == 0 && data[3] == 0x79)
                {
                    bool fail = false;
                    if (R0 != data[4]) fail = true;
                    if (R1 != data[5]) fail = true;
                    if (R2 != data[6]) fail = true;
                    if (R3 != data[7]) fail = true;
                    if (fail == false)
                    {
                        LblPassFail.Text = "Pass-Correct Hardware Found";
                        LblPassFail.BackColor = System.Drawing.Color.Chartreuse;
                    }
                    else
                    {
                        LblPassFail.Text = "Fail-Correct Hardward Not Found";
                        LblPassFail.BackColor = System.Drawing.Color.Red;
                    }
                }

            }
        }

        

        
       
       
    }
    
    
}
