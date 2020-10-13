/****************************************************
文件：BattleSys.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/04 12:42:35
功能：战斗业务系统
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSys : SystemRoot
{
    public PlayerCtrlWnd playerCtrlWnd;
    public BattleEndWnd battleEndWnd;


    private int fbid;
    private double startFightTime;

    internal BattleMgr battleMgr;

    public static BattleSys Instance = null;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;


        Debug.Log(GetType() + "/InitSys()/Init BattleSys...");

    }

    public void StartBattle(int mapid) {

        fbid = mapid;

        GameObject go = new GameObject {
            name = "BattleRoot"
        };
        go.transform.SetParent(GameRoot.Instance.transform);
        battleMgr = go.AddComponent<BattleMgr>();
        battleMgr.Init(mapid,()=>{
            startFightTime = timerSvc.GetNowTime();
        });

        SetPlayerCtrlWndState();
    }

    public void EndBattle(bool isWin, int restHp) {
        playerCtrlWnd.SetWndState(false);
        GameRoot.Instance.dynamicWnd.RmvAllHpItemInfo();

        if (isWin == true)
        {
            double endFightTime = timerSvc.GetNowTime();
            // 发送数据到服务器结算
            GameMsg gameMsg = new GameMsg {
                cmd = (int)CMD.ReqFBFightEnd,
                reqFBFightEnd = new ReqFBFightEnd {
                    fbId = fbid,
                    win =isWin,
                    resthp=restHp,
                    costtime = (int)(endFightTime-startFightTime)
                }
            };

            netSvc.SendMsg(gameMsg);
        }
        else
        {
            SetBattleEndWndState(FBEndType.Lose);
        }
    }

    public void RspFBFightEnd(GameMsg msg) {
        RspFBFightEnd data = msg.rspFBFightEnd;
        GameRoot.Instance.SetPlayerDataByFBEnd(data);

        battleEndWnd.SetBattleEndData(data.fbId,data.costtime,data.resthp);
        SetBattleEndWndState(FBEndType.Win);
    }

    public void DestroyBattle() {
        SetPlayerCtrlWndState(false);
        SetBattleEndWndState(FBEndType.None,false);
        GameRoot.Instance.dynamicWnd.RmvAllHpItemInfo();
        Destroy(battleMgr.gameObject);
    }

    public void SetPlayerCtrlWndState(bool isActive = true) {
        playerCtrlWnd.SetWndState(isActive);
    }

    public void SetBattleEndWndState(FBEndType endType, bool isActive = true)
    {
        battleEndWnd.SetWndType(endType);
        battleEndWnd.SetWndState(isActive);
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
