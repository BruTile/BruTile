using Tiling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace BruTileTests
{
    
    
    /// <summary>
    ///This is a test class for TileKeyTest and is intended
    ///to contain all TileKeyTest Unit Tests
    ///</summary>
  [TestClass()]
  public class TileKeyTest
  {


    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    /// <summary>
    ///A test for CompareTo
    ///</summary>
    [TestMethod()]
    public void CompareToTest()
    {
      TileKey target = new TileKey(2, 4, 2);
      TileKey key = new TileKey(2, 5, 2); 
      int expected = -1; 
      int actual;
      actual = target.CompareTo(key);
      Assert.AreEqual(expected, actual);
    }
  }
}
