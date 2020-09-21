using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Runtime.InteropServices;
using CA200SRVRLib;
using CASDK2;
using System.Threading;

namespace Testplan
{
    public partial class CA410 : DockContent
    {
        static CASDK2Ca200 objCa200;
        static CASDK2Cas objCas;
        static CASDK2Ca objCa;
        static CASDK2Probes objProbes;
        static CASDK2OutputProbes objOutputProbes;
        static CASDK2Probe objProbe;
        static CASDK2Memory objMemory;
        static int err = 0;
        const int NOERR = 0;

        const int MAXPROBE = 10;

        const int MODE_Lvxy = 0;
        const int MODE_Tduv = 1;
        const int MODE_Lvdudv = 5;
        const int MODE_FMA = 6;
        const int MODE_XYZ = 7;
        const int MODE_JEITA = 8;
        const int MODE_LvPeld = 9;
        const int MODE_Waveform = 10;
        const int MODE_FMA2 = 11;
        const int MODE_JEITA2 = 12;
        const int MODE_Waveform2 = 13;

        const int RED = 0;
        const int GREEN = 1;
        const int BLUE = 2;
        const int WHITE = 3;

        static bool autoconnectflag = true; // auro or manual
        static bool triggerfinish = true;

        //public static double LvValue;
        public static double Lv = 0.0;
        public static double sx = 0.0;
        public static double sy = 0.0;
        public static double X = 0.0;
        public static double Y = 0.0;
        public static double Z = 0.0;
        public static double JEITA = 0.0;
        public static double FMA = 0.0;


        public CA410()
        {
            InitializeComponent();
        }

