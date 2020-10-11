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

        // 判断玩家的技能是否忽略碰撞效果
        if (skillCfg.isCollide ==false)
        {
            // 忽略掉Player 和 Monster 层的碰撞
            Physics.IgnoreLayerCollision(9,10);

            // 技能释放完成后恢复碰撞
            TimerSvc.Instance.AddTimeTask((tid)=> {
                Physics.IgnoreLayerCollision(9, 10, false);
            }, skillCfg.skillTime);
        }

        // 玩家才进行方向锁定攻击，怪物不需要（避免怪物攻击抖动）
        if (entity.entityType == EntityType.Player)
        {        
            // player 为了连招中的方向控制
            if (entity.GetCurDirInput() == Vector2.zero)
            {
                // 搜索最近怪物攻击
                Vector2 dir = entity.CalcTargetDir();
                if (dir != Vector2.zero)
                {
                    entity.SetAtkRotation(dir);
                }
            }
            else {
                entity.SetAtkRotation(entity.GetCurDirInput(),true);
            }

        }


        entity.SetAction(skillCfg.aniAction);
        entity.SetFX(skillCfg.fx,skillCfg.skillTime);


        CalcSkillMove(entity,skillCfg);

        // 是否可操作
        entity.canControll = false;
        entity.SetDir(Vector2.zero);    // 方向移动失效

        // 判断攻击是否事可中断的，不是，则进入霸体状态
        if (skillCfg.isBreak == false)
        {
            entity.entityState = EntityState.BatiState;
        }


        entity.skCBID = timerSvc.AddTimeTask((tid)=>{
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
                int actid = timerSvc.AddTimeTask((tid) =>
                {
                    SkillAction(entity, skillCfg, index);
                    entity.RmvActionCB(tid);
                }, sum);
                entity.skActionCBLst.Add(actid);
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

        // 判断是怪物还是玩家攻击，避免怪物打怪物
        if (caster.entityType ==EntityType.Monster)
        {
            EntityPlayer player = caster.battleMgr.entityPlayer;
            // 判断距离和角度
            if (InRange(caster.GetPos(), player.GetPos(), skillActionCfg.radius) == true
                && InAngle(caster.GetTrans(), player.GetPos(), skillActionCfg.angle) == true
                )
            {
                // 计算伤害
                CalcDamage(caster, player, skillCfg, damage);
            }
            Debug.Log(GetType() + "/SkillAction()/ ...");
        }
        else if(caster.entityType == EntityType.Player)
        {
            // 获取场景中所有的怪物尸体，遍历运算伤害
            List<EntityMonster> monsterLst = caster.battleMgr.GetEntityMonsters();
            for (int i = 0; i < monsterLst.Count; i++)
            {
                EntityMonster monster = monsterLst[i];

                // 判断距离和角度
                if (InRange(caster.GetPos(), monster.GetPos(), skillActionCfg.radius) == true
                    && InAngle(caster.GetTrans(), monster.GetPos(), skillActionCfg.angle) == true
                    )
                {
                    // 计算伤害
                    CalcDamage(caster, monster, skillCfg, damage);
                }
                Debug.Log(GetType() + "/SkillAction()/ ...");
            }
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
                target.SetDodge();
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

                // UI 暴击
                Debug.Log(GetType() + "/()/ Critical Rate : " + criticalNum + "/" + caster.Props.critical);
                target.SetCiritical(dmgSum);
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

        // UI 显示最终伤害
        target.SetHurt(dmgSum);

        if (target.HP < dmgSum)
        {
            target.HP = 0;
            //目标死亡
            target.Die();
            target.battleMgr.RmvMonster(target.Name);
        }
        else {
            target.HP -= dmgSum;

            // 霸体状态可受伤害，但是不会切换到受伤状态
            if (target.entityState ==  EntityState.None && target.GetBreakState() ==true)
            {
                // 目标受到攻击
                target.Hit();
            }
            
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
                int moveid = timerSvc.AddTimeTask((tid) => {
                    entity.SetSkillMoveState(true, speed);
                    entity.RmvMoveCB(tid);
                }, sumTime);
                entity.skMoveCBLst.Add(moveid);
            }
            else
            {
                entity.SetSkillMoveState(true, speed);

            }
            sumTime += skillMoveCfg.moveTime;
            int stopid= timerSvc.AddTimeTask((tid) => {
                entity.SetSkillMoveState(false);
                entity.RmvMoveCB(tid);
            }, sumTime);
            entity.skMoveCBLst.Add(stopid);

        }
    }
}
