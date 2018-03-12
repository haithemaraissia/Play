using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySnapps.Data;
using MySnapps.MVVM.Model;
using MySnapps.MVVM.ViewModel.Concrete;

namespace MySnappsTest
{
    [TestClass]
    public class UrlViewModelTest
    {
        private UrlViewModel _sViewModel;

        [TestInitialize]
        public void Initialize()
        {
            _sViewModel = new UrlViewModel();
        }

        [TestMethod]
        public void InitUrlViewModel_ShouldReturn_AtLeastOneUrl()
        {
            int count = _sViewModel.Urls.Count;
            Assert.IsTrue(count > 0);
        }

        [TestMethod]
        public void AddUrl_ShouldIncreaseUrlsCount_ByOne()
        {
            var originalCount = _sViewModel.Urls.Count;
            _sViewModel.AddRow();
            Assert.IsTrue(originalCount + 1 == _sViewModel.Urls.Count);
        }

        [TestMethod]
        public void RemoveUrl_ShouldDecreaseUrlsCount_ByOne()
        {
            _sViewModel.AddRow();
            var originalCount = _sViewModel.Urls.Count;
            _sViewModel.UrlSelectedItem = new Url { Link = ""};
            _sViewModel.RemoveUrl();
            Assert.IsTrue(originalCount - 1 == _sViewModel.Urls.Count);
        }

        [TestMethod]
        public void SaveUrls_ShouldOnlySave_ValidUrls()
        {
            var originalCount = _sViewModel.Urls.Count;

            _sViewModel.AddRow();
            _sViewModel.Urls[_sViewModel.Urls.Count - 1].Link = "";
            _sViewModel.AddRow();
            _sViewModel.Urls[_sViewModel.Urls.Count - 1].Link = "http://www.google.com";
            _sViewModel.AddRow();
            _sViewModel.Urls[_sViewModel.Urls.Count - 1].Link = null;

            _sViewModel.SaveData();
            var results = new MyData().GetRows();
            Assert.IsTrue(originalCount + 1 == results.Count());
        }
    }
}
