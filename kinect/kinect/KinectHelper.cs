using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <returns>An average of A and B</returns>
        public static CameraSpacePoint Average2CameraSpacePoints(CameraSpacePoint a, CameraSpacePoint b)
        {
            CameraSpacePoint c = new CameraSpacePoint();
            c.X = (a.X + b.X) / 2;
            c.Y = (a.Y + b.Y) / 2;
            c.Z = (a.Z + b.Z) / 2;
            return c;
        }
    }
}
