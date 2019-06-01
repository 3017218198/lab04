using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using ZedGraph;
using System.Drawing;
using System.Threading;
using System.Globalization;

namespace lab04
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // Serialport instance here
        SerialPort serialport;
        
        // port number and baud rate
        string portName = "";
        int baudRate = 0;


        //string CommandRed = "";
        //string CommandGreen = "";
        //string CommandYellow = "";
        //string CommandBlue = "";
        //string CommandWhite = "";

        int NumberRed = 0;
        int NumberGreen = 0;
        int NumberYellow = 0;
        int NumberBlue = 0;
        int NumberWhite = 0;
        int tickStart = 0;
        //Thread thread1 = null;

        public MainWindow()
        {
            InitializeComponent();
            CreateGraph(zedGraphControl1);
        }

        #region Serial Port connnection

        /// <summary>
        /// Dynamically read the port names while drop down the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortSelection_DropDownOpened(object sender, EventArgs e)
        {
            string[] portnames = SerialPort.GetPortNames();
            ComboBox portnameBox = sender as ComboBox;
            portnameBox.Items.Clear();
            foreach (string portname in portnames)
            {
                portnameBox.Items.Add(portname);
            }
        }

        /// <summary>
        /// Select baud rate before connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BPSSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (BPSSelection.SelectedIndex)
            {
                case 1:
                    baudRate = 9600;
                    break;
                case 2:
                    baudRate = 19200;
                    break;
                case 3:
                    baudRate = 38400;
                    break;
                case 4:
                    baudRate = 57600;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Connect to the selected serial port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            portName = SerialPortSelection.SelectedItem.ToString();
            serialport = new SerialPort(portName, baudRate);
            serialport.Open();
            Console.WriteLine("Open port.");
            serialport.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        /// <summary>
        /// Close connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Break_Click(object sender, RoutedEventArgs e)
        {
            serialport.Close();
            Console.WriteLine("Close port.");
            serialport.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);            
        }

        #endregion

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialport == null)
            {
                return;
            }
            int numOfByte = serialport.BytesToRead;
            for (int i = 0; i < numOfByte; i++)
            {

                int indata = serialport.ReadByte();

                //string tem = "";
                //string light = "";
                //string temp = serialport.ReadLine();
                //if (temp.StartsWith("Temperature"))
                //{
                //    tem = temp.Substring(11);

                //}
                //else if (temp.StartsWith("Light"))
                //{
                //    light = temp.Substring(5);
                //}
                string a = indata.ToString() + "\n";

                SetTextInTextBox(receive, a);
                AddDataPoint(indata);
            }
        }

            
        private delegate void SetTextCallback(TextBox control, string text);
        public void SetTextInTextBox(TextBox control, string msg)
        {

            if (receive.Dispatcher.CheckAccess())
            {
                receive.AppendText(msg);

                receive.ScrollToEnd();
            }
            else
            {
                SetTextCallback d = new SetTextCallback(SetTextInTextBox);
                Dispatcher.Invoke(d, new object[] { control, msg });
            }
        }




        #region LED PWM controller
        private void TextRed_TextChanged(object sender, TextChangedEventArgs e)
        {
            string TextRedStr = TextRed.Text.ToString();            
            NumberRed = 255 - (int)Convert.ToDouble(TextRedStr);
            //CommandRed = "r" + NumberRed.ToString();
            byte[] buffer = new byte[3];
            buffer[0] = 0xD4;
            buffer[1] = (byte)(NumberRed & 0x7F);
            buffer[2] = (byte)((NumberRed >> 7) & 0x7F);
            if (serialport != null && OpenLED.IsChecked == true)
            {
                serialport.Write(buffer, 0, 3);
            }
        }

        private void TextGreen_TextChanged(object sender, TextChangedEventArgs e)
        {
            string TextGreenStr = TextGreen.Text.ToString();
            NumberGreen = (int)Convert.ToDouble(TextGreenStr);
            byte[] buffer = new byte[3];
            buffer[0] = 0xD3;
            buffer[1] = (byte)(NumberGreen & 0x7F);
            buffer[2] = (byte)((NumberGreen >> 7) & 0x7F);
            if (serialport != null && OpenLED.IsChecked == true)
            {
                serialport.Write(buffer, 0, 3);
            }
        }

        private void TextYellow_TextChanged(object sender, TextChangedEventArgs e)
        {
            string TextYellowStr = TextYellow.Text.ToString();
            NumberYellow = (int)Convert.ToDouble(TextYellowStr);
            byte[] buffer = new byte[3];
            buffer[0] = 0xD2;
            buffer[1] = (byte)(NumberYellow & 0x7F);
            buffer[2] = (byte)((NumberYellow >> 7) & 0x7F);
            if (serialport != null && OpenLED.IsChecked == true)
            {
                serialport.Write(buffer, 0, 3);
            }
        }

        private void TextBlue_TextChanged(object sender, TextChangedEventArgs e)
        {
            string TextBlueStr = TextBlue.Text.ToString();
            NumberBlue = (int)Convert.ToDouble(TextBlueStr);
            byte[] buffer = new byte[3];
            buffer[0] = 0xD6;
            buffer[1] = (byte)(NumberBlue & 0x7F);
            buffer[2] = (byte)((NumberBlue >> 7) & 0x7F);
            if (serialport != null && OpenLED.IsChecked == true)
            {
                serialport.Write(buffer, 0, 3);
            }
        }

        private void TextWhite_TextChanged(object sender, TextChangedEventArgs e)
        {
            string TextWhiteStr = TextWhite.Text.ToString();
            NumberWhite = (int)Convert.ToDouble(TextWhiteStr);
            byte[] buffer = new byte[3];
            buffer[0] = 0xD6;
            buffer[1] = (byte)(NumberWhite & 0x7F);
            buffer[2] = (byte)((NumberWhite >> 7) & 0x7F);
            if (serialport != null && OpenLED.IsChecked == true)
            {
                serialport.Write(buffer, 0, 3);
            }
        }

        #endregion

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(NumberRed.ToString());
            byte[] buffer = new byte[3];
            buffer = getByteArray( send.Text.ToString());
            serialport.Write(buffer, 0, 3);
            //MessageBox.Show(data.ToString());
            send.Text = "";

        }

        /// <summary>
        /// transfer from string to byte array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] getByteArray(string str)
        {
            
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            string[] splited = str.Split(new Char[] { ' ', ',', '.', ':', '\t' });
            byte[] buffer = new byte[splited.Length];
            for (int i = 0; i < splited.Length; i++)
            {
                if (!(byte.TryParse(splited[i], NumberStyles.HexNumber, null, out buffer[i])))
                {
                    buffer[i] = 0;
                }
            }
            return buffer;
        }

        /// <summary>
        /// Open LED and use sliders to control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenLED_Click(object sender, RoutedEventArgs e)
        {
            // Initialize
            byte[] buffer = new byte[3];
            buffer[0] = 0x92;
            buffer[1] = 0x00;
            buffer[2] = 0x00;
            for (int i = 0; i < 3; ++i)
            {
                serialport.Write(buffer, 0, 3);
            }
            buffer[0] = 0x93;
            for (int i = 0; i < 3; ++i)
            {
                serialport.Write(buffer, 0, 3);
            }
            buffer[0] = 0x94;
            for (int i = 0; i < 3; ++i)
            {
                serialport.Write(buffer, 0, 3);
            }
            buffer[0] = 0x95;
            for (int i = 0; i < 3; ++i)
            {
                serialport.Write(buffer, 0, 3);
            }
            buffer[0] = 0x96;
            for (int i = 0; i < 3; ++i)
            {
                serialport.Write(buffer, 0, 3);
            }
            Console.WriteLine("inititalize complete");
        }

        /// <summary>
        /// Open temperature and light and use textbox to present
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTL_Click(object sender, RoutedEventArgs e)
        {
            // temperature
            byte[] buffer1 = new byte[3];
            buffer1[0] = 0xD0;
            buffer1[1] = 0x11;
            buffer1[2] = 0x11;
            // light
            byte[] buffer2 = new byte[3];
            buffer2[0] = 0xD1;
            buffer2[1] = 0x11;
            buffer2[2] = 0x11;
            
            for (int i = 0; i < 5; ++i)
            {
                serialport.Write(buffer1, 0, 3);
                serialport.Write(buffer2, 0, 3);
            }

        }

        public void CreateGraph(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;
            
            myPane.Title.Text = "dynamic data graph";
            myPane.XAxis.Title.Text = "time";
            myPane.YAxis.Title.Text = "data";
            // declare a font size here
            FontSpec myFont = new FontSpec("Arial", 20, System.Drawing.Color.Blue , false, false, false);
            myPane.Title.FontSpec = myFont;
            myPane.XAxis.Title.FontSpec = myFont;
            myPane.YAxis.Title.FontSpec = myFont;
            RollingPointPairList list1 = new RollingPointPairList(1200);
            RollingPointPairList list2 = new RollingPointPairList(1200);
            /// Initially, a curve is added with no data points (list is empty) 
            /// Color is blue,  and there will be no symbols 
            /// 开始，增加的线是没有数据点的(也就是list为空)   
            ///增加一条名称 :Voltage ，颜色 Color.Bule ，无符号，无数据的空线条

            LineItem curve1 = myPane.AddCurve("Curve1", list1, System.Drawing.Color.Blue, SymbolType.None/*.Diamond*/ );
            //LineItem curve2 = myPane.AddCurve("Voltage2", list2, System.Drawing.Color.Red, SymbolType.None);
            //PointPairList list1 = new PointPairList();

            //for (int i = 0; i < 2000; i++)
            //{
            //    list1.Add(i, i);
            //}
            //LineItem myCurve = myPane.AddCurve("Porsche", list1, System.Drawing.Color.Red, SymbolType.Diamond);
            tickStart = Environment.TickCount;
            zgc.AxisChange();
            //Random y = new Random();

            //PointPairList list1 = new PointPairList();

            //for (int i = 0; i < 36; i++)

            //{

            //    double x = i;

            //    //double y1 = 1.5 + Math.Sin((double)i * 0.2);

            //    double y1 = y.NextDouble() * 1000;

            //    list1.Add(x, y1); //添加一组数据

            //}
            //LineItem myCurve = myPane.AddCurve("东航", list1, System.Drawing.Color.Red, SymbolType.Star);
            //myPane.Fill = new Fill(System.Drawing.Color.White, System.Drawing.Color.FromArgb(200, 200, 255), 45.0f);
            //string[] labels = new string[36];

            //for (int i = 0; i < 36; i++)

            //{

            //    labels[i] = System.DateTime.Now.AddDays(i).ToShortDateString();

            //}

            //myPane.XAxis.Scale.TextLabels = labels; //X轴文本取值

            //myPane.XAxis.Type = AxisType.Text;   //X轴类型
            //zgc.AxisChange();
            //zedGraphControl.Invalidate();
            //Refresh();
        }
        void AddDataPoint(double dataX)
        {
            // Make sure that the curvelist has at least one curve 
            //确保CurveList不为空
            if (zedGraphControl1.GraphPane.CurveList.Count <= 0) return;

            // Get the  first CurveItem in the graph 

            //取Graph第一个曲线，也就是第一步:在 GraphPane.CurveList 集合中查找 CurveItem 
            for (int idxList = 0; idxList < zedGraphControl1.GraphPane.CurveList.Count; idxList++)
            {
                LineItem curve = zedGraphControl1.GraphPane.CurveList[idxList] as LineItem;
                if (curve == null) return;

                // Get the PointPairList 
                //第二步:在CurveItem中访问PointPairList(或者其它的IPointList)，根据自己的需要增加新数据或修改已存在的数据

                IPointListEdit list = curve.Points as IPointListEdit;

                // If this is null, it means the reference at curve.Points does not  
                // support IPointListEdit, so we won't be able to modify it 

                if (list == null) return;


                // Time is measured in seconds 
                double time = (Environment.TickCount - tickStart) / 1000.0;
                // 3 seconds per cycle 

                list.Add(time, dataX);

                // Keep the X scale at a rolling 30 second interval, with one 

                // major step between the max X value and the end of the axis 
                Scale xScale = zedGraphControl1.GraphPane.XAxis.Scale;
                if (time > xScale.Max - xScale.MajorStep)
                {
                    xScale.Max = time + xScale.MajorStep;
                    xScale.Min = xScale.Max - 30.0;
                }

            }
            // Make sure the Y axis is rescaled to accommodate actual data 
            //第三步:调用ZedGraphControl.AxisChange()方法更新X和Y轴的范围


            zedGraphControl1.AxisChange(); // Force a redraw  
                                   //第四步:调用Form.Invalidate()方法更新图表


            zedGraphControl1.Invalidate();
        }
    }
}
