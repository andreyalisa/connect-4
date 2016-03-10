using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Connect4;
namespace TestingProject
{
    [TestClass]
    public class TestLogic
    {
        GameLogic gameLogic;
        bool verification;
        public TestLogic()
        {
            gameLogic = GameLogic.getInstance();
        }

        [TestMethod]
        public void TestIsWonFirstTeam1_ReturnTrue()
        {
            gameLogic.Reset();
            gameLogic.AddonWonObserver(FirstTeamWon);
            verification = false;
            for (int i = 0; i < 4; i++)
            {
                Assert.IsFalse(verification); //firstteamwon does not called yet
                gameLogic.MakeMove(1, 0);
            }
            Assert.IsTrue(verification);
        }

        [TestMethod]
        public void TestIsWonFirstTeam_ReturnFalse()
        {
            gameLogic.Reset();
            gameLogic.AddonWonObserver(FirstTeamWon);
            verification = false;
            for (int i = 0; i < 3; i++)
            {
                Assert.IsFalse(verification); //firstteamwon does not called yet
                gameLogic.MakeMove(1, 0);
            }
            gameLogic.MakeMove(2, 0);
            Assert.IsFalse(verification);
        }

        [TestMethod]
        public void TestIsFull_ReturnFalse()
        {
            gameLogic.Reset();
            Assert.IsFalse(gameLogic.IsFull());
        }

        [TestMethod]
        public void TestIsFull_ReturnTrue()
        {
            gameLogic.Reset();
            for (int j = 0; j < 7; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    Assert.IsFalse(gameLogic.IsFull());
                    gameLogic.MakeMove(j % 2 + 1, j);
                }
                for (int i = 0; i < 3; i++)
                {
                    Assert.IsFalse(gameLogic.IsFull());
                    gameLogic.MakeMove((j + 1) % 2 + 1, j);
                }
            }
            Assert.IsTrue(gameLogic.IsFull());
        }

        [TestMethod]
        public void TestOnDraw_ReturnTrue()
        {
            gameLogic.Reset();
            gameLogic.AddonDrawObserver(Draw);
            verification = false;
            for (int j = 0; j < 7; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    Assert.IsFalse(verification);
                    gameLogic.MakeMove(j % 2 + 1, j);
                }
                for (int i = 0; i < 3; i++)
                {
                    Assert.IsFalse(verification);
                    gameLogic.MakeMove((j + 1) % 2 + 1, j);
                }
            }
            Assert.IsTrue(verification);
        }

        private void FirstTeamWon(int arg)
        {
            if (arg == 1)
            {
                verification = true;
            }
        }

        private void Draw()
        {
            verification = true;
        }
    }
}
