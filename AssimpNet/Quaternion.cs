/*
* Copyright (c) 2012-2020 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Assimp
{
    /// <summary>
    /// A 4D vector that represents a rotation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion : IEquatable<Quaternion>
    {
        /// <summary>
        /// Rotation component of the quaternion/
        /// </summary>
        public double W;

        /// <summary>
        /// X component of the vector part of the quaternion.
        /// </summary>
        public double X;

        /// <summary>
        /// Y component of the vector part of the quaternion.
        /// </summary>
        public double Y;

        /// <summary>
        /// Z component of the vector part of the quaternion.
        /// </summary>
        public double Z;

        /// <summary>
        /// Constructs a new Quaternion.
        /// </summary>
        /// <param name="w">W component</param>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        public Quaternion(double w, double x, double y, double z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Constructs a new Quaternion from a rotation matrix.
        /// </summary>
        /// <param name="matrix">Rotation matrix to create the Quaternion from.</param>
        public Quaternion(Matrix3x3 matrix)
        {
            double trace = matrix.A1 + matrix.B2 + matrix.C3;

            if(trace > 0)
            {
                double s = (double) Math.Sqrt(trace + 1.0) * 2.0;
                W = 0.25 * s;
                X = (matrix.C2 - matrix.B3) / s;
                Y = (matrix.A3 - matrix.C1) / s;
                Z = (matrix.B1 - matrix.A2) / s;
            }
            else if((matrix.A1 > matrix.B2) && (matrix.A1 > matrix.C3))
            {
                double s = (double) Math.Sqrt(((1.0 + matrix.A1) - matrix.B2) - matrix.C3) * 2.0;
                W = (matrix.C2 - matrix.B3) / s;
                X = 0.25 * s;
                Y = (matrix.A2 + matrix.B1) / s;
                Z = (matrix.A3 + matrix.C1) / s;
            }
            else if(matrix.B2 > matrix.C3)
            {
                double s = (double) Math.Sqrt(((1.0f + matrix.B2) - matrix.A1) - matrix.C3) * 2.0f;
                W = (matrix.A3 - matrix.C1) / s;
                X = (matrix.A2 + matrix.B1) / s;
                Y = 0.25 * s;
                Z = (matrix.B3 + matrix.C2) / s;
            }
            else
            {
                double s = (double) Math.Sqrt(((1.0f + matrix.C3) - matrix.A1) - matrix.B2) * 2.0f;
                W = (matrix.B1 - matrix.A2) / s;
                X = (matrix.A3 + matrix.C1) / s;
                Y = (matrix.B3 + matrix.C2) / s;
                Z = 0.25 * s;
            }

            Normalize();
        }

        /// <summary>
        /// Constructs a new Quaternion from three euler angles.
        /// </summary>
        /// <param name="pitch">Pitch</param>
        /// <param name="yaw">Yaw</param>
        /// <param name="roll">Roll</param>
        public Quaternion(double pitch, double yaw, double roll)
        {
            double sinPitch = (double) Math.Sin(pitch * 0.5);
            double cosPitch = (double) Math.Cos(pitch * 0.5);
            double sinYaw = (double) Math.Sin(yaw * 0.5);
            double cosYaw = (double) Math.Cos(yaw * 0.5);
            double sinRoll = (double) Math.Sin(roll * 0.5);
            double cosRoll = (double) Math.Cos(roll * 0.5);
            double cosPitchCosYaw = cosPitch * cosYaw;
            double sinPitchSinYaw = sinPitch * sinYaw;

            X = (sinRoll * cosPitchCosYaw) - (cosRoll * sinPitchSinYaw);
            Y = (cosRoll * sinPitch * cosYaw) + (sinRoll * cosPitch * sinYaw);
            Z = (cosRoll * cosPitch * sinYaw) - (sinRoll * sinPitch * cosYaw);
            W = (cosRoll * cosPitchCosYaw) + (sinRoll * sinPitchSinYaw);
        }

        /// <summary>
        /// Constructs a new Quaternion from an axis-angle.
        /// </summary>
        /// <param name="axis">Axis</param>
        /// <param name="angle">Angle about the axis</param>
        public Quaternion(Vector3D axis, double angle)
        {
            axis.Normalize();

            double halfAngle = angle * 0.5;
            double sin = (double) Math.Sin(halfAngle);
            double cos = (double) Math.Cos(halfAngle);
            X = axis.X * sin;
            Y = axis.Y * sin;
            Z = axis.Z * sin;
            W = cos;
        }

        /// <summary>
        /// Normalizes the quaternion.
        /// </summary>
        public void Normalize()
        {
            double mag = (X * X) + (Y * Y) + (Z * Z) + (W * W);
            if(mag != 0)
            {
                X /= mag;
                Y /= mag;
                Z /= mag;
                W /= mag;
            }
        }

        /// <summary>
        /// Transforms this quaternion into its conjugate.
        /// </summary>
        public void Conjugate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
        }

        /// <summary>
        /// Returns a matrix representation of the quaternion.
        /// </summary>
        /// <returns>Rotation matrix representing the quaternion.</returns>
        public Matrix3x3 GetMatrix()
        {
            double xx = X * X;
            double yy = Y * Y;
            double zz = Z * Z;

            double xy = X * Y;
            double zw = Z * W;
            double zx = Z * X;
            double yw = Y * W;
            double yz = Y * Z;
            double xw = X * W;

            Matrix3x3 mat;
            mat.A1 = 1.0 - (2.0 * (yy + zz));
            mat.B1 = 2.0 * (xy + zw);
            mat.C1 = 2.0 * (zx - yw);

            mat.A2 = 2.0 * (xy - zw);
            mat.B2 = 1.0 - (2.0 * (zz + xx));
            mat.C2 = 2.0 * (yz + xw);

            mat.A3 = 2.0 * (zx + yw);
            mat.B3 = 2.0 * (yz - xw);
            mat.C3 = 1.0 - (2.0 * (yy + xx));

            return mat;
        }

        /// <summary>
        /// Spherical interpolation between two quaternions.
        /// </summary>
        /// <param name="start">Start rotation when factor == 0</param>
        /// <param name="end">End rotation when factor == 1</param>
        /// <param name="factor">Interpolation factor between 0 and 1, values beyond this range yield undefined values</param>
        /// <returns>Interpolated quaternion.</returns>
        public static Quaternion Slerp(Quaternion start, Quaternion end, double factor)
        {
            //Calc cosine theta
            double cosom = (start.X * end.X) + (start.Y * end.Y) + (start.Z * end.Z) + (start.W * end.W);

            //Reverse signs if needed
            if(cosom < 0.0)
            {
                cosom = -cosom;
                end.X = -end.X;
                end.Y = -end.Y;
                end.Z = -end.Z;
                end.W = -end.W;
            }

            //calculate coefficients
            double sclp, sclq;
            //0.0001 -> some episilon
            if((1.0 - cosom) > 0.0001)
            {
                //Do a slerp
                double omega, sinom;
                omega = (double) Math.Acos(cosom); //extract theta from the product's cos theta
                sinom = (double) Math.Sin(omega);
                sclp = (double) Math.Sin((1.0 - factor) * omega) / sinom;
                sclq = (double) Math.Sin(factor * omega) / sinom;
            }
            else
            {
                //Very close, do a lerp instead since its faster
                sclp = 1.0 - factor;
                sclq = factor;
            }

            Quaternion q;
            q.X = (sclp * start.X) + (sclq * end.X);
            q.Y = (sclp * start.Y) + (sclq * end.Y);
            q.Z = (sclp * start.Z) + (sclq * end.Z);
            q.W = (sclp * start.W) + (sclq * end.W);
            return q;
        }

        /// <summary>
        /// Rotates a point by this quaternion.
        /// </summary>
        /// <param name="pt">Point to rotate</param>
        /// <param name="quat">Quaternion representing the rotation</param>
        /// <returns>Rotated point.</returns>
        public static Vector3D Rotate(Vector3D pt, Quaternion quat)
        {
            double x2 = quat.X + quat.X;
            double y2 = quat.Y + quat.Y;
            double z2 = quat.Z + quat.Z;

            double wx2 = quat.W * x2;
            double wy2 = quat.W * y2;
            double wz2 = quat.W * z2;

            double xx2 = quat.X * x2;
            double xy2 = quat.X * y2;
            double xz2 = quat.X * z2;

            double yy2 = quat.Y * y2;
            double yz2 = quat.Y * z2;

            double zz2 = quat.Z * z2;

            double x = ((pt.X * ((1.0 - yy2) - zz2)) + (pt.Y * (xy2 - wz2))) + (pt.Z * (xz2 + wy2));
            double y = ((pt.X * (xy2 + wz2)) + (pt.Y * ((1.0 - xx2) - zz2))) + (pt.Z * (yz2 - wx2));
            double z = ((pt.X * (xz2 - wy2)) + (pt.Y * (yz2 + wx2))) + (pt.Z * ((1.0 - xx2) - yy2));

            Vector3D v;
            v.X = x;
            v.Y = y;
            v.Z = z;
            return v;
        }

        /// <summary>
        /// Multiplies two quaternions.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>Resulting quaternion</returns>
        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            Quaternion q;
            q.W = (a.W * b.W) - (a.X * b.X) - (a.Y * b.Y) - (a.Z * b.Z);
            q.X = (a.W * b.X) + (a.X * b.W) + (a.Y * b.Z) - (a.Z * b.Y);
            q.Y = (a.W * b.Y) + (a.Y * b.W) + (a.Z * b.X) - (a.X * b.Z);
            q.Z = (a.W * b.Z) + (a.Z * b.W) + (a.X * b.Y) - (a.Y * b.X);
            return q;
        }

        /// <summary>
        /// Tests equality between two quaternions.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>True if the quaternions are equal, false otherwise.</returns>
        public static bool operator ==(Quaternion a, Quaternion b)
        {
            return (a.W == b.W) && (a.X == b.X) && (a.Y == b.Y) && (a.Z == b.Z);
        }

        /// <summary>
        /// Tests inequality between two quaternions.
        /// </summary>
        /// <param name="a">First quaternion</param>
        /// <param name="b">Second quaternion</param>
        /// <returns>True if the quaternions are not equal, false otherwise.</returns>
        public static bool operator !=(Quaternion a, Quaternion b)
        {
            return (a.W != b.W) || (a.X != b.X) || (a.Y != b.Y) || (a.Z != b.Z);
        }

        /// <summary>
        /// Tests equality between two quaternions.
        /// </summary>
        /// <param name="other">Quaternion to compare</param>
        /// <returns>True if the quaternions are equal.</returns>
        public bool Equals(Quaternion other)
        {
            return (W == other.W) && (X == other.X) && (Y == other.Y) && (Z == other.Z);
        }

        /// <summary>
        /// Tests equality between this color and another object.
        /// </summary>
        /// <param name="obj">Object to test against</param>
        /// <returns>True if the object is a color and the components are equal</returns>
        public override bool Equals(object obj)
        {
            if(obj is Quaternion)
            {
                return Equals((Quaternion) obj);
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return W.GetHashCode() + X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            return String.Format(info, "{{W:{0} X:{1} Y:{2} Z:{3}}}",
                new Object[] { W.ToString(info), X.ToString(info), Y.ToString(info), Z.ToString(info) });
        }
    }
}
