using System;
using WindowsInput;
using Microsoft.Kinect;

namespace kinect
{
    /// <summary>
    /// Drives the main functionality of the kinect dance pad.
    /// Used when running the game, matching input to 
    /// </summary>
    public class KinectDriver
    {
        // The Virtual Keyboard
        protected InputSimulator Keyboard;

        protected Body[] Bodies;

        public KinectDriver(CameraSpacePoint up, CameraSpacePoint right, CameraSpacePoint down, CameraSpacePoint left)
        {
            // Initialize the virtual keyboard
            Keyboard = new InputSimulator();

            // Find the connected Kinect
            KinectSensor kinect = KinectSensor.GetDefault();

            // Create a space to store skeleton data
            Bodies = new Body[kinect.BodyFrameSource.BodyCount];

            // Setup the kinect to read the skeletons
            BodyFrameReader reader = kinect.BodyFrameSource.OpenReader();
            reader.FrameArrived += ParseFrame;
        }

        private void ParseFrame(object sender, BodyFrameArrivedEventArgs bodyFrameArrivedEventArgs)
        {
            BodyFrame frame = bodyFrameArrivedEventArgs.FrameReference.AcquireFrame();
            using (frame)
            {
                // Check if frame is usable
                if (frame != null)
                {
                    frame.GetAndRefreshBodyData(Bodies);
                    foreach (Body body in Bodies)
                    {
                        // Check if the Kinect found a skeleton
                        if (body.IsTracked)
                        {
                            // NOTE: Kinect refers to the "foot" as the position of your toes
                            CameraSpacePoint leftFoot = body.Joints[JointType.FootLeft].Position;
                            CameraSpacePoint rightFoot = body.Joints[JointType.FootRight].Position;
                            
                            Console.WriteLine("{3} Left Foot: {0}, {1}, {2}", leftFoot.X, leftFoot.Y, leftFoot.Z, body.TrackingId);
                            Console.WriteLine("{3} Right Foot: {0}, {1}, {2}", rightFoot.X, rightFoot.Y, rightFoot.Z, body.TrackingId);
                        }
                    }
                }
            }
        }
    }
}
