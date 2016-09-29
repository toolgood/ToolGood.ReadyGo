using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo;
using ToolGood.ReadyGo.WhereHelpers;
using PetaTest;
using ToolGood.ReadyGo.Attributes;

namespace ToolGood.ReadyGo.UnitTest.UpdateProxy
{
    [TestFixture]
    public class UpdateProxy_Test
    {
        #region Test Number
        #region Integer Number
        [Test]
        public void Test_Short()
        {
            var dict = DoAction(q => q.Short = 32);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((short)32, dict["Short"]);
        }
        [Test]
        public void Test_ShortNull()
        {
            var dict = DoAction(q => q.ShortNull = 32);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((short)32, dict["ShortNull"]);

            dict = DoAction(q => q.ShortNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["ShortNull"]);

        }
        [Test]
        public void Test_Int()
        {
            var dict = DoAction(q => q.Int = 9999);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(9999, dict["Int"]);
        }
        [Test]
        public void Test_IntNull()
        {
            var dict = DoAction(q => q.IntNull = 9999);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(9999, dict["IntNull"]);

            dict = DoAction(q => q.IntNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["IntNull"]);
        }
        [Test]
        public void Test_Long()
        {
            var dict = DoAction(q => q.Long = 9999);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(9999L, dict["Long"]);
        }
        [Test]
        public void Test_LongNull()
        {
            var dict = DoAction(q => q.LongNull = 9999);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(9999L, dict["LongNull"]);

            dict = DoAction(q => q.LongNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["LongNull"]);
        }




        [Test]
        public void Test_uShort()
        {
            var dict = DoAction(q => q.uShort = 32);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((ushort)32, dict["uShort"]);
        }
        [Test]
        public void Test_uShortNull()
        {
            var dict = DoAction(q => q.uShortNull = 32);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((ushort)32, dict["uShortNull"]);

            dict = DoAction(q => q.uShortNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["uShortNull"]);

        }
        [Test]
        public void Test_uInt()
        {
            var dict = DoAction(q => q.uInt = 9999);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((uint)09999, dict["uInt"]);
        }
        [Test]
        public void Test_uIntNull()
        {
            var dict = DoAction(q => q.uIntNull = 9999);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((uint)9999, dict["uIntNull"]);

            dict = DoAction(q => q.uIntNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["uIntNull"]);
        }
        [Test]
        public void Test_uLong()
        {
            var dict = DoAction(q => q.uLong = 9999);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((ulong)9999L, dict["uLong"]);
        }
        [Test]
        public void Test_uLongNull()
        {
            var dict = DoAction(q => q.uLongNull = 9999);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((ulong)9999, dict["uLongNull"]);

            dict = DoAction(q => q.uLongNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["uLongNull"]);
        }
        #endregion
        #region Float Number

        [Test]
        public void Test_Float()
        {
            var dict = DoAction(q => q.Float = 32.1f);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(32.1f, dict["Float"]);
        }
        [Test]
        public void Test_FloatNull()
        {
            var dict = DoAction(q => q.FloatNull = 32.02f);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(32.02f, dict["FloatNull"]);

            dict = DoAction(q => q.FloatNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["FloatNull"]);

        }



        [Test]
        public void Test_Double()
        {
            var dict = DoAction(q => q.Double = 32.1d);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(32.1d, dict["Double"]);
        }
        [Test]
        public void Test_DoubleNull()
        {
            var dict = DoAction(q => q.DoubleNull = 32.02d);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(32.02d, dict["DoubleNull"]);

            dict = DoAction(q => q.DoubleNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["DoubleNull"]);

        }

        [Test]
        public void Test_Decimal()
        {
            var dict = DoAction(q => q.Decimal = 32.1M);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(32.1M, dict["Decimal"]);
        }
        [Test]
        public void Test_DecimalNull()
        {
            var dict = DoAction(q => q.DecimalNull = 32.02M);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(32.02M, dict["DecimalNull"]);

            dict = DoAction(q => q.DecimalNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["DecimalNull"]);

        }

        #endregion

        #endregion

        #region Test Text
        [Test]
        public void Test_String()
        {
            var dict = DoAction(q => q.String = "123456");
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual("123456", dict["String"]);

            dict = DoAction(q => q.String = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["String"]);
        }
        [Test]
        public void Test_Char()
        {
            var dict = DoAction(q => q.Char = '2');
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual('2', dict["Char"]);

        }
        [Test]
        public void Test_CharNull()
        {
            var dict = DoAction(q => q.CharNull = '2');
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual('2', dict["CharNull"]);

            dict = DoAction(q => q.CharNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["CharNull"]);
        }
        [Test]
        public void Test_AnsiString()
        {
            var dict = DoAction(q => q.AnsiString =new AnsiString("123"));
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(new AnsiString("123"), dict["AnsiString"]);

            dict = DoAction(q => q.AnsiString = new AnsiString(null));
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(new AnsiString(null), dict["AnsiString"]);
        }

        #endregion

        #region Test DateTime
        [Test]
        public void Test_DateTime()
        {
            var dt = DateTime.Now;
            var dict = DoAction(q => q.DateTime = dt);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(dt, dict["DateTime"]);
        }
        [Test]
        public void Test_DateTimeNull()
        {
            var dt = DateTime.Now;
            var dict = DoAction(q => q.DateTimeNull = dt);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(dt, dict["DateTimeNull"]);

            dict = DoAction(q => q.DateTimeNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["DateTimeNull"]);
        }

