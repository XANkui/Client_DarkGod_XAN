/****************************************************
文件：StateIdle.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/05 10:26:11
功能：Idle 状态
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : IState
{
    public void Enter(EntityBase entityBase, params object[] args)
    {
        entityBase.currentAniState = AniState.Idle;
        entityBase.SetDir(Vector2.zero);
        //Debug.Log(GetType()+ "/Enter()/ StateIdle ");
    }

    public void Process(EntityBase entityBase, params object[] args)
    {
        //Debug.Log(GetType() + "/Process()/ StateIdle ");
        // 判断有连招不
        if (entityBase.nextSkillID != 0)
        {
            entityBase.Attack(entityBase.nextSkillID);
        }
        else
        {
            // 进入Idle 状态的时候可以释放技能了
            if (entityBase.entityType == EntityType.Player)
            {
                entityBase.canRlsSkill = true;
            }

            // 判断有移动操作不
            if (entityBase.GetCurDirInput() != Vector2.zero)
            {
                entityBase.Move();
                entityBase.SetDir(entityBase.GetCurDirInput());
            }
            else
            {
                entityBase.SetBlend(Constants.BlendIdle);
            }
        }

    }

    public void Exit(EntityBase entityBase, params object[] args)
    {
       // Debug.Log(GetType() + "/Exit()/ StateIdle ");
    }
}
