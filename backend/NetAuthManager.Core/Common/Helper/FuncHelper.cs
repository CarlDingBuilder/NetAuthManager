using BPM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Helper;

/// <summary>
/// 方法帮助
/// </summary>
public class FuncHelper
{
    /// <summary>
    /// 无返回值的方法委托
    /// </summary>
    public delegate void TryDoDelegate();
    public delegate void TryDoErrorDelegate(Exception exception, bool isDebug);
    public delegate T TryDoErrorReturnDelegate<T>(Exception exception, bool isDebug);

    /// <summary>
    /// 尝试做某事并返回结果，出错时返回友好错误
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="funcDo">做某事</param>
    /// <param name="isDebug">是否调试</param>
    /// <returns>返回值</returns>
    public static T TryDoReturn<T>(Func<T> funcDo, bool isDebug = false)
    {
        try
        {
            return funcDo();
        }
        catch (CustomException exception)
        {
            if (isDebug)
                throw new CustomException(exception.ToString());
            else
                throw;
        }
        catch (Exception exception)
        {
            if (isDebug)
                throw new CustomException(exception.ToString());
            else
                throw new CustomException(exception.Message);
        }
    }

    /// <summary>
    /// 尝试做某事并返回结果，出错时做某事
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="funcDo">做某事</param>
    /// <param name="errorDo">错误时执行</param>
    /// <param name="isDebug">是否调试</param>
    /// <returns>返回值</returns>
    public static T TryDoReturn<T>(Func<T> funcDo, TryDoErrorReturnDelegate<T> errorDo, bool isDebug = false)
    {
        try
        {
            return funcDo();
        }
        catch (CustomException exception)
        {
            return errorDo.Invoke(exception, isDebug);
        }
        catch (Exception exception)
        {
            CustomException error;
            if (isDebug)
                error = new CustomException(exception.ToString());
            else
                error = new CustomException(exception.Message);
            return errorDo.Invoke(error, isDebug);
        }
    }

    /// <summary>
    /// 尝试做某事，出错时返回友好错误
    /// </summary>
    /// <param name="funcDo">做某事</param>
    /// <param name="isDebug">是否调试</param>
    /// <returns>返回值</returns>
    public static void TryDo(TryDoDelegate funcDo, bool isDebug = false)
    {
        try
        {
            funcDo();
        }
        catch (CustomException exception)
        {
            if (isDebug)
                throw new CustomException(exception.ToString());
            else
                throw;
        }
        catch (Exception exception)
        {
            if(isDebug)
                throw new CustomException(exception.ToString());
            else
                throw new CustomException(exception.Message);
        }
    }

    /// <summary>
    /// 尝试做某事，出错时做某事
    /// </summary>
    /// <param name="funcDo">做某事</param>
    /// <param name="errorDo">错误时执行</param>
    /// <param name="isDebug">是否调试</param>
    /// <returns>返回值</returns>
    public static void TryDo(TryDoDelegate funcDo, TryDoErrorDelegate errorDo, bool isDebug = false)
    {
        try
        {
            funcDo();
        }
        catch (CustomException exception)
        {
            errorDo(exception, isDebug);
        }
        catch (Exception exception)
        {
            errorDo(exception, isDebug);
        }
    }
}
