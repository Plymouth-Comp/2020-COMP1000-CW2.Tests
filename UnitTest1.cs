using Xunit;

namespace Crawler.Tests
{
    public class Tests
    {
        CMDCrawler crawler;

        private void Setup()
        {
            crawler = new CMDCrawler();
        }

        [Fact]
        public void TestInit()
        {
            Setup();

            Assert.False(crawler.GameIsRunning(),"The game should not be running as we have not loaded the map yet");
            Assert.True(crawler.GetPlayerAction() == 0, "No action should have been triggered yet");

            int[] pos = crawler.GetPlayerPosition();
            Assert.True( pos[0]== 0 && pos[1] == 0, "The player should still be on [0,0]");
            char[][] map = crawler.GetOriginalMap();
            Assert.True(map.Length == 0, "The map should still be empty ");
        }

        [Fact]
        public void TestInput()
        {
            Setup();

            crawler.ProcessUserInput("lod Simple.Map");
            crawler.ProcessUserInput("lod Simple.Mp");
            crawler.ProcessUserInput("play Simple.Map");
            crawler.ProcessUserInput("load play");
            Assert.False(crawler.GameIsRunning(), "The game should not be running as we have not loaded the map correctly");

        }

        [Fact]
        public void TestMapLoading()
        {
            Setup();
            bool result = crawler.InitializeMap("Simple.map");
            int yDim = crawler.GetOriginalMap().Length;
            Assert.True(result && yDim == 10, "Map loading is not working: The y dimension for the simple map shoudl be 10 but is " + yDim);
            int xDim = crawler.GetOriginalMap()[0].Length;
            Assert.True(result && xDim == 31, "Map loading is not working: The x dimension for the simple map shoudl be 31 but is "+xDim);
        }

        [Fact]
        public void TestGameInit()
        {
            Setup();
            crawler.ProcessUserInput("load Simple.map");
            char[][] orig = crawler.GetOriginalMap();
            Assert.True(crawler.GetOriginalMap().Length == 10, "Map loading is not working unsing the load command ");
            Assert.True(crawler.GetOriginalMap()[0].Length == 31, "Map loading is not working unsing the load command ");
            Assert.False(crawler.GameIsRunning(), "The game should not be running as we have a map and the user command play was used.");
            char[][] curr = crawler.GetCurrentMapState();
            for (int y = 0; y < orig.Length; y++)
                for (int x = 0; x < orig[0].Length; x++)
                    Assert.True(orig[y][x] == curr[y][x], $"The current map is not correctly showing tile [{y},{x}] which is {curr[y][x]} but should be {orig[y][x]}.");
        }

        [Fact]
        public void TestGameStart()
        {
            Setup();
            crawler.ProcessUserInput("load Simple.map");
            char[][] orig = crawler.GetOriginalMap();
            Assert.True(crawler.GetOriginalMap().Length == 10, "Map loading is not working unsing the load command ");
            Assert.True(crawler.GetOriginalMap()[0].Length == 31, "Map loading is not working unsing the load command ");
            Assert.False(crawler.GameIsRunning(), "The game should not be running as we have a map and the user command play was not used.");
            crawler.ProcessUserInput("play");
            Assert.True(crawler.GameIsRunning(), "The game should now be running as we have a map and the user command play was used.");
        }

        [Fact]
        public void TestUpdatedPlayerPosition()
        {
            Setup();
            crawler.InitializeMap("Simple.map");
            int[] pos = crawler.GetPlayerPosition();
            Assert.True(pos[0] == 1 && pos[1] == 8, "Player position is not set correctly!");
            char player = crawler.GetCurrentMapState()[pos[1]][pos[0]];
            Assert.True(player == '@' || player == 'S' , "Player position is not set correctly!");
        }

