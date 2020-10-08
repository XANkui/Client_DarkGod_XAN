/****************************************************
文件：SkillMgr.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/04 12:55:14
功能：技能管理器
*****************************************************/

using System;
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

    public void SkillEffect(EntityBase entity, int skillID) {

        AttackDamage(entity, skillID);
        AttackEffect(entity, skillID);
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

    /// <summary>
    /// 技能伤害计算
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="skillID"></param>
    public void AttackDamage(EntityBase entity, int skillID) {
        SkillCfg skillCfg = resSvc.GetSkillCfg(skillID);
        List<int> actionLst = skillCfg.skillActionLst;
        int sum = 0;
        for (int i = 0; i < actionLst.Count; i++)
        {
            SkillActionCfg skillActionCfg = resSvc.GetSkillActionCfg(actionLst[i]);
            sum += skillActionCfg.delayTime;
            int index = i;
            if (sum > 0)
            {
                timerSvc.AddTimeTask((tid) =>
                {
                    SkillAction(entity, skillCfg, index);
                }, sum);
            }
            else {

                SkillAction(entity, skillCfg, index);
            }
        }
    }

    private void SkillAction(EntityBase entity, SkillCfg skillCfg , int index)
    {

        SkillActionCfg skillActionCfg = resSvc.GetSkillActionCfg(skillCfg.skillActionLst[index]);

        int damage = skillCfg.skillDamageLst[index];

        // 获取场景中所有的怪物尸体，遍历运算伤害
        List<EntityMonster> monsterLst = entity.battleMgr.GetEntityMonsters();        
        for (int i = 0; i < monsterLst.Count; i++)
        {
            EntityMonster em = monsterLst[i];

            // 判断距离和角度
            if (InRange(entity.GetPos(),em.GetPos(),skillActionCfg.radiu)==true
                && InAngle(entity.GetTrans(),em.GetPos(),skillActionCfg.angle)== true
                )
            {
                // 计算伤害
                CalcDamage(entity,damage);
            }
        }
    }


    private void CalcDamage(EntityBase entity, int damage) {

    }

    /// <summary>
    /// 判断怪物是否在技能范围内
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    private bool InRange(Vector3 from, Vector3 to, float range) {

        float dis = Vector3.Distance(from,to);

        if (dis > range)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 计算怪物是否在技能角度内
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="to"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    private bool InAngle(Transform trans, Vector3 to , float angle) {
        if (angle == 360)
        {
            return true;
        }
        else {
            Vector3 start = trans.forward;
            Vector3 dir = (to - trans.position).normalized;

            float ang = Vector3.Angle(start,dir);

            if (ang <= (angle / 2.0f))
            {
                return true;
            }
            else {
                return false;
            }
        }

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
