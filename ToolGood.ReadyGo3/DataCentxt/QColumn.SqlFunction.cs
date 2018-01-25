using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QColumn
    {

        #region 常用SQL函数
        /// <summary>
        /// 返回字符串str的长度
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Len()
        {
            return new QSqlColumn(Enums.SqlFunction.Len, this);
        }
        /// <summary>
        /// 求最大值
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Max()
        {
            return new QSqlColumn(Enums.SqlFunction.Max, this);
        }
        /// <summary>
        /// 求最小值
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Min()
        {
            return new QSqlColumn(Enums.SqlFunction.Min, this);
        }
        /// <summary>
        /// 求平均值
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Avg()
        {
            return new QSqlColumn(Enums.SqlFunction.Avg, this);
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Sum()
        {
            return new QSqlColumn(Enums.SqlFunction.Sum, this);
        }
        /// <summary>
        /// 求个数
        /// </summary>
        /// <param name="distinct"></param>
        /// <returns></returns>
        public QSqlColumn Count(bool distinct)
        {
            if (distinct) {
                return new QSqlColumn(Enums.SqlFunction.CountDistinct, this);
            }
            return new QSqlColumn(Enums.SqlFunction.Count, this);
        }
        /// <summary>
        /// 返回日期格式，不适应所有数据库，请少用
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public QSqlColumn DatePart(string part)
        {
            return new QSqlColumn(Enums.SqlFunction.DatePart, this);
        }
        /// <summary>
        /// 求日期间隔天数
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public QSqlColumn DateDiff(DateTime date)
        {
            return new QSqlColumn(Enums.SqlFunction.DateDiff, this, date);
        }
        /// <summary>
        /// 获取年份
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Year()
        {
            return new QSqlColumn(Enums.SqlFunction.Year, this);
        }
        /// <summary>
        /// 获取月份
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Month()
        {
            return new QSqlColumn(Enums.SqlFunction.Month, this);
        }
        /// <summary>
        /// 获取日
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Day()
        {
            return new QSqlColumn(Enums.SqlFunction.Day, this);
        }
        /// <summary>
        /// 获取小时
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Hour()
        {
            return new QSqlColumn(Enums.SqlFunction.Hour, this);
        }
        /// <summary>
        /// 获取分钟
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Minute()
        {
            return new QSqlColumn(Enums.SqlFunction.Minute, this);
        }
        /// <summary>
        /// 获取分
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Second()
        {
            return new QSqlColumn(Enums.SqlFunction.Second, this);
        }
        /// <summary>
        /// 返回年份为日期的天，范围为1至366。
        /// </summary>
        /// <returns></returns>
        public QSqlColumn DayOfYear()
        {
            return new QSqlColumn(Enums.SqlFunction.DayOfYear, this);
        }
        /// <summary>
        /// 返回日期的星期数
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Week()
        {
            return new QSqlColumn(Enums.SqlFunction.Week, this);
        }
        /// <summary>
        /// 返回一个日期的工作日索引值，即星期一为0，星期二为1，星期日为6。
        /// </summary>
        /// <returns></returns>
        public QSqlColumn WeekDay()
        {
            return new QSqlColumn(Enums.SqlFunction.WeekDay, this);
        }
        /// <summary>
        /// 返回一个从位置start开始的长度为length子串
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public QSqlColumn SubString(int start, int length)
        {
            return new QSqlColumn(Enums.SqlFunction.SubString3, this,start,length);
        }
        /// <summary>
        /// 返回一个从位置start开始的子串
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public QSqlColumn SubString(int start)
        {
            return new QSqlColumn(Enums.SqlFunction.SubString2, this,start);
        }
        /// <summary>
        /// 返回字符串str最左边的 len 个字符
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public QSqlColumn Left(int length)
        {
            return new QSqlColumn(Enums.SqlFunction.Left, this,length);
        }
        /// <summary>
        /// 返回字符串str最右边的 len 个字符
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public QSqlColumn Right(int length)
        {
            return new QSqlColumn(Enums.SqlFunction.Right, this,length);
        }

        /// <summary>
        /// 返回根据当前字符集映射转为小写字母。
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Lower()
        {
            return new QSqlColumn(Enums.SqlFunction.Lower, this);
        }
        /// <summary>
        /// 返回根据当前字符集映射转为大写字母。
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Upper()
        {
            return new QSqlColumn(Enums.SqlFunction.Upper, this);
        }
        /// <summary>
        /// 返回字符串str的最左字符的数值。
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Ascii()
        {
            return new QSqlColumn(Enums.SqlFunction.Ascii, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Concat()
        {
            return new QSqlColumn(Enums.SqlFunction.Concat, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public QSqlColumn Trim()
        {
            return new QSqlColumn(Enums.SqlFunction.Trim, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public QSqlColumn LTrim()
        {
            return new QSqlColumn(Enums.SqlFunction.LTrim, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public QSqlColumn RTrim()
        {
            return new QSqlColumn(Enums.SqlFunction.RTrim, this);

        }

        /// <summary>
        /// 使用自定义函数，{0}代表当前Column
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public QSqlColumn Function(string format)
        {
            return new QSqlColumn(Enums.SqlFunction.Fuction, format, this);
        }
        #endregion
    }
}
