/****************************************************
文件：IState.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/05 10:24:25
功能：状态接口
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState 
{
    void Enter(EntityBase entityBase, params object[] args);
    void Process(EntityBase entityBase, params object[] args);
    void Exit(EntityBase entityBase, params object[] args);
}

public enum AniState {
    None,
    Born,
    Idle,
    Move,
    Attack,
    Hit,
    Die
}
