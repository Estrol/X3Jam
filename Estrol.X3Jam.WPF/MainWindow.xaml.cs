using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Estrol.X3Jam.Server;

namespace Estrol.X3Jam.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private O2JamServer Server;

        public MainWindow() {
            InitializeComponent();

            ConsoleWindow.Text = "[Message] Welcome to X3-JAM. written by DMJam Dev Group (Estrol and MatVeiQaaa)\n"
                + string.Format("[Message] Current time is {0}\n", DateTime.Now);

            Console.SetOut(new MultiTextWriter(new RichTextBoxWritter(ConsoleWindow), Console.Out));

            Server = new O2JamServer();
            Server.Intialize();
        }

        private void TopBarCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }
    }

    public class MultiTextWriter : TextWriter {
        private IEnumerable<TextWriter> writers;
        public MultiTextWriter(IEnumerable<TextWriter> writers) {
            this.writers = writers.ToList();
        }

        public MultiTextWriter(params TextWriter[] writers) {
            this.writers = writers;
        }

        public override void Write(char value) {
            foreach (var writer in writers)
                writer.Write(value);
        }

        public override void Write(string value) {
            foreach (var writer in writers)
                writer.Write(value);
        }

        public override void Flush() {
            foreach (var writer in writers)
                writer.Flush();
        }

        public override void Close() {
            foreach (var writer in writers)
                writer.Close();
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }

    public class RichTextBoxWritter : TextWriter {
        public TextBox tb;

        public RichTextBoxWritter(TextBox tb) {
            this.tb = tb;
        }

        public override void Write(char value) {
            tb.Dispatcher.Invoke(delegate {
                tb.AppendText(value.ToString());
            });
        }

        public override void Write(string value) {
            tb.Dispatcher.Invoke(delegate {
                tb.AppendText(value);
            });
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}
