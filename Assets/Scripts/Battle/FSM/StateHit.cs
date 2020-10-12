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
        entityBase.RmvSkillCB();
    }

    public void Process(EntityBase entityBase, params object[] args)
    {
        // 受攻击的时候技能也不能释放
        if (entityBase.entityType == EntityType.Player)
        {
            entityBase.canRlsSkill = false;
        }

        // 停止移动
        entityBase.SetDir(Vector2.zero);
        entityBase.SetAction(Constants.ActionHit);

        // 玩家受伤播放音效
        if (entityBase.entityType == EntityType.Player)
        {
            AudioSource audioSrc = entityBase.GetAudio();
            AudioSvc.Instance.PlayCharAudio(Constants.AssassinHit,audioSrc);
        }

        TimerSvc.Instance.AddTimeTask((tid) => {
            entityBase.SetAction(Constants.ActionDefault);
            entityBase.Idle();
        }, (int)(GetHitAniLen(entityBase) * 1000));
    }

    public void Exit(EntityBase entityBase, params object[] args)
    {
        
    }

    private float GetHitAniLen(EntityBase entity) {
        AnimationClip[] clips = entity.GetAniClips();
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
