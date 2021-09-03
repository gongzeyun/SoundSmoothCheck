using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundCheck
{
    public partial class Form1 : Form
    {
        AudioRecorder mAudioRecorder;
        public Form1()
        {
            InitializeComponent();
            mAudioRecorder = new AudioRecorder();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int deviceCount = AudioRecorder.getDeviceCount();
            for (int i = 0; i < deviceCount; i++)
            {
                comboBox1.Items.Add(mAudioRecorder.getDeviceName(i));
            }
            //set default value
            comboBox1.SelectedText = mAudioRecorder.getDeviceName(0);
            List<String> deviceRecordConfigs = mAudioRecorder.getDeviceConfigs(0);
            for (int i = 0; i < deviceRecordConfigs.Count; i++)
            {
                comboBox2.Items.Add(deviceRecordConfigs[i]);
            }
            comboBox2.SelectedText = deviceRecordConfigs[0];
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (button1.Text.Equals("Start")) {
                mAudioRecorder.startRecord();
                button1.Text = "Stop";
            } else {
                mAudioRecorder.stopRecord();
                button1.Text = "Start";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedValuechanged(object sender, EventArgs e)
        {
            List<String> deviceRecordConfigs = mAudioRecorder.getDeviceConfigs(comboBox1.SelectedIndex);
            for (int i = 0; i < deviceRecordConfigs.Count; i++)
            {
                comboBox2.Items.Add(deviceRecordConfigs[i]);
            }
            mAudioRecorder.selectDevice(comboBox1.SelectedIndex);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            mAudioRecorder.selectConfig(comboBox2.SelectedIndex);
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
