using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Helper;

public class MonitorHelper
{
    private static object LockCheckObj = new object();         //用于获取锁对象的锁对象
    private static Hashtable LockHashtable = new Hashtable();  //用于存放锁对象

    /// <summary>
    /// 锁定对象
    /// </summary>
    private object LockObject { get; set; }

    /// <summary>
    /// 是否会超时
    /// </summary>
    private bool CanWaitTimeout { get; set; }

    /// <summary>
    /// 等待秒数
    /// </summary>
    private int MillisecondsTimeout { get; set; }

    /// <summary>
    /// 不等待返回消息
    /// </summary>
    private string NoWaitMessage { get; set; }

    /// <summary>
    /// 初始化一个锁定
    /// </summary>
    /// <param name="lockGlobalKey"></param>
    /// <param name="canWaitTimeout">是否会超时</param>
    /// <param name="secondsTimeout">超时秒数</param>
    /// <param name="noWaitMessage">不等待返回消息</param>
    public MonitorHelper(string lockGlobalKey, bool canWaitTimeout = false, int secondsTimeout = 2, string noWaitMessage = null)
    {
        this.CanWaitTimeout = canWaitTimeout;
        this.MillisecondsTimeout = secondsTimeout * 1000;
        this.NoWaitMessage = noWaitMessage;

        //获取锁对象
        try
        {
            //Console.WriteLine("**************************************************");
            //Console.WriteLine("获取锁：" + lockGlobalKey);
            //Console.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Monitor.Enter(LockCheckObj);
            if (LockHashtable.ContainsKey(lockGlobalKey))
                LockObject = LockHashtable[lockGlobalKey];
            else
            {
                LockObject = new object();
                LockHashtable[lockGlobalKey] = LockObject;
            }
        }
        catch (Exception exception)
        {
            throw new Exception($"获取锁 {lockGlobalKey} 失败！", exception);
        }
        finally
        {
            Monitor.Exit(LockCheckObj);

            //Console.WriteLine("获取锁结束：" + lockGlobalKey);
            //Console.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //Console.WriteLine("**************************************************");
        }
    }

    /// <summary>
    /// 尝试做某事并返回结果，出错时返回友好错误
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="funcDo">做某事</param>
    /// <param name="isDebug">是否调试</param>
    /// <returns>返回值</returns>
    public T TryDoReturn<T>(Func<T> funcDo, bool isDebug = false)
    {
        //是否需要退出
        var shouldExit = true;
        try
        {
            try
            {
                if (this.CanWaitTimeout)
                {
                    //使用获取到的锁对象
                    if (!Monitor.TryEnter(LockObject, this.MillisecondsTimeout))
                        throw new Exception(this.NoWaitMessage ?? "数据或操作正在被进行中，请稍后刷新数据后尝试！");
                }
                else
                {
                    //使用获取到的锁对象
                    Monitor.Enter(LockObject);
                }
            }
            catch
            {
                shouldExit = false;
                throw;
            }

            return FuncHelper.TryDoReturn(funcDo, isDebug);
        }
        finally
        {
            if (shouldExit) Monitor.Exit(LockObject);
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
    public T TryDoReturn<T>(Func<T> funcDo, FuncHelper.TryDoErrorReturnDelegate<T> errorDo, bool isDebug = false)
    {
        //是否需要退出
        var shouldExit = true;
        try
        {
            try
            {
                if (this.CanWaitTimeout)
                {
                    //使用获取到的锁对象
                    if (!Monitor.TryEnter(LockObject, this.MillisecondsTimeout))
                        throw new Exception(this.NoWaitMessage ?? "数据或操作正在被进行中，请稍后刷新数据后尝试！");
                }
                else
                {
                    //使用获取到的锁对象
                    Monitor.Enter(LockObject);
                }
            }
            catch
            {
                shouldExit = false;
                throw;
            }

            return FuncHelper.TryDoReturn(funcDo, errorDo, isDebug);
        }
        finally
        {
            if (shouldExit) Monitor.Exit(LockObject);
        }
    }

    /// <summary>
    /// 尝试做某事，出错时返回友好错误
    /// </summary>
    /// <param name="funcDo">做某事</param>
    /// <param name="isDebug">是否调试</param>
    /// <returns>返回值</returns>
    public void TryDo(FuncHelper.TryDoDelegate funcDo, bool isDebug = false)
    {
        //是否需要退出
        var shouldExit = true;
        try
        {
            try
            {
                if (this.CanWaitTimeout)
                {
                    //使用获取到的锁对象
                    if (!Monitor.TryEnter(LockObject, this.MillisecondsTimeout))
                        throw new Exception(this.NoWaitMessage ?? "数据或操作正在被进行中，请稍后刷新数据后尝试！");
                }
                else
                {
                    //使用获取到的锁对象
                    Monitor.Enter(LockObject);
                }
            }
            catch
            {
                shouldExit = false;
                throw;
            }

            FuncHelper.TryDo(funcDo, isDebug);
        }
        finally
        {
            if(shouldExit) Monitor.Exit(LockObject);
        }
    }

    /// <summary>
    /// 尝试做某事，出错时做某事
    /// </summary>
    /// <param name="funcDo">做某事</param>
    /// <param name="errorDo">错误时执行</param>
    /// <param name="isDebug">是否调试</param>
    /// <returns>返回值</returns>
    public void TryDo(FuncHelper.TryDoDelegate funcDo, FuncHelper.TryDoErrorDelegate errorDo, bool isDebug = false)
    {
        //是否需要退出
        var shouldExit = true;
        try
        {
            try
            {
                if (this.CanWaitTimeout)
                {
                    //使用获取到的锁对象
                    if (!Monitor.TryEnter(LockObject, this.MillisecondsTimeout))
                        throw new Exception(this.NoWaitMessage ?? "数据或操作正在被进行中，请稍后刷新数据后尝试！");
                }
                else
                {
                    //使用获取到的锁对象
                    Monitor.Enter(LockObject);
                }
            }
            catch
            {
                shouldExit = false;
                throw;
            }

            FuncHelper.TryDo(funcDo, errorDo, isDebug);
        }
        finally
        {
            if (shouldExit) Monitor.Exit(LockObject);
        }
    }
}
