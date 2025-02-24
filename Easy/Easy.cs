using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mode;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Easy
{
    public class EasyAlgorithm : IAlgorithm
    {
        private Random _random = new Random();

        public (int, int) GetNextMove(int[,] board, int player)
        {
            List<(int, int)> availableMoves = new List<(int, int)>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0)
                    {
                        availableMoves.Add((i, j));
                    }
                }
            }

            if (availableMoves.Count == 0) return (-1, -1);

            return availableMoves[_random.Next(availableMoves.Count)];
        }
    }

}