        [Fact]
        public void TestPlayerActions()
        {
            Setup();
            crawler.ProcessUserInput("load Simple.map");
            Assert.True(crawler.GetPlayerAction() == (int)CMDCrawler.PlayerActions.NOTHING, "No player action should be received yet.");
            crawler.ProcessUserInput("W");
            Assert.True(crawler.GetPlayerAction() == (int)CMDCrawler.PlayerActions.NOTHING, "Even though player used a movement command the game is not active. " +
                "thus, no playeraction should be triggered.");
            crawler.ProcessUserInput("play");
            Assert.True(crawler.GetPlayerAction() == (int)CMDCrawler.PlayerActions.NOTHING, "No player action should be received yet.");
            Assert.True(crawler.GameIsRunning(), "The game should be running as we have a map and the user command play was used.");
            crawler.ProcessUserInput("D");
            Assert.True(crawler.GetPlayerAction() == (int)CMDCrawler.PlayerActions.EAST, "Game is Active and player triggered moving using 'D' but not the correct action was triggered");
            crawler.ProcessUserInput("W");
            Assert.True(crawler.GetPlayerAction() == (int)CMDCrawler.PlayerActions.NORTH, "Game is Active and player triggered moving using 'W' but not the correct action was triggered");
            crawler.ProcessUserInput("A");
            Assert.True(crawler.GetPlayerAction() == (int)CMDCrawler.PlayerActions.WEST, "Game is Active and player triggered moving using 'A' but not the correct action was triggered");
            crawler.ProcessUserInput("S");
            Assert.True(crawler.GetPlayerAction() == (int)CMDCrawler.PlayerActions.SOUTH, "Game is Active and player triggered moving using 'S' but not the correct action was triggered");
            crawler.ProcessUserInput("play");
            Assert.True(crawler.GetPlayerAction() == (int)CMDCrawler.PlayerActions.NOTHING, "Game is Active and player typed in play again which should do nothing.");
        }

