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
        Head2         = JointType.SpineShoulder,
        FootLeft1     = JointType.AnkleLeft,
        FootLeft2     = JointType.FootLeft,
        FootLeftHigh  = JointType.KneeLeft,
        FootRight1    = JointType.AnkleRight,
        FootRight2    = JointType.FootRight,
        FootRightHigh = JointType.KneeRight,
        Hip1          = JointType.SpineBase,
        Hip2          = JointType.SpineMid,
    }
    public class Kinect2Data {
        private readonly KinectSensor sensor;
        private readonly BodyFrameReader bodyReader;
        private Body[] skeletons;
        readonly bool Usable;
        public bool Enabled;
        public Kinect2Data(KinectSensor sensor) {
            this.sensor = sensor;
            try {
                this.sensor.Open();
            } catch (IOException) {
                this.Usable = false;
                return;
            } catch (InvalidOperationException) {
                this.Usable = false;
                return;
            }
            this.bodyReader = this.sensor.BodyFrameSource.OpenReader();
            this.Usable = true;
        }
        public bool IsUsable() {
            return this.Usable;
        }
        private void Update() {
            BodyFrame nextFrame = this.bodyReader.AcquireLatestFrame();
            if (nextFrame != null) {
                this.skeletons = new Body[nextFrame.BodyCount];
                nextFrame.GetAndRefreshBodyData(this.skeletons);
                nextFrame.Dispose();
            }
        }
        public int GetSkeletonCount() {
            this.Update();
            return this.skeletons?.Length ?? 0;
        }
        public Kinect2Position GetJoint(int skeleton, Kinect2JointName id) {
            this.Update();
            CameraSpacePoint pos = this.skeletons[skeleton].Joints[(JointType)id].Position;
            return new Kinect2Position() { X = pos.X, Y = pos.Y, Z = pos.Z };
        }
    }
    public class Kinect2Backend {
        private readonly int totalSensors = 0;
        private readonly Kinect2Data[] sensors;
        public Kinect2Backend() {
            this.sensors = new Kinect2Data[1];
            Kinect2Data sensor = new Kinect2Data(KinectSensor.GetDefault());
            if (sensor.IsUsable()) {
                this.sensors[0] = sensor;
            }
            this.totalSensors += 1;
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
