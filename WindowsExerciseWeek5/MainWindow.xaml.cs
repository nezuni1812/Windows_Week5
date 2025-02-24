using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
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
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            InitializeGameBoard();
        }

        class Config
        {
            public const int Margin = 5;
            public const int Width = 50;
            public const int Height = 50;
        };

        public int[,] Board = new int[3, 3];
        public int Player = 1;
        public string SelectedDifficulty = "Easy"; // Mặc định là Easy
        private dynamic aiModule = null;
        private Type aiType = null;
        private object aiInstance = null;
        private MethodInfo getNextMoveMethod = null;

        private void SetDifficulty_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                SelectedDifficulty = button.Tag.ToString();
                Title = $"TicTacToe - {SelectedDifficulty} Mode";
                LoadDifficultyDll();
            }
        }

        private void LoadDifficultyDll()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            var dllFiles = new DirectoryInfo(folder).GetFiles("*.dll");

            foreach (var file in dllFiles)
            {
                if (file.Name.StartsWith(SelectedDifficulty, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(file.FullName);
                        var types = assembly.GetTypes();

                        // Find a type that has GetNextMove method
                        foreach (var type in types)
                        {
                            var method = type.GetMethod("GetNextMove");
                            if (method != null)
                            {
                                aiType = type;
                                aiInstance = Activator.CreateInstance(type);
                                getNextMoveMethod = method;
                                Title = $"Loaded {file.Name}";
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Title = $"Error loading {file.Name}: {ex.Message}";
                    }
                }
            }
        }

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
            // Only initialize the game once on first activation
            if (aiInstance == null)
            {
                LoadDifficultyDll();
                InitializeGameBoard();
            }
        }

        private void InitializeGameBoard()
        {
            container.Children.Clear();
            Board = new int[3, 3];
            Player = 1;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var button = new Button()
                    {
                        Width = Config.Width,
                        Height = Config.Height,
                        Tag = new Tuple<int, int>(i, j),
                    };
                    button.Click += Button_Click;
                    container.Children.Add(button);
                    Canvas.SetLeft(button, j * (Config.Width + Config.Margin));
                    Canvas.SetTop(button, i * (Config.Height + Config.Margin));
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null || button.Tag == null) return;

            var (x, y) = (Tuple<int, int>)button.Tag;

            if (Board[x, y] == 0)
            {
                // Human player move (Player 1)
                Board[x, y] = Player;

                button.Content = new FontIcon()
                {
                    Glyph = Player == 1 ? "\uE894" : "\uEA3F",
                    FontSize = 25,
                    Foreground = new SolidColorBrush(Player == 1 ? Colors.Blue : Colors.Red),
                };

                // Check if the game is over after human move
                int winner = CheckWinner(Board);
                if (winner != 0)
                {
                    await ShowGameOverDialog(winner);
                    return;
                }

                // Switch to computer player
                Player = 2;

                // Perform computer move
                MakeComputerMove();
            }
        }

        private async void MakeComputerMove()
        {
            if (getNextMoveMethod != null && aiInstance != null)
            {
                try
                {
                    Title = "AI is thinking...";

                    var result = getNextMoveMethod.Invoke(aiInstance, new object[] { Board, Player });

                    // Ép kiểu ValueTuple đúng cách
                    if (result is (int x, int y))
                    {
                        Title = "AI move received";

                        // Kiểm tra nước đi hợp lệ
                        if (x >= 0 && x < 3 && y >= 0 && y < 3 && Board[x, y] == 0)
                        {
                            Board[x, y] = Player;

                            foreach (var child in container.Children)
                            {
                                if (child is Button btn && btn.Tag is Tuple<int, int> tag)
                                {
                                    if (tag.Item1 == x && tag.Item2 == y)
                                    {
                                        btn.Content = new FontIcon()
                                        {
                                            Glyph = "\uEA3F", // Ký tự của máy
                                            FontSize = 25,
                                            Foreground = new SolidColorBrush(Colors.Red),
                                        };
                                        break;
                                    }
                                }
                            }

                            int winner = CheckWinner(Board);
                            if (winner != 0)
                            {
                                await ShowGameOverDialog(winner);
                                return;
                            }
                        }
                        else
                        {
                            Title = "AI returned invalid move";
                        }
                    }
                    else
                    {
                        Title = "AI returned invalid move";
                    }
                }
                catch (Exception ex)
                {
                    Title = $"AI Error: {ex.Message}";
                }
            }
            else
            {
                Title = "Fallback to random move";
                MakeRandomMove();
            }

            Player = 1; // Quay lại lượt của người chơi
        }



        private void MakeRandomMove()
        {
            // Find empty cells
            List<Tuple<int, int>> emptyCells = new List<Tuple<int, int>>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Board[i, j] == 0)
                        emptyCells.Add(new Tuple<int, int>(i, j));
                }
            }

            if (emptyCells.Count > 0)
            {
                // Choose a random empty cell
                Random random = new Random();
                int index = random.Next(emptyCells.Count);
                var (x, y) = emptyCells[index];

                // Update the board
                Board[x, y] = Player;

                // Find and update the button
                foreach (var child in container.Children)
                {
                    if (child is Button button && button.Tag is Tuple<int, int> tag)
                    {
                        if (tag.Item1 == x && tag.Item2 == y)
                        {
                            button.Content = new FontIcon()
                            {
                                Glyph = "\uEA3F", // Computer's symbol
                                FontSize = 25,
                                Foreground = new SolidColorBrush(Colors.Red),
                            };
                            break;
                        }
                    }
                }
            }
        }

        private async Task ShowGameOverDialog(int winner)
        {
            string message = winner == -1 ? "It's a draw!" : $"Player {winner} wins!";
            var dialog = new ContentDialog
            {
                Title = "Game Over",
                Content = message,
                CloseButtonText = "OK",
                PrimaryButtonText = "New Game",
                XamlRoot = this.Content.XamlRoot,
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                InitializeGameBoard();
            }
        }

        private void ResetGame_Click(object sender, RoutedEventArgs e)
        {
            InitializeGameBoard();
        }
    }
}