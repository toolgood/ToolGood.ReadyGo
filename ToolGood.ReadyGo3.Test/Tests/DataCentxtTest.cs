using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PetaTest;
using ToolGood.ReadyGo3.Test.Datas;
using ToolGood.ReadyGo3.DataCentxt;
using ToolGood.ReadyGo3.Test;

namespace ToolGood.ReadyGo3.CoreTest.Tests
{
    [TestFixture]
    public class DataCentxtTest
    {
        [Test]
        public void SelectTest()
        {
            TbArea tb = new TbArea();
            tb.Where(tb.Id == 1);
            DoSelect(tb, true, true, true);
            DoSelect<DbArea, Area>(tb, true, true, true);


            tb.Clear();
            tb.Where(tb.Level == 2);
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);

            #region 类型 String 的操作
            tb.Clear();
            tb.Where(tb.NamePinyin.Lower() == "yazhou");
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);

            tb.Clear();
            tb.Where(tb.NamePinyin.Upper() == "yazhou".ToUpper());
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);

            tb.Clear();
            tb.Where(tb.NamePinyin.Contains("yazhou"));
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);

            tb.Clear();
            tb.Where(tb.NamePinyin.StartsWith("ya"));
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);

            tb.Clear();
            tb.Where(tb.NamePinyin.EndsWith("zhou"));
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);


            tb.Clear();
            tb.Where(tb.NamePinyin.SubString(0, 2) == "ya");
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);

            tb.Clear();
            tb.Where(tb.NamePinyin.Trim() == "yazhou");
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);


            tb.Clear();
            tb.Where(tb.NamePinyin.LTrim() == "yazhou");
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);

            tb.Clear();
            tb.Where(tb.NamePinyin.RTrim() == "yazhou");
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);


            #endregion

            tb.Clear();
            tb.Where(tb.AddingTime.Year() < 2019);
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);

            tb.Clear();
            tb.Where(tb.AddingTime.Month() < 2019);
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);

            tb.Clear();
            tb.Where(tb.AddingTime.Day() < 2019);
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);


            tb.Clear();
            tb.Where(tb.AddingTime.Hour() < 2019);
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);

            tb.Clear();
            tb.Where(tb.AddingTime.Minute() < 2019);
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);


            tb.Clear();
            tb.Where(tb.AddingTime.Second() < 2019);
            DoSelect(tb, false, false, true);
            DoSelect<DbArea, Area>(tb, false, false, true);




        }

        [Test]
        public void Update()
        {
            var helper = Config.DbHelper;

            TbArea tb = new TbArea(helper);
            tb.Name = null;
            tb.Where(tb.Id == 1563).Update();

            var db = helper.SingleById<DbArea>(1563);

            Assert.AreEqual(null, db.Name);

            List<string> list = new List<string>() { "aadd" };
            TbArea tb1 = new TbArea(helper);
            tb.Name = string.Join("、", list);
            tb.Where(tb.Id == 1563).Update();
            db = helper.SingleById<DbArea>(1563);

            Assert.AreEqual("aadd", db.Name);
        }

 


        #region DoSelect
        private void DoSelect<T>(QTable<T> tb, bool single = false, bool singleOrDefault = false, bool first = false) where T : class
        {
            if (single) {
                tb.Single();
            }
            if (singleOrDefault) {
                tb.SingleOrDefault();
            }
            if (first) {
                tb.First();
            }
            tb.FirstOrDefault();
            tb.Select();
            tb.Select(10);
            tb.Select(2, 10);
            tb.Page(2, 10);
            //Microsoft.Data.Sqlite Bug
            //https://github.com/aspnet/Microsoft.Data.Sqlite/issues/435
            //tb.ExecuteDataTable();


        }
        private void DoSelect<T, OutT>(QTable<T> tb, bool single = false, bool singleOrDefault = false, bool first = false)
            where T : class
            where OutT : class
        {
            if (single) {
                tb.Single<OutT>();
            }
            if (singleOrDefault) {
                tb.SingleOrDefault<OutT>();
            }
            if (first) {
                tb.First<OutT>();
            }
            tb.FirstOrDefault<OutT>();
            tb.Select<OutT>();
            tb.Select<OutT>(10);
            tb.Select<OutT>(2, 10);
            tb.Page<OutT>(2, 10);
        }
 
        #endregion
    }
    public class Area
    {
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string NamePinyin { get; set; }
        public string Code { get; set; }
    }

}
