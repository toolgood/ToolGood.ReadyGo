using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 一对多关系映射的帮助类，用于在多行结果合并时处理子集合的赋值。
    /// </summary>
    public class OneToManyHelper
    {
        /// <summary>
        /// 将当前对象的列表值合并到前一个对象的列表中。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="listFunc">用于获取对象列表属性的委托。</param>
        /// <param name="pocoMember">列表成员元数据。</param>
        /// <param name="prevPoco">前一个对象。</param>
        /// <param name="poco">当前对象。</param>
        public static void SetListValue<T>(Func<T, IList> listFunc, PocoMember pocoMember, object prevPoco, T poco)
        {
            var prevList = listFunc((T)prevPoco);
            var currentList = listFunc(poco);

            if (prevList == null && currentList != null)
            {
                prevList = pocoMember.CreateList();
                pocoMember.SetValue(prevPoco, prevList);
            }

            if (prevList != null && currentList != null)
            {
                foreach (var item in currentList)
                {
                    prevList.Add(item);
                }
            }
        }

        /// <summary>
        /// 为前一个对象列表中的每个元素设置外键成员值。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="listFunc">用于获取对象列表属性的委托。</param>
        /// <param name="foreignMember">外键成员元数据。</param>
        /// <param name="prevPoco">前一个对象。</param>
        public static void SetForeignList<T>(Func<T, IList> listFunc, PocoMember foreignMember, object prevPoco)
        {
            if (listFunc == null || foreignMember == null)
                return;

            var currentList = listFunc((T)prevPoco);

            if (currentList == null)
                return;

            foreach (var item in currentList)
            {
                foreignMember.SetValue(item, prevPoco);
            }
        }
    }
}
