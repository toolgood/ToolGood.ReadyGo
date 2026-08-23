using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 提供创建对象快照并基于快照执行更新的扩展方法。
    /// </summary>
    public static class Snapshotter
    {
        /// <summary>
        /// 为指定对象创建变更快照，用于后续跟踪字段变化。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="d">数据库配置。</param>
        /// <param name="obj">要跟踪的对象。</param>
        /// <returns>对象快照。</returns>
        public static Snapshot<T> StartSnapshot<T>(this IDatabaseConfig d, T obj)
        {
            return new Snapshot<T>(d.PocoDataFactory.ForType(obj.GetType()), obj);
        }

        /// <summary>
        /// 根据快照仅更新对象中发生变化的列。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="d">数据库实例。</param>
        /// <param name="obj">要更新的对象。</param>
        /// <param name="snapshot">对象快照。</param>
        /// <returns>受影响的行数。</returns>
        public static int Update<T>(this IDatabase d, T obj, Snapshot<T> snapshot)
        {
            return d.Update(obj, snapshot.UpdatedColumns());
        }

        /// <summary>
        /// 异步地根据快照仅更新对象中发生变化的列。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="d">异步数据库实例。</param>
        /// <param name="obj">要更新的对象。</param>
        /// <param name="snapshot">对象快照。</param>
        /// <returns>表示受影响行数的任务。</returns>
        public static Task<int> UpdateAsync<T>(this IAsyncDatabase d, T obj, Snapshot<T> snapshot)
        {
            return d.UpdateAsync(obj, snapshot.UpdatedColumns());
        }
    }

    /// <summary>
    /// 跟踪对象字段的原始值并计算发生的变更。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    public class Snapshot<T>
    {
        private readonly PocoData _pocoData;
        private T _trackedObject;
        private readonly Dictionary<PocoColumn, object> _originalValues = new Dictionary<PocoColumn, object>();

        /// <summary>
        /// 初始化 Snapshot 类的新实例并记录对象的原始值。
        /// </summary>
        /// <param name="pocoData">关联的 POCO 数据。</param>
        /// <param name="trackedObject">要跟踪的对象。</param>
        public Snapshot(PocoData pocoData, T trackedObject)
        {
            _pocoData = pocoData;
            _trackedObject = trackedObject;
            PopulateValues(trackedObject);
        }

        private void PopulateValues(T original)
        {
            var clone = original.Copy();
            foreach (var pocoColumn in _pocoData.Columns.Values)
            {
                _originalValues[pocoColumn] = pocoColumn.GetColumnValue(_pocoData, clone);
            }
        }

        /// <summary>
        /// 替换被跟踪的对象实例。
        /// </summary>
        /// <param name="obj">新的对象实例。</param>
        public void OverrideTrackedObject(T obj)
        {
            _trackedObject = obj;
        }

        /// <summary>
        /// 获取发生变化列的列名列表。
        /// </summary>
        /// <returns>列名列表。</returns>
        public List<string> UpdatedColumns()
        {
            return Changes().Select(x => x.ColumnName).ToList();
        }

        /// <summary>
        /// 表示一个列的变化信息。
        /// </summary>
        public class Change
        {
            /// <summary>
            /// 成员名称。
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 列名。
            /// </summary>
            public string ColumnName { get; set; }
            /// <summary>
            /// 旧值。
            /// </summary>
            public object OldValue { get; set; }
            /// <summary>
            /// 新值。
            /// </summary>
            public object NewValue { get; set; }
        }

        /// <summary>
        /// 计算对象当前值与原始值之间的所有变化。
        /// </summary>
        /// <returns>变化信息列表。</returns>
        public List<Change> Changes()
        {
            var list = new List<Change>();
            foreach (var pocoColumn in _originalValues)
            {
                var newValue = pocoColumn.Key.GetColumnValue(_pocoData, _trackedObject);
                if (!AreEqual(pocoColumn.Value, newValue))
                {
                    list.Add(new Change()
                    {
                        Name = pocoColumn.Key.MemberInfoData.Name,
                        ColumnName = pocoColumn.Key.ColumnName,
                        NewValue = newValue,
                        OldValue = pocoColumn.Value
                    });
                }
            }
            return list;
        }

        private bool AreEqual(object first, object second)
        {
            if (first == null && second == null) return true;
            if (first == null) return false;
            if (second == null) return false;

            var type = first.GetType();
            if (type.IsAClass() || type.IsArray)
            {
                return _pocoData.Mapper.ColumnSerializer.Serialize(first) == _pocoData.Mapper.ColumnSerializer.Serialize(second);
            }

            return first.Equals(second);
        }
    }
}
