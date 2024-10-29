using System.Windows;
using ViewModel;

namespace WpfApp
{
   //prog
    public partial class MainWindow : Window
    {
        public ViewData VD;
        public MainWindow()
        {
            VD = new ViewData();
            DataContext = VD;
            InitializeComponent();
        }
    }
}
