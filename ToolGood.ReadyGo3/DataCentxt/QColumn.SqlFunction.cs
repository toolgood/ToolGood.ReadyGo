using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolGood.ReadyGo3.DataCentxt.Interfaces;

namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QColumnBase
    {
        private QColumn CreateFunctionColumn(string funName, string funFormat, params object[] args)
        {
            QColumn column = new QColumn();
            if (string.IsNullOrEmpty(funName)) {
                column._columnType = Enums.ColumnType.FunctionFormat;
            } else {
                column._columnType = Enums.ColumnType.Function;
                column._functionName = funName;
            }
            column._functionFormat = funFormat;
            column._functionArgs = args;
            return column;
        }

        #region 常用SQL函数
        /// <summary>
        /// 返回字符串str的长度
        /// </summary>
        /// <returns></returns>
        public QColumn Len()
        {
            return CreateFunctionColumn("Len", "LEN({0})", this);
        }
        /// <summary>
        /// 求最大值
        /// </summary>
        /// <returns></returns>
        public QColumn Max()
        {
            return CreateFunctionColumn("Max", "MAX({0})", this);
        }
        /// <summary>
        /// 求最小值
        /// </summary>
        /// <returns></returns>
        public QColumn Min()
        {
            return CreateFunctionColumn("Min", "MIN({0})", this);
        }
        /// <summary>
        /// 求平均值
        /// </summary>
        /// <returns></returns>
        public QColumn Avg()
        {
            return CreateFunctionColumn("Avg", "AVG({0})", this);
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <returns></returns>
        public QColumn Sum()
        {
            return CreateFunctionColumn("Sum", "SUM({0})", this);
        }
        /// <summary>
        /// 求个数
        /// </summary>
        /// <param name="distinct"></param>
        /// <returns></returns>
        public QColumn Count(bool distinct)
        {
            if (distinct) {
                return CreateFunctionColumn("CountDistinct", "COUNT(DISTINCT {0})", this);
            }
            return CreateFunctionColumn("Count", "COUNT({0})", this);
        }
        /// <summary>
        /// 返回日期格式，不适应所有数据库，请少用
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public QColumn DatePart(string part)
        {
            return CreateFunctionColumn("DatePart", "DATEPART({0},{1})", part, this);
        }
        /// <summary>
        /// 求日期间隔天数
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public QColumn DateDiff(DateTime date)
        {
            return CreateFunctionColumn("DateDiff", "DATEDIFF({0},{1})", this, date);
        }
        /// <summary>
        /// 获取年份
        /// </summary>
        /// <returns></returns>
        public QColumn Year()
        {
            return CreateFunctionColumn("Year", "YEAR({0})", this);
        }
        /// <summary>
        /// 获取月份
        /// </summary>
        /// <returns></returns>
        public QColumn Month()
        {
            return CreateFunctionColumn("Month", "MONTH({0})", this);
        }
        /// <summary>
        /// 获取日
        /// </summary>
        /// <returns></returns>
        public QColumn Day()
        {
            return CreateFunctionColumn("Day", "DAY({0})", this);
        }
        /// <summary>
        /// 获取小时
        /// </summary>
        /// <returns></returns>
        public QColumn Hour()
        {
            return CreateFunctionColumn("Hour", "HOUR({0})", this);
        }
        /// <summary>
        /// 获取分钟
        /// </summary>
        /// <returns></returns>
        public QColumn Minute()
        {
            return CreateFunctionColumn("Minute", "MINUTE({0})", this);
        }
        /// <summary>
        /// 获取分
        /// </summary>
        /// <returns></returns>
        public QColumn Second()
        {
            return CreateFunctionColumn("Second", "SECOND({0})", this);
        }
        /// <summary>
        /// 返回年份为日期的天，范围为1至366。
        /// </summary>
        /// <returns></returns>
        public QColumn DayOfYear()
        {
            return CreateFunctionColumn("DayOfYear", "DAYOFYEAR({0})", this);
        }
        /// <summary>
        /// 返回日期的星期数
        /// </summary>
        /// <returns></returns>
        public QColumn Week()
        {
            return CreateFunctionColumn("Week", "WEEK({0})", this);
        }
        /// <summary>
        /// 返回一个日期的工作日索引值，即星期一为0，星期二为1，星期日为6。
        /// </summary>
        /// <returns></returns>
        public QColumn WeekDay()
        {
            return CreateFunctionColumn("WeekDay", "WEEKDAY({0})", this);
        }
        /// <summary>
        /// 返回一个从位置start开始的长度为length子串
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public QColumn SubString(int start, int length)
        {
            return CreateFunctionColumn("SubString3", "SUBSTRING({0},{1},{2})", this, start, length);
        }
        /// <summary>
        /// 返回一个从位置start开始的子串
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public QColumn SubString(int start)
        {
            return CreateFunctionColumn("SubString2", "SUBSTRING({0},{1})", this, start);
        }
        /// <summary>
        /// 返回字符串str最左边的 len 个字符
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public QColumn Left(int length)
        {
            return CreateFunctionColumn("Left", "LEFT({0},{1})", this, length);
        }
        /// <summary>
        /// 返回字符串str最右边的 len 个字符
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public QColumn Right(int length)
        {
            return CreateFunctionColumn("Right", "RIGHT({0},{1})", this, length);
        }

        /// <summary>
        /// 返回根据当前字符集映射转为小写字母。
        /// </summary>
        /// <returns></returns>
        public QColumn Lower()
        {
            return CreateFunctionColumn("Lower", "LOWER({0})", this);
        }
        /// <summary>
        /// 返回根据当前字符集映射转为大写字母。
        /// </summary>
        /// <returns></returns>
        public QColumn Upper()
        {
            return CreateFunctionColumn("Upper", "UPPER({0})", this);
        }
        /// <summary>
        /// 返回字符串str的最左字符的数值。
        /// </summary>
        /// <returns></returns>
        public QColumn Ascii()
        {
            return CreateFunctionColumn("Ascii", "ASCII({0})", this);
        }
        /// <summary>
        /// 使用自定义函数，{0}代表当前Column
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public QColumn Function(string format)
        {
            return CreateFunctionColumn("", format, this);
        }
        #endregion
    }
}
