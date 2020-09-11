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
        private Timer timer = new Timer();
        private Font font = new Font("Arial", 12);
        KinectMultiBackend backend;
        public MainWindow(KinectMultiBackend backend) {
            InitializeComponent();
            this.backend = backend;
            this.githubLink.LinkClicked += new LinkLabelLinkClickedEventHandler(this.OpenGithub);
            this.kinectView.Paint += new PaintEventHandler(this.Redraw);
            this.timer.Interval = 33;
            this.timer.Tick += new EventHandler(this.Frame);
            this.timer.Start();
        }

        private void Frame(object sender, EventArgs ev) {
            this.kinectView.Refresh();
        }

        private void OpenGithub(object sender, LinkLabelLinkClickedEventArgs ev) {
            this.githubLink.LinkVisited = true;
            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://github.com/BEN1JEN/multikinect-vr/");
        }
        private Point Point3D(Position pos, double width) {
            if (pos.Valid) {
                return new Point(0, 0);
            }
            return new Point((int)((pos.X / pos.Z / 2 + 0.5)*width), (int)((0.5 - pos.Y / pos.Z / 2)*360.0));
        }
        private void Redraw(object sender, PaintEventArgs ev) {
            ev.Graphics.Clear(Color.FromArgb(0, 0, 0));
            if (this.kinectSelect.SelectedIndex > 0 && this.kinectSelect.SelectedIndex <= this.backend.GetKinectCount()) {
                int kinect = this.kinectSelect.SelectedIndex;
                for (int i = 0; i < this.backend.GetSkeletonCount(kinect); i++) {
                    ev.Graphics.DrawEllipse(Pens.Red, new Rectangle(this.Point3D(this.backend.GetJoint(kinect, i, JointName.HandLeft), 480), new Size(10, 10)));
                    ev.Graphics.DrawEllipse(Pens.Green, new Rectangle(this.Point3D(this.backend.GetJoint(kinect, i, JointName.HandRight), 480), new Size(10, 10)));
                    ev.Graphics.DrawEllipse(Pens.Blue, new Rectangle(this.Point3D(this.backend.GetJoint(kinect, i, JointName.Head), 480), new Size(10, 10)));
                    ev.Graphics.DrawEllipse(Pens.Cyan, new Rectangle(this.Point3D(this.backend.GetJoint(kinect, i, JointName.FootLeft), 480), new Size(10, 10)));
                    ev.Graphics.DrawEllipse(Pens.Yellow, new Rectangle(this.Point3D(this.backend.GetJoint(kinect, i, JointName.FootRight), 480), new Size(10, 10)));
                }
            }
        }
    }
}
