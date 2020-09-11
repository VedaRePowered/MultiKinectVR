using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiKinectVR;

namespace TestKinectCS {
    static class Program {
        static MainWindow window;
        static KinectMultiBackend backend;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            backend = new KinectMultiBackend();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            window = new MainWindow(backend);
            Application.Run(window);
        }
    }
}
