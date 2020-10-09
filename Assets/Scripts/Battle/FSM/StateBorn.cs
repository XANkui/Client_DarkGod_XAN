/****************************************************
文件：StateBorn.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/09 09:50:45
功能：出生状态
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBorn : IState
{
    public void Enter(EntityBase entityBase, params object[] args)
    {
        entityBase.currentAniState = AniState.Born;
    }

    public void Process(EntityBase entityBase, params object[] args)
    {
        // 播放出生动画
        entityBase.SetAction(Constants.ActionBorn);
        // 延时切换为 -1
        TimerSvc.Instance.AddTimeTask((tid) => {
            entityBase.SetAction(Constants.ActionDefault);
        }, 500);
    }

    public void Exit(EntityBase entityBase, params object[] args)
    {
       
    }
}
