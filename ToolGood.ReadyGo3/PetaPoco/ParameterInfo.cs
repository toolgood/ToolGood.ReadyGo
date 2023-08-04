using System;
using System.Collections.Generic;
using System.Data;
using static System.Formats.Asn1.AsnWriter;
using System.Drawing;
using System.Runtime.InteropServices.JavaScript;

namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// SQL 参数集合
    /// </summary>
    public class SqlParameterCollection : List<SqlParameter>
    {
        /// <summary>
        /// 添加SQL参数
        /// </summary>
        /// <param name="parameterName">不带前缀</param>
        /// <param name="value">值</param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <param name="scale"></param>
        public void Add(string parameterName, object value, DbType? dbType = null, int? size = null, ParameterDirection? direction = null, byte? scale = null)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = value,
                DbType = dbType,
                ParameterDirection = direction ?? ParameterDirection.Input,
                Size = size,
                Scale = scale
            };
            this.Add(parameter);
        }
        public void AddLike(string parameterName, string value)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = "%" + value.ToEscapeLikeParam() + "%",
                DbType = DbType.String,
                ParameterDirection = ParameterDirection.Input,
            };
            this.Add(parameter);
        }
        public void AddLikeStart(string parameterName, string value)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = "%" + value.ToEscapeLikeParam(),
                DbType = DbType.String,
                ParameterDirection = ParameterDirection.Input,


            };
            this.Add(parameter);
        }
        public void AddLikeEnd(string parameterName, string value)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = value.ToEscapeLikeParam() + "End",
                DbType = DbType.String,
                ParameterDirection = ParameterDirection.Input,
            };
            this.Add(parameter);
        }

        /// <summary>
        /// 添加 开始日期 参数 ，格式化： yyyy-MM-dd 0:0:0
        /// </summary>
        /// <param name="parameterName">不带前缀</param>
        /// <param name="value">值</param>
        public void StartDate(string parameterName, DateTime value)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = value.Date,
                ParameterDirection = ParameterDirection.Input,
            };
            this.Add(parameter);
        }

        /// <summary>
        /// 添加 结束日期 参数 ，格式化 yyyy-MM-dd 23:59:59
        /// </summary>
        /// <param name="parameterName">不带前缀</param>
        /// <param name="value">值</param>
        public void EndDate(string parameterName, DateTime value)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59),
                ParameterDirection = ParameterDirection.Input,
            };
            this.Add(parameter);
        }




    }


    /// <summary>
    /// SQL 参数
    /// </summary>
    public class SqlParameter
    {
        /// <summary>
        /// 参数名字
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public ParameterDirection ParameterDirection { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public DbType? DbType { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// 获取或设置所解析的 Value 的小数位数。
        /// </summary>
        public byte? Scale { get; set; }

    }
}
