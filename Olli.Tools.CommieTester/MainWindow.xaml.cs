using System.Windows;
using Olli.Tools.CommieTester.ViewModels;

namespace Olli.Tools.CommieTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SerialReaderViewModel    srvm = new SerialReaderViewModel("Com12");
            SerialGeneratorViewModel sgvm = new SerialGeneratorViewModel("Com12");
            UdpReaderViewModel       urvm = new UdpReaderViewModel(5060);
            UdpGeneratorViewModel    ugvm = new UdpGeneratorViewModel(5060);
            ConverterViewModel       cvm  = new ConverterViewModel("Com12", 5060);

            view_SerialReader.DataContext    = srvm;
            view_SerialGenerator.DataContext = sgvm;
            view_UdpReader.DataContext       = urvm;
            view_UdpGenerator.DataContext    = ugvm;
            view_Converter.DataContext       = cvm;
        }
    }
}
