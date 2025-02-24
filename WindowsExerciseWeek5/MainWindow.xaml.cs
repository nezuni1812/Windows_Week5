using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
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

        public int[,] Board = new int[3, 3];
        public int Player = 1;
        public int CheckWinner(int[,] Board)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Board[i, 0] != 0 && Board[i, 0] == Board[i, 1] && Board[i, 1] == Board[i, 2])
                    return Board[i, 0]; 

                if (Board[0, i] != 0 && Board[0, i] == Board[1, i] && Board[1, i] == Board[2, i])
                    return Board[0, i]; 
            }

            if (Board[0, 0] != 0 && Board[0, 0] == Board[1, 1] && Board[1, 1] == Board[2, 2])
                return Board[0, 0];

            if (Board[0, 2] != 0 && Board[0, 2] == Board[1, 1] && Board[1, 1] == Board[2, 0])
                return Board[0, 2];

            foreach (int cell in Board)
            {
                if (cell == 0)
                    return 0; 
            }

            return -1; 
        }


        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            var button01 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = new Tuple<int, int>(0, 0),
            };
            button01.Click += Button_Click;
            container.Children.Add(button01);

            var button02 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = new Tuple<int, int>(0, 1),
            };
            button02.Click += Button_Click;
            container.Children.Add(button02);
            Canvas.SetLeft(button02, Config.Width + Config.Margin);

            var button03 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = new Tuple<int, int>(0, 2),
            };
            button03.Click += Button_Click;
            container.Children.Add(button03);
            Canvas.SetLeft(button03, 2 * Config.Width + 2 * Config.Margin);

            var button04 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = new Tuple<int, int>(1, 0),
            };
            button04.Click += Button_Click;
            container.Children.Add(button04);
            Canvas.SetTop(button04, Config.Height + Config.Margin);

            var button05 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = new Tuple<int, int>(1, 1),
            };
            button05.Click += Button_Click;
            container.Children.Add(button05);
            Canvas.SetLeft(button05, Config.Width + Config.Margin);
            Canvas.SetTop(button05, Config.Height + Config.Margin);

            var button06 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = new Tuple<int, int>(1, 2),
            };
            button06.Click += Button_Click;
            container.Children.Add(button06);
            Canvas.SetLeft(button06, 2 * Config.Width + 2 * Config.Margin);
            Canvas.SetTop(button06, Config.Height + Config.Margin);

            var button07 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = new Tuple<int, int>(2, 0),
            };
            button07.Click += Button_Click;
            container.Children.Add(button07);
            Canvas.SetTop(button07, 2 * Config.Height + 2 * Config.Margin);

            var button08 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = new Tuple<int, int>(2, 1),
            };
            button08.Click += Button_Click;
            container.Children.Add(button08);
            Canvas.SetLeft(button08, Config.Width + Config.Margin);
            Canvas.SetTop(button08, 2 * Config.Height + 2 * Config.Margin);

            var button09 = new Button()
            {
                Width = Config.Width,
                Height = Config.Height,
                Tag = new Tuple<int, int>(2, 2),
            };
            button09.Click += Button_Click;
            container.Children.Add(button09);
            Canvas.SetLeft(button09, 2 * Config.Width + 2 * Config.Margin);
            Canvas.SetTop(button09, 2 * Config.Height + 2 * Config.Margin);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null || button.Tag == null) return;

            var (x, y) = (Tuple<int, int>)button.Tag;
            Title = $"{x} - {y}";

            if (Board != null && Board.GetLength(0) > x && Board.GetLength(1) > y)
            {
                Board[x, y] = Player;
                button.Content = Player == 1 ? "X" : "O";
            }

            Player = Player == 1 ? 2 : 1;

            int winner = CheckWinner(Board);
            if (winner == 1 || winner == 2)
            {
                var dialog = new ContentDialog
                {
                    Title = "Result",
                    Content = $"Player {winner} wins!",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot,

                };

                await dialog.ShowAsync();
            }
            else if (winner == -1)
            {
                var dialog = new ContentDialog
                {
                    Title = "Result",
                    Content = "It's a draw!",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot,
                };

                await dialog.ShowAsync();
            }
        }



    }
}
