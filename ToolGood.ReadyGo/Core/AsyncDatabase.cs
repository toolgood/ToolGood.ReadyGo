using System.Collections;
using System.Linq.Expressions;
using ToolGood.ReadyGo.NPoco.Expressions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ToolGood.ReadyGo.NPoco.Extensions;
using ToolGood.ReadyGo.NPoco.Linq;
using System.Threading;
using System.Runtime.CompilerServices;
using ToolGood.ReadyGo;

namespace ToolGood.ReadyGo.NPoco
{
    public partial class Database
    {
        /// <summary>
        /// Performs an SQL Insert
        /// </summary>
        /// <param name="tableName">The name of the table to insert into</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>The auto allocated primary key of the new record</returns>
        public Task<object> InsertAsync(string tableName, string primaryKeyName, object poco, CancellationToken cancellationToken = default)
        {
            return InsertAsync(tableName, primaryKeyName, true, poco, cancellationToken);
        }


        /// <summary>
        /// Performs an SQL Insert
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
        /// <remarks>The name of the table, it's primary key and whether it's an auto-allocated primary key are retrieved
        /// from the POCO's attributes</remarks>
        public Task<object> InsertAsync<T>(T poco, CancellationToken cancellationToken = default)
        {
            var tableInfo = PocoDataFactory.TableInfoForType(poco.GetType());
            return InsertAsync(tableInfo.TableName, tableInfo.PrimaryKey, tableInfo.AutoIncrement, poco, cancellationToken);
        }

        /// <summary>
        /// Performs an SQL Insert
        /// </summary>
        /// <param name="tableName">The name of the table to insert into</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="autoIncrement">True if the primary key is automatically allocated by the DB</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
        /// <remarks>Inserts a poco into a table.  If the poco has a property with the same name 
        /// as the primary key the id of the new record is assigned to it.  Either way,
        /// the new id is returned.</remarks>
        public virtual Task<object> InsertAsync<T>(string tableName, string primaryKeyName, bool autoIncrement, T poco, CancellationToken cancellationToken = default)
        {
            var pd = PocoDataFactory.ForObject(poco, primaryKeyName, autoIncrement);
            return InsertAsyncImp(pd, tableName, primaryKeyName, autoIncrement, poco, false, cancellationToken);
        }

        private async Task<object> InsertAsyncImp<T>(PocoData pocoData, string tableName, string primaryKeyName, bool autoIncrement, T poco, bool sync, CancellationToken cancellationToken = default)
        {
            if (!OnInsertingInternal(new InsertContext(poco, tableName, autoIncrement, primaryKeyName)))
                return 0;

            try
            {
                if (sync) OpenSharedConnectionInternal();
                else await OpenSharedConnectionInternalAsync(cancellationToken);

                var preparedInsert = InsertStatements.PrepareInsertSql(this, pocoData, tableName, primaryKeyName, autoIncrement, poco);

                using (var cmd = CreateCommand(_sharedConnection, preparedInsert.Sql, preparedInsert.Rawvalues.ToArray()))
                {
                    // Assign the Version column
                    InsertStatements.AssignVersion(poco, preparedInsert);

                    object id;
                    if (!autoIncrement)
                    {
                        _ = sync
                            ? ExecuteNonQueryHelper(cmd)
                            : await ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);

                        id = InsertStatements.AssignNonIncrementPrimaryKey(primaryKeyName, poco, preparedInsert);
                    }
                    else
                    {
                        id = sync 
                            ? _dbType.ExecuteInsert(this, cmd, primaryKeyName, preparedInsert.PocoData.TableInfo.UseOutputClause, poco, preparedInsert.Rawvalues.ToArray())
                            : await _dbType.ExecuteInsertAsync(this, cmd, primaryKeyName, preparedInsert.PocoData.TableInfo.UseOutputClause, poco, preparedInsert.Rawvalues.ToArray(), cancellationToken).ConfigureAwait(false);
                        
                        InsertStatements.AssignPrimaryKey(primaryKeyName, poco, id, preparedInsert);
                    }

                    return id;
                }
            }
            catch (Exception x)
            {
                OnExceptionInternal(x);
                throw;
            }
            finally
            {
                if (sync) CloseSharedConnectionInternal();
                else await CloseSharedConnectionInternalAsync();
            }
        }

