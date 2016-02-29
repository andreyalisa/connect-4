using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Connect4
{
    public sealed class GameLogic
    {
        private GameLogic()
        {
            ResetBattleFields();
        }

        private static GameLogic instance = new GameLogic();

        public static GameLogic getInstance()
        {
            return instance;
        }

        private Action<int> onWon;
        private Action<int> onDraw;

        public void AddonWonObserver(Action<int> observer)
        {
            onWon += observer;
        }

        public void AddonDrawObserver(Action<int> observer)
        {
            onDraw += observer;
        }

        public void RemoveonWonObserver(Action<int> observer)
        {
            onWon -= observer;
        }

        public void RemoveonDrawObserver(Action<int> observer)
        {
            onDraw -= observer;
        }

        private int[,] battleField;

        /// <summary>
        /// Adds a new chip into the board
        /// </summary>
        /// <param name="color">the team of the chip to add (1 or 2)</param>
        /// <param name="column">the column to add</param>
        /// <returns>
        /// true, if move is done
        /// false, if the move isn't done
        /// </returns>
        public bool MakeMove(int team, int column, ref int x, ref int y)
        {
            bool result = false;
            if (team == 1)
            {
                for (int i = 1; i < battleField.GetLength(1); i++)
                {
                    if (battleField[column, i] > 0)
                    {
                        battleField[column, i - 1] = 1;
                        result = true;
                        x = column;
                        y = i;
                        break;
                    }
                }
                if (battleField[column, battleField.GetLength(1) - 1] == 0)
                {
                    battleField[column, battleField.GetLength(1) - 1] = 1;
                    x = column;
                    y = battleField.GetLength(1) - 1;
                    result = true;
                }
            }
            else if (team == 2)
            {
                for (int i = 1; i < battleField.GetLength(1); i++)
                {
                    if (battleField[column, i] > 0)
                    {
                        battleField[column, i - 1] = 2;
                        result = true;
                        x = column;
                        y = i;
                        break;
                    }
                }
                if (battleField[column, battleField.GetLength(1) - 1] == 0)
                {
                    battleField[column, battleField.GetLength(1) - 1] = 2;
                    x = column;
                    y = battleField.GetLength(1) - 1;
                    result = true;
                }
            }
            return result;
        }


        public bool isWon(int color)
        {
            for (int i = 0; i < battleField.GetLength(0); i++)
            {
                for (int j = 0; j < battleField.GetLength(1); j++)
                {
                    if (IsWonFromCurrent(color, i, j))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
         /// <summary>
         /// Returns true if some team wons after a some move
         /// </summary>
         /// <param name="color">the team of the chip to add (1 or 2)</param>
         /// <returns></returns>
        private bool IsWonFromCurrent(int color, int x, int y)
        {
            int n = Math.Min(battleField.GetLength(0), x + 4);
            int m = Math.Min(battleField.GetLength(1), y + 4);

            //column
            if (m - y == 4)
            {
                for (int i = y; i < m; i++)
                {
                    if (battleField[x, i] != color)
                        break;
                    if (i == 3)
                    {
                        onWon(color);
                        return true;
                    }
                }
            }

            //row
            if (n - x == 4)
            {
                for (int i = x; i < n; i++)
                {
                    if (battleField[i, y] != color)
                        break;
                    if (i == 3)
                    { 
                        onWon(color);
                        return true;
                    }
                }
            }

            //diag
            if (n - x == 4 && m - y == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (battleField[x + i, y + i] != color)
                        break;
                    if (i == 3)
                    {
                        onWon(color);
                        return true;
                    } 
                }
            }

            //antidiag
            if (n - x == 4 && y - 4 >= 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (battleField[x + i, y - i] != color)
                        break;
                    if (i == 3)
                    {
                        onWon(color);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Chech if team can make a move on the column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool CanMove(int column)
        {
            if (battleField[column, 0] == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if battlefield is full
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            for (int i = 0; i < battleField.GetLength(0); i++)
            {
                if (CanMove(i)) return true;
            }
            return false;
        }

        /// <summary>
        /// Resets the battle fields
        /// </summary>
        public void ResetBattleFields()
        {
            battleField = new int[7, 6];
        }
    }
}
