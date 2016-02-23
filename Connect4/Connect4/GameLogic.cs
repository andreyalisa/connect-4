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
        }

        private static GameLogic instance = new GameLogic();

        public static GameLogic getInstance()
        {
            return instance;
        }

        private Action<int> onWon;
        private Action<int> onDraw;

        /// <summary>
        /// Adds an action for onWon, which calls when someone wins
        /// </summary>
        /// <param name="observer">observer method</param>
        public void AddonWonObserver(Action<int> observer)
        {

        }

        /// <summary>
        /// adds an action for onDraw, wich calls when there are draw
        /// </summary>
        /// <param name="observer">observer method</param>
        public void AddonDrawObserver(Action<int> observer)
        {
        }

        /// <summary>
        /// removes an observer from onWin
        /// </summary>
        /// <param name="observer">observer method</param>
        public void RemoveonWonObserver(Action<int> observer)
        {

        }

        /// <summary>
        /// removes an observer from onDraw
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveonDrawObserver(Action<int> observer)
        {

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
            return false;
        }

         /// <summary>
         /// Returns true if some team wons after a some move
         /// </summary>
         /// <param name="color">the team of the chip to add (1 or 2)</param>
         /// <returns></returns>
        private bool IsWon(int color, int x, int y)
        {
            return false;
        }

        /// <summary>
        /// Chech if team can make a move on the column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool CanMove(int column)
        {

        }

        /// <summary>
        /// Returns true if battlefield is full
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {

        }

        /// <summary>
        /// Resets the battle fields
        /// </summary>
        public void ResetBattleFields()
        {

        }
    }
}
