/****************************************************
文件：StateMgr.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/04 12:55:25
功能：状态管理器
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMgr : MonoBehaviour
{
    private Dictionary<AniState, IState> fsm = new Dictionary<AniState, IState>();

    public void Init() {

        fsm.Add(AniState.Born, new StateBorn());
        fsm.Add(AniState.Idle, new StateIdle());
        fsm.Add(AniState.Move, new StateMove());
        fsm.Add(AniState.Attack, new StateAttack());
        fsm.Add(AniState.Hit, new StateHit());
        fsm.Add(AniState.Die, new StateDie());

        Common.Log("StateMgr init done");
    }


    public void ChangeState(EntityBase entity, AniState targetState, params object[] args) {
        if (entity.currentAniState == targetState)
        {
            //Debug.Log(GetType() + "/ChangeState()/ Return ");

            return;
        }

        if (fsm.ContainsKey(targetState)==true)
        {
            if (entity.currentAniState != AniState.None)
            {
                fsm[entity.currentAniState].Exit(entity, args);

            }
            fsm[targetState].Enter(entity,args);
            fsm[targetState].Process(entity, args);
        }
    }
}
