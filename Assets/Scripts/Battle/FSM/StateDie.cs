/****************************************************
文件：StateDie.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/09 10:19:58
功能：死亡状态
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDie : IState
{
    public void Enter(EntityBase entityBase, params object[] args)
    {
        entityBase.currentAniState = AniState.Die;
        entityBase.RmvSkillCB();
    }

    public void Process(EntityBase entityBase, params object[] args)
    {
        // 播放死亡动画
        entityBase.SetAction(Constants.ActionDie);

        Debug.Log(GetType()+ "/StateDie()/.....");

        // 延时切换为 -1
        if (entityBase.entityType == EntityType.Monster)
        {
            entityBase.GetCC().enabled = false;

            TimerSvc.Instance.AddTimeTask((tid) =>
            {

                entityBase.SetActive(false); ;


            }, Constants.DieAniLength);
        }
    }

    public void Exit(EntityBase entityBase, params object[] args)
    {
    }
}
