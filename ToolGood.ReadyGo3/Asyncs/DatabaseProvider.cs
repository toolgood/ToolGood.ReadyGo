using System.Threading.Tasks;

#if !NETSTANDARD2_0
using System.Data.SqlClient;
#endif

#if !NET40

using SqlCommand = System.Data.Common.DbCommand;


namespace ToolGood.ReadyGo3.PetaPoco.Core
{
    partial class DatabaseProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="cmd"></param>
        /// <param name="PrimaryKeyName"></param>
        /// <returns></returns>
        public virtual Task<object> ExecuteInsertAsync(Database db, SqlCommand cmd, string PrimaryKeyName)
        {
            cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            return db.ExecuteScalarHelperAsync(cmd);
        }

    }
}
#endif