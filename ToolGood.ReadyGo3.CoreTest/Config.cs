using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolGood.ReadyGo3.Test
{
    public class Config
    {
        public const string DbFile = "db.db3";

        [ThreadStatic]
        private static SqlHelper _dbHelper;
        [ThreadStatic]
        private static SqlHelper _insertHelper;


        public static SqlHelper DbHelper
        {
            get {
                if (_dbHelper==null) {
                    _dbHelper = SqlHelperFactory.OpenSqliteFile(DbFile);
                }
                return _dbHelper;
            }
        }
        public static SqlHelper InsertHelper
        {
            get {
                if (_insertHelper == null) {
                    _insertHelper = SqlHelperFactory.OpenSqliteFile(DbFile);
                    _insertHelper._Config.Insert_DateTime_Default_Now = true;
                    _insertHelper._Config.Insert_Guid_Default_New = true;
                    _insertHelper._Config.Insert_String_Default_NotNull = true;
                }
                return _insertHelper;
            }
        }


        public static SqlHelper TempHelper
        {
            get
            {
                return SqlHelperFactory.OpenSqliteFile(DbFile);
            }
        }


        public static SqlHelper SqlServerHelper {
            get {
                return SqlHelperFactory.OpenDatabase(@"Server=(LocalDB)\MSSQLLocalDB; Integrated Security=true ;AttachDbFileName=F:\git\ToolGood.ReadyGo\ToolGood.ReadyGo3.CoreTest\bin\Debug\test.mdf", "", SqlType.SqlServer);
            }
        }

    }
}