        /// <summary>
        /// 使用批量插入方式将多个实体插入数据库（SqlServer 使用 SqlBulkCopy，其他数据库退化为逐行插入）。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="pocos">要插入的实体集合。</param>
        /// <param name="options">批量插入选项。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        public async Task InsertBulkAsync<T>(IEnumerable<T> pocos, InsertBulkOptions options = null, CancellationToken cancellationToken = default)
        {
            try
            {
                await OpenSharedConnectionInternalAsync(cancellationToken);
                await _dbType.InsertBulkAsync(this, pocos, options, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception x)
            {
                OnExceptionInternal(x);
                throw;
            }
            finally
            {
                await CloseSharedConnectionInternalAsync();
            }
        }

        /// <summary>
        /// 通过拼接 SQL 语句批量插入多个实体。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="pocos">要插入的实体集合。</param>
        /// <param name="options">批量选项。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> InsertBatchAsync<T>(IEnumerable<T> pocos, BatchOptions options = null, CancellationToken cancellationToken = default)
        {
            return InsertBatchAsyncImp(pocos, options, false, cancellationToken);
        }

        private async Task<int> InsertBatchAsyncImp<T>(IEnumerable<T> pocos, BatchOptions options, bool sync, CancellationToken cancellationToken = default)
        {
            options = options ?? new BatchOptions();
            var result = 0;

            try
            {
                if (sync) OpenSharedConnectionInternal();
                else await OpenSharedConnectionInternalAsync(cancellationToken);

                PocoData pd = null;

                foreach (var batchedPocos in pocos.Chunkify(options.BatchSize))
                {
                    var preparedInserts = batchedPocos.Select(x =>
                    {
                        if (pd == null) pd = PocoDataFactory.ForType(x.GetType());
                        return InsertStatements.PrepareInsertSql(this, pd, pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, x);
                    }).ToArray();

                    if (DatabaseType.SupportsMultiStatementBatch == false)
                    {
                        // 不支持多语句批处理（如 Firebird），降级为逐条执行
                        foreach (var preparedInsertSql in preparedInserts)
                        {
                            using (var cmd = CreateCommand(_sharedConnection, preparedInsertSql.Sql, preparedInsertSql.Rawvalues.ToArray()))
                            {
                                result += sync
                                    ? ExecuteNonQueryHelper(cmd)
                                    : await ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
                            }
                        }
                        continue;
                    }

                    var sql = new Sql();
                    foreach (var preparedInsertSql in preparedInserts)
                    {
                        sql.Append(preparedInsertSql.Sql + options.StatementSeperator, preparedInsertSql.Rawvalues.ToArray());
                    }

                    using (var cmd = CreateCommand(_sharedConnection, sql.SQL, sql.Arguments))
                    {
                        result += sync
                            ? ExecuteNonQueryHelper(cmd)
                            : await ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception x)
            {
                OnExceptionInternal(x);
                throw;
            }
            finally
            {
                if (sync) CloseSharedConnectionInternal();
                else await CloseSharedConnectionInternalAsync();
            }

            return result;
        }

        /// <summary>
        /// 按约定或配置更新实体，仅更新指定字段。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="poco">要更新的实体对象。</param>
        /// <param name="fields">指定要更新字段的表达式。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> UpdateAsync<T>(T poco, Expression<Func<T, object>> fields, CancellationToken cancellationToken = default)
        {
            var expression = DatabaseType.ExpressionVisitor<T>(this, PocoDataFactory.ForType(typeof(T)));
            expression = expression.Select(fields);
            var columnNames = expression.SelectMembers.Select(x => x.PocoColumn.ColumnName);
            var otherNames = expression.GeneralMembers.Select(x => x.PocoColumn.ColumnName);
            return UpdateAsync(poco, columnNames.Union(otherNames), cancellationToken);
        }

        /// <summary>
        /// 按约定或配置更新实体。
        /// </summary>
        /// <param name="poco">要更新的实体对象。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> UpdateAsync(object poco, CancellationToken cancellationToken = default)
        {
            return UpdateAsync(poco, null, null, cancellationToken);
        }

        /// <summary>
        /// 按约定或配置更新实体，并指定要更新的列。
        /// </summary>
        /// <param name="poco">要更新的实体对象。</param>
        /// <param name="columns">要更新的列名集合。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> UpdateAsync(object poco, IEnumerable<string> columns, CancellationToken cancellationToken = default)
        {
            return UpdateAsync(poco, null, columns, cancellationToken);
        }

        /// <summary>
        /// 按约定或配置更新实体，并指定主键值与要更新的列。
        /// </summary>
        /// <param name="poco">要更新的实体对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <param name="columns">要更新的列名集合。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> UpdateAsync(object poco, object primaryKeyValue, IEnumerable<string> columns, CancellationToken cancellationToken = default)
        {
            var tableInfo = PocoDataFactory.TableInfoForType(poco.GetType());
            return UpdateAsync(tableInfo.TableName, tableInfo.PrimaryKey, poco, primaryKeyValue, columns, cancellationToken);
        }

        /// <summary>
        /// 按指定表名与主键更新实体。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">要更新的实体对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <param name="columns">要更新的列名集合。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public virtual Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns, CancellationToken cancellationToken = default)
        {
            return UpdateImpAsync(tableName, primaryKeyName, poco, primaryKeyValue, columns, false, cancellationToken);
        }

        /// <summary>
        /// 通过拼接 SQL 语句批量更新多个实体。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="pocos">要更新的实体批量集合。</param>
        /// <param name="options">批量选项。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> UpdateBatchAsync<T>(IEnumerable<UpdateBatch<T>> pocos, BatchOptions options = null, CancellationToken cancellationToken = default)
        {
            return UpdateBatchAsyncImp(pocos, options, false, cancellationToken);
        }

        private async Task<int> UpdateBatchAsyncImp<T>(IEnumerable<UpdateBatch<T>> pocos, BatchOptions options, bool sync, CancellationToken cancellationToken = default)
        {
            options = options ?? new BatchOptions();
            int result = 0;

            try
            {
                if (sync) OpenSharedConnectionInternal();
                else await OpenSharedConnectionInternalAsync(cancellationToken);

                PocoData pd = null;

                foreach (var batchedPocos in pocos.Chunkify(options.BatchSize))
                {
                    var preparedUpdates = batchedPocos.Select(x =>
                    {
                        if (pd == null) pd = PocoDataFactory.ForType(x.Poco.GetType());
                        return UpdateStatements.PrepareUpdate(this, pd, pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, x.Poco, null, x.Snapshot?.UpdatedColumns());
                    }).ToArray();

                    var sql = new Sql();
                    foreach (var preparedUpdate in preparedUpdates)
                    {
                        if (preparedUpdate.Sql != null)
                        {
                            sql.Append(preparedUpdate.Sql + options.StatementSeperator, preparedUpdate.Rawvalues.ToArray());
                        }
                    }

                    using (var cmd = CreateCommand(_sharedConnection, sql.SQL, sql.Arguments))
                    {
                        result += sync
                            ? ExecuteNonQueryHelper(cmd)
                            : await ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception x)
            {
                OnExceptionInternal(x);
                throw;
            }
            finally
            {
                if (sync) CloseSharedConnectionInternal();
                else await CloseSharedConnectionInternalAsync();
            }

            return result;
        }

        /// <summary>
        /// 创建一个异步批量更新的查询提供程序。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <returns>异步更新查询提供程序。</returns>
        public IAsyncUpdateQueryProvider<T> UpdateManyAsync<T>()
        {
            return new AsyncUpdateQueryProvider<T>(this);
        }

        /// <summary>
        /// 按约定或配置删除实体。
        /// </summary>
        /// <param name="poco">要删除的实体对象。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> DeleteAsync(object poco, CancellationToken cancellationToken = default)
        {
            var tableInfo = PocoDataFactory.TableInfoForType(poco.GetType());
            return DeleteAsync(tableInfo.TableName, tableInfo.PrimaryKey, poco, cancellationToken);
        }

        /// <summary>
        /// 按指定表名与主键删除实体。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">要删除的实体对象。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> DeleteAsync(string tableName, string primaryKeyName, object poco, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(tableName, primaryKeyName, poco, null, cancellationToken);
        }

        /// <summary>
        /// 按指定表名、主键与主键值删除实体。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="primaryKeyName">主键列名。</param>
        /// <param name="poco">要删除的实体对象。</param>
        /// <param name="primaryKeyValue">主键值。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public virtual Task<int> DeleteAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue, CancellationToken cancellationToken = default)
        {
            return DeleteImpAsync(tableName, primaryKeyName, poco, primaryKeyValue, false, cancellationToken);
        }

        /// <summary>
        /// 创建一个异步批量删除的查询提供程序。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <returns>异步删除查询提供程序。</returns>
        public IAsyncDeleteQueryProvider<T> DeleteManyAsync<T>()
        {
            return new AsyncDeleteQueryProvider<T>(this);
        }

        /// <summary>
        /// 异步分页查询。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="itemsPerPage">每页条目数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>分页结果。</returns>
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, Sql sql, CancellationToken cancellationToken = default)
        {
            return PageAsync<T>(page, itemsPerPage, sql.SQL, sql.Arguments, cancellationToken);
        }

        /// <summary>
        /// 异步分页查询。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="itemsPerPage">每页条目数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>分页结果。</returns>
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sql, CancellationToken cancellationToken = default)
        {
            return PageAsync<T>(page, itemsPerPage, sql, [], cancellationToken);
        }

        /// <summary>
        /// 异步分页查询。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="itemsPerPage">每页条目数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>分页结果。</returns>
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sql, object[] args, CancellationToken cancellationToken = default)
        {
            return PageImpAsync<T>(page, itemsPerPage, sql, args, false, cancellationToken);
        }

        /// <summary>
        /// 异步分页获取数据列表。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="itemsPerPage">每页条目数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>记录列表。</returns>
        public Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, string sql, CancellationToken cancellationToken = default)
        {
            return FetchAsync<T>(page, itemsPerPage, sql, [], cancellationToken);
        }

