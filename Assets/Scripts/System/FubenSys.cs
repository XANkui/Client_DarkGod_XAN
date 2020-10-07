/****************************************************
文件：FubenSys.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/03 22:07:37
功能：副本业务系统
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FubenSys : SystemRoot
{

    public FubenWnd fubenWnd;

    public static FubenSys Instance = null;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;


        Debug.Log(GetType() + "/InitSys()/Init FubenSys...");

    }

    public void EnterFubenWnd() {
        SetStateFubenWnd();
    }

    public void SetStateFubenWnd(bool isActive =true) {
        fubenWnd.SetWndState(isActive);
    }

    public void RspFBFight(GameMsg msg) {
        RspFBFight data = msg.rspFBFight;
        GameRoot.Instance.SetPlayerDataByFBStart(data);

        //关闭界面，开始战斗
        SetStateFubenWnd(false);
        MainCitySys.Instance.mainCityWnd.SetWndState(false);

        BattleSys.Instance.StartBattle(data.fbId);
    }
}
