using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace MultiKinectVR {
    public struct Kinect2Position {
        public double X;
        public double Y;
        public double Z;
    }
    public enum Kinect2JointName {
        HandLeft1     = JointType.WristLeft,
        HandLeft2     = JointType.HandLeft,
        HandRight1    = JointType.WristRight,
        HandRight2    = JointType.HandRight,
        Head1         = JointType.Head,
        Head2         = JointType.ShoulderCenter,
        FootLeft1     = JointType.AnkleLeft,
        FootLeft2     = JointType.FootLeft,
        FootLeftHigh  = JointType.KneeLeft,
        FootRight1    = JointType.AnkleRight,
        FootRight2    = JointType.FootRight,
        FootRightHigh = JointType.KneeRight,
        Hip1          = JointType.HipCenter,
        Hip2          = JointType.Spine,
    }
    public class Kinect2Data {
        private readonly KinectSensor sensor;
        private Skeleton[] skeletons;
        private readonly bool usable;
        public bool Enabled;
        public Kinect2Data(KinectSensor sensor) {
            this.sensor = sensor;
            try {
                this.sensor.Start();
            } catch (IOException) {
                this.usable = false;
                return;
            } catch (InvalidOperationException) {
                this.usable = false;
                return;
            }
            this.sensor.SkeletonStream.Enable();
            this.usable = true;
        }
        public bool IsUsable() {
            return this.usable && this.sensor.Status == KinectStatus.Connected;
        }
        private void Update() {
            SkeletonFrame nextFrame = sensor.SkeletonStream.OpenNextFrame(0);
            if (nextFrame != null) {
                this.skeletons = new Skeleton[nextFrame.SkeletonArrayLength];
                nextFrame.CopySkeletonDataTo(this.skeletons);
                nextFrame.Dispose();
            }
        }
        public int GetSkeletonCount() {
            this.Update();
            return this.skeletons.Length;
        }
        public Kinect2Position GetJoint(int skeleton, Kinect2JointName id) {
            this.Update();
            SkeletonPoint pos = this.skeletons[skeleton].Joints[(JointType)id].Position;
            return new Kinect2Position() { X = pos.X, Y = pos.Y, Z = pos.Z };
        }
    }
    public class Kinect2Backend {
        private readonly int totalSensors = 0;
        private readonly Kinect2Data[] sensors;
        public Kinect2Backend() {
            this.sensors = new Kinect2Data[KinectSensor.KinectSensors.Count];
            for (int i = 0; i < KinectSensor.KinectSensors.Count; i++) {
                Kinect2Data sensor = new Kinect2Data(KinectSensor.KinectSensors[i]);
                if (sensor.IsUsable()) {
                    this.sensors[i] = sensor;
                }
                this.totalSensors += 1;
            }
        }
        public int GetTotalKinectCount() {
            return this.totalSensors;
        }
        public int GetUsableKinectCount() {
            return this.sensors.Length;
        }
        public int GetEnabledKinectCount() {
            int enabled = 0;
            foreach (Kinect2Data sensor in this.sensors) {
                if (sensor.Enabled) {
                    enabled++;
                }
            }
            return enabled;
        }
        public Kinect2Data GetKinect(int id) {
            if (id >= 0 && id < this.sensors.Length) {
                return this.sensors[id];
            } else {
                return null;
            }
        }
    }
}
