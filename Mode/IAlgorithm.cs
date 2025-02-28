using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mode
{
    public interface IAlgorithm
    {
        (int, int) GetNextMove(int[,] board, int player);
    }
}
