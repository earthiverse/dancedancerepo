using System;
using System.Threading;
using System.Windows;
using Microsoft.Kinect;

namespace kinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            KinectSensor sensor = KinectSensor.GetDefault();
            sensor.Open();

            Output.Text += "Step on the left arrow with your left foot, and the right arrow with your right foot.\n";
            Show();
            Thread.Sleep(10);

            // Calibrate
            using (KinectCalibrator calibrator = new KinectCalibrator())
            {
                CameraSpacePoint leftFoot = calibrator.LeftFoot;
                CameraSpacePoint rightFoot = calibrator.RightFoot;
                Output.Text += String.Format("Left Foot: X:{0}, Y:{1}, Z:{2}\n", leftFoot.X, leftFoot.Y, leftFoot.Z);
                Output.Text += String.Format("Right Foot: X:{0}, Y:{1}, Z:{2}\n", rightFoot.X, rightFoot.Y, rightFoot.Z);
            }
            UpdateLayout();
            Thread.Sleep(120);

            //// Start
            //KinectDriver steps = new KinectDriver();
            //Debug.WriteLine(steps.ToString());
        }
    }
}
