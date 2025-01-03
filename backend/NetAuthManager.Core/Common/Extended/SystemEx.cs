using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Common.Extended;

public static class SystemEx
{
    #region DataTable

    /// <summary>
    /// 转换为List
    /// </summary>
    public static List<T> ToList<T>(this DataTable dataTable) where T : class, new()
    {
        var propertys = typeof(T).GetProperties();

        var list = new List<T>();
        foreach (DataRow row in dataTable.Rows)
        {
            var model = new T();
            foreach (var property in propertys)
            {
                if (!property.CanWrite) continue;

                var key = property.Name;
                if (!dataTable.Columns.Contains(key)) continue;

                var value = row.GetObject(key);
                if (value == null) continue;

                if (property.PropertyType == value.GetType())
                    property.SetValue(model, value, null);
                else
                {
                    if (property.PropertyType.FullName.Contains("Nullable"))
                    {
                        property.SetValue(model, value, null);
                    }
                    else
                    {
                        var newValue = Convert.ChangeType(value, property.PropertyType);
                        property.SetValue(model, newValue, null);
                    }
                }
            }
            list.Add(model);
        }

        return list;
    }

    /// <summary>
    /// 转换为 List Hashtable
    /// </summary>
    public static List<Hashtable> ToListHashtable(this DataTable dataTable)
    {
        var list = new List<Hashtable>();
        foreach (DataRow row in dataTable.Rows)
        {
            var model = new Hashtable();
            foreach (DataColumn column in dataTable.Columns)
            {
                //空值也需要赋值
                var value = row.GetObject(column.ColumnName);
                model[column.ColumnName] = value;
            }
            list.Add(model);
        }

        return list;
    }

    #endregion DataTable

    #region DataRow

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static string GetString(this DataRow row, string name, string defaultValue)
    {
        var obj = row[name];
        if (obj != DBNull.Value && obj != null) return Convert.ToString(obj);
        return defaultValue;
    }

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static string GetString(this DataRow row, string name)
    {
        return row.GetString(name, string.Empty);
    }

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static int GetInt32(this DataRow row, string name, int defaultValue = -1)
    {
        var obj = row[name];
        if (obj != DBNull.Value && obj != null) return Convert.ToInt32(obj);
        return defaultValue;
    }

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static object GetObject(this DataRow row, string name, object defaultValue = null)
    {
        var obj = row[name];
        if (obj != DBNull.Value && obj != null) return obj;
        return defaultValue;
    }

    #endregion DataRow

    #region IDataReader

    public static T FormatTo<T>(this IDataReader reader) where T : class, new()
    {
        var model = new T();
        foreach (var property in typeof(T).GetProperties())
        {
            var key = property.Name;
            if (!property.CanWrite) continue;

            var columnIndex = -1;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i).Trim();
                if (key == columnName)
                {
                    columnIndex = i;
                    break;
                }
            }

            if (columnIndex < 0) continue;

