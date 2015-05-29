using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using CR.Logic.ViewModels;

namespace CR.Logic.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Main_page_Has_Title()
        {
            var vm = new MainPageViewModel();
            Assert.IsNotNull(vm.Title);
        }
    }
}