        [Test]
        public void Test_TimeSpan()
        {
            var dt = DateTime.Now.TimeOfDay;
            var dict = DoAction(q => q.TimeSpan = dt);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(dt, dict["TimeSpan"]);
        }
        [Test]
        public void Test_TimeSpanNull()
        {
            var dt = DateTime.Now.TimeOfDay;
            var dict = DoAction(q => q.TimeSpanNull = dt);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(dt, dict["TimeSpanNull"]);

            dict = DoAction(q => q.TimeSpanNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["TimeSpanNull"]);
        }


        [Test]
        public void Test_DateTimeOffset()
        {
            var dt = DateTimeOffset.Now;
            var dict = DoAction(q => q.DateTimeOffset = dt);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(dt, dict["DateTimeOffset"]);
        }
        [Test]
        public void Test_DateTimeOffsetNull()
        {
            var dt = DateTimeOffset.Now;
            var dict = DoAction(q => q.DateTimeOffsetNull = dt);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(dt, dict["DateTimeOffsetNull"]);

            dict = DoAction(q => q.DateTimeOffsetNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["DateTimeOffsetNull"]);
        }

        #endregion

        #region Test Bool
        [Test]
        public void Test_Bool()
        {
            var dict = DoAction(q => q.Bool = true);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(true, dict["Bool"]);
        }
        [Test]
        public void Test_BoolNull()
        {
            var dt = DateTime.Now;
            var dict = DoAction(q => q.BoolNull = true);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(true, dict["BoolNull"]);

            dict = DoAction(q => q.BoolNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["BoolNull"]);
        }
        #endregion

        #region Test Byte
        [Test]
        public void Test_Byte()
        {
            var dict = DoAction(q => q.Byte = 12);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((byte)12, dict["Byte"]);
        }
        [Test]
        public void Test_ByteNull()
        {
            var dt = DateTime.Now;
            var dict = DoAction(q => q.ByteNull = 12);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((byte)12, dict["ByteNull"]);

            dict = DoAction(q => q.ByteNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["ByteNull"]);
        }
        [Test]
        public void Test_sByte()
        {
            var dict = DoAction(q => q.sByte = 12);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((sbyte)12, dict["sByte"]);
        }
        [Test]
        public void Test_sByteNull()
        {
            var dt = DateTime.Now;
            var dict = DoAction(q => q.sByteNull = 12);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual((sbyte)12, dict["sByteNull"]);

            dict = DoAction(q => q.sByteNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["sByteNull"]);
        }

        [Test]
        public void Test_Bytes()
        {
            var bs = new byte[] { 1, 2, 3, 4, 5 };
            var dict = DoAction(q => q.Bytes = bs);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(bs, dict["Bytes"]);
        }
        [Test]
        public void Test_sBytes()
        {
            var bs = new sbyte[] { 1, 2, 3, 4, 5 };
            var dict = DoAction(q => q.sBytes = bs);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(bs, dict["sBytes"]);
        }

        #endregion

        #region Test Emun

        [Test]
        public void Test_Emun()
        {
            var dict = DoAction(q => q.enumF = enumF.C);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(enumF.C, dict["enumF"]);
        }

        #endregion

        #region test Guid
        [Test]
        public void Test_Guid()
        {
            var g = Guid.NewGuid();
            var dict = DoAction(q => q.Guid = g);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(g, dict["Guid"]);
        }
        [Test]
        public void Test_GuidNull()
        {
            var g = Guid.NewGuid();

            var dict = DoAction(q => q.GuidNull = g);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(g, dict["GuidNull"]);

            dict = DoAction(q => q.GuidNull = null);
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(null, dict["GuidNull"]);
        }
        #endregion

        #region Test Attributes
        [Test]
        public void Test_Attributes()
        {
            var dict = DoAction(q => q.att = "123");
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual("123", dict["Att"]);
        }
        #endregion

        public Dictionary<string, object> DoAction(Action<ProxyData> act)
        {
            ProxyData a = new ProxyData();
            ProxyData b = UpdateProxy<ProxyData>.Create();

            act(a);
            act(b);
            return ((IUpdateChange<ProxyData>)b).__GetChanges__(a);
        }
    }

    public enum enumF
    {
        A,B,C
    }

    public class ProxyData
    {
        #region DateTime
        public DateTime DateTime { get; set; }
        public DateTime? DateTimeNull { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public DateTimeOffset? DateTimeOffsetNull { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public TimeSpan? TimeSpanNull { get; set; }

        #endregion
        #region Number
        public short Short { get; set; }
        public int Int { get; set; }
        public long Long { get; set; }
        public ushort uShort { get; set; }
        public uint uInt { get; set; }
        public ulong uLong { get; set; }

        public short? ShortNull { get; set; }
        public int? IntNull { get; set; }
        public long? LongNull { get; set; }
        public ushort? uShortNull { get; set; }
        public uint? uIntNull { get; set; }
        public ulong? uLongNull { get; set; }

        public float Float { get; set; }
        public double Double { get; set; }
        public decimal Decimal { get; set; }

        public float? FloatNull { get; set; }
        public double? DoubleNull { get; set; }
        public decimal? DecimalNull { get; set; }
        #endregion
        #region Text
        public string String { get; set; }
        public char Char { get; set; }
        public char? CharNull { get; set; }
        public AnsiString AnsiString { get; set; }

        #endregion
        #region bool
        public bool Bool { get; set; }
        public bool? BoolNull { get; set; }
        #endregion
        #region Byte
        public byte[] Bytes { get; set; }
        public sbyte[] sBytes { get; set; }
        public byte Byte { get; set; }
        public byte? ByteNull { get; set; }
        public sbyte sByte { get; set; }
        public sbyte? sByteNull { get; set; }

        #endregion
        #region GUID
        public Guid Guid { get; set; }
        public Guid? GuidNull { get; set; }
        #endregion
        #region enum
        public enumF enumF { get; set; }
        #endregion
        [Column("Att")]
        public string att { get; set; }
    }
}
