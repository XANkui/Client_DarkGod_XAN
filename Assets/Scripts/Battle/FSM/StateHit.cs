/****************************************************
文件：StateHit.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/10 10:20:36
功能：受伤状态
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHit : IState
{
    public void Enter(EntityBase entityBase, params object[] args)
    {
        entityBase.currentAniState = AniState.Hit;
    }

    public void Process(EntityBase entityBase, params object[] args)
    {
        // 停止移动
        entityBase.SetDir(Vector2.zero);
        entityBase.SetAction(Constants.ActionHit);

        TimerSvc.Instance.AddTimeTask((tid) => {
            entityBase.SetAction(Constants.ActionDefault);
            entityBase.Idle();
        }, (int)(GetHitAniLen(entityBase) * 1000));
    }

    public void Exit(EntityBase entityBase, params object[] args)
    {
        
    }

    private float GetHitAniLen(EntityBase entity) {
        AnimationClip[] clips = entity.controller.ani.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            string clipName = clips[i].name.ToLower();
            if (clipName.Contains("hit"))
            {
                return clips[i].length;
            }
        }

        return 1;
    }
}
