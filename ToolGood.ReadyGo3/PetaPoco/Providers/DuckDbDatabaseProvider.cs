using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.ReadyGo3.Enums;
using ToolGood.ReadyGo3.PetaPoco.Core;

namespace ToolGood.ReadyGo3.PetaPoco.Providers
{
	public class DuckDbDatabaseProvider : DatabaseProvider
	{

		public override DbProviderFactory GetFactory()
		{
			return GetFactory(
				"DuckDB.NET.Data.DuckDBClientFactory, DuckDB.NET.Data, Culture=neutral, PublicKeyToken=1d0aa5325e915c3b",
				"DuckDB.NET.Data.DuckDBClientFactory, DuckDB.NET.Data"
				);
		}
		public override object MapParameterValue(object value)
		{
			if(value is uint)
				return (long)((uint)value);

			return base.MapParameterValue(value);
		}

		public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string primaryKeyName)
		{
			if(primaryKeyName != null) {
				cmd.CommandText = cmd.CommandText.TrimEnd(';', ' ', '\r', '\n', '\t') + " RETURNING *;";
				return db.ExecuteScalarHelper(cmd);
			} else {
				db.ExecuteNonQueryHelper(cmd);
				return -1;
			}
		}

		public override string GetExistsSql()
		{
			return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
		}
		public override string EscapeSqlIdentifier(string sqlIdentifier)
		{
			return $"\"{sqlIdentifier}\"";
		}
		public override string GetTableName(string databaseName, string schemaName, string tableName)
		{
			return $"{tableName}";
		}

		public override string CreateSql(int limit, int offset, string columnSql, string fromtable, string order, string where)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("SELECT ");
			sb.Append(columnSql);
			sb.Append(" FROM ");
			sb.Append(fromtable);
			if(string.IsNullOrEmpty(where) == false) {
				sb.Append(" WHERE ");
				sb.Append(where);
			}
			if(string.IsNullOrEmpty(order) == false) {
				sb.Append(" ORDER BY ");
				sb.Append(order);
			}
			if(limit > 0) {
				sb.Append(" LIMIT ");
				if(offset > 0) {
					sb.Append(offset);
					sb.Append(",");
				}
				sb.Append(limit);
			}
			return sb.ToString();
		}

		public override string CreateSql(string columnSql, string fromtable, string order, string where)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("SELECT ");
			sb.Append(columnSql);
			sb.Append(" FROM ");
			sb.Append(fromtable);
			if(string.IsNullOrEmpty(where) == false) {
				sb.Append(" WHERE ");
				sb.Append(where);
			}
			if(string.IsNullOrEmpty(order) == false) {
				sb.Append(" ORDER BY ");
				sb.Append(order);
			}
			return sb.ToString();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="function"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public override string CreateFunction(SqlFunction function, params object[] args)
		{
			switch(function) {
				case SqlFunction.Fuction: break;
				case SqlFunction.Len: return CreateFunction("LENGTH({0})", args);
				case SqlFunction.Year: return CreateFunction("YEAR({0})", args);
				case SqlFunction.Month: return CreateFunction("MONTH({0})", args);
				case SqlFunction.Day: return CreateFunction("DAY({0})", args);
				case SqlFunction.Hour: return CreateFunction("HOUR({0})", args);
				case SqlFunction.Minute: return CreateFunction("MINUTE({0})", args);
				case SqlFunction.Second: return CreateFunction("SECOND({0})", args);
				case SqlFunction.DayOfYear: return CreateFunction("DAYOFYEAR({0})", args);
				case SqlFunction.WeekDay: return CreateFunction("DAYOFWEEK({0})", args);
				case SqlFunction.SubString3: return CreateFunction("SUBSTR({0},{1},{2})", args);
				case SqlFunction.SubString2: return CreateFunction("SUBSTR({0},{1})", args);
				case SqlFunction.Lower: break;
				case SqlFunction.Upper: break;
				default: break;
			}

			return base.CreateFunction(function, args);
		}

		public override string ToString()
		{
			return "DuckDbDatabaseProvider";
		}


	}
}
