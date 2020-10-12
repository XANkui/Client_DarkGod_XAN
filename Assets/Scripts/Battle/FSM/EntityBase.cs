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
    protected Controller controller = null;

    // 方向等操作是否可以控制
    public bool canControll = true;
    // 可不可以放技能（避免一个技能没有释放完，又触发另一个技能释放）
    public bool canRlsSkill = true;

   

    private BattleProps props;
    public BattleProps Props { get => props; protected set => props = value; }

    public EntityType entityType = EntityType.None;

    public EntityState entityState = EntityState.None;

    private string name;
    public string Name { get => name; set => name = value; }

    private int hp;
    public int HP { get => hp;
        set {
            // UI 血量变化
            Debug.Log(GetType()+"/HP/ hp "+hp+ " to " +value);
            SetHpVal(hp,value);
            hp = value;
        }
    }

    // 普通攻击连招部分
    public Queue<int> comboQue = new Queue<int>();
    public int nextSkillID = 0;


    public SkillCfg curtSkillCfg;


    // 技能位移的回调ID
    public List<int> skMoveCBLst = new List<int>(); 
    // 技能伤害计算的回调ID
    public List<int> skActionCBLst = new List<int>();
    // 玩家的技能回调ID
    public int skCBID = -1;

    public void Born() { stateMgr.ChangeState(this,AniState.Born, null); }
    public void Idle() { stateMgr.ChangeState(this,AniState.Idle,null); }
    public void Move() { stateMgr.ChangeState(this,AniState.Move,null); }
    public void Attack(int skillID) { stateMgr.ChangeState(this,AniState.Attack, skillID); }
    public void Hit() { stateMgr.ChangeState(this, AniState.Hit, null); }
    public void Die() { stateMgr.ChangeState(this, AniState.Die, null); }

    public virtual void TickAILogic() { }

    public virtual void SetCtrl(Controller ctrl) {
        controller = ctrl;
    }

    public virtual void SetActive(bool isActive = true) {
        if (controller != null)
        {
            controller.gameObject.SetActive(isActive);
        }
    }

    public virtual AnimationClip[] GetAniClips() {
        if (controller != null)
        {
            return controller.ani.runtimeAnimatorController.animationClips;
        }

        return null;
    }

    public AudioSource GetAudio() {
        return controller.gameObject.GetComponent<AudioSource>();
    }

    public virtual bool GetBreakState() {
        return true;
    }

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

    public virtual void SetAtkRotation(Vector2 dir, bool offset = false) {
        if (controller != null)
        {
            if (offset == true)
            {
                controller.SetAtkRotationCam(dir);
            }
            else {
                controller.SetAtkRotationLocal(dir);
            }
            
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

    #region 战斗 UI 显示部分

    public virtual void SetCiritical(int critical)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWnd.SetCiritical(Name, critical);
        }
    }

    public virtual void SetDodge()
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWnd.SetDodge(Name);
        }
    }

    public virtual void SetHurt(int hurt)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWnd.SetHurt(Name, hurt);
        }
    }

    public virtual void SetHpVal(int oldVal, int newVal)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWnd.SetHpVal(Name, oldVal, newVal);

        }
    }

    #endregion
       
    public virtual Vector2 CalcTargetDir() {
        return Vector2.zero;
    }

    public void ExitCurtSkill() {
        canControll = true;
        if (curtSkillCfg != null)
        {        
            // 如果当前是不可中断技能，之前肯可能进入霸体状体，现在攻击完成，回到正常状态
            if (curtSkillCfg.isBreak == false)
            {
                entityState = EntityState.None;
            }

            // 是连招技能才进入连招数据弹出判断
            if (curtSkillCfg.isCombo == true)
            {
                if (comboQue.Count > 0)
                {
                    nextSkillID = comboQue.Dequeue();
                }
                else
                {
                    nextSkillID = 0;
                }
            }

            curtSkillCfg = null;
        }

        SetAction(Constants.ActionDefault);
    }

    public void RmvMoveCB(int tid) {
        int index = -1;
        for (int i = 0; i < skMoveCBLst.Count; i++)
        {
            if (skMoveCBLst[i]==tid)
            {
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            skMoveCBLst.Remove(index);
        }
    }

    public void RmvActionCB(int tid)
    {
        int index = -1;
        for (int i = 0; i < skActionCBLst.Count; i++)
        {
            if (skActionCBLst[i] == tid)
            {
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            skActionCBLst.Remove(index);
        }
    }

    public void RmvSkillCB() {
        SetDir(Vector2.zero);
        SetSkillMoveState(false);


        // 受伤中断操作
        for (int i = 0; i < skActionCBLst.Count; i++)
        {
            int tid = skActionCBLst[i];
            TimerSvc.Instance.DelTask(tid);
        }
        for (int i = 0; i < skMoveCBLst.Count; i++)
        {
            int tid = skMoveCBLst[i];
            TimerSvc.Instance.DelTask(tid);
        }

        // 攻击被中断，删除定时回调
        if (skCBID != -1)
        {
            TimerSvc.Instance.DelTask(skCBID);
            skCBID = -1;
        }

        skActionCBLst.Clear();
        skMoveCBLst.Clear();

        // 中断后清空连招数据
        if (nextSkillID != 0 || comboQue.Count > 0)
        {
            nextSkillID = 0;
            comboQue.Clear();

            battleMgr.lastAtkTime = 0;
            battleMgr.comboIndex = 0;
        }
    }
}
