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

        //Debug.Log(GetType()+ "/AttackDamage()/ " );

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

    private void SkillAction(EntityBase caster, SkillCfg skillCfg , int index)
    {
        //Debug.Log(GetType() + "/SkillAction()/ ");

        SkillActionCfg skillActionCfg = resSvc.GetSkillActionCfg(skillCfg.skillActionLst[index]);

        int damage = skillCfg.skillDamageLst[index];

        // 获取场景中所有的怪物尸体，遍历运算伤害
        List<EntityMonster> monsterLst = caster.battleMgr.GetEntityMonsters();        
        for (int i = 0; i < monsterLst.Count; i++)
        {
            EntityMonster target = monsterLst[i];

            // 判断距离和角度
            if (InRange(caster.GetPos(),target.GetPos(),skillActionCfg.radius)==true
                && InAngle(caster.GetTrans(),target.GetPos(),skillActionCfg.angle)== true
                )
            {
                // 计算伤害
                CalcDamage(caster, target, skillCfg, damage);
            }
            Debug.Log(GetType() + "/SkillAction()/ ...");
        }
        Debug.Log(GetType() + "/SkillAction()/ End");
    }


    System.Random rd = new System.Random();
    /// <summary>
    /// 技能伤害计算
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="skillCfg"></param>
    /// <param name="damage"></param>
    private void CalcDamage(EntityBase caster,EntityBase target, SkillCfg skillCfg, int damage) {
        //Debug.Log(GetType() + "/CalcDamage()/ ");

        int dmgSum = damage;
        if (skillCfg.dmgType == DamageType.AD)
        {
            // 计算闪避
            int dodgeNum = PETools.RDInt(1, 100, rd);
            if (dodgeNum <= target.Props.dodge)
            {
                // UI 显示闪避
                Debug.Log(GetType() + "/()/ Dodge Rate : " + dodgeNum + "/" + target.Props.dodge);
                return;
            }

            // 计算属性加成
            dmgSum += caster.Props.ad;

            //计算暴击
            int criticalNum = PETools.RDInt(1, 100, rd);
            if (criticalNum <= caster.Props.critical)
            {
                float criticalRate = 1 + PETools.RDInt(1, 100, rd) / 100.0f;
                dmgSum += (int)criticalRate * dmgSum;

                Debug.Log(GetType() + "/()/ Critical Rate : " + criticalNum + "/" + caster.Props.critical);
            }

            //计算穿甲
            int addef = (int)((1 - caster.Props.pierce / 100.0f) * target.Props.addef);
            dmgSum -= addef;
        }
        else if (skillCfg.dmgType == DamageType.AP)
        {
            // 计算属性加成
            dmgSum += caster.Props.ap;

            // 计算魔法抗性
            dmgSum -= target.Props.apdef;

        }
        else { }

        // 最终的伤害不能小于0（根据自己的项目来设定）
        if (dmgSum < 0)
        {
            dmgSum = 0;

            return;
        }

        if (target.HP < dmgSum)
        {
            target.HP = 0;
            //目标死亡
            target.Die();
        }
        else {
            target.HP -= dmgSum;
            // 目标受到攻击
            target.Hit();
        }
    }

    /// <summary>
    /// 判断怪物是否在技能范围内
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    private bool InRange(Vector3 from, Vector3 to, float range) {
        //Debug.Log(GetType() + "/InRange()/ ");
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
       // Debug.Log(GetType() + "/InAngle()/ ");
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
