/****************************************************
文件：TimerSvc.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/02 18:45:36
功能：定时服务
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSvc : MonoBehaviour
{
    public static TimerSvc Instance = null;

    private PETimer pt;

    public void InitSvc()
    {
        Instance = this;

        pt = new PETimer();

        // 设置日志输出
        pt.SetLog((info)=> {
            Common.Log(info);
        });

        Debug.Log(GetType() + "/InitSvc()/ Init Timer Service...");
               
    }

    private void Update()
    {
        pt.Update();
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit unit=PETimeUnit.Millisecond,int count = 1) {
        return pt.AddTimeTask(callback,delay,unit,count);
    }

    public double GetNowTime()
    {
        return pt.GetMillisecondsTime();
    }
}
