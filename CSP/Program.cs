using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSP.NFLSchedule;

namespace CSP
{
    class Program
    {
        static void Main(string[] args)
        {
            //while(true)
            //{
            //    Console.WriteLine("WYBIERZ PROBLEM");
            //    Console.WriteLine("[1] Jolka");
            //    Console.WriteLine("[2] Sudoku");
            //    int option = int.Parse(Console.ReadLine());
            //    switch (option)
            //    {
            //        case 1:
            //            new JolkaMenu().PrintMenu();
            //            break;
            //        case 2:
            //            new SudokuMenu().PrintMenu();
            //            break;
            //        default:
            //            Console.WriteLine("Nie ma takiej opcji!");
            //            break;
            //    }
            //}
            Parser parser = new Parser();
            parser.parse();
        }
    }
}
