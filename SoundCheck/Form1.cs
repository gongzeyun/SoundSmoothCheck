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
    public partial class Form1 : Form, UIOwner
    {
        AudioRecoder mAudioRecorder;
        SynchronizationContext m_SyncContext = null;
        List<double> mXTimeDataBinding = new List<double>();
        List<double> mYVolumeDBDataBinding = new List<double>();
        List<double> mYMinAlarmValueDataBinding = new List<double>();
        List<double> mYMaxAlarmValueDataBinding = new List<double>();
        //DataBindingCollection mXTimeDBDataBinding = new DataBindingCollection();
        //DataBindingCollection mYVolumeDBDataBinding = new DataBindingCollection();
        //DataBindingCollection mYminAlarmValueDataBinding = new DataBindingCollection();
        //DataBindingCollection mYmaxAlarmValueDataBinding = new DataBindingCollection();
        double[] test = { 2.8, 4.4, 6.5, 8.3, 3.6, 5.6, 7.3, 9.2, 1.0 };
        public Form1()
        {
            InitializeComponent();
            m_SyncContext = SynchronizationContext.Current;
            mAudioRecorder = new AudioRecoder();
            mAudioRecorder.registerUIOwner(this);
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

            //启动error dump线程
            DumpErrorInfos.startErrorDumpTask();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                DumpErrorInfos.exitErrorDumpTask();
                if (mAudioRecorder.getRecordState() != AudioRecoder.RECORD_STATE_CLOSED)
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
            int deviceCount = AudioRecoder.getDeviceCount();
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
            if (StartStopRecord.Text.Equals("Start")) {
                updateAlarmLimit();
                chart1.ChartAreas[0].AxisX.ScaleView.Position = 0;
                mAudioRecorder.setRecordDuration(int.Parse(textRecordDuration.Text));
                mAudioRecorder.startRecord();
                StartStopRecord.Text = "Stop";
                chart1.Series[0].Points.Clear(); //volume 
                chart1.Series[1].Points.Clear(); //min limit
                chart1.Series[2].Points.Clear(); //max limit
                mXTimeDataBinding.Clear();
                mYVolumeDBDataBinding.Clear();
                mYMinAlarmValueDataBinding.Clear();
                mYMaxAlarmValueDataBinding.Clear();
            } else {
                mAudioRecorder.stopRecord();
                StartStopRecord.Text = "Start";


                chart1.Series[0].Points.Clear(); //volume 
                chart1.Series[1].Points.Clear(); //min limit
                chart1.Series[2].Points.Clear(); //max limit

                chart1.Series[0].Points.DataBindXY(mXTimeDataBinding, mYVolumeDBDataBinding);
                chart1.Series[1].Points.DataBindXY(mXTimeDataBinding, mYMinAlarmValueDataBinding);
                chart1.Series[2].Points.DataBindXY(mXTimeDataBinding, mYMaxAlarmValueDataBinding);
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

        public void UpdateUIAccordMsg(int msgType, object msgObject)
        {
            switch (msgType)
            {
                case AudioRecoder.MSG_RECORD_COMPLETELY:
                    m_SyncContext.Post(RecordComplete, null);
                    break;
                case AudioRecoder.MSG_ERROR_REPORTED:
                    m_SyncContext.Post(genErrorLabel, msgObject);
                    break;
                case AudioRecoder.MSG_UPDATE_VOLUME_POINT:
                    m_SyncContext.Post(UpdateChart, msgObject);
                    break;
                default:
                    break;
            }
        }

        private void RecordComplete(object msgObject)
        {
            StartStopRecord.PerformClick();
        }

        private int calcYPosition()
        {
            int pos = 0;
            System.Windows.Forms.Control.ControlCollection allControl = richTextBox1.Controls; 
            for (int i = 0; i < allControl.Count; i++)
            {
                if (allControl[i].GetType() == typeof(LinkLabel))
                {
                    pos += 10;
                }
            }
            return pos;
        }
        private void genErrorLabel(object msgObject)
        {
            ErrorContainer errorContainer = (ErrorContainer)msgObject;
            Console.WriteLine("genErrorLabel, error time:" + errorContainer.getErrorOccuredTime() + ", error report path:" + errorContainer.getReportPath());
            LinkLabel errorLink = new LinkLabel();
            errorLink.Text = errorContainer.getErrorOccuredTime();
            errorLink.LinkClicked += ErrorLink_LinkClicked;
            errorLink.Height = 10;
            errorLink.Location = new Point(0, calcYPosition());
            //errorLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.errorLinkClicked);
            richTextBox1.Controls.Add(errorLink);
        }

        private void ErrorLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel errorLink = (LinkLabel)sender;
            String errorReportPath = DumpErrorInfos.getErrorReportPathByErrorTime(errorLink.Text);
            Console.WriteLine("ErrorLink_LinkClicked, label txt:" + errorLink.Text + ", error report path:" + errorReportPath);
            errorLink.LinkVisited = true;
            System.Diagnostics.Process.Start(errorReportPath);

            return;
        }
        private void removePointsOutOfScaleView(DataPointCollection points, float xValue)
        {
            double xValueStartInView = xValue - chart1.ChartAreas[0].AxisX.ScaleView.Size;
            if (xValueStartInView <= 0)
            {
                return;
            }

            for (int i = 0; i<points.Count; i++)
            {
                if (points[i].XValue<xValueStartInView)
                {
                    points.RemoveAt(i);
                }
            }
        }

        private void UpdateChart(object msgObject)
        {
            if (mAudioRecorder.getRecordState() == AudioRecoder.RECORD_STATE_CLOSED)
            {
                return;
            }
            TimeAndVolumeDBPoint point= (TimeAndVolumeDBPoint)msgObject;
            float xValue = (float)point.mTime / 1000;

            mXTimeDataBinding.Add(xValue);

            //draw volume line 
            chart1.Series[0].Points.AddXY(xValue, point.mVolumeDB);
            removePointsOutOfScaleView(chart1.Series[0].Points, xValue);
            mYVolumeDBDataBinding.Add(point.mVolumeDB);

            //draw  min limit line
            chart1.Series[1].Points.AddXY(xValue, mAudioRecorder.getMinAlarmValue());
            removePointsOutOfScaleView(chart1.Series[1].Points, xValue);
            mYMinAlarmValueDataBinding.Add(mAudioRecorder.getMinAlarmValue());

            //draw max limit line
            chart1.Series[2].Points.AddXY(xValue, mAudioRecorder.getMaxAlarmValue());
            removePointsOutOfScaleView(chart1.Series[2].Points, xValue);
            mYMaxAlarmValueDataBinding.Add(mAudioRecorder.getMaxAlarmValue());


            ChartArea chartArea = chart1.ChartAreas[0];
            float viewPos = (float)((float)xValue - chartArea.AxisX.ScaleView.Size);
            if (xValue - chartArea.AxisX.ScaleView.Size < 0)
            {
                viewPos = 0;
            }
            chartArea.AxisX.ScaleView.Position = viewPos;

            
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
            if (min_alarm_value < -100)
            {
                min_alarm_value = -100;
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