        /// <summary>
        /// 异步分页获取数据列表。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="itemsPerPage">每页条目数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>记录列表。</returns>
        public Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, string sql, object[] args, CancellationToken cancellationToken = default)
        {
            return SkipTakeAsync<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args, cancellationToken);
        }

        /// <summary>
        /// 异步分页获取数据列表。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="page">页码（从 1 开始）。</param>
        /// <param name="itemsPerPage">每页条目数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>记录列表。</returns>
        public Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, Sql sql, CancellationToken cancellationToken = default)
        {
            return SkipTakeAsync<T>((page - 1) * itemsPerPage, itemsPerPage, sql.SQL, sql.Arguments, cancellationToken);
        }

        /// <summary>
        /// 异步跳过指定条数后获取指定数量的记录。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="skip">要跳过的记录数。</param>
        /// <param name="take">要获取的记录数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>记录列表。</returns>
        public Task<List<T>> SkipTakeAsync<T>(long skip, long take, string sql, CancellationToken cancellationToken = default)
        {
            return SkipTakeAsync<T>(skip, take, sql, [], cancellationToken);
        }

        /// <summary>
        /// 异步跳过指定条数后获取指定数量的记录。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="skip">要跳过的记录数。</param>
        /// <param name="take">要获取的记录数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>记录列表。</returns>
        public Task<List<T>> SkipTakeAsync<T>(long skip, long take, string sql, object[] args, CancellationToken cancellationToken = default)
        {
            string sqlCount, sqlPage;
            BuildPageQueries<T>(skip, take, sql, ref args, out sqlCount, out sqlPage);
            return FetchAsync<T>(sqlPage, args, cancellationToken);
        }

        /// <summary>
        /// 异步跳过指定条数后获取指定数量的记录。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="skip">要跳过的记录数。</param>
        /// <param name="take">要获取的记录数。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>记录列表。</returns>
        public Task<List<T>> SkipTakeAsync<T>(long skip, long take, Sql sql, CancellationToken cancellationToken = default)
        {
            return SkipTakeAsync<T>(skip, take, sql.SQL, sql.Arguments, cancellationToken);
        }

        /// <summary>Checks if a poco represents a new record.</summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="poco">要检查的实体对象。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>若为新记录返回 true，否则返回 false。</returns>
        public Task<bool> IsNewAsync<T>(T poco, CancellationToken cancellationToken = default)
        {
            return IsNewAsync(poco, false, cancellationToken);
        }

        /// <summary>
        /// 保存实体：若该对象为新记录则执行插入，否则执行更新。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="poco">要保存的实体对象。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        public async Task SaveAsync<T>(T poco, CancellationToken cancellationToken = default)
        {
            var tableInfo = PocoDataFactory.TableInfoForType(poco.GetType());
            if (await IsNewAsync(poco, cancellationToken).ConfigureAwait(false))
            {
                await InsertAsync(tableInfo.TableName, tableInfo.PrimaryKey, tableInfo.AutoIncrement, poco, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await UpdateAsync(tableInfo.TableName, tableInfo.PrimaryKey, poco, null, null, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<bool> PocoExistsAsync<T>(T poco, bool sync, CancellationToken cancellationToken = default)
        {
            var sql = GetExistsSql<T>(poco, true);
            var result = sync 
                ? ExecuteScalar<int>(sql)
                : await ExecuteScalarAsync<int>(sql, cancellationToken).ConfigureAwait(false);
            return result > 0;
        }

        private async Task<bool> ExistsAsync<T>(object primaryKey, bool sync, CancellationToken cancellationToken = default)
        {
            var sql = GetExistsSql<T>(primaryKey, false);
            var result = sync 
                ? ExecuteScalar<int>(sql)
                : await ExecuteScalarAsync<int>(sql, cancellationToken).ConfigureAwait(false);
            return result > 0;
        }

        private Sql GetExistsSql<T>(object primaryKeyOrPoco, bool isPoco)
        {
            var index = 0;
            var pd = PocoDataFactory.ForType(typeof(T));
            var primaryKeyValuePairs = GetPrimaryKeyValues(this, pd, pd.TableInfo.PrimaryKey, primaryKeyOrPoco, isPoco);
            var sql = string.Format(DatabaseType.GetExistsSql(), DatabaseType.EscapeTableName(pd.TableInfo.TableName), BuildPrimaryKeySql(this, primaryKeyValuePairs, ref index));
            var args = primaryKeyValuePairs.Select(x => x.Value).ToArray();
            return new Sql(sql, args);
        }

        /// <summary>
        /// 异步获取指定类型的所有记录。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>记录列表。</returns>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken = default)
        {
            return FetchAsync<T>("", cancellationToken);
        }

        /// <summary>
        /// 异步执行查询并返回实体列表。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>记录列表。</returns>
        public Task<List<T>> FetchAsync<T>(string sql, CancellationToken cancellationToken = default)
        {
            return FetchAsync<T>(sql, [], cancellationToken);
        }

        /// <summary>
        /// 异步执行查询并返回实体列表。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>记录列表。</returns>
        public Task<List<T>> FetchAsync<T>(string sql, object[] args, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(sql, args, cancellationToken).ToListAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步执行查询并返回实体列表。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>记录列表。</returns>
        public Task<List<T>> FetchAsync<T>(Sql sql, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(sql, cancellationToken).ToListAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步执行多结果集查询，并使用回调将各结果集映射为 TRet 类型。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">回调返回类型。</typeparam>
        /// <param name="cb">用于组合各结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>回调返回的结果。</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, TRet>(Func<List<T1>, List<T2>, TRet> cb, string sql, CancellationToken cancellationToken = default) => FetchMultipleAsync(cb, sql, [], default);
        /// <summary>
        /// 异步执行多结果集查询，并使用回调将各结果集映射为 TRet 类型。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">回调返回类型。</typeparam>
        /// <param name="cb">用于组合各结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>回调返回的结果。</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, TRet>(Func<List<T1>, List<T2>, TRet> cb, string sql, object[] args, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, DontMap, DontMap, TRet>(new[] { typeof(T1), typeof(T2) }, cb, new Sql(sql, args), false, cancellationToken); }
        /// <summary>
        /// 异步执行多结果集查询，并使用回调将各结果集映射为 TRet 类型。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">回调返回类型。</typeparam>
        /// <param name="cb">用于组合各结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>回调返回的结果。</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, T3, TRet>(Func<List<T1>, List<T2>, List<T3>, TRet> cb, string sql, CancellationToken cancellationToken = default) => FetchMultipleAsync(cb, sql, [], default);
        /// <summary>
        /// 异步执行多结果集查询，并使用回调将各结果集映射为 TRet 类型。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">回调返回类型。</typeparam>
        /// <param name="cb">用于组合各结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>回调返回的结果。</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, T3, TRet>(Func<List<T1>, List<T2>, List<T3>, TRet> cb, string sql, object[] args, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, T3, DontMap, TRet>(new[] { typeof(T1), typeof(T2), typeof(T3) }, cb, new Sql(sql, args), false, cancellationToken); }
        /// <summary>
        /// 异步执行多结果集查询，并使用回调将各结果集映射为 TRet 类型。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">回调返回类型。</typeparam>
        /// <param name="cb">用于组合各结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>回调返回的结果。</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, T3, T4, TRet>(Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> cb, string sql, CancellationToken cancellationToken = default) => FetchMultipleAsync(cb, sql, [], default);
        /// <summary>
        /// 异步执行多结果集查询，并使用回调将各结果集映射为 TRet 类型。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">回调返回类型。</typeparam>
        /// <param name="cb">用于组合各结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>回调返回的结果。</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, T3, T4, TRet>(Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> cb, string sql, object[] args, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, T3, T4, TRet>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, new Sql(sql, args), false, cancellationToken); }
        /// <summary>
        /// 异步执行多结果集查询，并使用回调将各结果集映射为 TRet 类型。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">回调返回类型。</typeparam>
        /// <param name="cb">用于组合各结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>回调返回的结果。</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, TRet>(Func<List<T1>, List<T2>, TRet> cb, Sql sql, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, DontMap, DontMap, TRet>(new[] { typeof(T1), typeof(T2) }, cb, sql, false, cancellationToken); }
        /// <summary>
        /// 异步执行多结果集查询，并使用回调将各结果集映射为 TRet 类型。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">回调返回类型。</typeparam>
        /// <param name="cb">用于组合各结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>回调返回的结果。</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, T3, TRet>(Func<List<T1>, List<T2>, List<T3>, TRet> cb, Sql sql, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, T3, DontMap, TRet>(new[] { typeof(T1), typeof(T2), typeof(T3) }, cb, sql, false, cancellationToken); }
        /// <summary>
        /// 异步执行多结果集查询，并使用回调将各结果集映射为 TRet 类型。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <typeparam name="TRet">回调返回类型。</typeparam>
        /// <param name="cb">用于组合各结果集的回调。</param>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>回调返回的结果。</returns>
        public Task<TRet> FetchMultipleAsync<T1, T2, T3, T4, TRet>(Func<List<T1>, List<T2>, List<T3>, List<T4>, TRet> cb, Sql sql, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, T3, T4, TRet>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, sql, false, cancellationToken); }

        /// <summary>
        /// 异步执行多结果集查询，并返回各结果集的元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>包含各结果集的元组。</returns>
        public Task<(List<T1>, List<T2>)> FetchMultipleAsync<T1, T2>(string sql, CancellationToken cancellationToken = default) => FetchMultipleAsync<T1, T2>(sql, [], cancellationToken);
        /// <summary>
        /// 异步执行多结果集查询，并返回各结果集的元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>包含各结果集的元组。</returns>
        public Task<(List<T1>, List<T2>)> FetchMultipleAsync<T1, T2>(string sql, object[] args, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, DontMap, DontMap, (List<T1>, List<T2>)>(new[] { typeof(T1), typeof(T2) }, new Func<List<T1>, List<T2>, (List<T1>, List<T2>)>((y, z) => (y, z)), new Sql(sql, args), false, cancellationToken); }
        /// <summary>
        /// 异步执行多结果集查询，并返回各结果集的元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>包含各结果集的元组。</returns>
        public Task<(List<T1>, List<T2>, List<T3>)> FetchMultipleAsync<T1, T2, T3>(string sql, CancellationToken cancellationToken = default) => FetchMultipleAsync<T1, T2, T3>(sql, [], cancellationToken);
        /// <summary>
        /// 异步执行多结果集查询，并返回各结果集的元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>包含各结果集的元组。</returns>
        public Task<(List<T1>, List<T2>, List<T3>)> FetchMultipleAsync<T1, T2, T3>(string sql, object[] args, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, T3, DontMap, (List<T1>, List<T2>, List<T3>)>(new[] { typeof(T1), typeof(T2), typeof(T3) }, new Func<List<T1>, List<T2>, List<T3>, (List<T1>, List<T2>, List<T3>)>((x, y, z) => (x, y, z)), new Sql(sql, args), false, cancellationToken); }
        /// <summary>
        /// 异步执行多结果集查询，并返回各结果集的元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>包含各结果集的元组。</returns>
        public Task<(List<T1>, List<T2>, List<T3>, List<T4>)> FetchMultipleAsync<T1, T2, T3, T4>(string sql, CancellationToken cancellationToken = default) => FetchMultipleAsync<T1, T2, T3, T4>(sql, [], cancellationToken);
        /// <summary>
        /// 异步执行多结果集查询，并返回各结果集的元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>包含各结果集的元组。</returns>
        public Task<(List<T1>, List<T2>, List<T3>, List<T4>)> FetchMultipleAsync<T1, T2, T3, T4>(string sql, object[] args, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, T3, T4, (List<T1>, List<T2>, List<T3>, List<T4>)>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, new Func<List<T1>, List<T2>, List<T3>, List<T4>, (List<T1>, List<T2>, List<T3>, List<T4>)>((w, x, y, z) => (w, x, y, z)), new Sql(sql, args), false, cancellationToken); }
        /// <summary>
        /// 异步执行多结果集查询，并返回各结果集的元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>包含各结果集的元组。</returns>
        public Task<(List<T1>, List<T2>)> FetchMultipleAsync<T1, T2>(Sql sql, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, DontMap, DontMap, (List<T1>, List<T2>)>(new[] { typeof(T1), typeof(T2) }, new Func<List<T1>, List<T2>, (List<T1>, List<T2>)>((y, z) => (y, z)), sql, false, cancellationToken); }
        /// <summary>
        /// 异步执行多结果集查询，并返回各结果集的元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>包含各结果集的元组。</returns>
        public Task<(List<T1>, List<T2>, List<T3>)> FetchMultipleAsync<T1, T2, T3>(Sql sql, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, T3, DontMap, (List<T1>, List<T2>, List<T3>)>(new[] { typeof(T1), typeof(T2), typeof(T3) }, new Func<List<T1>, List<T2>, List<T3>, (List<T1>, List<T2>, List<T3>)>((x, y, z) => (x, y, z)), sql, false, cancellationToken); }
        /// <summary>
        /// 异步执行多结果集查询，并返回各结果集的元组。
        /// </summary>
        /// <typeparam name="T1">第一个结果集元素类型。</typeparam>
        /// <typeparam name="T2">第二个结果集元素类型。</typeparam>
        /// <typeparam name="T3">第三个结果集元素类型。</typeparam>
        /// <typeparam name="T4">第四个结果集元素类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>包含各结果集的元组。</returns>
        public Task<(List<T1>, List<T2>, List<T3>, List<T4>)> FetchMultipleAsync<T1, T2, T3, T4>(Sql sql, CancellationToken cancellationToken = default) { return FetchMultipleImp<T1, T2, T3, T4, (List<T1>, List<T2>, List<T3>, List<T4>)>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, new Func<List<T1>, List<T2>, List<T3>, List<T4>, (List<T1>, List<T2>, List<T3>, List<T4>)>((w, x, y, z) => (w, x, y, z)), sql, false, cancellationToken); }

        /// <summary>
        /// 创建一个异步查询提供程序。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <returns>异步查询提供程序。</returns>
        public IAsyncQueryProviderWithIncludes<T> QueryAsync<T>()
        {
            return new AsyncQueryProvider<T>(this);
        }

        /// <summary>
        /// 异步执行查询并返回可异步枚举的结果序列。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>可异步枚举的结果序列。</returns>
        public IAsyncEnumerable<T> QueryAsync<T>(string sql, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(sql, [], cancellationToken);
        }

        /// <summary>
        /// 异步执行查询并返回可异步枚举的结果序列。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>可异步枚举的结果序列。</returns>
        public IAsyncEnumerable<T> QueryAsync<T>(string sql, object[] args, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(new Sql(sql, args), cancellationToken);
        }

        /// <summary>
        /// 异步执行查询并返回可异步枚举的结果序列。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>可异步枚举的结果序列。</returns>
        public IAsyncEnumerable<T> QueryAsync<T>(Sql sql, CancellationToken cancellationToken = default)
        {
            return QueryAsync(default(T), null, null, sql, null, cancellationToken);
        }

        internal async IAsyncEnumerable<T> QueryAsync<T>(T instance, Expression<Func<T, IList>> listExpression, Func<T, object[]> idFunc, Sql Sql, PocoData pocoData = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            pocoData ??= PocoDataFactory.ForType(typeof(T));

            var sql = Sql.SQL;
            var args = Sql.Arguments;

            if (EnableAutoSelect) sql = AutoSelectHelper.AddSelectClause(this, typeof(T), sql);

            try
            {
                await OpenSharedConnectionInternalAsync(cancellationToken);
                using var cmd = CreateCommand(_sharedConnection, sql, args); 
                using var reader = await ExecuteDataReader(cmd, false, cancellationToken).ConfigureAwait(false);
                var read = (listExpression != null ? ReadOneToManyAsync(instance, reader, listExpression, idFunc, pocoData, cancellationToken) : ReadAsync<T>(instance, reader, pocoData, cancellationToken));
                await foreach (var item in read.WithCancellation(cancellationToken))
                {
                    yield return item;
                }
            }
            finally
            {
                await CloseSharedConnectionInternalAsync();
            }
        }

        /// <summary>
        /// 异步执行查询并返回唯一一条记录。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>唯一一条记录。</returns>
        public Task<T> SingleAsync<T>(string sql, CancellationToken cancellationToken = default)
        {
            return SingleAsync<T>(sql, [], cancellationToken);
        }

        /// <summary>
        /// 异步执行查询并返回唯一一条记录。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>唯一一条记录。</returns>
        public Task<T> SingleAsync<T>(string sql, object[] args, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(sql, args, cancellationToken).SingleAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步执行查询并返回唯一一条记录。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>唯一一条记录。</returns>
        public Task<T> SingleAsync<T>(Sql sql, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(sql, cancellationToken).SingleAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步执行查询并返回唯一一条记录，无结果时返回默认值。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>唯一一条记录或默认值。</returns>
        public Task<T> SingleOrDefaultAsync<T>(string sql, CancellationToken cancellationToken = default)
        {
            return SingleOrDefaultAsync<T>(sql, [], cancellationToken);
        }

        /// <summary>
        /// 异步执行查询并返回唯一一条记录，无结果时返回默认值。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>唯一一条记录或默认值。</returns>
        public Task<T> SingleOrDefaultAsync<T>(string sql, object[] args, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(sql, args, cancellationToken).SingleOrDefaultAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步执行查询并返回唯一一条记录，无结果时返回默认值。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>唯一一条记录或默认值。</returns>
        public Task<T> SingleOrDefaultAsync<T>(Sql sql, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(sql, cancellationToken).SingleOrDefaultAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 按主键异步查询唯一一条记录。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="primaryKey">主键值。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>唯一一条记录。</returns>
        public Task<T> SingleByIdAsync<T>(object primaryKey, CancellationToken cancellationToken = default)
        {
            var sql = GenerateSingleByIdSql<T>(primaryKey);
            return QueryAsync<T>(sql, cancellationToken).SingleAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 按主键异步查询唯一一条记录，无结果时返回默认值。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="primaryKey">主键值。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>唯一一条记录或默认值。</returns>
        public Task<T> SingleOrDefaultByIdAsync<T>(object primaryKey, CancellationToken cancellationToken = default)
        {
            var sql = GenerateSingleByIdSql<T>(primaryKey);
            return QueryAsync<T>(sql, cancellationToken).SingleOrDefaultAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步执行查询并返回第一条记录。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>第一条记录。</returns>
        public Task<T> FirstAsync<T>(string sql, CancellationToken cancellationToken = default)
        {
            return FirstAsync<T>(sql, [], cancellationToken);
        }

        /// <summary>
        /// 异步执行查询并返回第一条记录。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>第一条记录。</returns>
        public Task<T> FirstAsync<T>(string sql, object[] args, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(sql, args, cancellationToken).FirstAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步执行查询并返回第一条记录。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>第一条记录。</returns>
        public Task<T> FirstAsync<T>(Sql sql, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(sql, cancellationToken).FirstAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步执行查询并返回第一条记录，无结果时返回默认值。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>第一条记录或默认值。</returns>
        public Task<T> FirstOrDefaultAsync<T>(string sql, CancellationToken cancellationToken = default)
        {
            return FirstOrDefaultAsync<T>(sql, [], cancellationToken);
        }

        /// <summary>
        /// 异步执行查询并返回第一条记录，无结果时返回默认值。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>第一条记录或默认值。</returns>
        public Task<T> FirstOrDefaultAsync<T>(string sql, object[] args, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(sql, args, cancellationToken).FirstOrDefaultAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步执行查询并返回第一条记录，无结果时返回默认值。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">查询 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>第一条记录或默认值。</returns>
        public Task<T> FirstOrDefaultAsync<T>(Sql sql, CancellationToken cancellationToken = default)
        {
            return QueryAsync<T>(sql, cancellationToken).FirstOrDefaultAsync(cancellationToken).AsTask();
        }

        /// <summary>
        /// 异步执行指定的 SQL 语句。
        /// </summary>
        /// <param name="sql">要执行的 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> ExecuteAsync(string sql, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(new Sql(sql), cancellationToken);
        }

        /// <summary>
        /// 异步执行指定的 SQL 语句与参数。
        /// </summary>
        /// <param name="sql">要执行的 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public Task<int> ExecuteAsync(string sql, object[] args, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(new Sql(sql, args), cancellationToken);
        }

        /// <summary>
        /// 异步执行指定的 SQL 语句。
        /// </summary>
        /// <param name="Sql">要执行的 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>受影响的行数。</returns>
        public async Task<int> ExecuteAsync(Sql Sql, CancellationToken cancellationToken = default)
        {
            var sql = Sql.SQL;
            var args = Sql.Arguments;

            try
            {
                await OpenSharedConnectionInternalAsync(cancellationToken);
                using var cmd = CreateCommand(_sharedConnection, sql, args);
                var result = await ExecuteNonQueryHelperAsync(cmd, cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (Exception x)
            {
                OnExceptionInternal(x);
                throw;
            }
            finally
            {
                await CloseSharedConnectionInternalAsync();
            }
        }

        /// <summary>
        /// 异步执行指定的 SQL 与参数，并将结果转换为 T 类型。
        /// </summary>
        /// <typeparam name="T">结果类型。</typeparam>
        /// <param name="sql">要执行的 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>转换后的结果。</returns>
        public Task<T> ExecuteScalarAsync<T>(string sql, CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync<T>(sql, [], cancellationToken);
        }

        /// <summary>
        /// 异步执行指定的 SQL 与参数，并将结果转换为 T 类型。
        /// </summary>
        /// <typeparam name="T">结果类型。</typeparam>
        /// <param name="sql">要执行的 SQL。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>转换后的结果。</returns>
        public Task<T> ExecuteScalarAsync<T>(string sql, object[] args, CancellationToken cancellationToken = default)
        {
            return ExecuteScalarAsync<T>(new Sql(sql, args), cancellationToken);
        }

        /// <summary>
        /// 异步执行指定的 SQL，并将结果转换为 T 类型。
        /// </summary>
        /// <typeparam name="T">结果类型。</typeparam>
        /// <param name="Sql">要执行的 SQL。</param>
        /// <param name="cancellationToken">用于取消操作的取消标记。</param>
        /// <returns>转换后的结果。</returns>
        public async Task<T> ExecuteScalarAsync<T>(Sql Sql, CancellationToken cancellationToken = default)
        {
            var sql = Sql.SQL;
            var args = Sql.Arguments;

            try
            {
                await OpenSharedConnectionInternalAsync(cancellationToken);
                using var cmd = CreateCommand(_sharedConnection, sql, args);
                object val = await ExecuteScalarHelperAsync(cmd, cancellationToken).ConfigureAwait(false);

                if (val == null || val == DBNull.Value)
                    return default(T);

                Type t = typeof(T);
                Type u = Nullable.GetUnderlyingType(t);

                return (T)Convert.ChangeType(val, u ?? t);
            }
            catch (Exception x)
            {
                OnExceptionInternal(x);
                throw;
            }
            finally
            {
                await CloseSharedConnectionInternalAsync();
            }
        }

        internal async Task<int> ExecuteNonQueryHelperAsync(DbCommand cmd, CancellationToken cancellationToken = default)
        {
            DoPreExecute(cmd);
            var result = await ExecutionHookAsync((ct) => _dbType.ExecuteNonQueryAsync(this, cmd, ct), cancellationToken).ConfigureAwait(false);
            OnExecutedCommandInternal(cmd);
            return result;
        }

        internal async Task<object> ExecuteScalarHelperAsync(DbCommand cmd, CancellationToken cancellationToken = default)
        {
            DoPreExecute(cmd);
            var result = await ExecutionHookAsync((ct) => _dbType.ExecuteScalarAsync(this, cmd, ct), cancellationToken).ConfigureAwait(false);
            OnExecutedCommandInternal(cmd);
            return result;
        }

        internal async Task<DbDataReader> ExecuteReaderHelperAsync(DbCommand cmd, CancellationToken cancellationToken = default)
        {
            DoPreExecute(cmd);
            var reader = await ExecutionHookAsync((ct) => _dbType.ExecuteReaderAsync(this, cmd, ct), cancellationToken).ConfigureAwait(false);
            OnExecutedCommandInternal(cmd);
            return reader;
        }

        /// <summary>
        /// 异步释放数据库实例占用的资源，关闭共享连接。
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (KeepConnectionAlive) return;
            await CloseSharedConnectionAsync();
        }
    }
}