            var value = reader.GetValue(columnIndex);
            if (value != null && value != DBNull.Value)
            {
                if (property.PropertyType == value.GetType())
                    property.SetValue(model, value, null);
                else
                {
                    if (property.PropertyType.FullName.Contains("Nullable"))
                    {
                        property.SetValue(model, value, null);
                    }
                    else if (property.PropertyType.IsEnum)
                    {
                        var newValue = Enum.Parse(property.PropertyType, value.ToString());
                        property.SetValue(model, newValue, null);
                    }
                    else
                    {
                        var newValue = Convert.ChangeType(value, property.PropertyType);
                        property.SetValue(model, newValue, null);
                    }
                }
            }
        }
        return model;
    }

    /// <summary>
    /// Reader 转换成 List<Hashtable>
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static List<Hashtable> FormatToListHashtable(this IDataReader reader)
    {
        var list = new List<Hashtable>();
        while (reader.Read())
        {
            list.Add(reader.FormatToHashtable());
        }
        return list;
    }

    public static JObject FormatToJObject(this IDataReader reader)
    {
        var model = new JObject();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            var columnName = reader.GetName(i).Trim();
            var value = reader.GetValue(i);
            if (value != null && value != DBNull.Value)
            {
                model[columnName] = JToken.FromObject(value);
            }
            else
            {
                model[columnName] = null;
            }
        }
        return model;
    }

    public static dynamic FormatToDynamic(this IDataReader reader)
    {
        var hashtable = new Hashtable();
        dynamic model = new ExpandoObject();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            var key = reader.GetName(i);
            var value = reader.GetValue(i);
            if (value == DBNull.Value)
                value = null;

            hashtable.Add(key, value);
        }
        return new DynamicHashtable(hashtable);
    }

    public static Hashtable FormatToHashtable(this IDataReader reader)
    {
        var hashtable = new Hashtable();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            var key = reader.GetName(i);
            var value = reader.GetValue(i);
            if (value == DBNull.Value)
                value = null;

            hashtable.Add(key, value);
        }
        return hashtable;
    }

    private sealed class DynamicHashtable : DynamicObject
    {
        private readonly Hashtable _hashtable;

        internal DynamicHashtable(Hashtable hashtable) { _hashtable = hashtable; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var retVal = _hashtable.ContainsKey(binder.Name);
            result = retVal ? _hashtable[binder.Name] : null;
            return retVal;
        }
    }

    #endregion IDataReader

    //#region IRow、ICell

    ///// <summary>
    ///// 获取行指定单元格值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellIndex"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static object GetValue(this IRow row, int cellIndex, Type type)
    //{
    //    if (row == null) return null;
    //    if (cellIndex < 0) throw new CustomMessageException("Error cell index");

    //    ICell cell = row.GetCell(cellIndex);

    //    if (type == null) type = typeof(string);
    //    if (type == typeof(string))
    //    {
    //        return GetStringTrimValue(cell, null);
    //    }
    //    else if (type == typeof(int))
    //    {
    //        return GetInt32Value(cell, 0);
    //    }
    //    else if (type == typeof(int?))
    //    {
    //        return GetInt32Value(cell, null);
    //    }
    //    else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
    //    {
    //        return GetDecimalValue(cell, 0);
    //    }
    //    else if (type == typeof(decimal?) || type == typeof(double?) || type == typeof(float?))
    //    {
    //        return GetDecimalValue(cell, null);
    //    }
    //    else if (type == typeof(DateTime))
    //    {
    //        return GetDateTimeValue(cell, DateTime.MinValue);
    //    }
    //    else if (type == typeof(DateTime?))
    //    {
    //        return GetDateTimeValue(cell, null);
    //    }
    //    else if (type == typeof(bool))
    //    {
    //        return GetBooleanValue(cell, false);
    //    }
    //    else if (type == typeof(bool?))
    //    {
    //        return GetBooleanValue(cell, null);
    //    }
    //    else
    //    {
    //        var value = GetStringTrimValue(cell);
    //        if (string.IsNullOrEmpty(value)) return null;
    //        return Convert.ChangeType(value, type);
    //    }
    //}

    ///// <summary>
    ///// 获取行指定单元格值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellIndex"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static string GetStringTrimValue(this IRow row, int cellIndex, string defaultValue = null)
    //{
    //    if (row == null) return defaultValue;
    //    if (cellIndex < 0) throw new CustomMessageException("Error cell index");

    //    ICell cell = row.GetCell(cellIndex);

    //    return GetStringTrimValue(cell, defaultValue);
    //}

    ///// <summary>
    ///// 获取单元格值
    ///// </summary>
    ///// <param name="cell"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static string GetStringTrimValue(this ICell cell, string defaultValue = null)
    //{
    //    if (cell == null)
    //        return defaultValue;

    //    switch (cell.CellType)
    //    {
    //        case CellType.String:
    //        case CellType.Formula:
    //            if (string.IsNullOrWhiteSpace(cell.StringCellValue))
    //                return defaultValue;
    //            else
    //                return cell.StringCellValue.Trim();
    //        case CellType.Numeric:
    //            return cell.NumericCellValue.ToString();
    //        case CellType.Boolean:
    //            return cell.BooleanCellValue.ToString();
    //        case CellType.Blank:
    //        default:
    //            return defaultValue;
    //    }
    //}

    ///// <summary>
    ///// 获取单元格值
    ///// </summary>
    ///// <param name="cell"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static int GetInt32Value(this ICell cell, int defaultValue = -1)
    //{
    //    if (cell == null)
    //        return defaultValue;

    //    switch (cell.CellType)
    //    {
    //        case CellType.String:
    //            if (string.IsNullOrWhiteSpace(cell.StringCellValue))
    //                return defaultValue;
    //            else
    //                return Int32.Parse(cell.StringCellValue.Trim());
    //        case CellType.Numeric:
    //        case CellType.Formula:
    //            return (int)cell.NumericCellValue;
    //        case CellType.Boolean:
    //        case CellType.Blank:
    //        default:
    //            return defaultValue;
    //    }
    //}

    ///// <summary>
    ///// 获取单元格值
    ///// </summary>
    ///// <param name="cell"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static int? GetInt32Value(this ICell cell, int? defaultValue = null)
    //{
    //    if (cell == null)
    //        return defaultValue;

    //    switch (cell.CellType)
    //    {
    //        case CellType.String:
    //            if (string.IsNullOrWhiteSpace(cell.StringCellValue))
    //                return defaultValue;
    //            else
    //                return Int32.Parse(cell.StringCellValue.Trim());
    //        case CellType.Numeric:
    //        case CellType.Formula:
    //            return (int)cell.NumericCellValue;
    //        case CellType.Boolean:
    //            return cell.BooleanCellValue ? 1 : 0;
    //        case CellType.Blank:
    //        default:
    //            return defaultValue;
    //    }
    //}

    ///// <summary>
    ///// 获取行指定单元格值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellIndex"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static decimal GetDecimalValue(this IRow row, int cellIndex, decimal defaultValue = 0)
    //{
    //    if (row == null) return defaultValue;
    //    if (cellIndex < 0) throw new CustomMessageException("Error cell index");

    //    ICell cell = row.GetCell(cellIndex);

    //    return GetDecimalValue(cell, defaultValue);
    //}

    ///// <summary>
    ///// 获取行指定单元格值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellIndex"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static decimal? GetDecimalValue(this IRow row, int cellIndex, decimal? defaultValue = null)
    //{
    //    if (row == null) return defaultValue;
    //    if (cellIndex < 0) throw new CustomMessageException("Error cell index");

    //    ICell cell = row.GetCell(cellIndex);

    //    return GetDecimalValue(cell, defaultValue);
    //}

    ///// <summary>
    ///// 获取单元格值
    ///// </summary>
    ///// <param name="cell"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static decimal GetDecimalValue(this ICell cell, decimal defaultValue = 0)
    //{
    //    if (cell == null)
    //        return defaultValue;

    //    switch (cell.CellType)
    //    {
    //        case CellType.String:
    //            if (string.IsNullOrWhiteSpace(cell.StringCellValue))
    //                return defaultValue;
    //            else
    //                return Convert.ToDecimal(cell.StringCellValue.Trim());
    //        case CellType.Numeric:
    //        case CellType.Formula:
    //            return Convert.ToDecimal(cell.NumericCellValue);
    //        case CellType.Boolean:
    //            return cell.BooleanCellValue ? 1 : 0;
    //        case CellType.Blank:
    //        default:
    //            return defaultValue;
    //    }
    //}

    ///// <summary>
    ///// 获取单元格值
    ///// </summary>
    ///// <param name="cell"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static decimal? GetDecimalValue(this ICell cell, decimal? defaultValue = null)
    //{
    //    if (cell == null)
    //        return defaultValue;

    //    switch (cell.CellType)
    //    {
    //        case CellType.String:
    //            if (string.IsNullOrWhiteSpace(cell.StringCellValue))
    //                return defaultValue;
    //            else
    //                return Convert.ToDecimal(cell.StringCellValue.Trim());
    //        case CellType.Numeric:
    //        case CellType.Formula:
    //            return Convert.ToDecimal(cell.NumericCellValue);
    //        case CellType.Boolean:
    //            return cell.BooleanCellValue ? 1 : 0;
    //        case CellType.Blank:
    //        default:
    //            return defaultValue;
    //    }
    //}

    ///// <summary>
    ///// 获取行指定单元格值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellIndex"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static DateTime GetDateTimeValue(this IRow row, int cellIndex, DateTime defaultValue)
    //{
    //    if (row == null) return defaultValue;
    //    if (cellIndex < 0) throw new CustomMessageException("Error cell index");

    //    ICell cell = row.GetCell(cellIndex);

    //    return GetDateTimeValue(cell, defaultValue);
    //}

    ///// <summary>
    ///// 获取行指定单元格值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellIndex"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static DateTime? GetDateTimeValue(this IRow row, int cellIndex, DateTime? defaultValue)
    //{
    //    if (row == null) return defaultValue;
    //    if (cellIndex < 0) throw new CustomMessageException("Error cell index");

    //    ICell cell = row.GetCell(cellIndex);

    //    return GetDateTimeValue(cell, defaultValue);
    //}

    ///// <summary>
    ///// 获取单元格值
    ///// </summary>
    ///// <param name="cell"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static DateTime GetDateTimeValue(this ICell cell, DateTime defaultValue)
    //{
    //    if (cell == null)
    //        return defaultValue;

    //    switch (cell.CellType)
    //    {
    //        case CellType.Numeric:
    //        case CellType.Formula:
    //            return cell.DateCellValue;
    //        case CellType.String:
    //            if (string.IsNullOrWhiteSpace(cell.StringCellValue))
    //                return defaultValue;
    //            else
    //                return Convert.ToDateTime(cell.StringCellValue.Trim(), new System.Globalization.DateTimeFormatInfo()
    //                {
    //                    ShortDatePattern = "yyyy-MM-dd"
    //                });
    //        case CellType.Boolean:
    //        case CellType.Blank:
    //        default:
    //            return defaultValue;
    //    }
    //}

    ///// <summary>
    ///// 获取单元格值
    ///// </summary>
    ///// <param name="cell"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static DateTime? GetDateTimeValue(this ICell cell, DateTime? defaultValue)
    //{
    //    if (cell == null)
    //        return defaultValue;

    //    switch (cell.CellType)
    //    {
    //        case CellType.Numeric:
    //        case CellType.Formula:
    //            return cell.DateCellValue;
    //        case CellType.String:
    //            if (string.IsNullOrWhiteSpace(cell.StringCellValue))
    //                return defaultValue;
    //            else
    //            {
    //                var cellValue = cell.StringCellValue;
    //                if (string.IsNullOrWhiteSpace(cellValue)) return defaultValue;
    //                else
    //                {
    //                    return Convert.ToDateTime(cellValue.Trim(), new System.Globalization.DateTimeFormatInfo()
    //                    {
    //                        ShortDatePattern = "yyyy-MM-dd",
    //                        LongDatePattern = "yyyy-MM-dd HH:mm:ss"
    //                    });
    //                }
    //            }
    //        case CellType.Boolean:
    //        case CellType.Blank:
    //        default:
    //            return defaultValue;
    //    }
    //}

    ///// <summary>
    ///// 获取单元格值
    ///// </summary>
    ///// <param name="cell"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static bool GetBooleanValue(this ICell cell, bool defaultValue)
    //{
    //    if (cell == null)
    //        return defaultValue;

    //    switch (cell.CellType)
    //    {
    //        case CellType.Numeric:
    //        case CellType.Formula:
    //            var value = Convert.ToInt32(cell.NumericCellValue);
    //            if (value == 1) return true;
    //            else return false;
    //        case CellType.String:
    //            var valueString = cell.StringCellValue.ToLower().Trim();
    //            switch (valueString)
    //            {
    //                case "1":
    //                case "true":
    //                case "是":
    //                    return true;
    //                case "0":
    //                case "false":
    //                case "否":
    //                    return false;
    //            }
    //            return false;
    //        case CellType.Boolean:
    //            return cell.BooleanCellValue;
    //        case CellType.Blank:
    //        default:
    //            return defaultValue;
    //    }
    //}

    ///// <summary>
    ///// 获取单元格值
    ///// </summary>
    ///// <param name="cell"></param>
    ///// <param name="defaultValue"></param>
    ///// <returns></returns>
    //public static bool? GetBooleanValue(this ICell cell, bool? defaultValue)
    //{
    //    if (cell == null)
    //        return defaultValue;

    //    switch (cell.CellType)
    //    {
    //        case CellType.Numeric:
    //        case CellType.Formula:
    //            var value = Convert.ToInt32(cell.NumericCellValue);
    //            if (value == 1) return true;
    //            if (value == 0) return false;
    //            else return null;
    //        case CellType.String:
    //            var valueString = cell.StringCellValue.ToLower().Trim();
    //            switch (valueString)
    //            {
    //                case "1":
    //                case "true":
    //                case "是":
    //                    return true;
    //                case "0":
    //                case "false":
    //                case "否":
    //                    return false;
    //            }
    //            return null;
    //        case CellType.Boolean:
    //            return cell.BooleanCellValue;
    //        case CellType.Blank:
    //        default:
    //            return defaultValue;
    //    }
    //}

    ///// <summary>
    ///// 设置日期值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static ICell SetCellDateValue(this IRow row, int cellNum, DateTime? value, string format = "yyyy-MM-dd")
    //{
    //    var cell = row.GetCell(cellNum);
    //    if (cell == null) cell = row.CreateCell(cellNum);

    //    if (value != null)
    //    {
    //        cell.SetCellValue(value.Value.Date);

    //        var dataFormat = row.Sheet.Workbook.CreateDataFormat();
    //        cell.CellStyle.DataFormat = dataFormat.GetFormat(format);
    //    }
    //    else
    //        cell.SetCellValue(string.Empty);
    //    return cell;
    //}

    ///// <summary>
    ///// 设置数值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static ICell SetCellDecimalValue(this IRow row, int cellNum, double? value, string format = null)
    //{
    //    var cell = row.GetCell(cellNum);
    //    if (cell == null) cell = row.CreateCell(cellNum);
    //    cell.SetCellType(CellType.Numeric);
    //    if (value != null)
    //    {
    //        cell.SetCellValue(value.Value);

    //        var dataFormat = row.Sheet.Workbook.CreateDataFormat();

    //        if (format == null)
    //        {
    //            if (value.Value > -1000 && value.Value < 1000) format = "0.00";
    //            else if (value.Value >= 1000 || value.Value <= -1000) format = "0,000.00";
    //            else format = "0,000,000.00";
    //        }

    //        var tempCellStyle = row.Sheet.Workbook.CreateCellStyle();
    //        if (cell.CellStyle != null)
    //        {
    //            tempCellStyle.CloneStyleFrom(cell.CellStyle);
    //        }
    //        tempCellStyle.DataFormat = dataFormat.GetFormat(format);
    //        tempCellStyle.IsLocked = true;
    //        cell.CellStyle = tempCellStyle;
    //    }
    //    else
    //        cell.SetCellValue(string.Empty);
    //    return cell;
    //}

    ///// <summary>
    ///// 设置数值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static ICell SetCellIntValue(this IRow row, int cellNum, int? value, string format = null)
    //{
    //    var cell = row.GetCell(cellNum);
    //    if (cell == null) cell = row.CreateCell(cellNum);
    //    cell.SetCellType(CellType.Numeric);
    //    if (value != null)
    //    {
    //        cell.SetCellValue(value.Value);

    //        var dataFormat = row.Sheet.Workbook.CreateDataFormat();
    //        cell.CellStyle.DataFormat = dataFormat.GetFormat("0");
    //    }
    //    else
    //        cell.SetCellValue(string.Empty);
    //    return cell;
    //}

    ///// <summary>
    ///// 设置值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static ICell SetCellStringValue(this IRow row, int cellNum, string value)
    //{
    //    var cell = row.GetCell(cellNum);
    //    if (cell == null) cell = row.CreateCell(cellNum);

    //    if (value != null)
    //        cell.SetCellValue(value);
    //    else
    //        cell.SetCellValue(string.Empty);

    //    return cell;
    //}

    ///// <summary>
    ///// 设置值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static ICell AppendCellStringValue(this IRow row, int cellNum, string value)
    //{
    //    var cell = row.GetCell(cellNum);
    //    if (cell != null)
    //    {
    //        cell.SetCellValue(cell.StringCellValue + value);
    //    }
    //    return cell;
    //}

    //#endregion IRow、ICell

    //#region ISheet

    ///// <summary>
    ///// 获取值
    ///// </summary>
    ///// <param name="sheet"></param>
    ///// <param name="rowNum">从 0 开始</param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static string GetCellStringValue(this ISheet sheet, int rowNum, int cellNum, string defaultValue = null)
    //{
    //    var row = sheet.GetRow(rowNum);
    //    if (row == null) return defaultValue;

    //    var cell = row.GetCell(cellNum);
    //    if (cell == null) return defaultValue;

    //    return cell.GetStringTrimValue(defaultValue);
    //}

    ///// <summary>
    ///// 获取值
    ///// </summary>
    ///// <param name="sheet"></param>
    ///// <param name="rowNum">从 0 开始</param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static string GetCellStringValue(this ISheet sheet, string index, string defaultValue = null)
    //{
    //    int cellNum, rowNum;
    //    GetRowCellNum(index, out cellNum, out rowNum);

    //    var row = sheet.GetRow(rowNum);
    //    if (row == null) return defaultValue;

    //    var cell = row.GetCell(cellNum);
    //    if (cell == null) return defaultValue;

    //    return cell.GetStringTrimValue(defaultValue);
    //}

    ///// <summary>
    ///// 获取值
    ///// </summary>
    ///// <param name="sheet"></param>
    ///// <param name="rowNum">从 0 开始</param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static object GetCellValue(this ISheet sheet, string index, Type type)
    //{
    //    int cellNum, rowNum;
    //    GetRowCellNum(index, out cellNum, out rowNum);

    //    var row = sheet.GetRow(rowNum);
    //    if (row == null) return null;

    //    var cell = row.GetCell(cellNum);
    //    if (cell == null) return null;

    //    if (type == typeof(string))
    //    {
    //        return GetStringTrimValue(cell, null);
    //    }
    //    else if (type == typeof(int))
    //    {
    //        return GetInt32Value(cell, 0);
    //    }
    //    else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
    //    {
    //        return GetDecimalValue(cell, 0);
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 设置值
    ///// </summary>
    ///// <param name="sheet"></param>
    ///// <param name="rowNum">从 0 开始</param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static void SetCellStringValue(this ISheet sheet, int rowNum, int cellNum, string value)
    //{
    //    var row = sheet.GetRow(rowNum);
    //    if (row == null) row = sheet.CreateRow(rowNum);

    //    var cell = row.GetCell(cellNum);
    //    if (cell == null) cell = row.CreateCell(cellNum);

    //    cell.SetCellValue(value);
    //}

    ///// <summary>
    ///// 设置值
    ///// </summary>
    ///// <param name="sheet"></param>
    ///// <param name="index">如：A1</param>
    ///// <param name="value"></param>
    //public static void SetCellStringValue(this ISheet sheet, string index, string value)
    //{
    //    int cellNum, rowNum;
    //    GetRowCellNum(index, out cellNum, out rowNum);
    //    SetCellStringValue(sheet, rowNum, cellNum, value);
    //}

    ///// <summary>
    ///// 设置值
    ///// </summary>
    ///// <param name="sheet"></param>
    ///// <param name="rowNum">从 0 开始</param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static void AppendCellStringValue(this ISheet sheet, int rowNum, int cellNum, string value)
    //{
    //    var row = sheet.GetRow(rowNum);
    //    if (row == null) row = sheet.CreateRow(rowNum);

    //    var cell = row.GetCell(cellNum);
    //    if (cell == null) cell = row.CreateCell(cellNum);

    //    cell.SetCellValue(cell.StringCellValue + value);
    //}

    ///// <summary>
    ///// 设置值
    ///// </summary>
    ///// <param name="sheet"></param>
    ///// <param name="index">如：A1</param>
    ///// <param name="value"></param>
    //public static void AppendCellStringValue(this ISheet sheet, string index, string value)
    //{
    //    int cellNum, rowNum;
    //    GetRowCellNum(index, out cellNum, out rowNum);
    //    AppendCellStringValue(sheet, rowNum, cellNum, value);
    //}

    ///// <summary>
    ///// 设置数值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static ICell SetCellDecimalValue(this ISheet sheet, string index, double? value, string format = null)
    //{
    //    int cellNum, rowNum;
    //    GetRowCellNum(index, out cellNum, out rowNum);

    //    return SetCellDecimalValue(sheet, rowNum, cellNum, value, format);
    //}

    ///// <summary>
    ///// 设置数值
    ///// </summary>
    ///// <param name="row"></param>
    ///// <param name="cellNum">从 0 开始</param>
    ///// <param name="value"></param>
    //public static ICell SetCellDecimalValue(this ISheet sheet, int rowNum, int cellNum, double? value, string format = null)
    //{
    //    var row = sheet.GetRow(rowNum);
    //    if (row == null) row = sheet.CreateRow(rowNum);

    //    return SetCellDecimalValue(row, cellNum, value, format);
    //}

    //#region 注释方法
    /////// <summary>
    /////// 设置值
    /////// </summary>
    /////// <param name="sheet"></param>
    /////// <param name="index">如：A1</param>
    /////// <param name="value"></param>
    ////public static void InsertImage(this ISheet sheet, string index, string fileId, int width, int height)
    ////{
    ////    var strMatch = Regex.Match(index, @"[^0-9]+");
    ////    var str = strMatch.Success ? strMatch.Value : string.Empty;
    ////    char[] chrs = str.ToUpper().ToCharArray(); // 转为大写字母组成的 char数组
    ////    int length = chrs.Length;
    ////    int cellNum = -1;
    ////    for (int i = 0; i < length; i++)
    ////    {
    ////        cellNum += (chrs[i] - 'A' + 1) * (int)Math.Pow(26, (length - i - 1)); // 当做26进制来算 AAA=111 26^2+26^1+26^0
    ////    }
    ////    var rowNum = Convert.ToInt32(Regex.Replace(index, @"[^0-9]+", "")) - 1;
    ////    CellImage(sheet, rowNum, cellNum, fileId, width, height);
    ////}

    /////// <summary>
    /////// 图片在单元格等比缩放居中显示
    /////// </summary>
    /////// <param name="cell">单元格</param>
    /////// <param name="value">图片二进制流</param>
    ////private static void CellImage(ISheet sheet, int rowNum, int cellNum, string fileId, int width, int height)
    ////{
    ////    var row = sheet.GetRow(rowNum);
    ////    if (row == null) return;

    ////    var cell = row.GetCell(cellNum);
    ////    if (cell == null) return;

    ////    var size = AttachmentHelper.GetImageSize(fileId, width, height);
    ////    var value = AttachmentHelper.GetImage(fileId,
    ////        YZSoftx.Services.REST.Attachment.AttachmentServiceBase.ScaleMode.Scale, ImageFormat.Png, width, height);

    ////    if (value.Length == 0) return;//空图片处理
    ////    double scalx = size.Width / width;//x轴缩放比例
    ////    double scaly = size.Height / height;//y轴缩放比例
    ////    int Dx1 = 0;//图片左边相对excel格的位置(x偏移) 范围值为:0~1023,超过1023就到右侧相邻的单元格里了
    ////    int Dy1 = 0;//图片上方相对excel格的位置(y偏移) 范围值为:0~256,超过256就到下方的单元格里了

    ////    ///计算单元格的长度和宽度
    ////    double cellWidth = 0;
    ////    double cellHeight = 0;

    ////    int rowSpanCount = 1;
    ////    int colSpanCount = 1;
    ////    for (int i = 0; i < sheet.NumMergedRegions; i++)
    ////    {
    ////        var range = sheet.GetMergedRegion(i);
    ////        //sheet.IsMergedRegion(range);
    ////        if (range.FirstRow == rowNum && range.FirstColumn == cellNum)
    ////        {
    ////            rowSpanCount = range.LastRow - range.FirstRow + 1;
    ////            colSpanCount = range.LastColumn - range.FirstColumn + 1;
    ////            break;
    ////        }
    ////    }

    ////    int j = 0;
    ////    for (j = 0; j < rowSpanCount; j++)//根据合并的行数计算出高度
    ////    {
    ////        cellHeight += cell.Sheet.GetRow(cell.RowIndex + j).Height;
    ////    }
    ////    for (j = 0; j < colSpanCount; j++)
    ////    {
    ////        cellWidth += cell.Row.Sheet.GetColumnWidth(cell.ColumnIndex + j);
    ////    }

    ////    //单元格长度和宽度与图片的长宽单位互换是根据实例得出
    ////    cellWidth = cellWidth / 23;
    ////    cellHeight = cellHeight / 12;

    ////    scalx = size.Width / cellWidth;//x轴缩放比例
    ////    scaly = size.Height / cellHeight;//y轴缩放比例

    ////    double imageScalWidth = 0;//缩放后显示在单元格上的图片长度
    ////    double imageScalHeight = 0;//缩放后显示在单元格上的图片宽度

    ////    imageScalWidth = scalx * cellWidth;
    ////    imageScalHeight = scaly * cellHeight;

    ////    Dx1 = Convert.ToInt32((cellWidth - imageScalWidth) / cellWidth * 1023 / 2);
    ////    Dy1 = Convert.ToInt32((cellHeight - imageScalHeight) / cellHeight * 256 / 2);
    ////    IClientAnchor anchor = cell.Sheet.Workbook.GetCreationHelper().CreateClientAnchor();
    ////    anchor.AnchorType = AnchorType.MoveDontResize;
    ////    anchor.Col1 = cell.ColumnIndex;
    ////    anchor.Col2 = cell.ColumnIndex + colSpanCount;
    ////    anchor.Row1 = cell.RowIndex;
    ////    anchor.Row2 = cell.RowIndex + rowSpanCount;
    ////    //anchor.Dy1 = Dy1;//图片下移量
    ////    //anchor.Dx1 = Dx1;//图片右移量，通过图片下移和右移，使得图片能居中显示，因为图片不同文字，图片是浮在单元格上的，文字是钳在单元格里的

    ////    int pictureIdx = sheet.Workbook.AddPicture(value, PictureType.PNG);
    ////    IDrawing patriarch = sheet.CreateDrawingPatriarch();
    ////    IPicture picture = patriarch.CreatePicture(anchor, pictureIdx);
    ////    picture.Resize(scalx, scaly);//等比缩放
    ////    picture.ClientAnchor.Dx1 = Dx1;
    ////    picture.ClientAnchor.Dy1 = Dy1;
    ////}

    //#endregion 注释方法

    ///// <summary>
    ///// 设置背景色
    ///// </summary>
    ///// <param name="sheet"></param>
    ///// <param name="index"></param>
    ///// <param name="color"></param>
    //public static void SetBackgroundColor(this ISheet sheet, string index, Color color)
    //{
    //    int cellNum, rowNum;
    //    GetRowCellNum(index, out cellNum, out rowNum);

    //    SetBackgroundColor(sheet, rowNum, cellNum, color);
    //}

    //public static void SetBackgroundColor(this ISheet sheet, int rowNum, int cellNum, Color color)
    //{
    //    var row = sheet.GetRow(rowNum);
    //    if (row == null) return;

    //    var cell = row.GetCell(cellNum);
    //    if (cell == null) return;

    //    if (cell is XSSFCell)
    //    {
    //        byte[] colorRgb = { color.R, color.G, color.B };
    //        XSSFColor hssFColor = new XSSFColor(colorRgb);
    //        hssFColor.SetRgb(colorRgb);

    //        var cellStyle = sheet.Workbook.CreateCellStyle();
    //        cellStyle.CloneStyleFrom(cell.CellStyle);
    //        cellStyle.FillPattern = FillPattern.SolidForeground;
    //        ((XSSFCellStyle)cellStyle).FillForegroundColorColor = hssFColor;
    //        cell.CellStyle = cellStyle;
    //        //cell.CellStyle.FillPattern = FillPattern.SolidForeground;
    //        //((XSSFCellStyle)cell.CellStyle).FillForegroundColorColor = hssFColor;
    //        //cell.CellStyle.FillForegroundColor = HSSFColor.SeaGreen.Index;
    //    }
    //    else if (cell is HSSFCell)
    //    {
    //        var palette = ((HSSFWorkbook)sheet.Workbook).GetCustomPalette();
    //        palette.SetColorAtIndex(8, color.R, color.G, color.B);
    //        HSSFColor hssFColor = palette.FindColor(color.R, color.G, color.B);
    //        cell.CellStyle.FillBackgroundColor = hssFColor.Indexed;
    //    }
    //}

    ///// <summary>
    ///// 根据坐标获取行号和列号
    ///// </summary>
    ///// <param name="index"></param>
    ///// <param name="cellNum"></param>
    ///// <param name="rowNum"></param>
    //private static void GetRowCellNum(string index, out int cellNum, out int rowNum)
    //{
    //    var strMatch = Regex.Match(index, @"[^0-9]+");
    //    var str = strMatch.Success ? strMatch.Value : string.Empty;
    //    char[] chrs = str.ToUpper().ToCharArray(); // 转为大写字母组成的 char数组
    //    int length = chrs.Length;
    //    int cellNumNo = -1;
    //    for (int i = 0; i < length; i++)
    //    {
    //        cellNumNo += (chrs[i] - 'A' + 1) * (int)Math.Pow(26, (length - i - 1)); // 当做26进制来算 AAA=111 26^2+26^1+26^0
    //    }
    //    var rowNumNo = Convert.ToInt32(Regex.Replace(index, @"[^0-9]+", "")) - 1;

    //    cellNum = cellNumNo;
    //    rowNum = rowNumNo;
    //}

    //#endregion ISheet

    #region JToken

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static int GetInt32(this JToken model, string name, int defaultValue = -1)
    {
        var obj = model[name];
        if (obj != null) return Convert.ToInt32(obj);
        return defaultValue;
    }

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static string GetString(this JToken model, string name, string defaultValue = null)
    {
        var obj = model[name];
        if (obj != null) return Convert.ToString(obj);
        return defaultValue;
    }

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static bool GetBool(this JToken model, string name, bool defaultValue)
    {
        var obj = model[name];
        if (obj != null) return Convert.ToBoolean(obj);
        return defaultValue;
    }

    #endregion JToken

    #region JObject

    /// <summary>
    /// 转换到Hashtable
    /// </summary>
    public static Hashtable FormatToHashtable(this JObject model, Hashtable hashtable = null)
    {
        if (hashtable == null) hashtable = new Hashtable();

        foreach (var item in model)
        {
            hashtable[item.Key] = item.Value;
        }
        return hashtable;
    }

    #endregion JObject

    #region Hashtable

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static int GetInt32(this Hashtable model, string name, int defaultValue = -1)
    {
        var obj = model[name];
        if (obj != null)
        {
            if (obj is int) return (int)obj;
            else
            {
                try
                {
                    return (int)Convert.ChangeType(obj, typeof(int));
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
        return defaultValue;
    }

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static int? GetInt32Null(this Hashtable model, string name, int? defaultValue = null)
    {
        var obj = model[name];
        if (obj != null)
        {
            if (obj is int) return (int)obj;
            else
            {
                try
                {
                    return (int)Convert.ChangeType(obj, typeof(int));
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
        return defaultValue;
    }

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static string GetString(this Hashtable model, string name, string defaultValue = null)
    {
        var obj = model[name];
        if (obj != null) return Convert.ToString(obj);
        return defaultValue;
    }

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static bool GetBool(this Hashtable model, string name, bool defaultValue = false)
    {
        var obj = model[name];
        if (obj != null)
        {
            if (obj is bool) return Convert.ToBoolean(obj);
            else return (bool)Convert.ChangeType(obj, typeof(bool));
        }
        return defaultValue;
    }

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static decimal GetDecimal(this Hashtable model, string name, decimal defaultValue = 0)
    {
        var obj = model[name];
        if (obj != null) return Convert.ToDecimal(obj);
        return defaultValue;
    }

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static DateTime? GetDateTimeNull(this Hashtable model, string name, DateTime? defaultValue = null)
    {
        var obj = model[name];
        if (obj != null) return Convert.ToDateTime(obj);
        return defaultValue;
    }

    /// <summary>
    /// 获取指定名称的 Key 的值
    /// </summary>
    public static DateTime GetDateTime(this Hashtable model, string name, DateTime defaultValue)
    {
        var obj = model[name];
        if (obj != null) return Convert.ToDateTime(obj);
        return defaultValue;
    }

    /// <summary>
    /// 格式化到对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="hashtable"></param>
    /// <returns></returns>
    public static T FormatTo<T>(this Hashtable hashtable) where T : class, new()
    {
        if (hashtable == null) return null;

        var model2 = new T();
        foreach (PropertyInfo property in typeof(T).GetProperties())
        {
            if (!property.CanWrite) continue;

            if (hashtable.ContainsKey(property.Name))
            {
                var value = hashtable[property.Name];
                if (value != null)
                {
                    if (property.PropertyType == value.GetType())
                        property.SetValue(model2, value, null);
                    else
                    {
                        if (property.PropertyType.FullName.Contains("Nullable"))
                        {
                            property.SetValue(model2, value, null);
                        }
                        else
                        {
                            var newValue = Convert.ChangeType(value, property.PropertyType);
                            property.SetValue(model2, newValue, null);
                        }
                    }
                }
            }
        }
        return model2;
    }

    /// <summary>
    /// 转换到字符串列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <param name="formatString"></param>
    /// <returns></returns>
    public static List<string> FormatHashtableToPropertyStringList(this Hashtable hashtable, string formatString, List<string> excludeColumns = null)
    {
        if (hashtable == null) return null;

        var list = new List<string>();
        foreach (var key in hashtable.Keys)
        {
            if (excludeColumns != null && excludeColumns.Count > 0)
            {
                if (excludeColumns.Contains(key)) continue;
            }
            list.Add(string.Format(formatString, key));
        }
        return list;
    }

    #endregion Hashtable

    #region T

    /// <summary>
    /// 类型转换
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="model"></param>
    /// <param name="caseInsensitive">不区分大小写，false：大小敏感，true：大小写不敏感</param>
    /// <returns></returns>
    public static TTo FormatTo<TFrom, TTo>(this TFrom model, bool caseInsensitive = false) where TTo : class, new()
    {
        if (model == null) return null;

        var model2 = new TTo();
        var model2Properties = typeof(TTo).GetProperties();
        foreach (PropertyInfo property in typeof(TFrom).GetProperties())
        {
            if (!property.CanWrite || !property.CanRead) continue;
            object value;
            try
            {
                value = property.GetValue(model);
            }
            catch (System.Reflection.TargetParameterCountException)
            {
                continue;
            }
            catch
            {
                throw;
            }
            if (value != null && value != DBNull.Value)
            {
                PropertyInfo propertyNew;
                if (caseInsensitive)
                {
                    propertyNew = model2Properties.FirstOrDefault(p => p.Name.ToUpper() == property.Name.ToUpper());
                }
                else
                {
                    propertyNew = model2Properties.FirstOrDefault(p => p.Name == property.Name);
                }
                if (propertyNew != null)
                {
                    if (propertyNew.PropertyType == value.GetType())
                        propertyNew.SetValue(model2, value, null);
                    else
                    {
                        if (propertyNew.PropertyType.FullName.Contains("Nullable"))
                        {
                            var genericTypeArguments = propertyNew.PropertyType.GenericTypeArguments;
                            if (genericTypeArguments.Length > 0)
                            {
                                var reallyType = genericTypeArguments[0];
                                if (reallyType == value.GetType())
                                {
                                    propertyNew.SetValue(model2, value, null);
                                }
                                else
                                {
                                    var newValue = Convert.ChangeType(value, reallyType);
                                    propertyNew.SetValue(model2, newValue, null);
                                }
                            }
                            else propertyNew.SetValue(model2, value, null);
                        }
                        else
                        {
                            var newValue = Convert.ChangeType(value, propertyNew.PropertyType);
                            propertyNew.SetValue(model2, newValue, null);
                        }
                    }
                }
            }
        }
        return model2;
    }

    /// <summary>
    /// 格式化到子对象
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    /// <param name="model">父对象</param>
    /// <returns></returns>
    public static TChild FormatToChild<TParent, TChild>(this TParent model) where TChild : class, TParent, new()
    {
        if (model == null) return null;

        var model2 = new TChild();
        foreach (PropertyInfo property in typeof(TParent).GetProperties())
        {
            if (!property.CanWrite || !property.CanRead) continue;
            var value = property.GetValue(model);
            property.SetValue(model2, value);
        }
        return model2;
    }

    /// <summary>
    /// 转换到字符串列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <param name="formatString"></param>
    /// <returns></returns>
    public static List<string> FormatToPropertyStringList<T>(this T model, string formatString, List<string> excludeColumns = null) where T : class, new()
    {
        if (model is Hashtable)
            return FormatHashtableToPropertyStringList(model as Hashtable, formatString, excludeColumns);

        if (model == null) return null;

        var list = new List<string>();
        foreach (PropertyInfo property in typeof(T).GetProperties())
        {
            if (!property.CanRead) continue;

            if (excludeColumns != null && excludeColumns.Count > 0)
            {
                if (excludeColumns.Contains(property.Name)) continue;
            }

            list.Add(string.Format(formatString, property.Name));
        }
        return list;
    }

    /// <summary>
    /// 转换到Hashtable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <param name="formatString"></param>
    /// <returns></returns>
    public static Hashtable FormatToHashtable<T>(this T model, Hashtable hashtable = null, List<string> ignoreColumns = null) where T : class, new()
    {
        if (hashtable == null) hashtable = new Hashtable();

        foreach (PropertyInfo property in typeof(T).GetProperties())
        {
            if (ignoreColumns != null && ignoreColumns.Contains(property.Name)) continue;
            if (!property.CanRead) continue;
            var value = property.GetValue(model);
            if (value != null && value != DBNull.Value)
            {
                hashtable[property.Name] = value;
            }
        }
        return hashtable;
    }

    /// <summary>
    /// 转换到字符串列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <param name="formatString"></param>
    /// <returns></returns>
    public static JObject FormatToJObject<T>(this T model, JObject hashtable = null) where T : class, new()
    {
        if (hashtable == null) hashtable = JObject.FromObject(model);
        else
        {
            var tempObj = JObject.FromObject(model);
            foreach (var tempItem in tempObj)
            {
                hashtable[tempItem.Key] = tempItem.Value;
            }
        }
        return hashtable;
    }

    /// <summary>
    /// 转换到字符串列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model"></param>
    /// <param name="formatString"></param>
    /// <returns></returns>
    public static JToken FormatToJToken<T>(this T model, JToken hashtable = null) where T : class, new()
    {
        if (hashtable == null) hashtable = JToken.FromObject(model);
        else
        {
            var tempObj = JObject.FromObject(model);
            foreach (var tempItem in tempObj)
            {
                hashtable[tempItem.Key] = tempItem.Value;
            }
        }
        return hashtable;
    }

    #endregion T

    //#region FlowDataRow

    ///// <summary>
    ///// 获取 bool 类型值
    ///// </summary>
    //public static bool GetBoolean(this FlowDataRow reader, string name, bool defaultValue)
    //{
    //    var obj = reader[name];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToBoolean(obj);
    //    return defaultValue;
    //}

    ///// <summary>
    ///// 获取 bool 类型值
    ///// </summary>
    //public static bool GetBoolean(this FlowDataRow reader, string name)
    //{
    //    return reader.GetBoolean(name, false);
    //}

    ///// <summary>
    ///// 获取 bool 类型值
    ///// </summary>
    //public static bool? GetBooleanNull(this FlowDataRow reader, string name, bool? defaultValue = null)
    //{
    //    var obj = reader[name];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToBoolean(obj);
    //    return defaultValue;
    //}

    ///// <summary>
    ///// 获取 string 类型值
    ///// </summary>
    //public static string GetString(this FlowDataRow reader, string name, string defaultValue)
    //{
    //    var obj = reader[name];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToString(obj);
    //    return defaultValue;
    //}
    ///// <summary>
    ///// 获取 string 类型值
    ///// </summary>
    //public static string GetString(this FlowDataRow reader, string name)
    //{
    //    return reader.GetString(name, string.Empty);
    //}
    ///// <summary>
    ///// 获取 Char 类型值
    ///// </summary>
    //public static char GetChar(this FlowDataRow reader, string name)
    //{
    //    var str = reader.GetString(name, string.Empty);
    //    return str[0];
    //}
    ///// <summary>
    ///// 获取 int 类型值
    ///// </summary>
    //public static int GetInt32(this FlowDataRow row, string name, int defaultValue = -1)
    //{
    //    if (row == null) return defaultValue;

    //    var obj = row[name];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToInt32(obj);
    //    return defaultValue;
    //}

    ///// <summary>
    ///// 获取 decimal 类型值
    ///// </summary>
    //public static decimal GetDecimal(this FlowDataRow row, string name, int defaultValue = -1)
    //{
    //    if (row == null) return defaultValue;

    //    var obj = row[name];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToDecimal(obj);
    //    return defaultValue;
    //}

    ///// <summary>
    ///// 获取 decimal 类型值
    ///// </summary>
    //public static decimal? GetDecimalNull(this FlowDataRow row, string name, decimal? defaultValue = null)
    //{
    //    if (row == null) return defaultValue;

    //    var obj = row[name];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToDecimal(obj);
    //    return defaultValue;
    //}

    ///// <summary>
    ///// 获取 double 类型值
    ///// </summary>
    //public static double GetDouble(this FlowDataRow row, string name, double defaultValue = 0)
    //{
    //    if (row == null) return defaultValue;

    //    var obj = row[name];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToDouble(obj);
    //    return defaultValue;
    //}

    ///// <summary>
    ///// 获取可空的 double 类型值
    ///// </summary>
    //public static double? GetDoubleNull(this FlowDataRow row, string name, double? defaultValue = null)
    //{
    //    if (row == null) return defaultValue;

    //    var obj = row[name];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToDouble(obj);
    //    return defaultValue;
    //}

    ///// <summary>
    ///// 获取可空的 DateTime 类型值
    ///// </summary>
    //public static DateTime GetDateTime(this FlowDataRow row, string name, DateTime? defaultValue = null)
    //{
    //    if (row == null)
    //    {
    //        if (defaultValue == null)
    //            throw new CustomMessageException($"无任何数据。");
    //        else return defaultValue.Value;
    //    }

    //    var obj = row[name];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToDateTime(obj);

    //    if (defaultValue == null)
    //        throw new CustomMessageException($"日期时间字段 {name} 不允许为空。");
    //    else return defaultValue.Value;
    //}

    ///// <summary>
    ///// 获取可空的 DateTime 类型值
    ///// </summary>
    //public static DateTime? GetDateTimeNull(this FlowDataRow row, string name, DateTime? defaultValue = null)
    //{
    //    if (row == null) return defaultValue;

    //    var obj = row[name];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToDateTime(obj);
    //    return defaultValue;
    //}

    ///// <summary>
    ///// 转换到指定模型
    ///// </summary>
    ///// <param name="defaultModel">默认模型</param>
    //public static T ToModel<T>(this FlowDataRow row, T defaultModel)
    //{
    //    Type type = typeof(T);
    //    PropertyInfo[] propertyInfo = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    //    foreach (PropertyInfo property in propertyInfo)
    //    {
    //        if (property.CanWrite)
    //        {
    //            if (property.MemberType == MemberTypes.Property || property.PropertyType.IsPublic)
    //            {
    //                var obj = row[property.Name];
    //                if (obj != null && obj != DBNull.Value)
    //                {
    //                    if (obj.GetType() == property.PropertyType)
    //                    {
    //                        property.SetValue(defaultModel, obj, null);
    //                    }
    //                    else
    //                    {
    //                        var objTemp = Convert.ChangeType(obj, property.PropertyType);
    //                        property.SetValue(defaultModel, objTemp, null);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    return defaultModel;
    //}

    ///// <summary>
    ///// 把 FlowDataRow 转成符合 DataTable 的数据行
    ///// </summary>
    //public static DataRow ToDataRow(this FlowDataRow flowDataRow, DataTable table)
    //{
    //    DataRow dataRow = table.NewRow();
    //    foreach (string key in flowDataRow.Keys)
    //    {
    //        object obj = flowDataRow[key];
    //        if (obj == null)
    //        {
    //            obj = DBNull.Value;
    //        }
    //        dataRow[key] = obj;
    //    }
    //    return dataRow;
    //}

    /////// <summary>
    /////// 转换到指定模型
    /////// </summary>
    /////// <param name="dataModel">数据对象</param>
    ////public static FlowDataRow FromModel<T>(this FlowDataRow row, T dataModel)
    ////{
    ////    if (dataModel == null) return row;

    ////    var columns = row.ParentTable.Columns;
    ////    Type type = typeof(T);
    ////    PropertyInfo[] propertyInfo = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    ////    foreach (PropertyInfo property in propertyInfo)
    ////    {
    ////        if (property.CanRead)
    ////        {
    ////            if (property.MemberType == MemberTypes.Property || property.PropertyType.IsPublic)
    ////            {
    ////                if (columns.Contains(property.Name))
    ////                {
    ////                    var obj = property.GetValue(dataModel);
    ////                    if (obj != null && obj != DBNull.Value)
    ////                    {
    ////                        row[property.Name] = obj;
    ////                    }
    ////                }
    ////            }
    ////        }
    ////    }
    ////    return row;
    ////}

    /////// <summary>
    /////// 转换到指定模型
    /////// </summary>
    /////// <param name="dataModel">数据对象</param>
    /////// <param name="isLimitColumn">是否按照数据对象限制列，如果限制则会删除其他列</param>
    ////public static FlowDataRow FromHashtable(this FlowDataRow row, IDictionary dataModel, bool isLimitColumn = false)
    ////{
    ////    if (dataModel == null) return row;

    ////    var columns = row.ParentTable.Columns;
    ////    foreach (string key in dataModel.Keys)
    ////    {
    ////        if (columns.Contains(key))
    ////        {
    ////            var obj = dataModel[key];
    ////            if (obj != null && obj != DBNull.Value)
    ////            {
    ////                row[key] = obj;
    ////            }
    ////        }
    ////    }

    ////    if (isLimitColumn)
    ////    {
    ////        for (var index = columns.Count - 1; index >= 0; index--)
    ////        {
    ////            var ccolumn = columns[index];
    ////            if (!dataModel.Contains(ccolumn.ColumnName))
    ////            {
    ////                row.ParentTable.Columns.Remove(ccolumn);
    ////                row.RemoveColumn(ccolumn.ColumnName);
    ////            }
    ////        }
    ////    }
    ////    return row;
    ////}

    ///// <summary>
    ///// 获取 object 类型值
    ///// </summary>
    //public static object GetObject(this FlowDataRow row, string fieldName, object defaultValue)
    //{
    //    var obj = row[fieldName];
    //    if (obj != DBNull.Value && obj != null) return obj;
    //    return defaultValue;
    //}

    ///// <summary>
    ///// 获取 object 类型值
    ///// </summary>
    //public static object GetObject(this FlowDataRow row, string fieldName)
    //{
    //    return row.GetObject(fieldName, null);
    //}

    ///// <summary>
    ///// 设置值
    ///// </summary>
    //public static void SetValue(this FlowDataRow row, string fieldName, object value)
    //{
    //    if (row.Contains(fieldName))
    //        row[fieldName] = value;
    //    else
    //        throw new CustomMessageException($"不存在字段 {fieldName}。");
    //}

    //#endregion FlowDataRow

    //#region FlowDataSet

    /////// <summary>
    /////// 获取 string 类型值
    /////// </summary>
    ////public static string GetString(this FlowDataSet reader, string fullName, string defaultValue)
    ////{
    ////    var obj = reader.[fullName];
    ////    if (obj != DBNull.Value && obj != null) return Convert.ToString(obj);
    ////    return defaultValue;
    ////}

    /////// <summary>
    /////// 获取 string 类型值
    /////// </summary>
    ////public static bool GetBoolean(this FlowDataSet reader, string fullName, bool defaultValue)
    ////{
    ////    var obj = reader[fullName];
    ////    if (obj != DBNull.Value && obj != null) return Convert.ToBoolean(obj);
    ////    return defaultValue;
    ////}

    /////// <summary>
    /////// 获取 string 类型值
    /////// </summary>
    ////public static string GetStringNotEmpty(this FlowDataSet reader, string fullName, string defaultValue)
    ////{
    ////    var obj = reader[fullName];
    ////    var value = string.Empty;
    ////    if (obj != DBNull.Value && obj != null) value = Convert.ToString(obj);
    ////    if (string.IsNullOrWhiteSpace(value))
    ////        value = defaultValue;
    ////    return value;
    ////}

    ///// <summary>
    ///// 获取 string 类型值
    ///// </summary>
    //public static string GetStringByField(this FlowDataSet reader, string tableName, string fieldName, string defaultValue)
    //{
    //    var table = reader.Tables[tableName];
    //    if (table == null || table.Rows.Count == 0) return defaultValue;

    //    var obj = table.Rows[0][fieldName];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToString(obj);
    //    return defaultValue;
    //}

    /////// <summary>
    /////// 获取 string 类型值
    /////// </summary>
    ////public static string GetString(this FlowDataSet reader, string fullName)
    ////{
    ////    return reader.GetString(fullName, string.Empty);
    ////}

    ///// <summary>
    ///// 获取 string 类型值
    ///// </summary>
    //public static string GetStringByField(this FlowDataSet reader, string tableName, string fieldName)
    //{
    //    return reader.GetStringByField(tableName, fieldName, string.Empty);
    //}

    /////// <summary>
    /////// 获取 string 类型值
    /////// </summary>
    ////public static DateTime GetDateTime(this FlowDataSet reader, string fullName, DateTime defaultValue)
    ////{
    ////    var obj = reader[fullName];
    ////    if (obj != DBNull.Value && obj != null) return Convert.ToDateTime(obj);
    ////    return defaultValue;
    ////}

    ///// <summary>
    ///// 获取 string 类型值
    ///// </summary>
    //public static DateTime GetDateTimeByField(this FlowDataSet reader, string tableName, string fieldName, DateTime defaultValue)
    //{
    //    var table = reader.Tables[tableName];
    //    if (table == null || table.Rows.Count == 0) return defaultValue;

    //    var obj = table.Rows[0][fieldName];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToDateTime(obj);
    //    return defaultValue;
    //}

    /////// <summary>
    /////// 获取 string 类型值
    /////// </summary>
    ////public static DateTime? GetDateTime(this FlowDataSet reader, string fullName)
    ////{
    ////    var obj = reader[fullName];
    ////    if (obj != DBNull.Value && obj != null) return Convert.ToDateTime(obj);
    ////    return null;
    ////}

    ///// <summary>
    ///// 获取 string 类型值
    ///// </summary>
    //public static DateTime? GetDateTimeByField(this FlowDataSet reader, string tableName, string fieldName)
    //{
    //    var table = reader.Tables[tableName];
    //    if (table == null || table.Rows.Count == 0) return null;

    //    var obj = table.Rows[0][fieldName];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToDateTime(obj);
    //    return null;
    //}

    ///// <summary>
    ///// 获取 string 类型值
    ///// </summary>
    //public static int GetIntByField(this FlowDataSet reader, string tableName, string fieldName, int defaultValue)
    //{
    //    var table = reader.Tables[tableName];
    //    if (table == null || table.Rows.Count == 0) return defaultValue;

    //    var obj = table.Rows[0][fieldName];
    //    if (obj != DBNull.Value && obj != null) return Convert.ToInt32(obj);
    //    return defaultValue;
    //}

    ///// <summary>
    ///// 获取 string 类型值
    ///// </summary>
    //public static int GetIntByField(this FlowDataSet reader, string tableName, string fieldName)
    //{
    //    return reader.GetIntByField(tableName, fieldName, -1);
    //}

    /////// <summary>
    /////// 获取 object 类型值
    /////// </summary>
    ////public static object GetObject(this FlowDataSet reader, string fullName, object defaultValue)
    ////{
    ////    var obj = reader[fullName];
    ////    if (obj != DBNull.Value && obj != null) return obj;
    ////    return defaultValue;
    ////}

    ///// <summary>
    ///// 获取 object 类型值
    ///// </summary>
    //public static object GetObject(this FlowDataSet reader, string fullName)
    //{
    //    return reader.GetObject(fullName, null);
    //}

    ///// <summary>
    ///// 获取 object 类型值
    ///// </summary>
    //public static object GetObject(this FlowDataSet reader, string tableName, string fieldName, object defaultValue)
    //{
    //    var table = reader.Tables[tableName];
    //    if (table == null || table.Rows.Count == 0) return defaultValue;

    //    var obj = table.Rows[0][fieldName];
    //    if (obj != DBNull.Value && obj != null) return obj;
    //    return defaultValue;
    //}

    ///// <summary>
    ///// 获取 object 类型值
    ///// </summary>
    //public static object GetObject(this FlowDataSet reader, string tableName, string fieldName)
    //{
    //    return reader.GetObject(tableName, fieldName, null);
    //}

    ///// <summary>
    ///// 获取 string 类型值列表
    ///// </summary>
    //public static List<string> GetStringListByField(this FlowDataSet reader, string tableName, string fieldName)
    //{
    //    var list = new List<string>();
    //    var table = reader.Tables[tableName];
    //    if (table == null || table.Rows.Count == 0) return list;

    //    foreach (FlowDataRow row in table.Rows)
    //    {
    //        if (row[fieldName] is string) list.Add(row[fieldName] as string);
    //        else list.Add((string)Convert.ChangeType(row[fieldName], typeof(string)));
    //    }
    //    return list;
    //}

    /////// <summary>
    /////// 获取表第一行数据
    /////// </summary>
    /////// <param name="table"></param>
    /////// <param name="isShouldNew">不存在数据时是否新增行</param>
    /////// <returns></returns>
    ////public static FlowDataRow GetFirstFlowDataRow(this FlowDataSet dataSet, string tableName, bool isShouldNew = true)
    ////{
    ////    var table = dataSet.GetFlowDataTable(tableName);
    ////    return table.GetFirstFlowDataRow(isShouldNew);
    ////}

    ///// <summary>
    ///// 获取表
    ///// </summary>
    ///// <param name="dataSet"></param>
    ///// <param name="tableName"></param>
    ///// <returns></returns>
    //public static FlowDataTable GetFlowDataTable(this FlowDataSet dataSet, string tableName)
    //{
    //    var table = dataSet.Tables[tableName];
    //    if (table == null)
    //        throw new CustomMessageException($"当前表单中不存在表 {table.TableName}。");

    //    return table;
    //}

    //#endregion
}
