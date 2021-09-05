using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SoundCheck
{
    public partial class Form1 : Form, VolumeDBUpdateListener
    {
        AudioRecorder mAudioRecorder;
        SynchronizationContext m_SyncContext = null;
        public Form1()
        {
            InitializeComponent();
            m_SyncContext = SynchronizationContext.Current;
            mAudioRecorder = new AudioRecorder();
            mAudioRecorder.registerVolumeDBUpdateListener(this);
            ChartArea chartArea = chart1.ChartAreas[0];
            
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisX.ScrollBar.Enabled = true;
            chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chartArea.AxisX.IntervalType = DateTimeIntervalType.NotSet;
            chartArea.AxisX.LabelStyle.Format = "#";
            //chart1.Series["Volumes"].Label = "#VAL{P}";
            chartArea.AxisX.ScaleView.Size = 20;

            //set default alarm value
            updateAlarmLimit();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                if (mAudioRecorder.getRecordState() != AudioRecorder.RECORD_STATE_CLOSED)
                {
                    Console.WriteLine("Form is Closing");
                    mAudioRecorder.stopRecord();
                    Thread.Sleep(50);
                }
            }
            base.WndProc(ref m);

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
                updateAlarmLimit();
                chart1.ChartAreas[0].AxisX.ScaleView.Position = 0;
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

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        public void onVolumeDBUpdate(TimeAndVolumeDBPoint point)
        {
            m_SyncContext.Post(UpdateChart, point);
            return;
        }

        private void UpdateChart(object state)
        {
            if (mAudioRecorder.getRecordState() == AudioRecorder.RECORD_STATE_CLOSED)
            {
                return;
            }
            TimeAndVolumeDBPoint point= (TimeAndVolumeDBPoint)state;
            if (chart1 != null && chart1.Series != null &&  chart1.Series["Volumes"] != null && chart1.Series["Volumes"].Points != null) {
                float xValue = (float)point.mTime / 1000;
                chart1.Series[0].Points.AddXY(xValue, point.mVolumeDB);
                ChartArea chartArea = chart1.ChartAreas[0];
                float viewPos = (float)((float)xValue - chartArea.AxisX.ScaleView.Size);
                if (xValue - chartArea.AxisX.ScaleView.Size < 0)
                {
                    viewPos = 0;
                }
                chartArea.AxisX.ScaleView.Position = viewPos;

                //draw limit line
                chart1.Series[1].Points.AddXY(xValue, mAudioRecorder.getMinAlarmValue());
                chart1.Series[2].Points.AddXY(xValue, mAudioRecorder.getMaxAlarmValue());
            }
            return;
        }

        private void label5_Click_1(object sender, EventArgs e)
        {

        }

        private void label7_Click_1(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            updateAlarmLimit();
        }

        private void updateAlarmLimit()
        {
            //设置警报上下限
            int min_alarm_value = int.Parse(txtbox_alarm_min.Text);
            if (min_alarm_value < 0)
            {
                min_alarm_value = 0;
            }
            int max_alarm_value = int.Parse(txtbox_alarm_max.Text);
            if (max_alarm_value > 100)
            {
                max_alarm_value = 100;
            }
            if (min_alarm_value >= max_alarm_value)
            {
                MessageBox.Show("报警上下限设置错误，将使用默认值！！");
                min_alarm_value = 0;
                max_alarm_value = 100;
                txtbox_alarm_max.Text = "100";
                txtbox_alarm_min.Text = "0";
            }

            mAudioRecorder.setAlarmLimit(min_alarm_value, max_alarm_value);
        }
    }
}