using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo3.PetaPoco.Core
{
    partial class DatabaseProvider
    {
        public virtual async Task<object> ExecuteInsertAsync(Database db, SqlCommand cmd, string PrimaryKeyName)
        {
            cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            var result = await db.ExecuteScalarHelperAsync(cmd);
            return result;
        }

    }
}
