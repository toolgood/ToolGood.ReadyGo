using System;
using System.Collections.Generic;
using System.Text;
using PetaTest;
using ToolGood.ReadyGo3.CoreTest.Datas;

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

            tb.Single();
            tb.SingleOrDefault();


            tb.Clear();
            tb.Where(tb.Level == 2);
            tb.First();
            tb.FirstOrDefault();


            tb.Select();
            tb.Select(10);
            tb.Select(2, 10);
            tb.Page(2, 10);
        }

        [Test]
        public async void SelectAsyncTest()
        {
            TbArea tb = new TbArea();
            tb.Where(tb.Id == 1);

            await tb.SingleAsync();
            await tb.SingleOrDefaultAsync();


            tb.Clear();
            tb.Where(tb.Level == 2);
            await tb.FirstAsync();
            await tb.FirstOrDefaultAsync();


            await tb.SelectAsync();
            await tb.SelectAsync(10);
            await tb.SelectAsync(2, 10);
            await tb.PageAsync(2, 10);
        }

    }
}
