using System.Collections.Generic;


namespace ToolGood.ReadyGo3.DataCentxt
{
    partial class QTable<T> //: IDataSet<QTable<T>>
    {
        /// <summary>
        /// 根据字段名（非数据字段）设置更新值
        /// </summary>
        /// <param name="values"></param>
        public QTable<T> SetValues(Dictionary<string, object> values)
        {
            foreach (var item in values) {
                QTableColumn col;
                if (_columns.TryGetValue(item.Key.ToLower(), out col)) {
                    col.SetValue(item.Value);
                }
            }
            return this;
        }

        /// <summary>
        /// 根据字段名（非数据字段）设置更新值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public QTable<T> SetValue(string name, object value)
        {
            QTableColumn col;
            if (_columns.TryGetValue(name.ToLower(), out col)) {
                col.SetValue(value);
            }
            return this;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public int Update()
        {
            return GetSqlBuilder().Update();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public int Delete()
        {
            return GetSqlBuilder().Delete();
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="returnInsertId">是否返回插入的ID</param>
        /// <returns></returns>
        public object Insert(bool returnInsertId = false)
        {
            var config = GetSqlHelper()._Config;
            if (config.Insert_DateTime_Default_Now
                || config.Insert_Guid_Default_New
                || config.Insert_String_Default_NotNull) {
                SetDefaultValue(config.Insert_DateTime_Default_Now,
                    config.Insert_String_Default_NotNull, config.Insert_Guid_Default_New);
            }
            return GetSqlBuilder().Insert(returnInsertId);
        }
    }
}
