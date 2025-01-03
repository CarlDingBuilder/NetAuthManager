using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Common.Extended
{
    public static class ModelBuilderEx
    {
        /// <summary>
        /// 用 NUMBER(1) 代替 Boolean
        /// </summary>
        public static ModelBuilder UseNumberForBoolean(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // 遍历实体中的所有属性
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool) || property.ClrType == typeof(bool?))
                    {
                        var columnType = property.GetColumnType();
                        if (string.IsNullOrEmpty(columnType))
                        {
                            // 设置 SQL 列类型和值转换
                            property.SetColumnType("NUMBER(1)");
                            property.SetValueConverter(new BoolToZeroOneConverter<Int16>());
                        }
                    }
                }
            }

            return modelBuilder;
        }

        /// <summary>
        /// 用 DECIMAL 代替 Decimal, 默认是 NUMBER(18, 2)
        /// </summary>
        public static ModelBuilder UseDecimal(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // 遍历实体中的所有属性
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        var columnType = property.GetColumnType();
                        if (string.IsNullOrEmpty(columnType))
                        {
                            // 设置 SQL 列类型和值转换
                            property.SetColumnType("NUMBER(18, 2)");
                        }
                    }
                }
            }

            return modelBuilder;
        }
    }
}
