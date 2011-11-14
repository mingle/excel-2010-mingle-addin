using ThoughtWorksCoreLib;
using ThoughtWorksMingleLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for MingleServerTest and is intended
    ///to contain all MingleServerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MingleServerTest
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
        ///A test for GetProjects
        ///</summary>
        [TestMethod()]
        public void GetProjectsTest()
        {
            SettingsHelper.MingleHost = "http://localhost:8443";
            SettingsHelper.MingleUser = "test";
            SettingsHelper.MinglePassword = "P2ssw0rd";
            int expected = 4; // TODO: Initialize to an appropriate value
            SortedList actual;
            actual = MingleServer.GetProjects();
            Assert.AreEqual(expected, actual.Count);
        }
    }
}
