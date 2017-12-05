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
    /// Interaction logic for PrinterUserControl.xaml
    /// </summary>


    public class PrinterEventArgs
    {
        public readonly bool worning_kritic;
        public readonly DateTime time;
        public readonly string worning;
        public readonly string name_printer;

        public PrinterEventArgs(bool _worning_kritic, string _worning, string _name_printer)
        {
            time = DateTime.Now;
            worning_kritic = _worning_kritic;
            worning = _worning;
            name_printer = _name_printer;
        }
    }

    class rand
    { public static Random r = new Random(); }

    public partial class PrinterUserControl : UserControl
    {
        //-----------------consts--------------
        public static readonly int MAX_INK = 100;
        public static readonly int MIN_ADD_INK = MAX_INK / 10;
        public static readonly int MAX_PRINT_INK = MAX_INK / 50;

        public static readonly int MAX_PAGES = 400;
        public static readonly int MIN_ADD_PAGES = MAX_PAGES / 40;
        public static readonly int MAX_PRINT_PAGES = MAX_PAGES / 16;
        //--------------------------------------

        public static double MaxPages { get { return MAX_PAGES; } }

        private static int num_printers;
        private string PrinterName
        {
            set { printerNameLabel.Content = value; }
            get { return printerNameLabel.Content.ToString(); }
        }
        private double InkCount
        {
            set { inkCountProgressBar.Value = value; }
            get { return inkCountProgressBar.Value; }
        }
        private int PageCount
        {
            set { pageCountSlider.Value = value; }
            get { return (int)pageCountSlider.Value; }
        }
        public bool available { get { return PageCount > 0 && InkCount >= 1; } }


        private string cut_str(string ink)
        {
            for (int i = 0; i < ink.Length; i++)
            {
                if (ink[i] == '.')
                    return ink.Substring(0, i + 2);
            }
            return ink;
        }

        public PrinterUserControl()
        {
            InitializeComponent();
            PrinterName = "printer " + (num_printers++).ToString();
            InkCount = inkCountProgressBar.Value;
            PageCount = Convert.ToInt32(pageCountSlider.Value);

            inkCountProgressBar.ToolTip = new ToolTip();
            pageCountSlider.ToolTip = new ToolTip();
        }
        //---------------events-------------
        public EventHandler<PrinterEventArgs> PageMissing;
        public EventHandler<PrinterEventArgs> InkEmpty;

        private void page_missing(int num_page_missing)
        {
            pageLabel.Foreground = Brushes.Red;
            PageMissing(this, new PrinterEventArgs(true, "missing " + num_page_missing.ToString() + " pages", PrinterName));
        }
        private void ink_empty()
        {
            if (InkCount > 10 && InkCount <= 15)
            {
                inkLabel.Foreground = Brushes.Yellow;
                InkEmpty(this, new PrinterEventArgs(false, "your ink is only " + cut_str(InkCount.ToString()) + "%", PrinterName));
            }
            else if (InkCount <= 10 && InkCount > 1)
            {
                inkLabel.Foreground = Brushes.Orange;
                InkEmpty(this, new PrinterEventArgs(false, "your ink is only " + cut_str(InkCount.ToString()) + "%", PrinterName));
            }
            else if (InkCount <= 1)
            {
                inkLabel.Foreground = Brushes.Red;
                InkEmpty(this, new PrinterEventArgs(true, "your ink is only " + cut_str(InkCount.ToString()) + "%", PrinterName));
            }
        }

        //-------------------------------------------------
        public void print()
        {
            int print_page = rand.r.Next(MAX_PRINT_PAGES);
            if (PageCount > print_page)
                PageCount -= print_page;
            else
            {
                PageCount = 0;
                page_missing(print_page - PageCount);
            }

            double print_ink = ((double)rand.r.Next(MAX_PRINT_INK * 10) / 10);
            InkCount -= print_ink;
            if (InkCount < 0)
                InkCount = 0;
            ink_empty();
        }

        public void add_ink()
        {
            InkCount += ((double)rand.r.Next(MIN_ADD_INK, (MAX_INK - (int)InkCount) * 10) / 10);
            if (InkCount > 15)
            {
                inkLabel.Foreground = Brushes.Black;
            }
            ink_empty();
        }

        public void add_pages()
        {
            PageCount = rand.r.Next(MIN_ADD_PAGES, MAX_PAGES - PageCount);
            pageLabel.Foreground = Brushes.Black;
        }
        //--------------------------------------------------
        private void printerNameLabel_MouseMove(object sender, MouseEventArgs e)
        {
            printerNameLabel.FontSize = 26;
        }

        private void printerNameLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            printerNameLabel.FontSize = 16;
        }
        //---------------------------------------------------
        //---------------tool-tips--------------

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            (inkCountProgressBar.ToolTip as ToolTip).Content= cut_str(InkCount.ToString()) + "%";
        }

        private void pageCountSlider_MouseEnter(object sender, MouseEventArgs e)
        {
            (pageCountSlider.ToolTip as ToolTip).Content = PageCount;
        }
    }
}
