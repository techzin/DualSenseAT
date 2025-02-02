﻿using Ets2SdkClient;
using Memory;
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


namespace DualSenseAT
{
    public partial class Form1 : Form
    {
        public Ets2SdkTelemetry Telemetry;

        public Form1()
        {
            InitializeComponent();

            Telemetry = new Ets2SdkTelemetry();
            Telemetry.Data += Telemetry_Data;

            Telemetry.JobFinished += TelemetryOnJobFinished;
            Telemetry.JobStarted += TelemetryOnJobStarted;

            if (Telemetry.Error != null)
            {
                //lbGeneral.Text =
                    //"General info:\r\nFailed to open memory map " + Telemetry.Map +
                      //  " - on some systems you need to run the client (this app) with elevated permissions, because e.g. you're running Steam/ETS2 with elevated permissions as well. .NET reported the following Exception:\r\n" +
                      //  Telemetry.Error.Message + "\r\n\r\nStacktrace:\r\n" + Telemetry.Error.StackTrace;
            }
        }

        private void TelemetryOnJobFinished(object sender, EventArgs args)
        {
            //MessageBox.Show("Job finished, or at least unloaded nearby cargo destination.");
        }

        private void TelemetryOnJobStarted(object sender, EventArgs e)
        {
            //MessageBox.Show("Just started job OR loaded game with active.");
        }

        private void Telemetry_Data(Ets2Telemetry data, bool updated)
        {


            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new TelemetryData(Telemetry_Data), new object[2] { data, updated });
                    return;
                }

                if (gameList.SelectedIndex < 0 || gameList.SelectedItem.ToString() != "Euro Truck Simulator 2 (Steam)")
                {
                   // Controller.WriteController.ResetTrigger(Controller.Triggers.LeftTrigger);
                   // Controller.WriteController.ResetTrigger(Controller.Triggers.RightTrigger);
                    return;
                }
                   
                debuglbl.Text = "Paused? " + data.Paused;


