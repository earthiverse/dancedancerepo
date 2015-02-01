using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Kinect;

namespace kinect
{
    /// <summary>
    /// Used for tuning the dance pad, getting the initial locations of down, up, left and right.
    /// </summary>
    public class KinectCalibrator
    {
        protected const int NumberOfFramesToAverage = 150;
        protected const float MaximumError = 0.01f;

        public ManualResetEvent CalibrationFinished;

        protected uint FrameNo;
        protected ulong TrackingID;
        public CameraSpacePoint LeftFoot;
        public CameraSpacePoint RightFoot;
        public CameraSpacePoint Center;

        protected Body[] Bodies;

        public KinectSensor Kinect;
        public BodyFrameReader BodyFrameReader;

        protected Queue<CameraSpacePoint> LeftFootPoints;
        protected Queue<CameraSpacePoint> RightFootPoints;

        public KinectCalibrator(BodyFrameReader reader = null)
        {
            CalibrationFinished = new ManualResetEvent(false);
            Kinect = KinectSensor.GetDefault();
            Bodies = new Body[Kinect.BodyFrameSource.BodyCount];
            LeftFootPoints = new Queue<CameraSpacePoint>(NumberOfFramesToAverage);
            RightFootPoints = new Queue<CameraSpacePoint>(NumberOfFramesToAverage);

            // Setup the kinect to read the skeletons
            BodyFrameReader = reader ?? Kinect.BodyFrameSource.OpenReader();

            Thread thread = new Thread(() =>
            {
                // Setup averaging
                BodyFrameReader.FrameArrived += ParseFrame;
            });
            thread.Start();

            // Wait for calibration to finish
            CalibrationFinished.WaitOne();
            thread.Join();

            BodyFrameReader.FrameArrived -= ParseFrame;
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
                                // Verify there is only one person in front of the Kinect to start
                                foreach (Body body2 in Bodies)
                                {
                                    if (body2 == body)
                                        // Same body
                                        continue;

                                    if (body2.IsTracked)
                                        // Multiple users in the first frame, don't start calibrating until one moves out
                                        break;
                                }

                                // Start tracking this body!
                                TrackingID = body.TrackingId;
                            }
                            else if (TrackingID != body.TrackingId)
                            {
                                // Either lost tracking, or there are two people in front of the Kinect
                                bool verifiedStillVisible = false;
                                foreach (Body body2 in Bodies)
                                {
                                    if (body2 == body)
                                        // Same body
                                        continue;

                                    if (body2.IsTracked && body2.TrackingId == TrackingID)
                                    {
                                        // There's multiple people
                                        verifiedStillVisible = true;
                                        break;
                                    }
                                }
                                if (!verifiedStillVisible)
                                {
                                    // Restart calibration
                                    FrameNo = 0;
                                    TrackingID = 0;
                                    break;
                                }
                            }

                            if (LeftFootPoints.Count < NumberOfFramesToAverage)
                            {
                                // Add the frame without doing any calculations
                                LeftFootPoints.Enqueue(body.Joints[JointType.FootLeft].Position);
                                RightFootPoints.Enqueue(body.Joints[JointType.FootRight].Position);
                            }
                            else if (LeftFootPoints.Count == NumberOfFramesToAverage)
                            {
                                // See if the last 5 seconds are somewhat the same

                                // Average the left foot
                                CameraSpacePoint firstPoint = LeftFootPoints.Peek();
                                CameraSpacePoint averagePoint = firstPoint;
                                uint i = 2;
                                foreach (CameraSpacePoint leftFootPoint in LeftFootPoints)
                                {
                                    averagePoint = KinectHelper.Average2CameraSpacePoints(averagePoint, leftFootPoint, i);
                                    i++;
                                }

                                float error = KinectHelper.MaxDifference(averagePoint, firstPoint);
                                if (error > MaximumError)
                                {
                                    Debug.WriteLine("Left Foot Error: {0}", error);
                                    Debug.WriteLine("Left Foot Average: {0}, {1}, {2}", averagePoint.X, averagePoint.Y, averagePoint.Z);
                                    Debug.WriteLine("Left Foot Last: {0}, {1}, {2}", firstPoint.X, firstPoint.Y, firstPoint.Z);

                                    // Too much error detected, remove half of the frames
                                    for (int j = 0; j < NumberOfFramesToAverage / 2; j++)
                                    {
                                        LeftFootPoints.Dequeue();
                                        RightFootPoints.Dequeue();
                                    }
                                    break;
                                }
                                LeftFoot = averagePoint;

                                // Check the right foot
                                firstPoint = RightFootPoints.Peek();
                                averagePoint = firstPoint;
                                i = 2;
                                foreach (CameraSpacePoint rightFootPoint in RightFootPoints)
                                {
                                    averagePoint = KinectHelper.Average2CameraSpacePoints(averagePoint, rightFootPoint, i);
                                    i++;
                                }

                                error = KinectHelper.MaxDifference(averagePoint, firstPoint);
                                if (error > MaximumError)
                                {
                                    Debug.WriteLine("Right Foot Error: {0}", error);
                                    Debug.WriteLine("Right Foot Average: {0}, {1}, {2}", averagePoint.X, averagePoint.Y, averagePoint.Z);
                                    Debug.WriteLine("Right Foot Last: {0}, {1}, {2}", firstPoint.X, firstPoint.Y, firstPoint.Z);

                                    // Too much error detected, remove half of the frames
                                    for (int j = 0; j < NumberOfFramesToAverage / 2; j++)
                                    {
                                        LeftFootPoints.Dequeue();
                                        RightFootPoints.Dequeue();
                                    }
                                    break;
                                }
                                RightFoot = averagePoint;

                                Center = body.Joints[JointType.SpineBase].Position;

                                // Calibration is within error!
                                CalibrationFinished.Set();
                            }
                        }
                    }
                }
            }
        }
    }
}
