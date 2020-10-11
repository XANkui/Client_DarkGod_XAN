/****************************************************
文件：MonsterController.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/08 21:32:17
功能：怪物表現角色控制器
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : Controller
{


    private void Update()
    {
        if (isMove==true)
        {
            SetDir();

            SetMove();
        }
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1)) ;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
    private void SetMove()
    {
        ctrl.Move(transform.forward * Time.deltaTime * Constants.MonsterMoveSpeed);
        // 修正 怪物的Animator 没有勾选 Apply roor motion  怪物不能向下自动坠落的问题
        ctrl.Move(transform.up *-1 * Time.deltaTime * Constants.MonsterMoveSpeed);
    }
}
