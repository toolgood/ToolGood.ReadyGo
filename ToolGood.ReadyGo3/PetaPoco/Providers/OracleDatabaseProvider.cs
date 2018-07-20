using System;
using System.Data;
using System.Data.Common;
using ToolGood.ReadyGo3.PetaPoco.Core;
using ToolGood.ReadyGo3.PetaPoco.Internal;
using ToolGood.ReadyGo3.PetaPoco.Utilities;

namespace ToolGood.ReadyGo3.PetaPoco.Providers
{
    public class OracleDatabaseProvider : DatabaseProvider
    {
        public override string GetParameterPrefix(string connectionString)
        {
            return ":";
        }

        public override void PreExecute(IDbCommand cmd)
        {
            cmd.GetType().GetProperty("BindByName").SetValue(cmd, true, null);
            cmd.GetType().GetProperty("InitialLONGFetchSize").SetValue(cmd, -1, null);
        }

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (parts.SqlSelectRemoved.StartsWith("*"))
                throw new Exception("Query must alias '*' when performing a paged query.\neg. select t.* from table t order by t.id");

            // Same deal as SQL Server
            return Singleton<SqlServerDatabaseProvider>.Instance.BuildPageQuery(skip, take, parts, ref args);
        }

        public override DbProviderFactory GetFactory()
        {
            // "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess" is for Oracle.ManagedDataAccess.dll
            // "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess" is for Oracle.DataAccess.dll
            return GetFactory(
                "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Culture=neutral, PublicKeyToken=89b483f429c47342",
                "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, PublicKeyToken=89b483f429c47342",
                "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess",

                "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess, Culture=neutral, PublicKeyToken=89b483f429c47342",
                "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess, PublicKeyToken=89b483f429c47342",
                "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess",

                "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient, PublicKeyToken=b77a5c561934e089",
                "System.Data.OracleClient.OracleClientFactory, System.Data.OracleClient"
                );
        }

        public override string EscapeSqlIdentifier(string sqlIdentifier)
        {
            return $"\"{sqlIdentifier.ToUpperInvariant()}\"";
        }

        public override string GetAutoIncrementExpression(TableInfo ti)
        {
            if (!string.IsNullOrEmpty(ti.SequenceName))
                return $"{ti.SequenceName}.nextval";

            return null;
        }

        public override object ExecuteInsert(Database db, IDbCommand cmd, string primaryKeyName)
        {
            if (primaryKeyName != null)
            {
                cmd.CommandText += $" returning {EscapeSqlIdentifier(primaryKeyName)} into :newid";
                var param = cmd.CreateParameter();
                param.ParameterName = ":newid";
                param.Value = DBNull.Value;
                param.Direction = ParameterDirection.ReturnValue;
                param.DbType = DbType.Int64;
                cmd.Parameters.Add(param);
                db.ExecuteNonQueryHelper(cmd);
                return param.Value;
            }
            else
            {
                db.ExecuteNonQueryHelper(cmd);
                return -1;
            }
        }
    }
}