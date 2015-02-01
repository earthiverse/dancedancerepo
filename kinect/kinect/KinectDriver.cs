using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
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
        protected readonly CameraSpacePoint Up;
        protected readonly CameraSpacePoint Right;
        protected readonly CameraSpacePoint Down;
        protected readonly CameraSpacePoint Left;

        protected readonly double HorizontalThreshold;
        protected readonly double VerticalThreshold;

        public ManualResetEvent StopTracking;

        protected KinectHelper.Arrows LastArrows = KinectHelper.Arrows.None;

        // The Virtual Keyboard
        protected InputSimulator Keyboard;

        protected Body[] Bodies;

        public KinectDriver(CameraSpacePoint up, CameraSpacePoint right, CameraSpacePoint down, CameraSpacePoint left, BodyFrameReader reader)
        {
            // Keep the calibration data
            Up = up;
            Right = right;
            Down = down;
            Left = left;
            HorizontalThreshold = KinectHelper.ComputeHorizontalThreshold(up, right, down, left);
            VerticalThreshold = KinectHelper.ComputeVerticalThreshold(up, right, down, left);

            StopTracking = new ManualResetEvent(false);
            // Initialize the virtual keyboard
            Keyboard = new InputSimulator();

            // Find the connected Kinect
            KinectSensor kinect = KinectSensor.GetDefault();

            // Create a space to store skeleton data
            Bodies = new Body[kinect.BodyFrameSource.BodyCount];

            // Setup the kinect to read the skeletons
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
                            CameraSpacePoint center = body.Joints[JointType.SpineBase].Position;

                            CameraSpacePoint leftFoot = body.Joints[JointType.FootLeft].Position;
                            CameraSpacePoint rightFoot = body.Joints[JointType.FootRight].Position;

                            Debug.WriteLine("{3} Left Foot: {0}, {1}, {2}", leftFoot.X, leftFoot.Y, leftFoot.Z, body.TrackingId);
                            Debug.WriteLine("{3} Right Foot: {0}, {1}, {2}", rightFoot.X, rightFoot.Y, rightFoot.Z, body.TrackingId);
                            Debug.WriteLine("{3} Center: {0}, {1}, {2}", center.X, center.Y, center.Z, body.TrackingId);

                            //KinectHelper.GetArrowsPressed(Up, Right, Down, Left, leftFoot, rightFoot, HorizontalThreshold, VerticalThreshold);
                            KinectHelper.Arrows arrows = KinectHelper.GetArrowsPressed(Up, Right, Down, Left, leftFoot, rightFoot, 0.075, 0.025);

                            if (arrows.HasFlag(KinectHelper.Arrows.Up) && !LastArrows.HasFlag(KinectHelper.Arrows.Up))
                            {
                                InputManager.Keyboard.KeyDown(Keys.Up);
                            }
                            else if (!arrows.HasFlag(KinectHelper.Arrows.Up) && LastArrows.HasFlag(KinectHelper.Arrows.Up))
                            {
                                InputManager.Keyboard.KeyUp(Keys.Up);
                            }
                            if (arrows.HasFlag(KinectHelper.Arrows.Right) && !LastArrows.HasFlag(KinectHelper.Arrows.Right))
                            {
                                InputManager.Keyboard.KeyDown(Keys.Right);
                            }
                            else if (!arrows.HasFlag(KinectHelper.Arrows.Right) && LastArrows.HasFlag(KinectHelper.Arrows.Right))
                            {
                                InputManager.Keyboard.KeyUp(Keys.Right);
                            }
                            if (arrows.HasFlag(KinectHelper.Arrows.Down) && !LastArrows.HasFlag(KinectHelper.Arrows.Down))
                            {
                                InputManager.Keyboard.KeyDown(Keys.Down);
                            }
                            else if (!arrows.HasFlag(KinectHelper.Arrows.Down) && LastArrows.HasFlag(KinectHelper.Arrows.Down))
                            {
                                InputManager.Keyboard.KeyUp(Keys.Down);
                            }
                            if (arrows.HasFlag(KinectHelper.Arrows.Left) && !LastArrows.HasFlag(KinectHelper.Arrows.Left))
                            {
                                InputManager.Keyboard.KeyDown(Keys.Left);
                            }
                            else if (!arrows.HasFlag(KinectHelper.Arrows.Left) && LastArrows.HasFlag(KinectHelper.Arrows.Left))
                            {
                                InputManager.Keyboard.KeyUp(Keys.Left);
                            }

                            LastArrows = arrows;
                        }
                    }
                }
            }
        }

        public void Start()
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

            StopTracking.WaitOne();
        }
    }
}
