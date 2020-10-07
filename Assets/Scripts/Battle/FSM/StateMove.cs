/****************************************************
文件：StateMove.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/05 10:26:21
功能：移动状态
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMove : IState
{
   

    public void Enter(EntityBase entityBase, params object[] args)
    {
        entityBase.currentAniState = AniState.Move;

        Debug.Log(GetType() + "/Enter()/ StateMove ");

    }

    public void Process(EntityBase entityBase, params object[] args)
    {
        Debug.Log(GetType() + "/Process()/ StateMove ");
        entityBase.SetBlend(Constants.BlendMove);
    }

    public void Exit(EntityBase entityBase, params object[] args)
    {
        Debug.Log(GetType() + "/Exit()/ StateMove ");
    }
}
