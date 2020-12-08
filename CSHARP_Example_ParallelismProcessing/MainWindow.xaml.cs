using System;
using System.Collections.Generic;
using System.Windows;

namespace CSHARP_Example_ParallelismProcessing
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private List<string> value = new List<string>();
        const int MAX = 100000;
        const int SUB = 1000000;

        private void btn_nomal_Click(object sender, RoutedEventArgs e)
        {
            DateTime start = DateTime.Now;

            value.Clear();
            for (int i = 0; i < MAX; i++)
            {

                for (int k = 0; k < SUB; k++)
                { }
                value.Add(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
            }

            MessageBox.Show(value.Count + " " + (DateTime.Now - start).ToString());
        }

        private void btn_ParallelismProcessing_Click(object sender, RoutedEventArgs e)
        {
            DateTime start = DateTime.Now;
            value.Clear();

            //프로세서 개수
            int processorCount = Environment.ProcessorCount;
            //반복 회수
            int loopCount = 0;


            try
            {
                using (System.Threading.ManualResetEvent done = new System.Threading.ManualResetEvent(false))
                {
                    for (int i = 0; i < processorCount; i++)
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(
                            delegate
                            {
                                while (loopCount <= MAX)
                                {
                                    for (int k = 0; k < SUB; k++)
                                    { }

                                    lock (value)
                                    {            
                                        if(loopCount++ < MAX)
                                            value.Add(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffff"));                                      
                                    }
                                }


                                if (System.Threading.Interlocked.Decrement(ref processorCount) == 0) 
                                    done.Set();
                            }
                        );
                    }

                    done.WaitOne();
                    MessageBox.Show(value.Count + " " +(DateTime.Now - start).ToString());
                }
            }
            finally
            {
                
            }
        }
    }
}
