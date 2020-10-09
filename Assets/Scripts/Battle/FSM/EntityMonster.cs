/****************************************************
文件：EntityMonster.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/08 21:33:31
功能：怪物的逻辑实体
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMonster : EntityBase
{
    public MonsterData md;

    public override void SetBattleProps(BattleProps props)
    {
        int level = md.mLevel;

        BattleProps p = new BattleProps {
            hp = props.hp * level,
            ad = props.ad * level,
            ap = props.ap * level,
            addef = props.addef * level,
            apdef = props.apdef * level,
            dodge = props.dodge * level,
            pierce = props.pierce * level,
            critical = props.critical * level
         
        };

        Props = p;
        HP = p.hp;
    }
}
