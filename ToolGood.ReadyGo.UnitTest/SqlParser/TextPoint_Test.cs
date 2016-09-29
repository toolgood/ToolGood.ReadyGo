using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using ToolGood.ReadyGo.SqlParser;

namespace ToolGood.ReadyGo.Test.SqlParser
{
    [TestFixture]
    public class TextPoint_Test
    {
        [Test]
        public void Test1()
        {
            var list = TextPoint.Analysis("select * from [ui];");
            var index = 0;
            Assert.AreEqual("select", list[index++].Text);
            Assert.AreEqual("*", list[index++].Text);
            Assert.AreEqual("from", list[index++].Text);
            Assert.AreEqual("[ui]", list[index++].Text);
            Assert.AreEqual(";", list[index++].Text);
        }
        [Test]
        public void Test2()
        {
            var list = TextPoint.Analysis("select * from ui where  (ui.cc=12.01 or ui.dd>2);");
            var index = 0;
            Assert.AreEqual("select", list[index++].Text);
            Assert.AreEqual("*", list[index++].Text);
            Assert.AreEqual("from", list[index++].Text);
            Assert.AreEqual("ui", list[index++].Text);
            Assert.AreEqual("where", list[index++].Text);
            Assert.AreEqual("(ui.cc=12.01 or ui.dd>2)", list[index].Text);
            int t = 0;
            Assert.AreEqual("ui.cc", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("=", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("12.01", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("or", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("ui.dd", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual(">", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("2", list[index].SubTextPoint[t++].Text);

            index++;
            Assert.AreEqual(";", list[index++].Text);

        }
        [Test]
        public void Test3()
        {
            var list = TextPoint.Analysis("select * from ui where /*132456*/ (ui.cc=12.01 or ui.dd>2);");
            var index = 0;
            Assert.AreEqual("select", list[index++].Text);
            Assert.AreEqual("*", list[index++].Text);
            Assert.AreEqual("from", list[index++].Text);
            Assert.AreEqual("ui", list[index++].Text);
            Assert.AreEqual("where", list[index++].Text);
            Assert.AreEqual("/*132456*/", list[index++].Text);
            Assert.AreEqual("(ui.cc=12.01 or ui.dd>2)", list[index].Text);
            int t = 0;
            Assert.AreEqual("ui.cc", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("=", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("12.01", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("or", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("ui.dd", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual(">", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("2", list[index].SubTextPoint[t++].Text);

            index++;
            Assert.AreEqual(";", list[index++].Text);

        }
        [Test]
        public void Test5()
        {
            var list = TextPoint.Analysis("select * from --111122 \r\n ui;");
            var index = 0;
            Assert.AreEqual("select", list[index++].Text);
            Assert.AreEqual("*", list[index++].Text);
            Assert.AreEqual("from", list[index++].Text);
            Assert.AreEqual("--111122 \r\n", list[index++].Text);
            Assert.AreEqual("ui", list[index++].Text);
            Assert.AreEqual(";", list[index++].Text);
        }
        [Test]
        public void Test6()
        {
            var list = TextPoint.Analysis("select * from ui where uii='aaa' and dd=N'123465'   # ;");
            var index = 0;
            Assert.AreEqual("select", list[index++].Text);
            Assert.AreEqual("*", list[index++].Text);
            Assert.AreEqual("from", list[index++].Text);
            Assert.AreEqual("ui", list[index++].Text);
            Assert.AreEqual("where", list[index++].Text);
            Assert.AreEqual("uii", list[index++].Text);
            Assert.AreEqual("=", list[index++].Text);
            Assert.AreEqual("'aaa'", list[index++].Text);
            Assert.AreEqual("and", list[index++].Text);
            Assert.AreEqual("dd", list[index++].Text);
            Assert.AreEqual("=", list[index++].Text);
            Assert.AreEqual("N'123465'", list[index++].Text);
            Assert.AreEqual("# ;", list[index++].Text);
        }

        [Test]
        public void Test7()
        {
            var list = TextPoint.Analysis("select * from ui where uii=-123.03E2 and dd=123465E-2   # ;");
            var index = 0;
            Assert.AreEqual("select", list[index++].Text);
            Assert.AreEqual("*", list[index++].Text);
            Assert.AreEqual("from", list[index++].Text);
            Assert.AreEqual("ui", list[index++].Text);
            Assert.AreEqual("where", list[index++].Text);
            Assert.AreEqual("uii", list[index++].Text);
            Assert.AreEqual("=", list[index++].Text);
            Assert.AreEqual("-123.03E2", list[index++].Text);
            Assert.AreEqual("and", list[index++].Text);
            Assert.AreEqual("dd", list[index++].Text);
            Assert.AreEqual("=", list[index++].Text);
            Assert.AreEqual("123465E-2", list[index++].Text);
            Assert.AreEqual("# ;", list[index++].Text);
        }

        [Test]
        public void Test4()
        {
            try {
                var list = TextPoint.Analysis("select * from [ui]);");

            } catch (Exception) {
                return;
            }
            Assert.Throw(true, () => "");
        }

        [Test]
        public void Test9()
        {
            try {
                var list = TextPoint.Analysis("select * from [ui] where a=123K;");

            } catch (Exception) {
                return;
            }
            Assert.Throw(true, () => "");
        }
        [Test]
        public void Test10()
        {
            try {
                var list = TextPoint.Analysis("select * from [ui] where a=0x123K;");

            } catch (Exception) {
                return;
            }
            Assert.Throw(true, () => "");
        }


        [Test]
        public void Test11()
        {
            var list = TextPoint.Analysis("select a,b from ui where  sum(ui.cc=12.01 or ui.dd>2)=1;");
            var index = 0;
            Assert.AreEqual("select", list[index++].Text);
            Assert.AreEqual("a", list[index++].Text);
            Assert.AreEqual(",", list[index++].Text);
            Assert.AreEqual("b", list[index++].Text);
            Assert.AreEqual("from", list[index++].Text);
            Assert.AreEqual("ui", list[index++].Text);
            Assert.AreEqual("where", list[index++].Text);
            Assert.AreEqual("sum(ui.cc=12.01 or ui.dd>2)", list[index].Text);
            int t = 0;
            Assert.AreEqual("ui.cc", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("=", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("12.01", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("or", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("ui.dd", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual(">", list[index].SubTextPoint[t++].Text);
            Assert.AreEqual("2", list[index].SubTextPoint[t++].Text);

            index++;
            Assert.AreEqual("=", list[index++].Text);
            Assert.AreEqual("1", list[index++].Text);


            Assert.AreEqual(";", list[index++].Text);

        }


        [Test]
        public void Test12()
        {
            var list = TextPoint.Analysis(@"CREATE PROCEDURE wifi86.`UserPay_GetSalesRecord`(_UserID int)
BEGIN
    SELECT o.Title as Title, count(*) as amount, sum(o.OriginalPrice - o.PayMoney) as ProfitMoney

    from wifi86.OutletPay as o
    where o.OutletID = _UserID and o.State = 1
        group by o.Title;
            END;");
            var index = 0;
            Assert.AreEqual("CREATE", list[index++].Text);
            Assert.AreEqual("PROCEDURE", list[index++].Text);
            Assert.AreEqual("wifi86", list[index++].Text);
            Assert.AreEqual(".", list[index++].Text);
            Assert.AreEqual("`UserPay_GetSalesRecord`", list[index++].Text);


            Assert.AreEqual("(_UserID int)", list[index++].Text);
            Assert.AreEqual("BEGIN", list[index++].Text);
            Assert.AreEqual("SELECT", list[index++].Text);
        }
    }
}
