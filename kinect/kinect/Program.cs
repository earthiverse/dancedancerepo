using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Kinect;

namespace kinect
{
    class Program
    {
        static void Main(string[] args)
        {
            KinectSensor sensor = KinectSensor.GetDefault();
            sensor.Open();

            Console.WriteLine("Stand on the left and right arrows, facing forward.\n");

            // Calibrate Left/Right
            KinectCalibrator calibrator = new KinectCalibrator();
            Debug.WriteLine("Left/Right: {0}", DateTime.Now);
            CameraSpacePoint leftArrow = calibrator.LeftFoot;
            CameraSpacePoint rightArrow = calibrator.RightFoot;
            CameraSpacePoint center = calibrator.Center;
            Console.WriteLine("Left Arrow: X:{0}, Y:{1}, Z:{2}", leftArrow.X, leftArrow.Y, leftArrow.Z);
            Console.WriteLine("Right Arrow: X:{0}, Y:{1}, Z:{2}", rightArrow.X, rightArrow.Y, rightArrow.Z);
            Console.WriteLine("Center: X:{0}, Y:{1}, Z:{2}", center.X, center.Y, center.Z);
            
            Console.WriteLine("\nLeft + Right Calibrated!\n");
            Console.WriteLine("Turn 90 Degrees CW and stand on the up and down arrows.");

            // Give them some time to figure out what CCW means
            Thread.Sleep(2500);
            
            // Calibrate Up/Down
            calibrator = new KinectCalibrator();
            Debug.WriteLine("Up/Down: {0}", DateTime.Now);

            // TODO: Figure out which direction they turned. Right now it assumes they turned 90 degrees clockwise
            // NOTE: It does ask them to turn 90 degrees CW, but it shouldn't have to
            // NOTE: It works a lot better when you're facing the kinect

            CameraSpacePoint upArrow = calibrator.LeftFoot;
            CameraSpacePoint downArrow = calibrator.RightFoot;
            center = calibrator.Center;
            Console.WriteLine("Up Arrow: X:{0}, Y:{1}, Z:{2}", upArrow.X, upArrow.Y, upArrow.Z);
            Console.WriteLine("Right Foot: X:{0}, Y:{1}, Z:{2}", downArrow.X, downArrow.Y, downArrow.Z);
            Console.WriteLine("Center: X:{0}, Y:{1}, Z:{2}", center.X, center.Y, center.Z);

            Console.WriteLine("\nUp + Down Calibrated!\n");

            KinectDriver driver = new KinectDriver(upArrow, rightArrow, downArrow, leftArrow, calibrator.BodyFrameReader);
            driver.Start();
        }
    }
}
