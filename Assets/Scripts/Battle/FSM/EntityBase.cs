/****************************************************
文件：EntityBase.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/05 10:24:41
功能：实体基类
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase 
{
    public AniState currentAniState = AniState.None;

    public BattleMgr battleMgr = null;
    public StateMgr stateMgr = null;
    public SkillMgr skillMgr = null;
    public Controller ctrl = null;

    // 方向等操作是否可以控制
    public bool canControll = true;

    public void Idle() { stateMgr.ChangeState(this,AniState.Idle,null); }
    public void Move() { stateMgr.ChangeState(this,AniState.Move,null); }
    public void Attack(int skillID) { stateMgr.ChangeState(this,AniState.Attack, skillID); }

    public virtual void SetBlend(float blend) {
        if (ctrl!=null)
        {
            ctrl.SetBlend(blend);
        }
    }

    public virtual void SetDir(Vector2 dir)
    {
        if (ctrl != null)
        {
            ctrl.Dir=(dir);
        }
    }

    public virtual void SetAction(int action)
    {
        if (ctrl != null)
        {
            ctrl.SetAction(action);
        }
    }

    public virtual void SetFX(string fxName,float delayDestory) {

        if (ctrl != null)
        {
            ctrl.SetFX(fxName,delayDestory);
        }
    }

    public virtual void AttackEffect(int skillID) {
        skillMgr.AttackEffect(this,skillID);
    }

    public virtual void SetSkillMoveState(bool move, float speed =0) {
        if (ctrl != null)
        {
            ctrl.SetSkillMoveState(move, speed);
        }
    }

    public virtual Vector2 GetCurDirInput() {
        return Vector2.zero;
    }
}
