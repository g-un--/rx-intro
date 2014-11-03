using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
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
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading;

namespace RxExcelLikeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private BehaviorSubject<string> cell1Signal = new BehaviorSubject<string>(string.Empty);
        private BehaviorSubject<string> cell2Signal = new BehaviorSubject<string>(string.Empty);
        private BehaviorSubject<string> cell3Signal = new BehaviorSubject<string>(string.Empty);
 
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AttachTextBoxToInputSignal(textCell1, cell1Signal);
            AttachTextBoxToInputSignal(textCell2, cell2Signal);
            AttachOutputSignalToTextBox(textCell3, cell3Signal);

            var sumModel = new SumModel(cell1Signal, cell2Signal);
            sumModel
                .Compute().Select(sum => sum.ToString())
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(cell3Signal);
        }

        void AttachTextBoxToInputSignal(TextBox textBox, BehaviorSubject<string> signal)
        {
            textBox.TextChanged += (sender, textChanged) => { signal.OnNext(textBox.Text.Trim()); };
        }

        void AttachOutputSignalToTextBox(TextBox textBox, BehaviorSubject<string> signal)
        {
            signal.Subscribe(value => textBox.Text = value);
        }
    }
}
