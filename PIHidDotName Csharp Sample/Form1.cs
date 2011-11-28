// PIE Example code -- last revision 11/27/2011

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

      int[] iCboToDevice = null;     // for each item in the CboDevice list maps this index to the device index.  Max devices =100 
      byte[] wData = null;           // write data buffer
      int selectedDevice = -1;       // set to the index of CboDevice

      // For a thread-safe way to call a Windows Forms control, this delegate enables asynchronous calls for setting
      //   the text property on a TextBox control.
      delegate void SetTextCallback(string text);

      Control c;
      // end thread-safe

      // ctor
      public Form1()
      {
         InitializeComponent();
         textBoxA1.ReadOnly = true;
         textBoxA25.ReadOnly = true;
         textBoxD11.ReadOnly = true;
         textBoxD125.ReadOnly = true;
         textBoxD21.ReadOnly = true;
         textBoxD225.ReadOnly = true;
      } // end ctor

      // TODO: We'll need this functionality
      private void BtnEnumerate_Click(object sender, EventArgs e)
      {
         // Clear out the combo-box
         CboDevices.Items.Clear();

         iCboToDevice = new int[128];   // 128 = max # of devices

         // Enumerate and set-up interfaces for all devices
         devices = PIEHidDotNet.PIEDevice.EnumeratePIE();

         // If no devices are enumerated, then none were found
         if (devices.Length == 0)
         {
            toolStripStatusLabel1.Text = "No Devices Found";
         }

         else
         {
            // System.Media.SystemSounds.Beep.Play(); 

            // Keep track of how many valid devices were added to the CboDevice box
            int iCboCount = 0;  
  
            for (int i = 0; i < devices.Length; i++)
            {
               // Information about device
               //PID = devices[i].Pid);
               //HID Usage = devices[i].HidUsage);
               //HID Usage Page = devices[i].HidUsagePage);
               //HID Version = devices[i].Version);
               if (devices[i].HidUsagePage == 0xc) // 0b1100
               {
                  switch (devices[i].Pid)
                  {
                     case 217:
                        CboDevices.Items.Add("ReDAC IO (" + devices[i].Pid + "), ID: " + i);
                        iCboToDevice[iCboCount] = i;
                        iCboCount++;
                        break;

                     default:
                        CboDevices.Items.Add("Unknown Device (" + devices[i].Pid + "), ID: " + i);
                        iCboToDevice[iCboCount] = i;
                        iCboCount++;
                        break;
                  }
                  devices[i].SetupInterface();
               }
            }
         }

         if (CboDevices.Items.Count > 0)
         {
            CboDevices.SelectedIndex = 0;
            selectedDevice = iCboToDevice[CboDevices.SelectedIndex];
            wData = new byte[devices[selectedDevice].WriteLength];   // go ahead and setup for write
         }
      } // end btnEnumerate_Click() event handler

      // TODO: May not need this, if we're not running a form
      private void Form1_Load(object sender, EventArgs e)
      {
         toolStripStatusLabel1.Text = "";
      }

      // TODO: Same here--not needed unless a form is being run
      private void Form1_FormClosed(object sender, FormClosedEventArgs e)
      {
         // Close interfaces on all devices
         for (int i = 0; i < CboDevices.Items.Count; i++)
         {
            devices[iCboToDevice[i]].CloseInterface();
         }
         System.Environment.Exit(0);
      }

      // TODO: I don't think we'll need this CBO, as we're not likely to have more than one device
      private void CboDevices_SelectedIndexChanged(object sender, EventArgs e)
      {
         selectedDevice = iCboToDevice[CboDevices.SelectedIndex];

         // Byte array to hold data stream
         wData = new byte[devices[selectedDevice].WriteLength];
      }

      // TODO: Probably will only need to set up one callback, since we'll likely have only one device
      private void BtnCallback_Click(object sender, EventArgs e)
      {
         // Setup callback for each device, if there are devices found  -- if the index is -1, then there are
         //   no devices in the combo-box
         if (CboDevices.SelectedIndex != -1)
         {
            for (int i = 0; i < CboDevices.Items.Count; i++)
            {
               //use the cbotodevice array which contains the mapping of the devices in the CboDevices to the actual device IDs
               devices[iCboToDevice[i]].SetErrorCallback(this);
               devices[iCboToDevice[i]].SetDataCallback(this, DataCallbackFilterType.callOnChangedData);
            }
         }
      }


      // Data callback    
      public void HandlePIEHidData(Byte[] data, PIEDevice sourceDevice)
      {
         // Check the sourceDevice and make sure it is the same device as selected in CboDevice   
         if (sourceDevice == devices[selectedDevice])
         {
            // byte array used for read data
            byte[] rData = null;

            // I think an MCU would use while(1) here, as an equivalent structure (TB, 11/12/2011)
            while (0 == sourceDevice.ReadData(ref rData)) // do this so don't ever miss any data
            {
               // Read the unit ID
               // TODO: If we do only have one device, then we could simply grab the ID once up above, and load it into an
               //         instance variable on form_load. Need to verify that we are using only one device, and then how
               //         that device is numbered
               c = this.LblUnitID;
               this.SetText(rData[sourceDevice.ReadLength - 1].ToString());

               //write raw data to listbox1
               String output = "Callback: " + this.devices[selectedDevice].Pid + ", ID: " + selectedDevice.ToString() + ", data=";
               for (int i = 0; i < sourceDevice.ReadLength; i++)
               {
                  output = output + rData[i].ToString() + " ";
               }
               this.SetListBox(output);

// TODO: We'll need to convert THESE data into the correct formats needed by SimConnect (TB, 11/12/2011)
               // Write Analog data from each pin, to the corresponding individual textboxes. Each of these pins
               //   will have specific data assigned (if all are used), and these data will have to be translated
               //   into the proper SimConnect Event calls.

               // TODO: Use the debugger to check the format of these bytes, and how the data changes. I believe 
               //         these to be the analog data, and thus it should be a simply matter of writing the values
               //         to the appropriate P3D event/variable.
               c = this.textBoxA2;
               this.SetText(rData[1].ToString());  // pin 0

               c = this.textBoxA3;
               this.SetText(rData[2].ToString());

               c = this.textBoxA4;
               this.SetText(rData[3].ToString());

               c = this.textBoxA5;
               this.SetText(rData[4].ToString());

               c = this.textBoxA6;
               this.SetText(rData[5].ToString());

               c = this.textBoxA7;
               this.SetText(rData[6].ToString());

               c = this.textBoxA8;
               this.SetText(rData[7].ToString());

               c = this.textBoxA9;
               this.SetText(rData[8].ToString());

               c = this.textBoxA10;
               this.SetText(rData[9].ToString());

               c = this.textBoxA11;
               this.SetText(rData[10].ToString());

               c = this.textBoxA12;
               this.SetText(rData[11].ToString());

               c = this.textBoxA13;
               this.SetText(rData[12].ToString());

               c = this.textBoxA14;
               this.SetText(rData[13].ToString());

               c = this.textBoxA15;
               this.SetText(rData[14].ToString());

               c = this.textBoxA16;
               this.SetText(rData[15].ToString());

               c = this.textBoxA17;
               this.SetText(rData[16].ToString());

               c = this.textBoxA18;
               this.SetText(rData[17].ToString());

               c = this.textBoxA19;
               this.SetText(rData[18].ToString());

               c = this.textBoxA20;
               this.SetText(rData[19].ToString());

               c = this.textBoxA21;
               this.SetText(rData[20].ToString());

               c = this.textBoxA22;
               this.SetText(rData[21].ToString());

               c = this.textBoxA23;
               this.SetText(rData[22].ToString());

               c = this.textBoxA24;
               this.SetText(rData[23].ToString()); // pin 24

               //write digital 1 data to the individual textboxes
               c = this.textBoxD12;
               this.SetText((rData[24] & 1).ToString()); // Bitwise AND with 0b00000001
               c = this.textBoxD13;
               this.SetText(((rData[24] & 2) >> 1).ToString());
               c = this.textBoxD14;
               this.SetText(((rData[24] & 4) >> 2).ToString());
               c = this.textBoxD15;
               this.SetText(((rData[24] & 8) >> 3).ToString());
               c = this.textBoxD16;
               this.SetText(((rData[24] & 16) >> 4).ToString());
               c = this.textBoxD17;
               this.SetText(((rData[24] & 32) >> 5).ToString());
               c = this.textBoxD18;
               this.SetText(((rData[24] & 64) >> 6).ToString());
               c = this.textBoxD19;
               this.SetText(((rData[24] & 128) >> 7).ToString());
               
               c = this.textBoxD110;
               this.SetText((rData[25] & 1).ToString());
               c = this.textBoxD111;
               this.SetText(((rData[25] & 2) >> 1).ToString());
               c = this.textBoxD112;
               this.SetText(((rData[25] & 4) >> 2).ToString());
               c = this.textBoxD113;
               this.SetText(((rData[25] & 8) >> 3).ToString());
               c = this.textBoxD114;
               this.SetText(((rData[25] & 16) >> 4).ToString());
               c = this.textBoxD115;
               this.SetText(((rData[25] & 32) >> 5).ToString());
               c = this.textBoxD116;
               this.SetText(((rData[25] & 64) >> 6).ToString());
               c = this.textBoxD117;
               this.SetText(((rData[25] & 128) >> 7).ToString());

               c = this.textBoxD118;
               this.SetText((rData[26] & 1).ToString());
               c = this.textBoxD119;
               this.SetText(((rData[26] & 2) >> 1).ToString());
               c = this.textBoxD120;
               this.SetText(((rData[26] & 4) >> 2).ToString());
               c = this.textBoxD121;
               this.SetText(((rData[26] & 8) >> 3).ToString());
               c = this.textBoxD122;
               this.SetText(((rData[26] & 16) >> 4).ToString());
               c = this.textBoxD123;
               this.SetText(((rData[26] & 32) >> 5).ToString());
               c = this.textBoxD124;
               this.SetText(((rData[26] & 64) >> 6).ToString()); //pin 24

               //write digital 2 data to the individual textboxes
               c = this.textBoxD22;
               this.SetText((rData[27] & 1).ToString());
               c = this.textBoxD23;
               this.SetText(((rData[27] & 2) >> 1).ToString());
               c = this.textBoxD24;
               this.SetText(((rData[27] & 4) >> 2).ToString());
               c = this.textBoxD25;
               this.SetText(((rData[27] & 8) >> 3).ToString());
               c = this.textBoxD26;
               this.SetText(((rData[27] & 16) >> 4).ToString());
               c = this.textBoxD27;
               this.SetText(((rData[27] & 32) >> 5).ToString());
               c = this.textBoxD28;
               this.SetText(((rData[27] & 64) >> 6).ToString());
               c = this.textBoxD29;
               this.SetText(((rData[27] & 128) >> 7).ToString());

               c = this.textBoxD210;
               this.SetText((rData[28] & 1).ToString());
               c = this.textBoxD211;
               this.SetText(((rData[28] & 2) >> 1).ToString());
               c = this.textBoxD212;
               this.SetText(((rData[28] & 4) >> 2).ToString());
               c = this.textBoxD213;
               this.SetText(((rData[28] & 8) >> 3).ToString());
               c = this.textBoxD214;
               this.SetText(((rData[28] & 16) >> 4).ToString());
               c = this.textBoxD215;
               this.SetText(((rData[28] & 32) >> 5).ToString());
               c = this.textBoxD216;
               this.SetText(((rData[28] & 64) >> 6).ToString());
               c = this.textBoxD217;
               this.SetText(((rData[28] & 128) >> 7).ToString());

               c = this.textBoxD218;
               this.SetText((rData[29] & 1).ToString());
               c = this.textBoxD219;
               this.SetText(((rData[29] & 2) >> 1).ToString());
               c = this.textBoxD220;
               this.SetText(((rData[29] & 4) >> 2).ToString());
               c = this.textBoxD221;
               this.SetText(((rData[29] & 8) >> 3).ToString());
               c = this.textBoxD222;
               this.SetText(((rData[29] & 16) >> 4).ToString());
               c = this.textBoxD223;
               this.SetText(((rData[29] & 32) >> 5).ToString());
               c = this.textBoxD224;
               this.SetText(((rData[29] & 64) >> 6).ToString()); //pin 24
            }
         }
      }


      // Error callback
      public void HandlePIEHidError(Int32 error, PIEDevice sourceDevice)
      {
         this.SetToolStrip("Error: " + error.ToString());
         if (error == 307)
            sourceDevice.CloseInterface();
      }

      // For thread-safe setting of Windows Forms control
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

      // For thread-safe setting of Windows Forms control
      private void SetListBox(string text)
      {
         // InvokeRequired required compares the thread ID of the calling thread to the thread 
         //   ID of the creating thread. If these threads are different, it returns true.
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

      // For thread-safe setting of Windows Forms control
      private void SetToolStrip(string text)
      {
         // InvokeRequired required compares the thread ID of the calling thread to the thread 
         //   ID of the creating thread. If these threads are different, it returns true.
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
         // Write to currently selected device - turn on the green LED
         if (CboDevices.SelectedIndex != -1)
         {
            for (int j = 0; j < devices[selectedDevice].WriteLength; j++)
            {
               wData[j] = 0;
            }
            wData[1] = 134;
            wData[2] = 16;

            int result = devices[selectedDevice].WriteData(wData);
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
         // Write to currently selected device - turn on the Red LED
         if (CboDevices.SelectedIndex != -1) // do nothing if not enumerated
         {

            for (int j = 0; j < devices[selectedDevice].WriteLength; j++)
            {
               wData[j] = 0;
            }

            wData[1] = 134;
            wData[2] = 32;

            int result = devices[selectedDevice].WriteData(wData);
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

            for (int j = 0; j < devices[selectedDevice].WriteLength; j++)
            {
               wData[j] = 0;
            }
            wData[1] = 134;
            wData[2] = 48;

            int result = devices[selectedDevice].WriteData(wData);
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

            for (int j = 0; j < devices[selectedDevice].WriteLength; j++)
            {
               wData[j] = 0;
            }
            wData[1] = 134;

            int result = devices[selectedDevice].WriteData(wData);
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
         for (int j = 0; j < devices[selectedDevice].WriteLength; j++)
         {
            wData[j] = 0;
         }

         wData[1] = 137;
         wData[2] = 137;
         wData[7] = (byte)(Convert.ToInt16(TxtSetUnitID.Text)); // how to get to a numeric val
         wData[8] = 16;
         int result = devices[selectedDevice].WriteData(wData);
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
         for (int j = 0; j < devices[selectedDevice].WriteLength; j++)
         {
            wData[j] = 0;
         }

         wData[1] = 147;

         // TODO: Will only need to write to the digital pins that correspond to the hardware we'll be supporting
         if (checkBox2.Checked == true) wData[2] = (byte)(wData[2] | 1);
         else wData[2] = (byte)(wData[2] & (~1));
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

         int result = devices[selectedDevice].WriteData(wData);
         if (result != 0)
         {
            toolStripStatusLabel1.Text = "Write Fail: " + result;
         }
         else
         {
            toolStripStatusLabel1.Text = "Write Success - Write Digital Outputs";
         }
      }


// TODO: Implement these features for hardware-checking, for FAA requirements 
      private void BtnSetKey_Click(object sender, EventArgs e)
      {
         //For users of the dongle feature 
         if (CboDevices.SelectedIndex != -1) //do nothing if not enumerated
         {
            // Programming switch must be up (red). Users of this feature may want to add a program 
            //   switch check. This routine is done once per unit by the developer prior to sale.

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

            int result = devices[selectedDevice].WriteData(wData);
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
         // This is done within the developer's application to check for the correct
         //   hardware.  The K0-K3 values must be the same as those entered in Set Key.
         if (CboDevices.SelectedIndex != -1)
         {
            LblPassFail.Text = "Pass/Fail";
            //check hardware

            //IMPORTANT turn off the callback if going so data isn't grabbed there, turn it back on later (not done here)
            devices[selectedDevice].SetDataCallback(this, DataCallbackFilterType.callNever);

            //IMPORTANT turn off the callback if going so data isn't grabbed there, turn it back on later (not done here)
            devices[selectedDevice].SetDataCallback(this, DataCallbackFilterType.callNever);

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

            devices[selectedDevice].ClearBuffer();
            int result = devices[selectedDevice].WriteData(wData);

            if (result != 0)
            {
               toolStripStatusLabel1.Text = "Write Fail: " + result;
            }
            else
            {
               toolStripStatusLabel1.Text = "Write Success-Check Dongle Key";
            }

            // After this write the next valid read will give 4 values which are used below for comparison

            byte[] data = null;
            int countout = 0;
            int ret = devices[selectedDevice].BlockingReadData(ref data, 100);

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
               ret = devices[selectedDevice].BlockingReadData(ref data, 100);
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
