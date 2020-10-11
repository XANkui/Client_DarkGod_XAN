/****************************************************
文件：BattleSys.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/04 12:42:35
功能：战斗业务系统
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSys : SystemRoot
{
    public PlayerCtrlWnd playerCtrlWnd;


    internal BattleMgr battleMgr;

    public static BattleSys Instance = null;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;


        Debug.Log(GetType() + "/InitSys()/Init BattleSys...");

    }

    public void StartBattle(int mapid) {
        GameObject go = new GameObject {
            name = "BattleRoot"
        };
        go.transform.SetParent(GameRoot.Instance.transform);
        battleMgr = go.AddComponent<BattleMgr>();
        battleMgr.Init(mapid);

        SetPlayerCtrlWndState();
    }

    public void SetPlayerCtrlWndState(bool isActive = true) {
        playerCtrlWnd.SetWndState(isActive);
    }

    public void SetSelfPlayerMoveDir(Vector2 dir)
    {
        battleMgr.SetSelfPlayerMoveDir(dir);
    }

    public void ReqReleaseSkill(int index) {


        battleMgr.ReqReleaseSkill(index);
    }

    public Vector2 GetCurrentDir() {
        return playerCtrlWnd.currentDir;
    }
}