                if (data.Paused == true)
                {
                    Controller.WriteController.ResetTrigger(Controller.Triggers.LeftTrigger);
                    Controller.WriteController.ResetTrigger(Controller.Triggers.RightTrigger);
                    return;
                }
                else {
                    if (data.Drivetrain.SpeedKmh < 1)
                    {
                        Controller.WriteController.SetRightTrigger(Controller.Types.Resistance, "(0)(8)");
                        Controller.WriteController.SetLeftTrigger(Controller.Types.Normal, "(0)(0)(0)(0)(0)(0)"); //0,9,4,3,19,2

                    }
                    else if (data.Drivetrain.SpeedKmh < 5)
                    {
                        Controller.WriteController.SetRightTrigger(Controller.Types.Resistance, "(0)(7)");
                        Controller.WriteController.SetLeftTrigger(Controller.Types.Normal, "(0)(0)(0)(0)(0)(0)"); //0,9,4,3,19,2

                    }

                    else if (data.Drivetrain.SpeedKmh < 10)
                    {
                        Controller.WriteController.SetRightTrigger(Controller.Types.Resistance, "(0)(7)");
                        Controller.WriteController.SetLeftTrigger(Controller.Types.Machine, "(0)(9)(4)(3)(19)(2)");

                    }

                    else if (data.Drivetrain.SpeedKmh < 15)
                    {
                        Controller.WriteController.SetRightTrigger(Controller.Types.Resistance, "(0)(6)");
                        Controller.WriteController.SetLeftTrigger(Controller.Types.Machine, "(0)(9)(4)(3)(19)(2)");
                    }
                    else if (data.Drivetrain.SpeedKmh < 20)
                    {
                        Controller.WriteController.SetRightTrigger(Controller.Types.Resistance, "(0)(5)");
                        Controller.WriteController.SetLeftTrigger(Controller.Types.Machine, "(0)(9)(4)(3)(19)(2)");
                    }
                    else if (data.Drivetrain.SpeedKmh < 25)
                    {
                        Controller.WriteController.SetRightTrigger(Controller.Types.Resistance, "(0)(4)");
                        Controller.WriteController.SetLeftTrigger(Controller.Types.Machine, "(0)(9)(4)(3)(19)(2)");
                    }
                    else if (data.Drivetrain.SpeedKmh < 30)
                    {
                        Controller.WriteController.SetRightTrigger(Controller.Types.Resistance, "(0)(3)");
                        Controller.WriteController.SetLeftTrigger(Controller.Types.Machine, "(0)(9)(4)(3)(19)(2)");
                    }
                    else if (data.Drivetrain.SpeedKmh < 35)
                    {
                        Controller.WriteController.SetRightTrigger(Controller.Types.Resistance, "(0)(2)");
                        Controller.WriteController.SetRightTrigger(Controller.Types.Normal);
                    }

                    else if (data.Drivetrain.SpeedKmh < 40)
                    {
                        Controller.WriteController.SetRightTrigger(Controller.Types.Resistance, "(0)(1)");
                        Controller.WriteController.SetRightTrigger(Controller.Types.Normal);
                    }
                }
            }
            catch
            {
            }
        }


        private Mem memLib = new Mem();

        private int processID;
        private bool processOpen = false;

        private bool validNumber = false;
        private int temporaryNumb = 0;

        public struct Controller
        {
            public enum Types
            {
                Normal = 1,
                Hard = 2,
                VeryHard = 3,
                Machine = 4,
                Resistance = 5,
            }
            public enum Triggers
            {
                LeftTrigger = 1,
                RightTrigger = 2,
            }

            public struct WriteController
            {
                public static void SetRightTrigger(Controller.Types type, string customargs = "(0)(0)")
                {
                    var MyIni = new IniFile(@"C:\DualsenseX_GameTriggers.txt");

                    switch (type)
                    {
                        case Controller.Types.Normal: 
                            MyIni.Write("RightTrigger", "Normal");
                            break;
                        case Controller.Types.Hard:
                            MyIni.Write("RightTrigger", "Hard");
                            break;
                        case Controller.Types.VeryHard:
                            MyIni.Write("RightTrigger", "VeryHard");
                            break;
                        case Controller.Types.Resistance:
                            MyIni.Write("RightTrigger", "Resistance");
                            MyIni.Write("ForceRightTrigger", customargs);
                            break;
                    }
                }

                public static void SetLeftTrigger(Controller.Types type, string customargs = "(0)(0)")
                {
                    var MyIni = new IniFile(@"C:\DualsenseX_GameTriggers.txt");

                    switch (type)
                    {
                        case Controller.Types.Normal:
                            MyIni.Write("LeftTrigger", "Normal");
                            break;
                        case Controller.Types.Hard:
                            MyIni.Write("LeftTrigger", "Hard");
                            break;
                        case Controller.Types.VeryHard:
                            MyIni.Write("LeftTrigger", "VeryHard");
                            break;
                        case Controller.Types.Machine:
                            MyIni.Write("LeftTrigger", "Machine");
                            MyIni.Write("ForceLeftTrigger", customargs);
                            break;
                    }
                }

                public static void ResetTrigger(Controller.Triggers trigger)
                {
                    var MyIni = new IniFile(@"C:\DualsenseX_GameTriggers.txt");
                    switch (trigger)
                    {
                        case Controller.Triggers.LeftTrigger:
                            MyIni.Write("LeftTrigger", "Normal");
                            break;
                        case Controller.Triggers.RightTrigger:
                            MyIni.Write("RightTrigger", "Normal");
                            break;
                    }
                }
            }
        }


        private void findProcessID(string procname)
        {
            processID = memLib.GetProcIdFromName(procname); //Gets process ID
            processOpen = memLib.OpenProcess(processID);

            if (processID > 0)
            {
                procIdLabel.Invoke((MethodInvoker)delegate
                {
                    procIdLabel.Text = processID.ToString();
                    procIdLabel.ForeColor = Color.Lime;
                });

                gameProcessNameLabel.Invoke((MethodInvoker)delegate {
                    gameProcessNameLabel.Text = procname + ".exe";
                    gameProcessNameLabel.ForeColor = Color.Lime;
                });
            }
            else
            {
                procIdLabel.Invoke((MethodInvoker)delegate
                {
                    procIdLabel.Text = "Process ID Not Found";
                    procIdLabel.ForeColor = Color.Red;
                });

                gameProcessNameLabel.Invoke((MethodInvoker)delegate {
                    gameProcessNameLabel.Text = "N/A";
                    gameProcessNameLabel.ForeColor = Color.Red;
                });
            }

            if (processOpen)
            {

                procOpenLabel.Invoke((MethodInvoker)delegate
                {
                    procOpenLabel.Text = "JOGO ENCONTRADO";
                    procOpenLabel.ForeColor = Color.Lime;
                });

            }
            else
            {
                procOpenLabel.Invoke((MethodInvoker)delegate
                {
                    procOpenLabel.Text = "N/A";
                    procOpenLabel.ForeColor = Color.Red;
                });
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

           

        }

        private void ResidentEvil5Steam()
        {
            var MyIni = new IniFile(@"C:\DualsenseX_GameTriggers.txt");

            int ammo = memLib.ReadInt("re5dx9.exe+00DA2604,84,238,CE4,5C,36C,0,F40");
            // MyIni.Write("RightTrigger", "Resistance");
            MyIni.Write("LeftTrigger", "Resistance");
            MyIni.Write("ForceLeftTrigger", "(4)(2)");

            if (ammo > 0)
            {
                Controller.WriteController.SetRightTrigger(Controller.Types.Hard);
              //  MyIni.Write("RightTrigger", "SemiAutomaticGun");
               // MyIni.Write("ForceLeftTrigger", "(2)(5)(2)");
            }
            else
                Controller.WriteController.SetRightTrigger(Controller.Types.Normal);

            Console.WriteLine("municao: " + ammo);
        }

        private void OnTime_Tick(object sender, EventArgs e)
        {
            if (!processOpen)
                return;
            if (gameList.SelectedIndex < 0)
                return;
            if (gameList.SelectedItem.ToString() == "Resident Evil 5 (Steam)")
                ResidentEvil5Steam();

            
        }

        private void gameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gameList.SelectedItem.ToString() == "Resident Evil 5 (Steam)")
                findProcessID("re5dx9");
            else if (gameList.SelectedItem.ToString() == "Euro Truck Simulator 2 (Steam)")
                findProcessID("eurotrucks2");
        }
    }
}
