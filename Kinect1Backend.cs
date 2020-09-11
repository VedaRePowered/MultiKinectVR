using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace MultiKinectVR {
    public struct Kinect1Position {
        public double X;
        public double Y;
        public double Z;
    }
    public enum Kinect1JointName {
        HandLeft1     = JointType.WristLeft,
        HandLeft2     = JointType.HandLeft,
        HandRight1    = JointType.WristRight,
        HandRight2    = JointType.HandRight,
        FootLeft1     = JointType.AnkleLeft,
        FootLeft2     = JointType.FootLeft,
        FootLeftHigh  = JointType.KneeLeft,
        FootRight1    = JointType.AnkleRight,
        FootRight2    = JointType.FootRight,
        FootRightHigh = JointType.KneeRight,
        Head1         = JointType.Head,
        Head2         = JointType.ShoulderCenter,
    }
    public class Kinect1Data {
        private KinectSensor sensor;
        private Skeleton[] skeletons;
        private bool usable;
        public bool Enabled;
        public Kinect1Data(KinectSensor sensor) {
            this.sensor = sensor;
            try {
                this.sensor.Start();
            } catch (IOException) {
                this.usable = false;
                return;
            }
            this.sensor.SkeletonStream.Enable();
        }
        public bool IsUsable() {
            return this.usable && this.sensor.Status == KinectStatus.Connected;
        }
        private void update() {
            SkeletonFrame nextFrame = sensor.SkeletonStream.OpenNextFrame(0);
            if (nextFrame != null) {
                this.skeletons = new Skeleton[nextFrame.SkeletonArrayLength];
                nextFrame.CopySkeletonDataTo(this.skeletons);
                nextFrame.Dispose();
            }
        }
        public int GetSkeletonCount() {
            this.update();
            return this.skeletons.Length;
        }
        public Kinect1Position GetJoint(int skeleton, Kinect1JointName id) {
            this.update();
            SkeletonPoint pos = this.skeletons[skeleton].Joints[(JointType)id].Position;
            return new Kinect1Position() { X = pos.X, Y = pos.Y, Z = pos.Z };
        }
    }
    public class Kinect1Backend {
        private int totalSensors = 0;
        private Kinect1Data[] sensors;
        public Kinect1Backend() {
            this.sensors = new Kinect1Data[KinectSensor.KinectSensors.Count];
            for (int i = 0; i < KinectSensor.KinectSensors.Count; i++) {
                this.sensors[i] = new Kinect1Data(KinectSensor.KinectSensors[i]);
                this.totalSensors += 1;
            }
        }
        public int GetKinectCount() {
            return this.sensors.Length;
        }
        public Kinect1Data GetKinect(int id) {
            if (id > 0 && id < this.sensors.Length) {
                return this.sensors[id];
            } else {
                return null;
            }
        }
    }
}
