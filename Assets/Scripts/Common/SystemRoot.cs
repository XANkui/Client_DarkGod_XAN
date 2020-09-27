/****************************************************
文件：SystemRoot.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/23 22:14:08
功能：业务系统基类
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemRoot : MonoBehaviour
{
    protected ResSvc resSvc = null;
    protected AudioSvc audioSvc = null;
    protected NetSvc netSvc = null;
    

    public virtual void InitSys() {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
    }

    public virtual void ClearSys()
    {
        resSvc = null;
        audioSvc = null;
        netSvc = null;
    }
}
