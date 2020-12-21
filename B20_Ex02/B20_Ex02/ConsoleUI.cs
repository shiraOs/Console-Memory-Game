using System;
using System.Threading;
using System.Text;

namespace B20_Ex02
{
    public class ConsoleUI
    {
        private const int k_FirstRevealed = 1;
        private const int k_SecondRevealed = 2;

        private readonly object[] r_CharsToPrint = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R' };
        private Game m_Game;

        internal ConsoleUI()
        {
            string player1Name, player2Name;
            bool isPvc = false;
            int row = 0, col = 0, choice = 0;
            
            Console.WriteLine("Welcome to Memory Game! \nPlease Enter Your Name:");
            player1Name = Console.ReadLine();

            while (choice != 1 && choice != 2)
            {
                Console.WriteLine("Press 1 to play Player Vs Player \nPress 2 to play Player Vs PC");
                int.TryParse(Console.ReadLine(), out choice);
            }

            if (choice == 1)
            {
                Console.WriteLine("Please Enter Other Player Name:");
                player2Name = Console.ReadLine();
            }
            else
            {
                player2Name = "Artificial Intelligence";
                isPvc = true;
            }

            getBoardSize(ref row, ref col);
            m_Game = new Game(player1Name, player2Name, isPvc, row, col);
        }

        private void getBoardSize(ref int io_Row, ref int io_Col)
        {
            do
            {
                io_Row = 0;
                io_Col = 0;

                while (io_Row < 4 || io_Row > 6)
                {
                    Console.WriteLine("Enter number of Rows (Between 4-6)");
                    int.TryParse(Console.ReadLine(), out io_Row);
                }

                while (io_Col < 4 || io_Col > 6)
                {
                    Console.WriteLine("Enter number of Cols (Between 4-6)");
                    int.TryParse(Console.ReadLine(), out io_Col);
                }

                if ((io_Row * io_Col) % 2 != 0)
                {
                    Console.WriteLine("The number of board tiles must be even!");
                }
            }
            while ((io_Row * io_Col) % 2 != 0);
        }

        private void playGame()
        {
            bool isTurnPlayer1 = true;
            int row1 = 0, col1 = 0, row2 = 0, col2 = 0;

            while (!m_Game.IsGameOver())
            {
                printGameBoard();
                revealTile(ref row1, ref col1, isTurnPlayer1, k_FirstRevealed);
                revealTile(ref row2, ref col2, isTurnPlayer1, k_SecondRevealed);
                m_Game.CheckTurn(row1, col1, row2, col2, ref isTurnPlayer1);
            }

            printScore();
        }

        private void revealTile(ref int io_Row, ref int io_Col, bool io_TurnPlayer1, int i_RevealedNumber)
        {
            string playerName = m_Game.Player1Name();

            if (!io_TurnPlayer1)
            {
                playerName = m_Game.Player2Name();
            }

            Console.WriteLine("{0}'s turn:{1}", playerName, Environment.NewLine);    
            getInput(ref io_Row, ref io_Col, io_TurnPlayer1);

            if (i_RevealedNumber == k_FirstRevealed)
            {
                m_Game.FirstReveal(io_Row, io_Col, io_TurnPlayer1);
            }
            else
            {
                m_Game.SecondReveal(io_Row, io_Col);
            }
            
            printGameBoard();
        }

        private void printScore()
        {
            StringBuilder prompt = new StringBuilder();
            string msg = string.Empty;

            if (m_Game.GetWinner(out string winner))
            { // Case of tie
                msg = "It's a TIE!\n";
            }
            else
            {
                msg = string.Format("{0} WON!{1}", winner, Environment.NewLine);
            }

            prompt.Append(msg);
            msg = string.Format(
                "{0} with {1} pairs revealed.{2}{3} with {4} pairs revealed.",
                m_Game.Player1Name(),
                m_Game.Player1Score(),
                Environment.NewLine,
                m_Game.Player2Name(),
                m_Game.Player2Score());
            prompt.Append(msg);
            Console.WriteLine(prompt);
        }

        private void getInput(ref int io_Row, ref int io_Col, bool io_TurnPlayer1)
        {
            if (!io_TurnPlayer1 && m_Game.IsAIPlay())
            {
                Thread.Sleep(1000);
                m_Game.GetInputFromAI(ref io_Row, ref io_Col);
            }
            else
            {
                getInputFromUser(ref io_Row, ref io_Col);
            }
        }

        private void getInputFromUser(ref int io_Row, ref int io_Col)
        {
            string turn = string.Empty;
            bool isValid = false;

            Console.WriteLine("Enter play (Enter 'Q' to Quit)");

            while (!isValid)
            {
                turn = Console.ReadLine();
                isValid = validInput(turn, ref io_Col, ref io_Row);                         
            }
        }

        private void printGameBoard()
        {            
            Ex02.ConsoleUtils.Screen.Clear();
            Console.WriteLine(m_Game.ToStringBuilder(r_CharsToPrint));
        }

        private bool validInput(string i_UserInput, ref int io_Col, ref int io_Row)
        {
            bool isValid = true;
            char maxLetter = (char)(m_Game.BoardCols() + 'A' - 1);
            char maxNumber = (char)(m_Game.BoardRows() + '1' - 1);

            if (i_UserInput == "Q")
            {
                Ex02.ConsoleUtils.Screen.Clear();
                Console.WriteLine("Bye Bye!{0}exiting game", Environment.NewLine);
                Thread.Sleep(1000);
                Environment.Exit(1);
            }

            if (i_UserInput.Length != 2 )
            {
                Console.WriteLine("Input must be 'col-row' (for example: 'A1')");
                isValid = false;
            }
            else
            {
                if (i_UserInput[0] > maxLetter || i_UserInput[0] < 'A')
                {
                    Console.WriteLine("Input col must be in board size ('A'-'{0}')", maxLetter);
                    isValid = false;
                }

                if (i_UserInput[1] > maxNumber || i_UserInput[1] < '1')
                {
                    Console.WriteLine("Input row must be in board size ('1'-'{0}')", maxNumber);
                    isValid = false;
                }
            }

            if (isValid)
            {
                int.TryParse((i_UserInput[0] - 'A').ToString(), out io_Col);
                int.TryParse((i_UserInput[1] - '1').ToString(), out io_Row);

                isValid = m_Game.CheckTile(io_Col, io_Row);

                if (!isValid) 
                {
                    Console.WriteLine("Input tile is already revealed! Try again");
                }
            }

            return isValid;        
        }

        public static void RunMainMenu()
        {
            bool play = true;
            char input = ' ';

            while (play) 
            {
                Ex02.ConsoleUtils.Screen.Clear();
                ConsoleUI Mygame = new ConsoleUI();

                Ex02.ConsoleUtils.Screen.Clear();
                Mygame.playGame();

                Console.WriteLine("{0}Do you wish to play again?{0}Type 'Y' for Yes{0}Type 'N' for No", Environment.NewLine);

                while (!char.TryParse(Console.ReadLine(), out input) && input != 'Y' && input != 'N') 
                {
                    Console.WriteLine("Wrong input!{0}Type 'Y' for Yes{0}Type 'N' for No", Environment.NewLine);
                }
                
                if (input == 'N')
                {
                    Ex02.ConsoleUtils.Screen.Clear();
                    Console.WriteLine("Bye Bye!{0}exiting game", Environment.NewLine);
                    Thread.Sleep(1000);
                    play = false;
                }
            }
        }
    }
}