﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiKinectVR {
    public struct Position {
        public double X;
        public double Y;
        public double Z;
        public bool Valid;
        public Position(Kinect1Position pos) {
            this.X = pos.X;
            this.Y = pos.Y;
            this.Z = pos.Z;
            this.Valid = pos.Z > 0.0;
        }
        public Position(double x, double y, double z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Valid = z > 0.0;
        }
        public static Position Invalid = new Position { Valid = false };
    }
    public enum RawJointName {
        HandLeft1,
        HandLeft2,
        HandRight1,
        HandRight2,
        FootLeft1,
        FootLeft2,
        FootLeftHigh,
        FootRight1,
        FootRight2,
        FootRightHigh,
        Head1,
        Head2,
    }
    public enum JointName {
        HandLeft,
        HandRight,
        FootLeft,
        FootRight,
        Head,
    }
    public class KinectMultiBackend {
        private Kinect1Backend kv1;
        //private Kinect2Backend kv2; TODO: Xbox One Kinect Support
        public KinectMultiBackend() {
            kv1 = new Kinect1Backend();
            //kv2 = new Kinect2Backend();
        }
        public int GetKinectCount() {
            return this.kv1.GetKinectCount();
        }
        public int GetSkeletonCount(int sensor) {
            return this.kv1.GetKinect(sensor).GetSkeletonCount();
        }
        public Position GetRawJoint(int sensor, int skeleton, RawJointName id) {
            if (sensor < 0) {
                return Position.Invalid;
            } else if (sensor < this.kv1.GetKinectCount()) {
                Kinect1Data kinect = this.kv1.GetKinect(sensor);
                if (kinect == null) {
                    return Position.Invalid;
                }
                switch (id) {
                    case RawJointName.HandLeft1: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.HandLeft1));
                    case RawJointName.HandLeft2: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.HandLeft2));
                    case RawJointName.HandRight1: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.HandRight1));
                    case RawJointName.HandRight2: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.HandRight2));
                    case RawJointName.FootLeft1: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.FootLeft1));
                    case RawJointName.FootLeft2: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.FootLeft2));
                    case RawJointName.FootLeftHigh: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.FootLeftHigh));
                    case RawJointName.FootRight1: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.FootRight1));
                    case RawJointName.FootRight2: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.FootRight2));
                    case RawJointName.FootRightHigh: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.FootRightHigh));
                    case RawJointName.Head1: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.Head1));
                    case RawJointName.Head2: return new Position(kinect.GetJoint(skeleton, Kinect1JointName.Head2));
                    default: return Position.Invalid;
                }
            } else {
                return Position.Invalid;
            }
        }
        private Position smartAverage(Position a, Position b) {
            if (a.Valid && b.Valid) {
                return new Position(a.X + b.X / 2.0, a.Y + b.Y / 2.0, a.Z + b.Z / 2.0);
            } else if (!a.Valid) {
                return b;
            } else if (!b.Valid) {
                return a;
            } else {
                return Position.Invalid;
            }
        }
        private Position footAverage(Position a, Position b, Position high) {
            Position realFoot = smartAverage(a, b);
            if (realFoot.Valid) {
                return realFoot;
            } else {
                return new Position(high.X, high.Y - 0.7, high.Z);
            }
        }
        public Position GetJoint(int sensor, int skeleton, JointName id) {
            switch (id) {
                case JointName.HandLeft: return smartAverage(this.GetRawJoint(sensor, skeleton, RawJointName.HandLeft1), this.GetRawJoint(sensor, skeleton, RawJointName.HandLeft2));
                case JointName.HandRight: return smartAverage(this.GetRawJoint(sensor, skeleton, RawJointName.HandRight1), this.GetRawJoint(sensor, skeleton, RawJointName.HandRight2));
                case JointName.Head: return smartAverage(this.GetRawJoint(sensor, skeleton, RawJointName.Head1), this.GetRawJoint(sensor, skeleton, RawJointName.Head2));
                case JointName.FootLeft: return footAverage(this.GetRawJoint(sensor, skeleton, RawJointName.FootLeft1), this.GetRawJoint(sensor, skeleton, RawJointName.FootLeft2), this.GetRawJoint(sensor, skeleton, RawJointName.FootLeftHigh));
                case JointName.FootRight: return footAverage(this.GetRawJoint(sensor, skeleton, RawJointName.FootRight1), this.GetRawJoint(sensor, skeleton, RawJointName.FootRight2), this.GetRawJoint(sensor, skeleton, RawJointName.FootRightHigh));
                default: return Position.Invalid;
            }
        }
    }
}