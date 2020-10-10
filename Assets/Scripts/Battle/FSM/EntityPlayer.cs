/****************************************************
文件：PlayerEntity.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/05 10:38:59
功能：玩家实体类
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : EntityBase
{

    public override Vector2 GetCurDirInput()
    {
        return battleMgr.GetCurDirInput();
    }

    public override Vector2 CalcTargetDir()
    {
        EntityMonster monster = FindClosedTarget();
        if (monster != null)
        {
            Vector3 target = monster.GetPos();
            Vector3 self = GetPos();
            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;

        }
        else {

            return Vector2.zero;
        }

        
    }

    private EntityMonster FindClosedTarget() {
        List<EntityMonster> lst = battleMgr.GetEntityMonsters();
        if (lst == null && lst.Count==0)
        {
            return null;
        }

        Vector3 self = GetPos();
        EntityMonster targetMonster = null;
        float dis = 0;
        for (int i = 0; i < lst.Count; i++)
        {
            Vector3 target = lst[i].GetPos();
            if (i == 0)
            {

                dis = Vector3.Distance(self, target);
                targetMonster = lst[i];
            }
            else {
                float calcDis = Vector3.Distance(self,target);
                if (dis > calcDis)
                {
                    dis = calcDis;
                    targetMonster = lst[i];
                }
            }
        }
        return targetMonster;
    }


}
