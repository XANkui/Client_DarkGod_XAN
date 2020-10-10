/****************************************************
文件：StateAttack.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/06 09:56:09
功能：攻击状态
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack : IState
{


    public void Enter(EntityBase entityBase,params object[] args)
    {
        entityBase.currentAniState = AniState.Attack;
        //entityBase.SetDir(Vector2.zero);
        entityBase.curtSkillCfg = ResSvc.Instance.GetSkillCfg((int)args[0]);
       // Debug.Log(GetType() + "/Enter()/ StateAttack ");
    }

    public void Process(EntityBase entityBase, params object[] args)
    {
        // 技能伤害计算       
        // 技能的效果表现
        entityBase.SkillEffect((int)args[0]);
        Debug.Log(GetType() + "/Process()/ StateAttack args[0]" + args[0]);

    }

    public void Exit(EntityBase entityBase, params object[] argss)
    {
        entityBase.ExitCurtSkill();
        
        //Debug.Log(GetType() + "/Exit()/ StateAttack ");
    }
}
