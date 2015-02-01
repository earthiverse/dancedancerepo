using System;
using System.Diagnostics.Contracts;
using System.Net;
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

        public static int GetArrowsPressed(CameraSpacePoint upArrow, CameraSpacePoint rightArrow, CameraSpacePoint downArrow, CameraSpacePoint leftArrow, CameraSpacePoint leftFoot, CameraSpacePoint rightFoot)
        {
            throw new NotImplementedException();
        }
    }
}
