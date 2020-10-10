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
    public Controller controller = null;

    // 方向等操作是否可以控制
    public bool canControll = true;

    private BattleProps props;
    public BattleProps Props { get => props; protected set => props = value; }
    

    private int hp;
    public int HP { get => hp;
        set {
            Debug.Log(GetType()+"/HP/ hp "+hp+ " to " +value);

            hp = value;
        }
    }

    public void Born() { stateMgr.ChangeState(this,AniState.Born, null); }
    public void Idle() { stateMgr.ChangeState(this,AniState.Idle,null); }
    public void Move() { stateMgr.ChangeState(this,AniState.Move,null); }
    public void Attack(int skillID) { stateMgr.ChangeState(this,AniState.Attack, skillID); }
    public void Hit() { stateMgr.ChangeState(this, AniState.Hit, null); }
    public void Die() { stateMgr.ChangeState(this, AniState.Die, null); }

    public virtual void SetBattleProps(BattleProps props) {
        HP = props.hp;
        Props = props;
    }

    public virtual void SetBlend(float blend) {
        if (controller!=null)
        {
            controller.SetBlend(blend);
        }
    }

    public virtual void SetDir(Vector2 dir)
    {
        if (controller != null)
        {
            controller.Dir=(dir);
        }
    }

    public virtual void SetAction(int action)
    {
        if (controller != null)
        {
            controller.SetAction(action);
        }
    }

    public virtual void SetFX(string fxName,float delayDestory) {

        if (controller != null)
        {
            controller.SetFX(fxName,delayDestory);
        }
    }

    public virtual void SkillEffect(int skillID) {
        skillMgr.SkillEffect(this,skillID);
    }

  

    public virtual void SetSkillMoveState(bool move, float speed =0) {
        if (controller != null)
        {
            controller.SetSkillMoveState(move, speed);
        }
    }

    public virtual Vector2 GetCurDirInput() {
        return Vector2.zero;
    }

    public Vector3 GetPos() {
        return controller.transform.position;
    }

    public Transform GetTrans() {
        return controller.transform;
    }
}
