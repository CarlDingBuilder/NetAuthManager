using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetAuthManager.EntityFramework.Core
{
    internal sealed class SqlCommandProfilerInterceptor : DbCommandInterceptor
    {
        // 创建命令对象之前
        public override InterceptionResult<DbCommand> CommandCreating(CommandCorrelatedEventData eventData, InterceptionResult<DbCommand> result)
        {
            return base.CommandCreating(eventData, result);
        }

        // 创建命令对象之后
        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            return base.CommandCreated(eventData, result);
        }

        // 创建命令对象失败
        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            base.CommandFailed(command, eventData);
        }

        // 创建命令对象失败（异步）
        public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            return base.CommandFailedAsync(command, eventData, cancellationToken);
        }

        // 读取数据之前
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            return base.ReaderExecuting(command, eventData, result);
        }

        // 读取数据之前（异步）
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        // 读取数据之后
        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            return base.ReaderExecuted(command, eventData, result);
        }

        // 读取数据之后（异步）
        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        // DataReader 对象释放之前
        public override InterceptionResult DataReaderDisposing(DbCommand command, DataReaderDisposingEventData eventData, InterceptionResult result)
        {
            return base.DataReaderDisposing(command, eventData, result);
        }

        // 无查询执行 sql 之前
        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            return base.NonQueryExecuting(command, eventData, result);
        }

        // 无查询执行 sql 之前（异步）
        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        // 无查询执行 sql 之后
        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            return base.NonQueryExecuted(command, eventData, result);
        }

        // 无查询执行 sql 之后（异步）
        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        // 执行 sql 返回单行单列之前
        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            return base.ScalarExecuting(command, eventData, result);
        }

        // 执行 sql 返回单行单列之前（异步）
        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        // 执行 sql 返回单行单列之后
        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            return base.ScalarExecuted(command, eventData, result);
        }

        // 执行 sql 返回单行单列之后（异步）
        public override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
        {
            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }
    }
}
