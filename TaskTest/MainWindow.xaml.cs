using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace TaskTest
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Cases = new ObservableCollection<Cases>(new List<Cases>()
            {
                new Cases()
                {
                    Name = "測試1(有breakpoint)" ,
                    isBreakPoint = true,
                    actions = Enumerable.Range(1,10).Select(y=>new Work()
                    {
                        Name = "動作" + y.ToString(),

                    }).ToList(),
                },
                new Cases()
                {
                    Name = "測試2(無breakpoint)" ,
                    isBreakPoint = false,
                    actions = Enumerable.Range(1,5).Select(y=>new Work()
                    {
                        Name = "動作" + y.ToString(),

                    }).ToList(),
                },
            });
            selectedCase = Cases[0];
        }




        public ObservableCollection<Cases> Cases
        {
            get { return (ObservableCollection<Cases>)GetValue(CasesProperty); }
            set { SetValue(CasesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cases.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CasesProperty =
            DependencyProperty.Register("Cases", typeof(ObservableCollection<Cases>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<Cases>()));




        public Cases selectedCase
        {
            get { return (Cases)GetValue(selectedCaseProperty); }
            set { SetValue(selectedCaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for selectedCase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty selectedCaseProperty =
            DependencyProperty.Register("selectedCase", typeof(Cases), typeof(MainWindow), new PropertyMetadata(new Cases()));

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
    
            await selectedCase.DoWorks();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            selectedCase.cancellation.Cancel();
        }

      
    }
    public class Cases
    {
        public string Name { get; set; }
        public bool isBreakPoint { get; set; }
        public List<Work> actions { get; set; }
        public CancellationTokenSource cancellation = new CancellationTokenSource();
        public async Task DoWorks()
        {
            Console.WriteLine($"開始Case:{Name}");
            foreach(var action in actions)
            {
                if (cancellation.IsCancellationRequested)
                {
                    Console.WriteLine($"結束Case:{Name}");
                    return;
                }

                await action.Do(cancellation.Token);
                while (isBreakPoint)
                {
                    if (cancellation.IsCancellationRequested)
                    {
                        Console.WriteLine($"結束Case:{Name}");
                        return;
                    }
                    Console.WriteLine("等待用戶...");
                    await Task.Delay(500);
                }
            }
        }
    }
    public class Work
    {
        public string Name { get; set; }
        public async Task Do(CancellationToken cancellation)
        {
            //do something here , i dont know what you want
           
            if(cancellation.IsCancellationRequested)
            {
                Console.WriteLine($"結束動作:{Name}");
                return;
            }
            await Task.Delay(2000);  
            Console.WriteLine($"執行動作:{this.Name}");
        }
    }
}
