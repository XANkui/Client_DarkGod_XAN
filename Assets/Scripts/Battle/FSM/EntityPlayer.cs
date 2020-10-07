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
}
