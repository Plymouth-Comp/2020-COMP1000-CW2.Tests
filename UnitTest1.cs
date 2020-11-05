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
            int xDim = crawler.GetOriginalMap().Length;
            Assert.True(result && xDim == 31, "Map loading is not working: The x dimension for the simple map shoudl be 31 but is "+xDim);
        }

        [Fact]
        public void TestMapThroughInput()
        {
            Setup();
            crawler.ProcessUserInput("load Simple.map");

            Assert.True(crawler.GetOriginalMap().Length == 31, "Map loading is not working unsing the load command ");
        }

        [Fact]
        public void TestUpdatedPlayerPosition()
        {
            Setup();
            crawler.InitializeMap("Simple.map");
            int [] pos = crawler.GetPlayerPosition();
            Assert.True(pos[0] == 1 && pos[1] == 8, "Player position is not set correctly!");
        }

        // More tests to come during the term
    }
}