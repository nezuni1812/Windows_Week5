using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Mode;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Medium
{
    public class MediumAlgorithm : IAlgorithm
    {
        private Random _random = new Random();

        // Tỉ lệ sử dụng thuật toán khó (được điều chỉnh để đạt tỉ lệ thắng 51/49)
        private const double SMART_PLAY_PROBABILITY = 0.55;

        public (int, int) GetNextMove(int[,] board, int player)
        {
            // Tính toán số ô trống còn lại
            int emptySpaces = CountEmptySpaces(board);

            // Kiểm tra xem có thể thắng ngay trong nước đi tiếp theo không
            var winningMove = FindWinningMove(board, player);
            if (winningMove != (-1, -1))
            {
                return winningMove;
            }

            // Kiểm tra xem có cần phòng thủ để tránh thua ngay không
            var defensiveMove = FindWinningMove(board, player == 1 ? 2 : 1);
            if (defensiveMove != (-1, -1))
            {
                return defensiveMove;
            }

            // Điều chỉnh cơ hội sử dụng thuật toán thông minh dựa vào số ô trống
            double smartProbability = SMART_PLAY_PROBABILITY;
            if (emptySpaces >= 8)
            {
                // Giảm khả năng dùng thuật toán khó ở các nước đi đầu tiên
                smartProbability = 0.52;
            }

            // Sử dụng thuật toán thông minh với xác suất đã điều chỉnh
            if (_random.NextDouble() < smartProbability)
            {
                return GetOptimalMove(board, player);
            }
            else
            {
                // Chọn nước đi ngẫu nhiên trong các ô trống
                List<(int, int)> availableMoves = GetAvailableMoves(board);

                if (availableMoves.Count == 0) return (-1, -1);

                // Ưu tiên chọn các ô góc và trung tâm khi chơi ngẫu nhiên
                if (emptySpaces > 5 && _random.NextDouble() < 0.6)
                {
                    var strategicMoves = availableMoves.Where(move =>
                        (move.Item1 == 1 && move.Item2 == 1) || // Trung tâm
                        (move.Item1 == 0 && move.Item2 == 0) || // Góc trên trái
                        (move.Item1 == 0 && move.Item2 == 2) || // Góc trên phải
                        (move.Item1 == 2 && move.Item2 == 0) || // Góc dưới trái
                        (move.Item1 == 2 && move.Item2 == 2)    // Góc dưới phải
                    ).ToList();

                    if (strategicMoves.Count > 0)
                    {
                        return strategicMoves[_random.Next(strategicMoves.Count)];
                    }
                }

                return availableMoves[_random.Next(availableMoves.Count)];
            }
        }

        // Tìm nước đi tối ưu sử dụng thuật toán Minimax
        private (int, int) GetOptimalMove(int[,] board, int player)
        {
            int bestScore = int.MinValue;
            (int, int) bestMove = (-1, -1);

            // Giới hạn độ sâu tìm kiếm để tạo ra các lựa chọn không hoàn hảo
            int maxDepth = 2; // Giới hạn độ sâu cho phép máy đôi khi bỏ lỡ nước đi tốt nhất

            foreach (var move in GetAvailableMoves(board))
            {
                int i = move.Item1;
                int j = move.Item2;

                board[i, j] = player;
                int score = Minimax(board, 0, false, player, maxDepth);
                board[i, j] = 0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = (i, j);
                }
            }

            return bestMove;
        }

        // Thuật toán Minimax có giới hạn độ sâu
        private int Minimax(int[,] board, int depth, bool isMaximizing, int player, int maxDepth)
        {
            int opponent = player == 1 ? 2 : 1;
            int winner = CheckWinner(board);

            // Điều kiện dừng: có người thắng, hòa, hoặc đạt giới hạn độ sâu
            if (winner == player) return 10 - depth; // Thắng (điểm cao hơn nếu thắng sớm)
            if (winner == opponent) return depth - 10; // Thua (điểm thấp hơn nếu thua sớm)
            if (winner == -1) return 0; // Hòa
            if (depth >= maxDepth) return 0; // Đạt giới hạn độ sâu

            List<(int, int)> availableMoves = GetAvailableMoves(board);
            if (availableMoves.Count == 0) return 0;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                foreach (var move in availableMoves)
                {
                    board[move.Item1, move.Item2] = player;
                    int score = Minimax(board, depth + 1, false, player, maxDepth);
                    board[move.Item1, move.Item2] = 0;
                    bestScore = Math.Max(bestScore, score);
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                foreach (var move in availableMoves)
                {
                    board[move.Item1, move.Item2] = opponent;
                    int score = Minimax(board, depth + 1, true, player, maxDepth);
                    board[move.Item1, move.Item2] = 0;
                    bestScore = Math.Min(bestScore, score);
                }
                return bestScore;
            }
        }

        // Lấy danh sách các nước đi có thể
        private List<(int, int)> GetAvailableMoves(int[,] board)
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
            return availableMoves;
        }

        // Đếm số ô trống trên bàn cờ
        private int CountEmptySpaces(int[,] board)
        {
            int count = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        // Tìm nước đi thắng ngay cho người chơi cụ thể
        private (int, int) FindWinningMove(int[,] board, int player)
        {
            // Kiểm tra hàng ngang
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == player && board[i, 1] == player && board[i, 2] == 0)
                    return (i, 2);
                if (board[i, 0] == player && board[i, 2] == player && board[i, 1] == 0)
                    return (i, 1);
                if (board[i, 1] == player && board[i, 2] == player && board[i, 0] == 0)
                    return (i, 0);
            }

            // Kiểm tra hàng dọc
            for (int j = 0; j < 3; j++)
            {
                if (board[0, j] == player && board[1, j] == player && board[2, j] == 0)
                    return (2, j);
                if (board[0, j] == player && board[2, j] == player && board[1, j] == 0)
                    return (1, j);
                if (board[1, j] == player && board[2, j] == player && board[0, j] == 0)
                    return (0, j);
            }

            // Kiểm tra đường chéo
            if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == 0)
                return (2, 2);
            if (board[0, 0] == player && board[2, 2] == player && board[1, 1] == 0)
                return (1, 1);
            if (board[1, 1] == player && board[2, 2] == player && board[0, 0] == 0)
                return (0, 0);

            if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == 0)
                return (2, 0);
            if (board[0, 2] == player && board[2, 0] == player && board[1, 1] == 0)
                return (1, 1);
            if (board[1, 1] == player && board[2, 0] == player && board[0, 2] == 0)
                return (0, 2);

            return (-1, -1);
        }

        // Kiểm tra xem ai là người thắng
        private int CheckWinner(int[,] board)
        {
            // Kiểm tra hàng ngang
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] != 0 && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                    return board[i, 0];
            }

            // Kiểm tra hàng dọc
            for (int j = 0; j < 3; j++)
            {
                if (board[0, j] != 0 && board[0, j] == board[1, j] && board[1, j] == board[2, j])
                    return board[0, j];
            }

            // Kiểm tra đường chéo chính
            if (board[0, 0] != 0 && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                return board[0, 0];

            // Kiểm tra đường chéo phụ
            if (board[0, 2] != 0 && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                return board[0, 2];

            // Kiểm tra nếu còn ô trống
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0)
                        return 0; // Trò chơi chưa kết thúc
                }
            }

            // Nếu không có ô trống và không có người thắng
            return -1; // Hòa
        }
    }
}
