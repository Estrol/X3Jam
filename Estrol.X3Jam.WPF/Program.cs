﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Estrol.X3Jam.WPF {
    public class Program : Application {

        static void Main() {
            bool createNew = true;
            using Mutex mutex = new Mutex(true, "X3JAMSERVER", out createNew);
            if (createNew) {
                Program pg = new Program();
                pg.IntializeComponent();
                pg.Run();
            } else {
                MessageBox.Show("The server program already open!", "Error");
            }
        }

        public void IntializeComponent() {
            this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
        }
    }
}
