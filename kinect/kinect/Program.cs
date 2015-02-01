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
            // Ready the Kinect
            KinectSensor sensor = KinectSensor.GetDefault();
            sensor.Open();
            
            // Start Calibration
            Console.WriteLine("Stand on the left and right arrows, facing forward.\n");

            // Calibrate Left/Right
            KinectCalibrator calibrator = new KinectCalibrator();

            Debug.WriteLine("Left/Right: {0}", DateTime.Now);

            CameraSpacePoint leftArrow = calibrator.LeftFoot;
            CameraSpacePoint rightArrow = calibrator.RightFoot;
            DepthSpacePoint leftDepth = sensor.CoordinateMapper.MapCameraPointToDepthSpace(leftArrow);
            DepthSpacePoint rightDepth = sensor.CoordinateMapper.MapCameraPointToDepthSpace(rightArrow);
            Console.WriteLine("Left Arrow: X:{0}, Y:{1}, Z:{2}", leftArrow.X, leftArrow.Y, leftArrow.Z);
            Console.WriteLine("Right Arrow: X:{0}, Y:{1}, Z:{2}", rightArrow.X, rightArrow.Y, rightArrow.Z);

            //CameraSpacePoint center = calibrator.Center;
            //Console.WriteLine("Center: X:{0}, Y:{1}, Z:{2}", center.X, center.Y, center.Z);
            
            Console.WriteLine("\nLeft + Right Calibrated!\n");
            Console.WriteLine("Turn 90 Degrees CW and stand on the up and down arrows.");

            // Give them some time to figure out what CW stands for
            Thread.Sleep(2500);
            
            // Calibrate Up/Down
            calibrator = new KinectCalibrator();

            Debug.WriteLine("Up/Down: {0}", DateTime.Now);

            // TODO: Figure out which direction they turned. Right now it assumes they turned 90 degrees clockwise
            // NOTE: It does ask them to turn 90 degrees CW, but it shouldn't have to
            // NOTE: It works a lot better when you're facing the kinect

            CameraSpacePoint upArrow = calibrator.LeftFoot;
            CameraSpacePoint downArrow = calibrator.RightFoot;
            DepthSpacePoint upDepth = sensor.CoordinateMapper.MapCameraPointToDepthSpace(upArrow);
            DepthSpacePoint downDepth = sensor.CoordinateMapper.MapCameraPointToDepthSpace(downArrow);
            Console.WriteLine("Up Arrow: X:{0}, Y:{1}, Z:{2}", upArrow.X, upArrow.Y, upArrow.Z);
            Console.WriteLine("Right Foot: X:{0}, Y:{1}, Z:{2}", downArrow.X, downArrow.Y, downArrow.Z);

            //center = calibrator.Center;
            //Console.WriteLine("Center: X:{0}, Y:{1}, Z:{2}", center.X, center.Y, center.Z);

            Console.WriteLine("\nUp + Down Calibrated!\n");

            // Compute where the center should be
            CameraSpacePoint center = KinectHelper.Average2CameraSpacePoints(upArrow, rightArrow);
            center = KinectHelper.Average2CameraSpacePoints(center, downArrow, 3);
            center = KinectHelper.Average2CameraSpacePoints(center, leftArrow, 4);
            DepthSpacePoint centerDepth = sensor.CoordinateMapper.MapCameraPointToDepthSpace(center);

            //KinectDepthDriver depthDriver = new KinectDepthDriver(upDepth, rightDepth, downDepth, leftDepth, centerDepth);
            //depthDriver.Calibrate();
            //depthDriver.Start();

            KinectDriver driver = new KinectDriver(upArrow, rightArrow, downArrow, leftArrow, calibrator.BodyFrameReader);
            driver.Start();
        }
    }
}
