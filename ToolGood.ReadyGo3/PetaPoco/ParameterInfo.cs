using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3
{
    public class SqlParameterCollection : List<SqlParameter>
    {
        /// <summary>
        /// 添加SQL参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public void Add(string parameterName, object value)
        {
            SqlParameter parameter = new SqlParameter(parameterName, value);
            this.Add(parameter);
        }

    }



    public class SqlParameter
    {
        public SqlParameter()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName">参数名字</param>
        /// <param name="value">参数值</param>
        public SqlParameter(string parameterName, object value)
        {
            ParameterName = parameterName.Replace("@", "");
            Value = value;
            //ParameterDirection = ParameterDirection.Input;
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="parameterName">参数名字</param>
        ///// <param name="value">参数值</param>
        ///// <param name="dbType">参数类型</param>
        ///// <param name="size">长度</param>
        ///// <param name="direction">参数类型</param>
        ///// <param name="scale">获取或设置所解析的 Value 的小数位数。</param>
        //public ParameterInfo(string parameterName, object value, DbType? dbType, int? size, ParameterDirection? direction, byte? scale = null)
        //{
        //    ParameterName = parameterName.Replace("@", "");
        //    Value = value;
        //    Size = size ?? 0;
        //    ParameterDirection = direction ?? ParameterDirection.Input;
        //    Scale = scale ?? 0;
        //}
        /// <summary>
        /// 参数名字
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }

        ///// <summary>
        ///// 参数类型
        ///// </summary>
        //public ParameterDirection ParameterDirection { get; set; }
        ///// <summary>
        ///// 数据类型
        ///// </summary>
        //public DbType DbType { get; set; }
        ///// <summary>
        ///// 长度
        ///// </summary>
        //public int Size { get; set; }
        ///// <summary>
        ///// 获取或设置所解析的 Value 的小数位数。
        ///// </summary>
        //public byte? Scale { get; set; }

    }
}
