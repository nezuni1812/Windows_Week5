using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowsExerciseWeek5
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        class Config
        {
            public const int Margin = 5;
            public const int Width = 50;
            public const int Height = 50;
        };

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            var button01 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = 1,

            };

            button01.Click += Button_Click;

            var button02 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = 2,
            };

            button02.Click += Button_Click;

            var button03 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = 3,
            };

            button03.Click += Button_Click;

            var button04 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = 4,
            };

            button04.Click += Button_Click;

            container.Children.Add(button01);
            
            Canvas.SetLeft(button02, Config.Width + Config.Margin);

            Canvas.SetTop(button03, Config.Height + Config.Margin);
            Canvas.SetLeft(button03, Config.Width + Config.Margin);

            Canvas.SetTop(button04, Config.Height + Config.Margin);
            //Canvas.SetLeft(button04, Config.Width + Config.Margin);

            container.Children.Add(button02);
            container.Children.Add(button03);
            container.Children.Add(button04);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (sender as Button)!;
            int number = (int)button.Tag;
            Title = number.ToString();
        }
    }
}
