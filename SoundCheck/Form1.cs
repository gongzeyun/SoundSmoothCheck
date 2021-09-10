using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        List<Int64> mAxisX_TimeInMs_Cached = new List<Int64>();
        List<double> mAxisY_Volume_Cached = new List<double>();
        List<double> mAxisY_MinAlarm_Cached = new List<double>();
        List<double> mAxisY_MaxAlarm_Cached = new List<double>();

        private const int mAxisYIntervalDefault = 10;
        public Form1()
        {
            InitializeComponent();
            m_SyncContext = SynchronizationContext.Current;
            mAudioRecorder = new AudioRecoder();
            mAudioRecorder.registerUIOwner(this);
            ChartArea chartArea = chart1.ChartAreas[0];
            
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisY.Minimum = 30;
            chartArea.AxisY.Maximum = 120;
            chartArea.AxisY.Interval = mAxisYIntervalDefault;

            chartArea.AxisY.ScaleView.Position = chartArea.AxisY.Minimum;
            chartArea.AxisY.ScaleView.Size = chartArea.AxisY.Maximum - chartArea.AxisY.Minimum;
            chartArea.AxisY.ScrollBar.Enabled = true;
            chartArea.AxisY.ScrollBar.BackColor = Color.DarkGray;


            chartArea.AxisX.ScrollBar.Enabled = false;
            chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chartArea.AxisX.IntervalType = DateTimeIntervalType.NotSet;
            chartArea.AxisX.LabelStyle.Format = "#";
            //chart1.Series["Volumes"].Label = "#VAL{P}";
            chartArea.AxisX.ScaleView.Size = 10;

            chart1.MouseWheel += Chart1_MouseWheel;
            hScrollBar1.Scroll += HScrollBar1_Scroll;
            //set default alarm value
            updateAlarmLimit();

            //启动error dump线程
            DumpErrorInfos.startErrorDumpTask();
        }

        private void Chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            ChartArea chartArea = chart1.ChartAreas[0];
            Keys key = Form1.ModifierKeys;
            Console.WriteLine("Chart1_MouseWheel, delta:" + e.Delta + ", key info:" + key.ToString());

            if (key.ToString().Equals("Control"))
            {
                if (e.Delta > 0) {
                    chartArea.AxisY.Interval = (int)chartArea.AxisY.Interval / 2;
                    if (chartArea.AxisY.Interval < 1)
                    {
                        chartArea.AxisY.Interval = 1;
                    }
                    chartArea.AxisY.ScaleView.Size = (int)chartArea.AxisY.ScaleView.Size / 2;
                    if (chartArea.AxisY.ScaleView.Size < 1)
                    {
                        chartArea.AxisY.ScaleView.Size = 1;
                    }
                    chartArea.AxisY.ScaleView.Position = chartArea.AxisY.Minimum;
                } else {
                    chartArea.AxisY.ScaleView.Position = chartArea.AxisY.Minimum;
                    chartArea.AxisY.Interval *= 2;
                    if (chartArea.AxisY.Interval > mAxisYIntervalDefault)
                    {
                        chartArea.AxisY.Interval = mAxisYIntervalDefault;
                    }
                    chartArea.AxisY.ScaleView.Size *= 2;
                    if (chartArea.AxisY.ScaleView.Size > chartArea.AxisY.Maximum - chartArea.AxisY.Minimum)
                    {
                        chartArea.AxisY.ScaleView.Size = chartArea.AxisY.Maximum - chartArea.AxisY.Minimum;
                    }
                }
            }
        }

        private void HScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            
            Console.WriteLine("HScrollBar1_Scroll, cur Value:" + hScrollBar1.Value + ", max Value:" + hScrollBar1.Maximum);
            updateScaleViewFromCache(hScrollBar1.Value * 1000);
            //throw new NotImplementedException();
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
            comboBox1.SelectedIndex = 0;
            List<String> deviceRecordConfigs = mAudioRecorder.getDeviceConfigs(0);
            for (int i = 0; i < deviceRecordConfigs.Count; i++)
            {
                comboBox2.Items.Add(deviceRecordConfigs[i]);
            }
            comboBox2.SelectedIndex = deviceRecordConfigs.Count - 1;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
        private void updateScaleViewFromCache(Int64 viewStartPosInMs)
        {
            chart1.Series[0].Points.Clear(); //volume 
            chart1.Series[1].Points.Clear(); //min limit
            chart1.Series[2].Points.Clear(); //max limit

            ChartArea chartArea = chart1.ChartAreas[0];

            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisY.Minimum = 30;
            chartArea.AxisX.ScrollBar.Enabled = false;
            chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chartArea.AxisX.IntervalType = DateTimeIntervalType.NotSet;
            chartArea.AxisX.LabelStyle.Format = "#";

            chart1.ChartAreas[0].AxisX.ScaleView.Position = (float)viewStartPosInMs / 1000;
            Int64 viewEndPosInMs = viewStartPosInMs + (Int64)chart1.ChartAreas[0].AxisX.ScaleView.Size * 1000;
            Console.WriteLine("updateScaleViewFromCache, viewStartPos:" + viewStartPosInMs + ",viewEndPos:" + viewEndPosInMs);
            for (int i = 0; i < mAxisX_TimeInMs_Cached.Count; i++)
            {
                Int64 axisXValueFromCached = mAxisX_TimeInMs_Cached[i];
                if (axisXValueFromCached >= viewStartPosInMs && axisXValueFromCached <= viewEndPosInMs)
                {
                    float xValue = (float)axisXValueFromCached / 1000;
                    Console.WriteLine("XValue:" + xValue + ", YValue:" + mAxisY_Volume_Cached[i]);
                    chart1.Series[0].Points.AddXY(xValue, mAxisY_Volume_Cached[i]);
                    chart1.Series[1].Points.AddXY(xValue, mAxisY_MinAlarm_Cached[i]);
                    chart1.Series[2].Points.AddXY(xValue, mAxisY_MaxAlarm_Cached[i]);
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("button1_Click:" + StartStopRecord.Text);
            if (StartStopRecord.Text.Equals("Start")) {
                updateAlarmLimit();
                chart1.ChartAreas[0].AxisX.ScaleView.Position = 0;
                int selectDevice = comboBox1.SelectedIndex;
                if (selectDevice < 0)
                {
                    selectDevice = 0;
                }
                mAudioRecorder.selectDevice(comboBox1.SelectedIndex);
                mAudioRecorder.selectConfig(comboBox2.SelectedIndex);
                mAudioRecorder.setRecordDuration((int)(double.Parse(textRecordDuration.Text) * 3600));
                mAudioRecorder.startRecord();
                StartStopRecord.Text = "Stop";
               
                chart1.Series[0].Points.Clear(); //volume 
                chart1.Series[1].Points.Clear(); //min limit
                chart1.Series[2].Points.Clear(); //max limit
                mAxisX_TimeInMs_Cached.Clear();
                mAxisY_Volume_Cached.Clear();
                mAxisY_MinAlarm_Cached.Clear();
                mAxisY_MaxAlarm_Cached.Clear();

                File.Delete("raw.pcm");
                File.Delete("dump.pcm");
            } else {
                mAudioRecorder.stopRecord();
                StartStopRecord.Text = "Start";

                Int64 lastCachedAxisXValueInMs = mAxisX_TimeInMs_Cached.Last<Int64>();
                Int64 viewPosInMs = lastCachedAxisXValueInMs - (Int64)(chart1.ChartAreas[0].AxisX.ScaleView.Size) * 1000;
                if (viewPosInMs < 0)
                {
                    viewPosInMs = 0;
                }
                
                updateScaleViewFromCache(viewPosInMs);
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
            String errorReportPath = DumpErrorInfos.getErrorReportPathByErrorRealTime(errorLink.Text);
            Console.WriteLine("ErrorLink_LinkClicked, label txt:" + errorLink.Text + ", error report path:" + errorReportPath);
            errorLink.LinkVisited = true;
            System.Diagnostics.Process.Start(errorReportPath);

            Int64 timeMSRecord = DumpErrorInfos.getRecordTimeByErrorRealTime(errorLink.Text);
            Console.WriteLine("ErrorLink_LinkClicked, label txt:" + errorLink.Text + ", record timeMS:" + timeMSRecord);
            chart1.ChartAreas[0].AxisX.ScaleView.Position = timeMSRecord / 1000;
            updateScaleViewFromCache(timeMSRecord - 1000);
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

            mAxisX_TimeInMs_Cached.Add(point.mTime);

            //draw volume line 
            chart1.Series[0].Points.AddXY(xValue, point.mVolumeDB);
            removePointsOutOfScaleView(chart1.Series[0].Points, xValue);
            mAxisY_Volume_Cached.Add(point.mVolumeDB);
            label9.Text = "Vol:" + (int)point.mVolumeDB;

            //draw  min limit line
            chart1.Series[1].Points.AddXY(xValue, mAudioRecorder.getMinAlarmValue());
            removePointsOutOfScaleView(chart1.Series[1].Points, xValue);
            mAxisY_MinAlarm_Cached.Add(mAudioRecorder.getMinAlarmValue());

            //draw max limit line
            chart1.Series[2].Points.AddXY(xValue, mAudioRecorder.getMaxAlarmValue());
            removePointsOutOfScaleView(chart1.Series[2].Points, xValue);
            mAxisY_MaxAlarm_Cached.Add(mAudioRecorder.getMaxAlarmValue());


            ChartArea chartArea = chart1.ChartAreas[0];
            float viewPos = (float)((float)xValue - chartArea.AxisX.ScaleView.Size);
            if (xValue - chartArea.AxisX.ScaleView.Size < 0)
            {
                viewPos = 0;
            }
            chartArea.AxisX.ScaleView.Position = viewPos;
            hScrollBar1.Maximum = (int)xValue + 1;
            hScrollBar1.Value = (int)xValue + 1;
           
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            mAudioRecorder.enablePCMDump(checkBox1.Checked);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Controls.Clear();
        }
    }
}