using Tiling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace BruTileTests
{
    
    
    /// <summary>
    ///This is a test class for ITileCacheTest and is intended
    ///to contain all ITileCacheTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ITileCacheTest
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
    ///A test for Remove
    ///</summary>
    public void RemoveTestHelper<T>()
    {
      ITileCache<T> target = CreateITileCache<T>(); // TODO: Initialize to an appropriate value
      TileKey key = new TileKey(); // TODO: Initialize to an appropriate value
      target.Remove(key);
      Assert.Inconclusive("A method that does not return a value cannot be verified.");
    }

    [TestMethod()]
    public void RemoveTest()
    {
      RemoveTestHelper<GenericParameterHelper>();
    }

    /// <summary>
    ///A test for Find
    ///</summary>
    public void FindTestHelper<T>()
    {
      ITileCache<T> target = CreateITileCache<T>(); // TODO: Initialize to an appropriate value
      TileKey key = new TileKey(); // TODO: Initialize to an appropriate value
      T expected = default(T); // TODO: Initialize to an appropriate value
      T actual;
      actual = target.Find(key);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    [TestMethod()]
    public void FindTest()
    {
      FindTestHelper<GenericParameterHelper>();
    }

    /// <summary>
    ///A test for Add
    ///</summary>
    public void AddTestHelper<T>()
    {
      ITileCache<T> target = CreateITileCache<T>(); // TODO: Initialize to an appropriate value
      TileKey key = new TileKey(); // TODO: Initialize to an appropriate value
      T image = default(T); // TODO: Initialize to an appropriate value
      target.Add(key, image);
      Assert.Inconclusive("A method that does not return a value cannot be verified.");
    }

    internal virtual ITileCache<T> CreateITileCache<T>()
    {
      // TODO: Instantiate an appropriate concrete class.
      ITileCache<T> target = null;
      return target;
    }

    [TestMethod()]
    public void AddTest()
    {
      AddTestHelper<GenericParameterHelper>();
    }
  }
}
