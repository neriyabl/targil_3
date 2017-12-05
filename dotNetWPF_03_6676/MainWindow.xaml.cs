using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dotNetWPF_03_6676
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        PrinterUserControl CourentPrinter = new PrinterUserControl();
        Queue<PrinterUserControl> queue = new Queue<PrinterUserControl>();
        //fljkglfkjgkljgvsd

        public static MessageBoxResult pop_window;
        public static Thread thread;

        public MainWindow()
        {

            InitializeComponent();
            foreach (Control item in printersGrid.Children)
            {
                if (item is PrinterUserControl)
                {
                    PrinterUserControl printer = item as PrinterUserControl;
                    printer.PageMissing += page_miss;
                    printer.InkEmpty += ink_empty;
                    queue.Enqueue(printer);
                }
            }
            CourentPrinter = queue.Dequeue();
        }

        private void page_miss(object sender, PrinterEventArgs e)
        {
            new Thread(() => { MessageBox.Show("at: " + e.time + "\nMessage from printer: " + e.worning, e.name_printer + " Page Missing !!!", MessageBoxButton.OK, MessageBoxImage.Error); }).Start();
            PrinterUserControl temp = sender as PrinterUserControl;
            next_printer();
            new Thread(() =>
            {
                Thread.Sleep(rand.r.Next(1000, 6000));
                Dispatcher.Invoke(() => { temp.add_pages(); });
                valid(temp);
            }).Start();

        }

        private void ink_empty(object sender, PrinterEventArgs e)
        {
            if (e.worning_kritic)
            {
                new Thread(() => { MessageBox.Show("at: " + e.time + "\nMessage from printer: " + e.worning, e.name_printer + " Ink Missing !!!", MessageBoxButton.OK, MessageBoxImage.Error); }).Start();
                PrinterUserControl temp = sender as PrinterUserControl;
                next_printer();
                new Thread(() =>
                {
                    Thread.Sleep(rand.r.Next(1000, 6000));
                    Dispatcher.Invoke(() => { temp.add_ink(); });
                    valid(temp);
                }).Start();

            }
            else
            {
                new Thread(() => { MessageBox.Show("at: " + e.time + "\nMessage from printer: " + e.worning, e.name_printer + " Ink Missing !!!", MessageBoxButton.OK, MessageBoxImage.Warning); }).Start();
            }
        }

       private void valid(PrinterUserControl temp)
        {
            Dispatcher.Invoke(() =>
            {
                if (!printButton.IsEnabled)
                {
                    if (temp != CourentPrinter)
                    {
                        queue.Enqueue(CourentPrinter);
                        CourentPrinter = temp;
                    }
                    if (CourentPrinter.available)
                    {
                        printButton.Content = "Print";
                        printButton.IsEnabled = true;
                    }
                }
            });
        }

        private void next_printer()
        {
            PrinterUserControl temp;
            int i = 0;
            for (; i < queue.Count && !CourentPrinter.available; i++)
            {
                temp = CourentPrinter;
                CourentPrinter = queue.Dequeue();
                queue.Enqueue(temp);
            }
            if (i == queue.Count)
            {
                printButton.IsEnabled = false;
                printButton.Content = "Please wait";
            }
        }

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            if (CourentPrinter.available)
                CourentPrinter.print();
        }

    }
}
