/****************************************************
文件：Controller.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/05 10:45:53
功能：实体抽象类控制器
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public Animator ani;
    public CharacterController ctrl;

    protected Dictionary<string, GameObject> fxDic = new Dictionary<string, GameObject>();
    protected TimerSvc timerSvc;

    protected bool skillMove = false;
    protected float skillMoveSpeed = 0.0f;

    protected bool isMove = false;
    private Vector2 dir = Vector2.zero;
    public Vector2 Dir
    {
        get => dir;
        set
        {
            dir = value;
            if (value != Vector2.zero)
            {
                isMove = true;
            }
            else
            {
                isMove = false;
            }
        }
    }

    public virtual void Init() {
        timerSvc = TimerSvc.Instance;
    }

    public virtual void SetBlend(float blend) {
        ani.SetFloat("Blend",blend);
    }

    public virtual void SetAction(int action)
    {
        Debug.Log(GetType()+ "/SetAction()/ action = "+ action);
        ani.SetInteger("Action", action);
    }

    public virtual void SetFX(string fxName, float delayDestory)
    {

    }

    public void SetSkillMoveState(bool move, float skillSpeed=0) {
        skillMove = move;
        skillMoveSpeed = skillSpeed;
    }
}
