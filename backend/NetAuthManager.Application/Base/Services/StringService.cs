using Furion.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Services;

/// <summary>
/// 字符串帮助类
/// </summary>
public class StringService: ITransient
{
    public bool EquName(string str1, string str2)
    {
        if (String.Compare(str1, str2, true) == 0)
            return true;
        else
            return false;
    }

    public string DateToString(DateTime date)
    {
        return DateToString(date, null);
    }

    public string DateToString(DateTime date, string nullString)
    {
        if (date == DateTime.MinValue)
            return nullString;
        else
            return date.ToString("yyyy-MM-dd", new System.Globalization.CultureInfo(1033));
    }

    public string DateToStringH(DateTime date)
    {
        return DateToStringH(date, null);
    }

    public string DateToStringH(DateTime date, string nullString)
    {
        if (date == DateTime.MinValue)
            return nullString;
        else
            return date.ToString("yyyy-MM-dd HH", new System.Globalization.CultureInfo(1033));
    }

    public string DateToStringM(DateTime date)
    {
        return DateToStringM(date, null);
    }

    public string DateToStringM(DateTime date, string nullString)
    {
        if (date == DateTime.MinValue)
            return nullString;
        else
            return date.ToString("yyyy-MM-dd HH:mm", new System.Globalization.CultureInfo(1033));
    }

    public string DateToStringL(DateTime date)
    {
        return DateToStringL(date, null);
    }

    public string DateToStringL(DateTime date, string nullString)
    {
        if (date == DateTime.MinValue)
            return nullString;
        else
            return date.ToString("yyyy-MM-dd HH:mm:ss", new System.Globalization.CultureInfo(1033));
    }

    /// <summary>
    /// 用户短名称
    /// </summary>
    /// <param name="account"></param>
    /// <param name="displayName"></param>
    /// <returns></returns>
    public string GetUserShortName(string account, string displayName)
    {
        if (String.IsNullOrEmpty(displayName))
            return account;
        else
            return displayName;
    }

    public string GetUserFriendlyName(string account, string displayName)
    {
        if (String.IsNullOrEmpty(account)) //共享任务，Account可能为空
            return "";

        if (String.IsNullOrEmpty(displayName))
            return account;
        else
            return displayName + "(" + account + ")";
    }

    /// <summary>
    /// 流程短名称
    /// </summary>
    public string GetProcessDefaultShortName(string processName)
    {
        if (String.IsNullOrEmpty(processName))
            return "";

        return processName.Substring(0, Math.Min(2, processName.Length));
    }

    public bool IsNumber(string strVar)
    {
        int count = strVar.Length;
        for (int i = 0; i < count; i++)
        {
            char ch = strVar[i];
            if (ch < '0' || ch > '9')
                return false;
        }

        return true;
    }
}
