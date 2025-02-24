using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Mode;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hard
{
    public class HardAlgorithm : IAlgorithm
    {
        public (int, int) GetNextMove(int[,] board, int player)
        {
            int bestScore = int.MinValue;
            (int, int) bestMove = (-1, -1);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0)
                    {
                        board[i, j] = player;
                        int score = Minimax(board, false, player);
                        board[i, j] = 0;
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = (i, j);
                        }
                    }
                }
            }
            return bestMove;
        }

        private int Minimax(int[,] board, bool isMaximizing, int player)
        {
            int winner = CheckWinner(board);
            if (winner == player) return 1;
            if (winner == (player == 1 ? 2 : 1)) return -1;
            if (winner == -1) return 0;

            int bestScore = isMaximizing ? int.MinValue : int.MaxValue;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0)
                    {
                        board[i, j] = isMaximizing ? player : (player == 1 ? 2 : 1);
                        int score = Minimax(board, !isMaximizing, player);
                        board[i, j] = 0;
                        bestScore = isMaximizing ? Math.Max(bestScore, score) : Math.Min(bestScore, score);
                    }
                }
            }
            return bestScore;
        }

        private int CheckWinner(int[,] board)
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] != 0 && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                    return board[i, 0];
            }

            for (int j = 0; j < 3; j++)
            {
                if (board[0, j] != 0 && board[0, j] == board[1, j] && board[1, j] == board[2, j])
                    return board[0, j];
            }

            if (board[0, 0] != 0 && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                return board[0, 0];

            if (board[0, 2] != 0 && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                return board[0, 2];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0)
                        return 0; 
                }
            }

            return -1; 
        }
    }

}
