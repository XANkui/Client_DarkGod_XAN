/****************************************************
文件：SkillMgr.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/04 12:55:14
功能：技能管理器
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMgr : MonoBehaviour
{
    private ResSvc resSvc;
    private TimerSvc timerSvc;

    public void Init() {
        resSvc = ResSvc.Instance;
        timerSvc = TimerSvc.Instance;
        Common.Log("SkillMgr init done");
    }

    /// <summary>
    /// 技能效果表现
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="skillID"></param>
    public void AttackEffect(EntityBase entity,int skillID)
    {
        SkillCfg skillCfg = resSvc.GetSkillCfg(skillID);

        entity.SetAction(skillCfg.aniAction);
        entity.SetFX(skillCfg.fx,skillCfg.skillTime);


        CalcSkillMove(entity,skillCfg);

        // 是否可操作
        entity.canControll = false;
        entity.SetDir(Vector2.zero);    // 方向移动失效

        timerSvc.AddTimeTask((tid)=>{
            entity.Idle();
        },skillCfg.skillTime);
    }

    private void CalcSkillMove(EntityBase entity, SkillCfg skillCfg) {
        List<int> skillMoveLst = skillCfg.skillMoveLst;
        int sumTime = 0;
        for (int i = 0; i < skillMoveLst.Count; i++)
        {
            SkillMoveCfg skillMoveCfg = resSvc.GetSkillMoveCfg(skillCfg.skillMoveLst[i]);
            float speed = skillMoveCfg.moveDis / (skillMoveCfg.moveTime / 1000f);
            sumTime += skillMoveCfg.delayTime;
            if (sumTime > 0)
            {
                timerSvc.AddTimeTask((tid) => {
                    entity.SetSkillMoveState(true, speed);
                }, sumTime);
            }
            else
            {
                entity.SetSkillMoveState(true, speed);

            }
            sumTime += skillMoveCfg.moveTime;
            timerSvc.AddTimeTask((tid) => {
                entity.SetSkillMoveState(false);
            }, sumTime);


        }
    }
}
