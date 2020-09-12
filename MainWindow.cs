using Microsoft.Win32;
using MultiKinectVR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestKinectCS {
    public partial class MainWindow : Form {
        private readonly Timer timer = new Timer();
        private readonly KinectMultiBackend backend;
        private int kinectCount = 0;
        public MainWindow(KinectMultiBackend backend) {
            InitializeComponent();
            this.backend = backend;
            this.githubLink.LinkClicked += new LinkLabelLinkClickedEventHandler(this.OpenGithub);
            this.kinectView.Paint += new PaintEventHandler(this.Redraw);
            this.timer.Interval = 33;
            this.timer.Tick += new EventHandler(this.Frame);
            this.timer.Start();
            this.kinectSelect.Items.Add("No Preview");
            this.kinectSelect.SelectedIndex = 0;
            this.connectionPage.Enter += ConnectionPage_Enter;
        }

        private void ConnectionPage_Enter(object sender, EventArgs ev) {
            this.kinnect1Count.Text = this.backend.GetTotalV1KinectCount().ToString();
            this.kinnect1Usable.Text = this.backend.GetUsableV1KinectCount().ToString();
            this.kinnect1Enabled.Text = this.backend.GetEnabledV1KinectCount().ToString();
        }

        private void Frame(object sender, EventArgs ev) {
            this.backend.Update();
            this.kinectView.Refresh();
        }

        private void OpenGithub(object sender, LinkLabelLinkClickedEventArgs ev) {
            this.githubLink.LinkVisited = true;
            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://github.com/BEN1JEN/MultiKinectVR/");
        }
        private Point Point3D(Position pos, double width) {
            if (!pos.Valid || pos.Z <= 0.0) {
                return new Point(0, 0);
            }
            double x = pos.X, y = pos.Y, z = pos.Z;
            double angle = this.kinectViewAngle.Value/180.0*Math.PI;
            z -= 2.0; // move point of rotation
            double ry = y * Math.Cos(angle) + z * Math.Sin(angle);
            double rz = z * Math.Cos(angle) - y * Math.Sin(angle);
            rz += 2.0;
            return new Point((int)((x / rz / 2 + 0.5)*360.0 + (width / 2.0 - 360.0 / 2.0)), (int)((0.5 - ry / rz / 2)*360.0));
        }
        private void Redraw(object sender, PaintEventArgs ev) {
            int newKinectCount = this.backend.GetKinectCount();
            while (this.kinectCount > newKinectCount) {
                this.kinectCount = this.kinectSelect.Items.Count;
                this.kinectSelect.Items.RemoveAt(this.kinectCount);
            }
            while (this.kinectCount < newKinectCount) {
                this.kinectCount = this.kinectSelect.Items.Count;
                this.kinectSelect.Items.Add("Kinect #" + this.kinectCount);
            }
            ev.Graphics.Clear(Color.FromArgb(0, 0, 0));
            if (this.kinectSelect.SelectedIndex > 0 && this.kinectSelect.SelectedIndex <= this.kinectCount) {
                for (double x = -2.0; x < 2.25; x += 0.25) {
                    ev.Graphics.DrawLine(Pens.Gray, Point3D(new Position(x, -0.5, 0.001), 480), Point3D(new Position(x, -0.5, 4.001), 480));
                    ev.Graphics.DrawLine(Pens.Gray, Point3D(new Position(-2.0, -0.5, x + 2.001), 480), Point3D(new Position(2.0, -0.5, x + 2.001), 480));
                }
                int kinect = this.kinectSelect.SelectedIndex-1;
                for (int i = 0; i < this.backend.GetSkeletonCount(kinect); i++) {
                    ev.Graphics.FillEllipse(Brushes.White, new Rectangle(this.Point3D(this.backend.GetJoint(kinect, i, JointName.HandLeft), 480), new Size(10, 10)));
                    ev.Graphics.FillEllipse(Brushes.White, new Rectangle(this.Point3D(this.backend.GetJoint(kinect, i, JointName.HandRight), 480), new Size(10, 10)));
                    ev.Graphics.FillEllipse(Brushes.White, new Rectangle(this.Point3D(this.backend.GetJoint(kinect, i, JointName.Head), 480), new Size(10, 10)));
                    ev.Graphics.FillEllipse(Brushes.White, new Rectangle(this.Point3D(this.backend.GetJoint(kinect, i, JointName.FootLeft), 480), new Size(10, 10)));
                    ev.Graphics.FillEllipse(Brushes.White, new Rectangle(this.Point3D(this.backend.GetJoint(kinect, i, JointName.FootRight), 480), new Size(10, 10)));
                    ev.Graphics.FillEllipse(Brushes.White, new Rectangle(this.Point3D(this.backend.GetJoint(kinect, i, JointName.Hip), 480), new Size(10, 10)));
                }
            }
        }
    }
}
