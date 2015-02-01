using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;
using Microsoft.Kinect;

namespace kinect
{
    public class KinectHelper
    {
        /// <summary>
        /// Average two camera space points to return the average position in the x, y and z axes
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <param name="influence">Point B will be divided by this number, </param>
        /// <returns>An average of A and B</returns>
        public static CameraSpacePoint Average2CameraSpacePoints(CameraSpacePoint a, CameraSpacePoint b, uint influence = 2)
        {
            Contract.Requires(influence >= 2);

            return new CameraSpacePoint
            {
                X = (a.X + ((b.X - a.X) / influence)),
                Y = (a.Y + ((b.Y - a.Y) / influence)),
                Z = (a.Z + ((b.Z - a.Z) / influence))
            };
        }

        /// <summary>
        /// Returns the maximum error between points A and B as a percent
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <returns>Maximum Error</returns>
        public static float MaxDifference(CameraSpacePoint a, CameraSpacePoint b)
        {
            float xError = (b.X - a.X);
            float yError = (b.Y - a.Y);
            float zError = (b.Z - a.Z);
            return Math.Max(xError, Math.Max(yError, zError));
        }

        /// <summary>
        /// Computes the cardinal distance between two points
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <returns>Cardinal distance between the two points</returns>
        public static double ComputeHorizontalDistance(CameraSpacePoint a, CameraSpacePoint b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public static double ComputeVerticalDistance(CameraSpacePoint a, CameraSpacePoint b)
        {
            return Math.Abs(a.Y - b.Y);
        }

        public static double ComputeHorizontalThreshold(CameraSpacePoint upArrow, CameraSpacePoint rightArrow, CameraSpacePoint downArrow, CameraSpacePoint leftArrow)
        {
            double upDistance = Math.Min(ComputeHorizontalDistance(upArrow, rightArrow), ComputeHorizontalDistance(upArrow, leftArrow));
            double downDistance = Math.Min(ComputeHorizontalDistance(downArrow, rightArrow), ComputeHorizontalDistance(downArrow, leftArrow));
            return Math.Abs(Math.Min(upDistance, downDistance));
        }

        public static double ComputeVerticalThreshold(CameraSpacePoint upArrow, CameraSpacePoint rightArrow, CameraSpacePoint downArrow, CameraSpacePoint leftArrow)
        {
            double upDistance = Math.Min(ComputeVerticalDistance(upArrow, downArrow), ComputeVerticalDistance(leftArrow, rightArrow));
            return Math.Abs(upDistance / 4);
        }

        [Flags]
        public enum Arrows
        {
            None = 0,
            Up = 1,
            Right = 2,
            Left = 4,
            Down = 8
        }

        public static Arrows GetArrowsPressed(CameraSpacePoint upArrow, CameraSpacePoint rightArrow, CameraSpacePoint downArrow, CameraSpacePoint leftArrow, CameraSpacePoint leftFoot, CameraSpacePoint rightFoot, double horizontalThreshold, double verticalThreshold)
        {
            Arrows arrowsPressed = Arrows.None;
            Console.Clear();

            double leftUpHorizontal = ComputeHorizontalDistance(leftFoot, upArrow);
            double leftUpVertical = ComputeVerticalDistance(leftFoot, upArrow);
            double leftRightHorizontal = ComputeHorizontalDistance(leftFoot, rightArrow);
            double leftRightVertical = ComputeVerticalDistance(leftFoot, rightArrow);
            double leftDownHorizontal = ComputeHorizontalDistance(leftFoot, downArrow);
            double leftDownVertical = ComputeVerticalDistance(leftFoot, downArrow);
            double leftLeftHorizontal = ComputeHorizontalDistance(leftFoot, leftArrow);
            double leftLeftVertical = ComputeVerticalDistance(leftFoot, leftArrow);

            if ((leftUpHorizontal < horizontalThreshold && leftUpVertical < verticalThreshold)
                || (leftRightHorizontal < horizontalThreshold && leftRightVertical < verticalThreshold)
                || (leftDownHorizontal < horizontalThreshold && leftDownVertical < verticalThreshold)
                || (leftLeftHorizontal < horizontalThreshold && leftLeftVertical < verticalThreshold))
            {
                double minHorizontal = Math.Min(leftUpHorizontal, Math.Min(leftRightHorizontal, Math.Min(leftDownHorizontal, leftLeftHorizontal)));
                if (minHorizontal == leftUpHorizontal)
                {
                    Debug.WriteLine("Left foot is pressing up! {0} {1}", leftUpHorizontal, leftUpVertical);
                    Console.WriteLine("Left foot is pressing up! {0} {1}", leftUpHorizontal, leftUpVertical);
                    arrowsPressed = arrowsPressed | Arrows.Up;
                }
                else if (minHorizontal == leftRightHorizontal)
                {
                    Debug.WriteLine("Left foot is pressing right! {0} {1}", leftRightHorizontal, leftRightVertical);
                    Console.WriteLine("Left foot is pressing right! {0} {1}", leftRightHorizontal, leftRightVertical);
                    arrowsPressed = arrowsPressed | Arrows.Right;
                }
                else if (minHorizontal == leftDownHorizontal)
                {
                    Debug.WriteLine("Left foot is pressing down! {0} {1}", leftDownHorizontal, leftDownVertical);
                    Console.WriteLine("Left foot is pressing down! {0} {1}", leftDownHorizontal, leftDownVertical);
                    arrowsPressed = arrowsPressed | Arrows.Down;
                }
                else if (minHorizontal == leftLeftHorizontal)
                {
                    Debug.WriteLine("Left foot is pressing left! {0} {1}", leftLeftHorizontal, leftLeftVertical);
                    Console.WriteLine("Left foot is pressing left! {0} {1}", leftLeftHorizontal, leftLeftVertical);
                    arrowsPressed = arrowsPressed | Arrows.Left;
                }
            }

            double rightUpHorizontal = ComputeHorizontalDistance(rightFoot, upArrow);
            double rightUpVertical = ComputeVerticalDistance(rightFoot, upArrow);
            double rightRightHorizontal = ComputeHorizontalDistance(rightFoot, rightArrow);
            double rightRightVertical = ComputeVerticalDistance(rightFoot, rightArrow);
            double rightDownHorizontal = ComputeHorizontalDistance(rightFoot, downArrow);
            double rightDownVertical = ComputeVerticalDistance(rightFoot, downArrow);
            double rightLeftHorizontal = ComputeHorizontalDistance(rightFoot, leftArrow);
            double rightLeftVertical = ComputeVerticalDistance(rightFoot, leftArrow);

            if ((rightUpHorizontal < horizontalThreshold && rightUpVertical < verticalThreshold)
                || (rightRightHorizontal < horizontalThreshold && rightRightVertical < verticalThreshold)
                || (rightDownHorizontal < horizontalThreshold && rightDownVertical < verticalThreshold)
                || (rightLeftHorizontal < horizontalThreshold && rightLeftVertical < verticalThreshold))
            {
                double minHorizontal = Math.Min(rightUpHorizontal, Math.Min(rightRightHorizontal, Math.Min(rightDownHorizontal, rightLeftHorizontal)));
                if (minHorizontal == rightUpHorizontal)
                {
                    Debug.WriteLine("Right foot is pressing up! {0} {1}", rightUpHorizontal, rightUpVertical);
                    Console.WriteLine("Right foot is pressing up! {0} {1}", rightUpHorizontal, rightUpVertical);
                    arrowsPressed = arrowsPressed | Arrows.Up;
                }
                else if (minHorizontal == rightRightHorizontal)
                {
                    Debug.WriteLine("Right foot is pressing right! {0} {1}", rightRightHorizontal, rightRightVertical);
                    Console.WriteLine("Right foot is pressing right! {0} {1}", rightRightHorizontal, rightRightVertical);
                    arrowsPressed = arrowsPressed | Arrows.Right;
                }
                else if (minHorizontal == rightDownHorizontal)
                {
                    Debug.WriteLine("Right foot is pressing down! {0} {1}", rightDownHorizontal, rightDownVertical);
                    Console.WriteLine("Right foot is pressing down! {0} {1}", rightDownHorizontal, rightDownVertical);
                    arrowsPressed = arrowsPressed | Arrows.Down;
                }
                else if (minHorizontal == rightLeftHorizontal)
                {
                    Debug.WriteLine("Right foot is pressing left! {0} {1}", rightLeftHorizontal, rightLeftVertical);
                    Console.WriteLine("Right foot is pressing left! {0} {1}", rightLeftHorizontal, rightLeftVertical);
                    arrowsPressed = arrowsPressed | Arrows.Left;
                }
            }

            //Thread.Sleep(1000 / 31);
            Thread.Sleep(10);
            return arrowsPressed;
        }
    }
}