        [Fact]
        public void TestGameWithWrongOrder()
        {
            Setup();
            crawler.ProcessUserInput("load Simple.map");
            char[][] orig = crawler.GetOriginalMap();
            Assert.True(crawler.GetOriginalMap().Length == 10, "Map loading is not working unsing the load command ");
            Assert.True(crawler.GetOriginalMap()[0].Length == 31, "Map loading is not working unsing the load command ");
            //crawler.ProcessUserInput("play");
            int[] pos = crawler.GetPlayerPosition();
            Assert.False(crawler.GameIsRunning(), "The game should not be running as we have a map and the user command play was not used.");
            crawler.ProcessUserInput("W");
            int[] pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0] == pos2[0] && pos[1] == pos2[1], $"The player is moving in the right direction, but the game was not started yet. The player should be at [{pos[0]},{pos[1]}] but is at [{pos2[0]},{pos2[1]}]");
            char[][] curr = crawler.GetCurrentMapState();
            char player = crawler.GetCurrentMapState()[pos[1]][pos[0]];
            Assert.True(player == '@' || player == 'S', "Player position is not set correctly!");
        }

        [Fact]
        public void TestGamePlayMoving()
        {
            Setup();
            crawler.ProcessUserInput("load Simple.map");
            char[][] orig = crawler.GetOriginalMap();
            Assert.True(crawler.GetOriginalMap().Length == 10, "Map loading is not working unsing the load command ");
            Assert.True(crawler.GetOriginalMap()[0].Length == 31, "Map loading is not working unsing the load command ");
            crawler.ProcessUserInput("play");

            //first move
            int[] pos = (int[])crawler.GetPlayerPosition().Clone();
            Assert.True(crawler.GameIsRunning(), "The game should now be running as we have a map and the user command play was used.");
            crawler.ProcessUserInput("W");
            crawler.GameLoop(true);
            int[] pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0] == pos2[0] && pos[1] == pos2[1] + 1, $"The player is not moving in the right direction. The player should be at [{pos[0]},{pos[1] - 1}] but is at [{pos2[0]},{pos2[1]}]");
            char[][] curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1]][pos[0]]== '.', $"The current map is not correctly showing an empty tile under the previous player pos but shows {curr[pos[1]][pos[0]]}.");
            Assert.True(curr[pos2[1]][pos2[0]] == '@', $"The current map is not correctly showing an empty tile under the previous player pos but shows {curr[pos2[1]][pos2[0]]}.");
            
            //second move
            pos = (int[])crawler.GetPlayerPosition().Clone();
            Assert.True(crawler.GameIsRunning(), "The game should now be running as we have a map and the user command play was used.");
            crawler.ProcessUserInput("W");
            crawler.GameLoop(true);
            pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0] == pos2[0] && pos[1] == pos2[1] + 1, $"The player is not moving in the right direction. The player should be at [{pos[0]},{pos[1] - 1}] but is at [{pos2[0]},{pos2[1]}]");
            curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1]][pos[0]] == '.', $"The current map is not correctly showing an empty tile under the previous player pos but shows {curr[pos[1]][pos[0]]}.");
            Assert.True(curr[pos[1]+1][pos[0]] == '.', $"The current map is not correctly showing an empty tile under the position 2 moves ago but shows {curr[pos[1]+1][pos[0]]}.");
            Assert.True(curr[pos2[1]][pos2[0]] == '@', $"The current map is not correctly showing an empty tile under the previous player pos but shows {curr[pos2[1]][pos2[0]]}.");

            //third move
            pos = (int[])crawler.GetPlayerPosition().Clone();
            crawler.ProcessUserInput("D");
            crawler.GameLoop(true);
            pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0]+1 == pos2[0] && pos[1] == pos2[1], $"The player is not moving in the right direction. The player should be at [{pos[0]+1},{pos[1]}] but is at [{pos2[0]},{pos2[1]}]");
            curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1]][pos[0]] == '.', $"The current map is not correctly showing an empty tile under the previous player pos but shows {curr[pos[1]][pos[0]]}.");
            Assert.True(curr[pos2[1]][pos2[0]] == '@', $"The current map is not correctly showing an empty tile under the previous player pos but shows {curr[pos2[1]][pos2[0]]}.");

            //forth move
            pos = (int[])crawler.GetPlayerPosition().Clone();
            crawler.ProcessUserInput("A");
            crawler.GameLoop(true);
            pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0] - 1 == pos2[0] && pos[1] == pos2[1], $"The player is not moving in the right direction. The player should be at [{pos[0] - 1},{pos[1]}] but is at [{pos2[0]},{pos2[1]}]");
            curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1]][pos[0]] == '.', $"The current map is not correctly showing an empty tile under the previous player pos but shows {curr[pos[1]][pos[0]]}.");
            Assert.True(curr[pos2[1]][pos2[0]] == '@', $"The current map is not correctly showing an empty tile under the previous player pos but shows {curr[pos2[1]][pos2[0]]}.");
        }

        [Fact]
        public void TestGamePlayRespectingWalls()
        {
            Setup();
            crawler.ProcessUserInput("load Simple.map");
            crawler.ProcessUserInput("play");

            //first move
            int[] pos = (int[])crawler.GetPlayerPosition().Clone();
            crawler.ProcessUserInput("A");
            crawler.GameLoop(true);
            int[] pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0] == pos2[0] && pos[1] == pos2[1], $"The player is moving in the right direction but should not be able to move onto a wall. " +
                $"The player should be at [{pos[0]},{pos[1]}] but is at [{pos2[0]},{pos2[1]}]");
            char[][] curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1]][pos[0]] == '@', $"The current map is not correctly showing the player standing still but shows {curr[pos[1]][pos[0]]}.");
            Assert.True(curr[pos[1]][pos[0]-1] == '#', $"The current map is not correctly showing the player standing still in front of the wall but shows {curr[pos[1]][pos[0]-1]} in the wall.");


            //second move
            pos = (int[])crawler.GetPlayerPosition().Clone();
            crawler.ProcessUserInput("S");
            crawler.GameLoop(true);
            pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0] == pos2[0] && pos[1] == pos2[1], $"The player is moving in the right direction but should not be able to move onto a wall. " +
                $"The player should be at [{pos[0]},{pos[1]}] but is at [{pos2[0]},{pos2[1]}]");
            curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1]][pos[0]] == '@', $"The current map is not correctly showing the player standing still but shows {curr[pos[1]][pos[0]]}.");
            Assert.True(curr[pos[1]+1][pos[0]] == '#', $"The current map is not correctly showing the player standing still in front of the wall but shows {curr[pos[1] + 1][pos[0]]} in the wall.");

            //series of moves
            pos = (int[])crawler.GetPlayerPosition().Clone();
            for (int x = 1; x < 17; x++)
            {
                crawler.ProcessUserInput("D");
                crawler.GameLoop(true);
            }
            pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0]+16 == pos2[0] && pos[1] == pos2[1], $"The player is moving to far, it should not be able to move onto a wall. " +
                $"The player should be at [{pos[0]+16},{pos[1]}] but is at [{pos2[0]},{pos2[1]}]");
            curr = crawler.GetCurrentMapState();
            //advanced Assert.True(curr[pos[1]+16][pos[0]] == '@', $"The current map is not correctly showing the player standing still but shows {curr[pos[1]][pos[0]]}.");
            Assert.True(curr[pos2[1] + 1][pos[0]] == '#', $"The current map is not correctly showing the player standing infront of a wall.");
        }

        [Fact]
        public void TestGamePlayFinish()
        {
            Setup();
            crawler.ProcessUserInput("load Simple.map");
            crawler.ProcessUserInput("play");
            char[][] orig = crawler.GetOriginalMap();
            //first move
            int[] pos = (int[])crawler.GetPlayerPosition().Clone();
            Assert.True(crawler.GameIsRunning(), "The game should now be running as we have a map and the user command play was used.");
            crawler.ProcessUserInput("W");
            crawler.GameLoop(true);
            int[] pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0] == pos2[0] && pos[1] == pos2[1] + 1, $"The player is not moving in the right direction. The player should be at [{pos[0]},{pos[1] - 1}] but is at [{pos2[0]},{pos2[1]}]");
            char[][] curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1]][pos[0]] == '.', $"The current map is not correctly showing an empty tile under the previous player pos but shows {curr[pos[1]][pos[0]]}.");
            Assert.True(curr[pos2[1]][pos2[0]] == '@', $"The current map is not correctly showing an empty tile under the previous player pos but shows {curr[pos2[1]][pos2[0]]}.");

            //moving North
            pos = (int[])crawler.GetPlayerPosition().Clone();
            for (int y = 0; y < 4; y++)
            {
                crawler.ProcessUserInput("W");
                crawler.GameLoop(true);
            }
            pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0] == pos2[0] && pos[1] == pos2[1]+4, $"The player is moving wrong. " +
                $"The player should be at [{pos[0]},{pos[1]-4}] but is at [{pos2[0]},{pos2[1]}]");
            curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1] - 4][pos[0]] == '@', $"The current map is not correctly showing the player but shows {curr[pos[1] - 4][pos[0]]}.");
        
            //moving East
            pos = (int[])crawler.GetPlayerPosition().Clone();
            for (int x = 0; x < 21; x++)
            {
                crawler.ProcessUserInput("D");
                crawler.GameLoop(true);
            }
            pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0]+21 == pos2[0] && pos[1] == pos2[1], $"The player is moving wrong. " +
                $"The player should be at [{pos[0]+21},{pos[1]}] but is at [{pos2[0]},{pos2[1]}]");
            curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1]][pos[0]+21] == '@', $"The current map is not correctly showing the player but shows {curr[pos[1]][pos[0]+21]}.");

            //moving South
            pos = (int[])crawler.GetPlayerPosition().Clone();
            for (int y = 0; y < 3; y++)
            {
                crawler.ProcessUserInput("S");
                crawler.GameLoop(true);
            }
            pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0] == pos2[0] && pos[1]+3 == pos2[1], $"The player is moving wrong. " +
                $"The player should be at [{pos[0]},{pos[1]+3}] but is at [{pos2[0]},{pos2[1]}]");
            curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1] + 3][pos[0]] == '@', $"The current map is not correctly showing the player but shows {curr[pos[1] + 3][pos[0]]}.");

            //moving West
            pos = (int[])crawler.GetPlayerPosition().Clone();
            for (int x = 0; x < 5; x++)
            {
                crawler.ProcessUserInput("A");
                crawler.GameLoop(true);
            }
            pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0]-5 == pos2[0] && pos[1] == pos2[1], $"The player is moving wrong. " +
                $"The player should be at [{pos[0] -5},{pos[1]}] but is at [{pos2[0]},{pos2[1]}]");
            curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1]][pos[0] - 5] == '@', $"The current map is not correctly showing the player but shows {curr[pos[1]][pos[0] - 5]}.");

            //moving North
            pos = (int[])crawler.GetPlayerPosition().Clone();
            crawler.ProcessUserInput("W");
            crawler.GameLoop(true);
            pos2 = crawler.GetPlayerPosition();
            Assert.True(pos[0] == pos2[0] && pos[1]-1 == pos2[1], $"The player is moving wrong. " +
                $"The player should be at [{pos[0]},{pos[1] - 1}] but is at [{pos2[0]},{pos2[1]}]");
            curr = crawler.GetCurrentMapState();
            Assert.True(curr[pos[1]-1][pos[0]] == 'E' || curr[pos[1] - 1][pos[0]] == '@', $"The current map is not correctly showing the player but shows {curr[pos[1]-1][pos[0]]}.");
            Assert.True(orig[pos2[1]][pos2[0]] == 'E' , $"The original map is not correctly showing the unchanged map with the Exit but shows {curr[pos[1] - 1][pos[0]]}.");
            //reaching the Exit
            Assert.False(crawler.GameIsRunning(), "The game should finished as the player reached the Exit.");

        }





        // More tests to come during the term
    }
}