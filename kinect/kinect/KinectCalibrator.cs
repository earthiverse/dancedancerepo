using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace kinect
{
    /// <summary>
    /// Used for tuning the dance pad, getting the initial locations of down, up, left and right.
    /// </summary>
    public class KinectCalibrator : IDisposable
    {
        public ManualResetEvent CalibrationFinished;

        protected uint FrameNo = 1;
        protected ulong TrackingID;
        protected CameraSpacePoint LeftFoot;
        protected CameraSpacePoint RightFoot;

        protected Body[] Bodies;

        public KinectCalibrator()
        {
            CalibrationFinished = new ManualResetEvent(false);

            // Find the connected Kinect
            KinectSensor kinect = KinectSensor.GetDefault();

            // Setup the kinect to read the skeletons
            using (BodyFrameReader reader = kinect.BodyFrameSource.OpenReader())
            {
                // Setup averaging
                reader.FrameArrived += ParseFrame;

                // Wait for calibration to finish
                CalibrationFinished.WaitOne();
            }
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
                            if (TrackingID == 0)
                            {
                                // Start tracking!
                                TrackingID = body.TrackingId;
                            }
                            else if (TrackingID != body.TrackingId)
                            {
                                // Either lost tracking, or there are two people in front of the Kinect
                                bool verifiedStillVisible = false;
                                foreach (Body body2 in Bodies)
                                {
                                    if (body2.TrackingId == TrackingID)
                                    {
                                        // There's multiple people
                                        verifiedStillVisible = true;
                                        break;
                                    }
                                }
                                if (!verifiedStillVisible)
                                {
                                    // Restart calibration
                                    FrameNo = 1;
                                    LeftFoot = body.Joints[JointType.FootLeft].Position;
                                    RightFoot = body.Joints[JointType.FootRight].Position;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
