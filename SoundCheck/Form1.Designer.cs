﻿
namespace SoundCheck
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.label1 = new System.Windows.Forms.Label();
            this.StartStopRecord = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textRecordDuration = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label5 = new System.Windows.Forms.Label();
            this.txtbox_alarm_max = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtbox_alarm_min = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.audioRecorderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.audioRecorderBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioRecorderBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioRecorderBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(48, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(357, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "断音测试程序";
            this.label1.UseCompatibleTextRendering = true;
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // StartStopRecord
            // 
            this.StartStopRecord.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StartStopRecord.Location = new System.Drawing.Point(456, 154);
            this.StartStopRecord.Name = "StartStopRecord";
            this.StartStopRecord.Size = new System.Drawing.Size(177, 38);
            this.StartStopRecord.TabIndex = 1;
            this.StartStopRecord.Text = "Start";
            this.StartStopRecord.UseVisualStyleBackColor = true;
            this.StartStopRecord.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.comboBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(48, 100);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(288, 29);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.comboBox2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(362, 100);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(271, 29);
            this.comboBox2.TabIndex = 3;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(48, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 28);
            this.label2.TabIndex = 6;
            this.label2.Text = "录音时长:";
            this.label2.UseCompatibleTextRendering = true;
            this.label2.Click += new System.EventHandler(this.label2_Click_1);
            // 
            // textRecordDuration
            // 
            this.textRecordDuration.Location = new System.Drawing.Point(218, 160);
            this.textRecordDuration.Name = "textRecordDuration";
            this.textRecordDuration.Size = new System.Drawing.Size(120, 28);
            this.textRecordDuration.TabIndex = 7;
            this.textRecordDuration.Text = "7200";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(362, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 28);
            this.label3.TabIndex = 8;
            this.label3.Text = "秒";
            this.label3.UseCompatibleTextRendering = true;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(48, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 28);
            this.label4.TabIndex = 12;
            this.label4.Text = "录音设备:";
            this.label4.UseCompatibleTextRendering = true;
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(362, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 28);
            this.label6.TabIndex = 13;
            this.label6.Text = "录音参数:";
            this.label6.UseCompatibleTextRendering = true;
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(658, 96);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(204, 40);
            this.checkBox1.TabIndex = 15;
            this.checkBox1.Text = "是否保存录音";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // chart1
            // 
            chartArea1.AxisX.Title = "Time";
            chartArea1.AxisY.Title = "Volume";
            chartArea1.BackColor = System.Drawing.Color.White;
            chartArea1.CursorX.AutoScroll = false;
            chartArea1.Name = "ChartArea1";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 94F;
            chartArea1.Position.Width = 93F;
            chartArea1.Position.Y = 3F;
            chartArea1.ShadowColor = System.Drawing.Color.White;
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Location = new System.Drawing.Point(48, 218);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Blue;
            series1.Legend = "Legend1";
            series1.Name = "Volumes";
            series1.ShadowColor = System.Drawing.Color.White;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Red;
            series2.LegendText = "报警下限";
            series2.Name = "minLimit";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.Red;
            series3.Name = "maxLimit";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(1268, 470);
            this.chart1.TabIndex = 19;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.chart1_Click);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(824, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 36);
            this.label5.TabIndex = 20;
            this.label5.Text = "警报上限:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.UseCompatibleTextRendering = true;
            this.label5.Click += new System.EventHandler(this.label5_Click_1);
            // 
            // txtbox_alarm_max
            // 
            this.txtbox_alarm_max.Location = new System.Drawing.Point(950, 100);
            this.txtbox_alarm_max.Name = "txtbox_alarm_max";
            this.txtbox_alarm_max.Size = new System.Drawing.Size(120, 28);
            this.txtbox_alarm_max.TabIndex = 21;
            this.txtbox_alarm_max.Text = "100";
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(824, 160);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 32);
            this.label7.TabIndex = 22;
            this.label7.Text = "警报下限:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label7.UseCompatibleTextRendering = true;
            this.label7.Click += new System.EventHandler(this.label7_Click_1);
            // 
            // txtbox_alarm_min
            // 
            this.txtbox_alarm_min.Location = new System.Drawing.Point(950, 164);
            this.txtbox_alarm_min.Name = "txtbox_alarm_min";
            this.txtbox_alarm_min.Size = new System.Drawing.Size(120, 28);
            this.txtbox_alarm_min.TabIndex = 23;
            this.txtbox_alarm_min.Text = "-100";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.Location = new System.Drawing.Point(1095, 99);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(220, 96);
            this.button3.TabIndex = 24;
            this.button3.Text = "确定";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(1356, 96);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(320, 98);
            this.label8.TabIndex = 25;
            this.label8.Text = "输出信息:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label8.UseCompatibleTextRendering = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(1356, 218);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(319, 468);
            this.richTextBox1.TabIndex = 26;
            this.richTextBox1.Text = "";
            // 
            // audioRecorderBindingSource
            // 
            this.audioRecorderBindingSource.DataSource = typeof(SoundCheck.AudioRecorder);
            // 
            // audioRecorderBindingSource1
            // 
            this.audioRecorderBindingSource1.DataSource = typeof(SoundCheck.AudioRecorder);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1704, 700);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.txtbox_alarm_min);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtbox_alarm_max);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textRecordDuration);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.StartStopRecord);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "SoundSmoothCheck";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioRecorderBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioRecorderBindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button StartStopRecord;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textRecordDuration;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.BindingSource audioRecorderBindingSource;
        private System.Windows.Forms.BindingSource audioRecorderBindingSource1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtbox_alarm_max;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtbox_alarm_min;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

