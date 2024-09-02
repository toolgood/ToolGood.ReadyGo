using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    public class SelectSql<T> where T : class
    {
        public string ColumnsSql { get; set; }
        public string TableSql { get; set; }
        public List<string> WhereSqls { get; private set; }
        public List<SqlParameter> SqlParameters { get; private set; }
        public string OrderSql { get; set; }

        public SelectSql()
        {
            var tb = PetaPoco.PocoData.ForType(typeof(T));
            TableSql = "Form " + tb.TableInfo.TableName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetWhereSql()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var sql in WhereSqls) {
                var x = RemoveStart(sql, "WHERE ");
                if (string.IsNullOrEmpty(x)) { continue; }

                while (stringBuilder.Length > 0) { stringBuilder.Append(" AND "); }
                stringBuilder.Append(x);
            }
            return stringBuilder.ToString();
        }
        private string RemoveStart(string txt, string startsText)
        {
            if (string.IsNullOrEmpty(txt) == false) {
                txt = txt.Trim();
                if (txt.StartsWith(startsText, StringComparison.InvariantCultureIgnoreCase)) {
                    txt = txt.Substring(startsText.Length);
                }
            }
            return txt;
        }


        /// <summary>
        /// 添加SQL参数
        /// </summary>
        /// <param name="whereSql">sql</param>
        public void Where(string whereSql)
        {
            WhereSqls.Add(whereSql);
        }

        /// <summary>
        /// 添加SQL参数
        /// </summary>
        /// <param name="whereSql">sql</param>
        /// <param name="parameterName">不带前缀</param>
        /// <param name="value">值</param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <param name="scale"></param>
        public void Where(string whereSql, string parameterName, object value, DbType? dbType = null, int? size = null, ParameterDirection? direction = null, byte? scale = null)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = value,
                DbType = dbType,
                ParameterDirection = direction ?? ParameterDirection.Input,
                Size = size,
                Scale = scale
            };
            WhereSqls.Add(whereSql);
            SqlParameters.Add(parameter);
        }

        /// <summary>
        /// 添加 like 参数
        /// </summary>
        /// <param name="whereSql">sql</param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public void Where_Like(string whereSql, string parameterName, string value)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = "%" + SqlUtil.ToEscapeLikeParam(value) + "%",
                DbType = DbType.String,
                ParameterDirection = ParameterDirection.Input,
            };
            WhereSqls.Add(whereSql);
            SqlParameters.Add(parameter);
        }

        /// <summary>
        ///  添加 like 参数
        /// </summary>
        /// <param name="whereSql">sql</param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public void Where_LikeStart(string whereSql, string parameterName, string value)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = "%" + SqlUtil.ToEscapeLikeParam(value),
                DbType = DbType.String,
                ParameterDirection = ParameterDirection.Input,
            };
            WhereSqls.Add(whereSql);
            SqlParameters.Add(parameter);
        }

        /// <summary>
        ///  添加 like 参数
        /// </summary>
        /// <param name="whereSql">sql</param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public void Where_LikeEnd(string whereSql, string parameterName, string value)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = SqlUtil.ToEscapeLikeParam(value) + "%",
                DbType = DbType.String,
                ParameterDirection = ParameterDirection.Input,
            };
            WhereSqls.Add(whereSql);
            SqlParameters.Add(parameter);
        }

        /// <summary>
        /// 添加 开始日期 参数 ，格式化： yyyy-MM-dd 0:0:0
        /// </summary>
        /// <param name="whereSql">sql</param>
        /// <param name="parameterName">不带前缀</param>
        /// <param name="value">值</param>
        public void Where_DateStart(string whereSql, string parameterName, DateTime value)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = value.Date,
                DbType = DbType.Date,
                ParameterDirection = ParameterDirection.Input,
            };
            WhereSqls.Add(whereSql);
            SqlParameters.Add(parameter);
        }

        /// <summary>
        /// 添加 结束日期 参数 ，格式化 yyyy-MM-dd 23:59:59
        /// </summary>
        /// <param name="whereSql">sql</param>
        /// <param name="parameterName">不带前缀</param>
        /// <param name="value">值</param>
        public void Where_DateEnd(string whereSql, string parameterName, DateTime value)
        {
            SqlParameter parameter = new SqlParameter {
                ParameterName = parameterName,
                Value = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59),
                DbType = DbType.DateTime,
                ParameterDirection = ParameterDirection.Input,
            };
            WhereSqls.Add(whereSql);
            SqlParameters.Add(parameter);
        }




    }
}
