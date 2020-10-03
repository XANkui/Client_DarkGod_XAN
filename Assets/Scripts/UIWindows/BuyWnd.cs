/****************************************************
文件：BuyWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/02 17:26:21
功能：购买界面
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuyType {
    BuyPower =0,     // 购买体力
    MKCoin =1,      // 铸造金币
}

public class BuyWnd : WindowRoot
{
    public Text txtInfo;
    public Button btnSure;
    public Button btnClose;

    private BuyType buyType;

    public void SetBuyType(BuyType buyType) {
        this.buyType = buyType;
        RefreshUI();
    }

    private void Start()
    {
        btnSure.onClick.AddListener(ClickSureBtn);
        btnClose.onClick.AddListener(ClickCloseBtn);
    }

    protected override void InitWnd()
    {
        base.InitWnd();
        btnSure.interactable = true;
        RefreshUI();
    }

    private void RefreshUI() {
        switch (buyType)
        {
            case BuyType.BuyPower:

                txtInfo.text = "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "购买"+Constants.Color("100体力", TxtColor.Green)+"?";

                break;
            case BuyType.MKCoin:
                txtInfo.text = "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "铸造" + Constants.Color("1000金币", TxtColor.Green) + "?";

                break;
            
        }
    }

    #region Click Evts
    private void ClickSureBtn() {
        GameMsg msg = new GameMsg {
            cmd = (int)CMD.ReqBuy,
            reqBuy = new ReqBuy {
                type = (int)buyType,
                cost = 10
            }
        };

        netSvc.SendMsg(msg);
        btnSure.interactable = false;
    }

    private void ClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetWndState(false);
    }
    #endregion
}