        private void CA310_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Thread.Sleep(500);
                AutoConnect();
                DefaultSetting();
                button1.BackColor = Color.FromArgb(255, 255, 0);


            }
            catch(Exception er)
            {
                DisplayError(er);
            }
        }

        private void DisplayError(Exception er)
        {
            String msg;
            msg = "Error from" + er.Source; /*+ "￥r￥n"*/
            msg += er.Message; /*"￥r￥n"*/

            //msg += "HR:" + (er.)

            MessageBox.Show(msg);
        }

        private void objCa_ExeCalZero()
        {
            if (MessageBox.Show("CalZero?", "CalZero", MessageBoxButtons.OKCancel) ==
                DialogResult.Cancel)
            {
                return;
            }
            try
            {
                objCa.CalZero();
            }
            catch (Exception er)
            {
                DisplayError(er);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                
                Measurement();                 
                Ca_Lv.Text = Convert.ToString(Lv);
                Ca_sx.Text = Convert.ToString(sx);
                Ca_sy.Text = Convert.ToString(sy);
                Ca_X.Text = Convert.ToString(X);
                Ca_Y.Text = Convert.ToString(Y);
                Ca_Z.Text = Convert.ToString(Z);
                Flicker.Text = Convert.ToString(JEITA);
                F.Text = Convert.ToString(FMA);


            }
            catch (Exception er)
            {
                DisplayError(er);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(255, 255, 255);
            Disconnect();
        }

        ///<summary>
        ///[SetSDK and connect device]
        ///This method connect CA-410 manually and set objects 
        ///</summary>
        private static void ManualConnect()
        {
            const int CA_ID = 1;
            const string connect_probe = "1";       //Select P1 probe
            const int port_auto = 0;                //Port number = 0 -> Auto
            const int port_manual = 1;              //COMPort number
            const int baudrate = 0;                 //Dummy baud rate
            const int interface_USB = 0;
            const int interface_RS = 1;
            const int interface_ETHER = 2;
            const int interface_BT = 3;
            const string IP = "111.111.111.111";
            

            objCa200 = new CASDK2Ca200();   // Generate application object

            //GetErrorMessage(objCa200.SetEthernetSetting(CA_ID, IP));
            //GetErrorMessage(objCa200.SetConfiguration(CA_ID, connect_probe, port_manual, baudrate, interface_ETHER));
            //GetErrorMessage(objCa200.SetConfiguration(CA_ID, connect_probe, port_manual, baudrate, interface_BT));
            //GetErrorMessage(objCa200.SetConfiguration(CA_ID, connect_probe, port_manual, baudrate, interface_RS));

            // Substitute object variables
            GetErrorMessage(objCa200.SetConfiguration(CA_ID, connect_probe, port_auto, baudrate, interface_USB));
            GetErrorMessage(objCa200.get_Cas(ref objCas));
            GetErrorMessage(objCas.get_Item(1, ref objCa));
            GetErrorMessage(objCa.get_Probes(ref objProbes));
            GetErrorMessage(objCa.get_OutputProbes(ref objOutputProbes));
            GetErrorMessage(objCa.get_Memory(ref objMemory));
            GetErrorMessage(objProbes.get_Item(1, ref objProbe));
            GetErrorMessage(objOutputProbes.AddAll());
            GetErrorMessage(objOutputProbes.get_Item(1, ref objProbe));

            autoconnectflag = false;
        }

        ///<summary>
        ///[SetSDK and connect device]
        ///This method connect CA-410 Automatically and set objects 
        ///</summary>
        private static void AutoConnect()
        {
            objCa200 = new CASDK2Ca200();   // Generate application object

            GetErrorMessage(objCa200.AutoConnect());
            // Substitute object variables
            //GetErrorMessage(objCa200.get_Cas(ref objCas));
            GetErrorMessage(objCa200.get_SingleCa(ref objCa));
            //GetErrorMessage(objCa.get_Probes(ref objProbes));
            //GetErrorMessage(objCa.get_OutputProbes(ref objOutputProbes));
            GetErrorMessage(objCa.get_Memory(ref objMemory));
            GetErrorMessage(objCa.get_SingleProbe(ref objProbe));

            autoconnectflag = true;
        }

        ///<summary>
        ///[SetSDK and connect device]
        ///This method disconnect CA-410 and delete objects 
        ///</summary>
        private static void Disconnect()
        {
            while (!triggerfinish)
            {
                System.Threading.Thread.Sleep(10);  //wait for completion of trigger measurement
            }
            //Disconnect CA-410
            if (autoconnectflag)
            {
                GetErrorMessage(objCa200.AutoDisconnect()); //Disconnect probe connected automatically
            }
            else
            {
                GetErrorMessage(objCa200.DisconnectAll());  //Disconnect probe connected manually
            }
        }

        ///<summary>
        ///[Set measurement conditions]
        ///This method set measurement configuration 
        ///</summary>
        private static void DefaultSetting()
        {
            int freqmode = 4;   // SyncMode : INT 
            double freq = 60.0; //frequency = 60.0Hz
            int speed = 1;      //Measurement speed : FAST
            int Lvmode = 1;     //Lv : cd/m2

            GetErrorMessage(objCa.CalZero());                       //Zero-Calibration
            MessageBox.Show("调零已完成");
            GetErrorMessage(objCa.put_DisplayProbe("P1"));          //Set display probe to P1
            GetErrorMessage(objCa.put_SyncMode(freqmode, freq));    //Set sync mode and frequency
            GetErrorMessage(objCa.put_AveragingMode(speed));        //Set measurement speed
            GetErrorMessage(objCa.put_BrightnessUnit(Lvmode));      //SetBrightness unit

            string PID = "";
            string dispprobe = "";
            int syncmode = 0;
            double syncfreq = 0.0;
            int measspeed = 0;

            //Get settings
            GetErrorMessage(objCa.get_PortID(ref PID));                             //Get connection interface
            //Console.WriteLine("PortID:" + PID);
            GetErrorMessage(objCa.get_DisplayProbe(ref dispprobe));                 //Get display probe
            //Console.WriteLine("DisplayProbe:" + dispprobe);
            GetErrorMessage(objCa.get_SyncMode(ref syncmode, ref syncfreq));        //Get sync mode and frequency
            //Console.WriteLine("SyncMode:" + syncmode + ",Syncfreq:" + syncfreq);
            GetErrorMessage(objCa.get_AveragingMode(ref measspeed));                //Get measurement speed
            //Console.WriteLine("MeasurementSpeed:" + measspeed);
        }

        ///<summary>
        ///[Measurement]
        ///This method measure color and flicker(JEITA and FMA) by CH1 
        ///</summary>
        private static void Measurement()
        {
            //SetZeroCalEvent();
            int chnum = 0;      //CalibrationCH : 1
            GetErrorMessage(objMemory.put_ChannelNO(chnum));

            //measurement result

        
            //double Lv = 0.0;
            //double sx = 0.0;
            //double sy = 0.0;
            //double X = 0.0;
            //double Y = 0.0;
            //double Z = 0.0;
            //double JEITA = 0.0;
            //double FMA = 0.0;

            GetErrorMessage(objCa.put_DisplayMode(MODE_Lvxy));  //Set mode:Color Lvxy

            GetErrorMessage(objCa.Measure());                   //Color measurement

            //Get Color result
            GetErrorMessage(objProbe.get_Lv(ref Lv));
            GetErrorMessage(objProbe.get_sx(ref sx));
            GetErrorMessage(objProbe.get_sy(ref sy));
            GetErrorMessage(objProbe.get_X(ref X));
            GetErrorMessage(objProbe.get_Y(ref Y));
            GetErrorMessage(objProbe.get_Z(ref Z));


            //LvValue = Lv;




            GetErrorMessage(objCa.put_DisplayMode(MODE_JEITA)); //Set mode:Flicker JEITA
            GetErrorMessage(objCa.Measure());                   //JEITA measurement

            //Get JEITA result

            GetErrorMessage(objProbe.get_FlckrJEITA(ref JEITA));

            GetErrorMessage(objCa.put_DisplayMode(MODE_FMA));   //Set mode:Flicker FMA
            GetErrorMessage(objCa.Measure());                   //FMA measurement

            //Get FMA result
            GetErrorMessage(objProbe.get_FlckrFMA(ref FMA));

            //Console.WriteLine("Lv:" + Lv + " x:" + sx + " y:" + sy);
            //Console.WriteLine("X:" + X + " Y:" + Y + " Z:" + Z);
            //Console.WriteLine("JEITA:" + JEITA + " FMA:" + FMA);

            //GetErrorMessage(objCa.put_DisplayMode(MODE_Lvxy));  //Set mode:Color Lvxy




        }

        ///<summary>
        ///[Calibration]
        ///This method execute RGB + W Matrix calibration
        ///This method measure 4 color without wait time
        ///</summary>
        private static void MatrixCalibration()
        {
            //Setting
            GetErrorMessage(objCa.put_DisplayProbe("P1"));      //Set display probe to P1
            GetErrorMessage(objMemory.put_ChannelNO(1));        //CalibrationCH 1 
            GetErrorMessage(objCa.put_DisplayMode(MODE_Lvxy));  //Set Color mode
            GetErrorMessage(objCa.CalZero());                   //Zero calibration

            //Set calibration mode
            GetErrorMessage(objCa.SetLvxyCalMode());

            //Measurement of 4 color
            GetErrorMessage(objCa.CalibMeasure(RED));           //Measure RED 
            GetErrorMessage(objCa.CalibMeasure(GREEN));         //Measure GREEN
            GetErrorMessage(objCa.CalibMeasure(BLUE));          //Measure BLUE
            GetErrorMessage(objCa.CalibMeasure(WHITE));         //Measure WHITE

            double Lv = 0.0;
            double x = 0.0;
            double y = 0.0;

            //Enter calibration target value

            Lv = 34.05;
            x = 0.6300;
            y = 0.3438;
            GetErrorMessage(objCa.SetLvxyCalData(RED, x, y, Lv));   //Set red target values

            Lv = 118.57;
            x = 0.2865;
            y = 0.6357;

            GetErrorMessage(objCa.SetLvxyCalData(GREEN, x, y, Lv)); //Set green target values

            Lv = 9.757;
            x = 0.1537;
            y = 0.0433;

            GetErrorMessage(objCa.SetLvxyCalData(BLUE, x, y, Lv));  //Set blue target values

            Lv = 160.1;
            x = 0.2952;
            y = 0.3253;

            GetErrorMessage(objCa.SetLvxyCalData(WHITE, x, y, Lv)); //Set white target values

            //Update calibration CH
            GetErrorMessage(objCa.Enter());   //Complete the matrix calibration
        }

        ///<summary>
        ///[Calibration]
        ///This method execute Single-point calibration
        ///This method measure single color without wait time
        ///</summary>
        private static void SingleCalibration()
        {
            //Setting
            GetErrorMessage(objCa.put_DisplayProbe("P1"));      //Set display probe to P1
            GetErrorMessage(objMemory.put_ChannelNO(2));        //CalibrationCH 2 
            GetErrorMessage(objCa.put_DisplayMode(MODE_Lvxy));  //Set color mode
            GetErrorMessage(objCa.CalZero());                   //Zero calibration

            //Set calibration mode
            GetErrorMessage(objCa.SetLvxyCalMode());
            //Measure
            //Use WHITE for Single-point calibration
            GetErrorMessage(objCa.CalibMeasure(WHITE));

            //Enter calibration target value
            //Use WHITE for Single-point calibration
            double Lv = 160.1;
            double x = 0.2952;
            double y = 0.3253;
            GetErrorMessage(objCa.SetLvxyCalData(WHITE, x, y, Lv));

            //Update calibration CH
            GetErrorMessage(objCa.Enter());   //Complete the single calibration
        }

        ///<summary>
        ///[Set Target]
        ///This method get target value as Lvxy of specified calibration CH
        ///</summary>
        ///<param name = "ChannelNO">Calibration CH No. for getting target value</param>
        ///<param name = "PID">Probe ID for getting target value</param>
        ///<param name = "refx">Acquired target value x</param>
        ///<param name = "refy">Acquired target value y</param>
        ///<param name = "refLv">Acquired target value Lv</param>
        private static void GetTarget(int ChannlNO, string PID, ref double refx, ref double refy, ref double refLv)
        {

            int tempCH = 0;

            GetErrorMessage(objMemory.get_ChannelNO(ref tempCH));                               //Get current calibration CH

            GetErrorMessage(objMemory.put_ChannelNO(ChannlNO));                                 //Set calibration CH to specified number
            GetErrorMessage(objMemory.GetReferenceColor(PID, ref refx, ref refy, ref refLv));   //Get current Target values

            GetErrorMessage(objMemory.put_ChannelNO(tempCH));                                   //Return original calibration CH 
        }

        ///<summary>
        ///[Set Target]
        ///This method set target value by measurement
        ///</summary>
        private static void SetTargetbyMeasure()
        {
            //Setting
            int chnum = 3;
            string probeID = "P1";
            GetErrorMessage(objCa.put_DisplayProbe(probeID));       //Set display probe
            GetErrorMessage(objMemory.put_ChannelNO(chnum));        //Set calibration CH number
            GetErrorMessage(objCa.put_DisplayMode(MODE_Lvxy));      //Set color mode
            //Set normal mode
            GetErrorMessage(objCa.ResetLvxyCalMode());              //Set normal mode not calibration mode

            GetErrorMessage(objCa.CalZero());                       //Zero calibration
            //Measure
            GetErrorMessage(objCa.Measure());
            //Update calibration CH
            GetErrorMessage(objCa.Enter());                         //Last measurement data become target values

            double Lv = 0.0;
            double x = 0.0;
            double y = 0.0;
            GetTarget(chnum, probeID, ref x, ref y, ref Lv);

            Console.WriteLine("Target Lv:" + Lv + " x:" + x + " y:" + y);
        }

        ///<summary>
        ///[Set Target]
        ///This method set target value by input
        ///</summary>
        private static void SetTargetbyInput()
        {
            //Setting
            int chnum = 2;
            string probeID = "P1";
            GetErrorMessage(objCa.put_DisplayProbe(probeID));                   //Set display probe
            GetErrorMessage(objMemory.put_ChannelNO(chnum));                    //Set calibration CH number
            //Set normal mode
            GetErrorMessage(objCa.ResetLvxyCalMode());                          //Set normal mode not calibration mode

            //Enter Target value and update calibration CH
            double refLv = 100.0;
            double refx = 0.33;
            double refy = 0.30;
            GetErrorMessage(objCa.SetLvxyCalData(WHITE, refx, refy, refLv));    //Set target values and update Calibration CH

            double Lv = 0.0;
            double x = 0.0;
            double y = 0.0;
            GetTarget(chnum, probeID, ref x, ref y, ref Lv);

            Console.WriteLine("Target Lv:" + Lv + " x:" + x + " y:" + y);
        }

        ///<summary>
        ///[Set Zero Calibration event]
        ///This method execute zerocalibration when temperature changes significantly
        ///</summary>
        private static int ExeCalZero(int dummy)
        {
            Console.WriteLine("Performing Zero Calibration");
            GetErrorMessage(objCa.CalZero());   //Zero calibration
            return err;
        }

        ///<summary>
        ///[Set Zero Calibration event]
        ///This method set zero calibration event
        ///</summary>
        private static void SetZeroCalEvent()
        {
            Func<int, int> funczerocal = ExeCalZero;
            GetErrorMessage(objCa.SetExeCalZeroCallback(funczerocal));      //Set function for zero calibration event
        }

        ///<summary>
        ///[Errorhandling]
        ///This method display Error message from Error number
        ///</summary>
        ///<param name = "errornum">Error number from API of SDK</param>
        private static void GetErrorMessage(int errornum)
        {
            string errormessage = "";
            if (errornum != 0)
            {
                //Get Error message from Error number
                err = GlobalFunctions.CASDK2_GetLocalizedErrorMsgFromErrorCode(0, errornum, ref errormessage);
                Console.WriteLine(errormessage);
            }
        }

        ///<summary>
        ///[Errorhandling]
        ///This method display Error message from Status Code
        ///</summary>
        private static void GetErrorMessageByStatusCode(int statuscode)
        {
            string errormessage = "";
            if (statuscode != 0)
            {
                //Get Error message from Error number
                err = GlobalFunctions.CASDK2_GetLocalizedErrorMsgFromMeasurementErrorCode(0, statuscode, ref errormessage);
                Console.WriteLine(errormessage);
            }
        }

        ///<summary>
        ///[Set Trigger event]
        ///This method get trigger finished CA number
        ///<param name = "canumber">Trigger result status</param>
        ///</summary>
        private static int GetTriggernum(int canumber)
        {
            Console.WriteLine("TrgrMeasCallback instance number = " + canumber);
            return 0;
        }

        ///<summary>
        ///[Set Trigger event]
        ///This method get trigger measurement data when trigger measurement is finished
        ///<param name = "result">Trigger result status</param>
        ///</summary>
        public static int TrgrMeasResult(int result)
        {
            const int TRGR_RESULT_SUCCEEDED = 0;
            const int TRGR_RESULT_TIMEOUT = -1;
            const int TRGR_RESULT_CANCELLED = -2;


            if (objCa != null)
            {
                //Trigger result handling
                if (result == TRGR_RESULT_TIMEOUT)              //Result for timeout
                {
                    Console.WriteLine("result: Timed Out");
                }
                else if (result == TRGR_RESULT_CANCELLED)       //Result for trigger cancelld
                {
                    Console.WriteLine("result: Cancelled");
                }
                else if (result == TRGR_RESULT_SUCCEEDED)       //Result for measurement succeeded
                {

                    Console.WriteLine("result: Success");
                    double Lv = 0.0;
                    double sx = 0.0;
                    double sy = 0.0;
                    double X = 0.0;
                    double Y = 0.0;
                    double Z = 0.0;
                    double JEITA = 0.0;
                    double FMA = 0.0;
                    GetErrorMessage(objProbe.get_Lv(ref Lv));
                    GetErrorMessage(objProbe.get_sx(ref sx));
                    GetErrorMessage(objProbe.get_sy(ref sy));
                    GetErrorMessage(objProbe.get_X(ref X));
                    GetErrorMessage(objProbe.get_Y(ref Y));
                    GetErrorMessage(objProbe.get_Z(ref Z));
                    GetErrorMessage(objProbe.get_FlckrJEITA(ref JEITA));
                    GetErrorMessage(objProbe.get_FlckrFMA(ref FMA));

                    Console.WriteLine("Lv：" + Lv + " x:" + sx + " y:" + sy);
                    Console.WriteLine("X:" + X + " Y:" + Y + " Z:" + Z);
                    Console.WriteLine("JEITA:" + JEITA + " FMA:" + FMA);

                }
            }
            else
            {
                Console.WriteLine("Global Ca variable is null!");
            }
            triggerfinish = true;
            return 0;
        }

        ///<summary>
        ///[Set Trigger event]
        ///This method set trigger event or cancel for probe trigger mode.
        ///</summary>
        ///<param name = "trigger">Trigger ON(=true) or OFF(=false)</param>
        private static void SetTrigger(bool trigger)
        {
            if (trigger)
            {
                int delay = 0;
                GetErrorMessage(objCa.put_ExtlTrgrMeasDelay(delay));                        //Set trigger delay time

                Func<int, int> functrigger = GetTriggernum;
                Func<int, int> funcresult = TrgrMeasResult;
                GetErrorMessage(objCa.SetTrgrMeasReceivedCallback(functrigger, funcresult)); //Set function for trigger finished event
                triggerfinish = false;
            }
            else
            {
                System.Threading.Thread.Sleep(10);
            }
            GetErrorMessage(objCa.EnableExtlTrgrMeas(trigger));                             //Set Trigger mode
        }

        ///<summary>
        ///[Get data of dataprocessor]
        ///This method get saved data of data processor.
        ///</summary>
        private static void GetSaveData()
        {
            int DataNo = 1;
            CASDK2MeasData[] measData = new CASDK2MeasData[10];

            //Get data of all probes
            GetErrorMessage(objCa.GetMeasurementData(DataNo, ref measData));

            for (int probe = 0; probe < 10; probe++)
            {
                Console.WriteLine("Probe" + (probe + 1));
                Console.WriteLine("Lv:" + measData[probe].data.lv);
                Console.WriteLine("x:" + measData[probe].data.sx);
                Console.WriteLine("y:" + measData[probe].data.sy + "\r\n");
            }
        }

        ///<summary>
        ///[Get data of dataprocessor]
        ///This method get log data of data processor.
        ///</summary>
        private static void GetLogData()
        {
            int lognumber = 0;
            int probenum = 1;
            const int dataset = 10;

            //Get number of log data
            GetErrorMessage(objCa.GetIntervalLogNum(ref lognumber));
            CASDK2IntervalMetadata[] intervalMetadata = new CASDK2IntervalMetadata[lognumber];

            for (int lnum = 0; lnum < lognumber; lnum++)
            {
                //Get information of specified log data
                GetErrorMessage(objCa.GetIntervalMetadata(lnum + 1, ref intervalMetadata[lnum]));
                int total = intervalMetadata[lnum].totalNumMeas;
                CASDK2IntervalData[] intervalData = new CASDK2IntervalData[total];
                for (int measnum = 0; measnum < total; measnum += dataset)
                {
                    //Get measurement result of log data
                    GetErrorMessage(objCa.GetIntervalData(lnum + 1, probenum, measnum + 1, ref intervalData));
                }
                for (int measnum = 0; measnum < total; measnum++)
                {
                    //Display results
                    Console.WriteLine("Lv:" + intervalData[measnum].measdata.data.lv);
                    Console.WriteLine("Lv:" + intervalData[measnum].measdata.data.sx);
                    Console.WriteLine("Lv:" + intervalData[measnum].measdata.data.sy + "\r\n");
                }
            }
        }

        ///<summary>
        ///[Set Weighting Factor]
        ///This method set weighting factor for JEITA.
        ///</summary>
        private static void SetJEITASensitivity()
        {
            int profile = 1;
            const string strFileName2 = ".\\EyeSensitivity.txt";
            //Load file of Weighting Factor 
            GetErrorMessage(objCa.LoadFlickerSensitivityFactorFromFile(profile, strFileName2));
            //Set Weighting Factor 
            GetErrorMessage(objCa.SetCurrentFlickerSensitivityFactorProfile(profile));
        }

        ///<summary>
        ///[Waveform Measurement]
        ///This method measure waveform 
        ///</summary>
        private static void WaveformMeasurement()
        {
            int chnum = 0;      //CalibrationCH : 0
            GetErrorMessage(objMemory.put_ChannelNO(chnum));
            GetErrorMessage(objCa.put_DisplayMode(MODE_Waveform2));        //Set mode:Waveform

            //Waveform settings
            int samplingcount = 11; //2048
            int samplinginterval = 1;
            int sensor = 1; //Y sensor
            int autoWVFMparam = 0;  //not auto
            int weightingfactor = 0;    //non setting
            GetErrorMessage(objCa.SetWaveformParams(samplingcount, samplinginterval, sensor));
            GetErrorMessage(objCa.SetWaveformConditionSwitch(autoWVFMparam));
            GetErrorMessage(objCa.SetWaveformFilterType(weightingfactor));

            GetErrorMessage(objCa.Measure());                      //Waveform measurement

            //Get flicker spectrum result in waveform
            int totalspectrumcount = 0;
            double minfreq = 0.0;
            double maxfreq = 0.0;
            double freqpitch = 0.0;
            GetErrorMessage(objProbe.GetFlickerDerivedParams(ref totalspectrumcount, ref minfreq, ref maxfreq, ref freqpitch));    //Get Waveform parameters
            double peakfreq = 0.0;
            double[] spectrumfreq = new double[totalspectrumcount];
            double[] flickerspectrum = new double[totalspectrumcount];
            double flicker = 0.0;
            GetErrorMessage(objProbe.get_PeakFrequency(ref peakfreq));        //Get peak flicker frequency

            const double MAX_FREQ = 200.0;
            for (int count = 0; count < totalspectrumcount; count++)
            {
                spectrumfreq[count] = minfreq + freqpitch * count;
                if (spectrumfreq[count] > MAX_FREQ)
                {
                    break;
                }
                GetErrorMessage(objProbe.GetSpectrum(spectrumfreq[count], ref flicker));  //Get flicker spectrum in Waveform
                flickerspectrum[count] = flicker;
                //Console.WriteLine(spectrumfreq[count] + " [Hz] \t\t" + flickerspectrum[count] + " [dB]");
            }
            int peaknumber = (int)(peakfreq / freqpitch);
            Console.WriteLine("Flicker:" + spectrumfreq[peaknumber] + " [Hz] \t\t" + flickerspectrum[peaknumber] + " [dB]");

            //Get Waveform result
            uint waveformdatasize = 0;
            GetErrorMessage(objProbe.get_WaveformDataNumber(ref waveformdatasize));   //Get number of Waveform data
            double[] waveformdata = new double[waveformdatasize];
            double[] weightingwaveformdata = new double[waveformdatasize];
            double measurementtime = 0.0;
            GetErrorMessage(objProbe.GetWaveformData(waveformdata, weightingwaveformdata, ref waveformdatasize, ref measurementtime));   //Get Waveform data
            double samplingpitch = measurementtime / waveformdatasize;
            for (int count = 0; count < waveformdatasize; count++)
            {
                //Console.WriteLine("luminance:" + (samplingpitch * count) + "[sec] \t\t" + waveformdata[count]);
            }

            GetErrorMessage(objCa.put_DisplayMode(MODE_Lvxy)); //Set mode:Color Lvxy
        }

        ///<summary>
        ///[Frequency detection]
        ///This method detect the emission frequency in the range from 10.00 to 240.00 [Hz]
        ///</summary>
        private static void EmissionFrequencyDetection()
        {
            int statuscode = NOERR;
            int freqmode = 4;           // SyncMode : INT 
            double detectedemissionfreq = 0.0;

            GetErrorMessage(objCa.DetectEmissionFrequency());                                  //Start emission frequency detection
            GetErrorMessage(objProbe.GetDetectedEmissionFrequency(ref detectedemissionfreq));  //Get detected frequency
            Console.WriteLine("Detected Emission frequency:" + detectedemissionfreq + " [Hz]");
            GetErrorMessage(objProbe.get_RD(ref statuscode));                                  //Get status code

            if (statuscode == NOERR)
            {
                GetErrorMessage(objCa.put_SyncMode(freqmode, detectedemissionfreq));           //Set sync mode and frequency
            }
            else
            {
                GetErrorMessageByStatusCode(statuscode);
            }
        }

        ///<summary>
        ///[Frequency detection]
        ///This method detect the VSync frequency in the range of the design value ±2.00 Hz.
        ///</summary>
        private static void VSyncFrequencyDetection()
        {
            int statuscode = NOERR;
            int freqmode = 4;           // SyncMode : INT 
            double designfreq = 60.0;
            double detectedVSync = 0.0;

            GetErrorMessage(objCa.DetectVSync(designfreq));                                    //Start VSync detection
            GetErrorMessage(objProbe.GetDetectedVSync(ref detectedVSync));                     //Get detected VSync
            Console.WriteLine("Detected VSync:" + detectedVSync + " [Hz]");
            GetErrorMessage(objProbe.get_RD(ref statuscode));                                  //Get status code

            if (statuscode == NOERR)
            {
                GetErrorMessage(objCa.put_SyncMode(freqmode, detectedVSync));                  //Set sync mode and frequency
            }
            else
            {
                GetErrorMessageByStatusCode(statuscode);
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
        
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void Ca_Lv_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
