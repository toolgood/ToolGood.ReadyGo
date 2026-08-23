using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace ToolGood.ReadyGo.NPoco
{
    /// <summary>
    /// 定义类型映射器接口，用于提供数据库值与对象值之间的转换委托。
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// 获取从数据库值到目标成员类型的转换委托。
        /// </summary>
        /// <param name="memberInfo">目标成员信息。</param>
        /// <param name="sourceType">数据库值的源类型。</param>
        /// <returns>转换委托，若无匹配的转换器则返回 null。</returns>
        Func<object, object> GetFromDbConverter(MemberInfo memberInfo, Type sourceType);
        /// <summary>
        /// 获取从数据库值到目标类型的转换委托。
        /// </summary>
        /// <param name="destType">目标类型。</param>
        /// <param name="sourceType">数据库值的源类型。</param>
        /// <returns>转换委托，若无匹配的转换器则返回 null。</returns>
        Func<object, object> GetFromDbConverter(Type destType, Type sourceType);
        /// <summary>
        /// 获取对象值到命令参数类型的转换委托。
        /// </summary>
        /// <param name="dbCommand">当前数据库命令。</param>
        /// <param name="sourceType">对象值的源类型。</param>
        /// <returns>转换委托，若无匹配的转换器则返回 null。</returns>
        Func<object, object> GetParameterConverter(DbCommand dbCommand, Type sourceType);
        /// <summary>
        /// 获取从源成员值到数据库目标类型的转换委托。
        /// </summary>
        /// <param name="destType">数据库目标类型。</param>
        /// <param name="sourceMemberInfo">源成员信息。</param>
        /// <returns>转换委托，若无匹配的转换器则返回 null。</returns>
        Func<object, object> GetToDbConverter(Type destType, MemberInfo sourceMemberInfo);
    }
}
