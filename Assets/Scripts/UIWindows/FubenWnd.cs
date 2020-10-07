/****************************************************
文件：FubenWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/03 22:04:43
功能：副本界面
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FubenWnd : WindowRoot
{
    public Button btnClose;
    public Button[] fbBtnArr;
    public Transform pointerTrans;

    private PlayerData playerData;

    private void Start()
    {
        btnClose.onClick.AddListener(ClickCloseBtn);

        FubenBtnsAddListener();
    }

    protected override void InitWnd()
    {
        base.InitWnd();
        playerData = GameRoot.Instance.PlayerData;

        RefreshUI();
    }

    public void RefreshUI() {
        int fbid = playerData.fuben;
        for (int i = 0; i < fbBtnArr.Length; i++)
        {
            // fuben id :10001,10002
            if (i<(fbid%10000))
            {
                SetActive(fbBtnArr[i].gameObject);
                if (i == (fbid % 10000 -1))
                {
                    pointerTrans.SetParent(fbBtnArr[i].transform);
                    pointerTrans.localPosition = new Vector3(25,100,0);
                }
            }
            else
            {
                SetActive(fbBtnArr[i].gameObject,false);
            }
        }
    }

    #region Click Events

    private void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetWndState(false);
    }

    private void ClickFubenBtn(int fubenID) {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        Debug.Log(GetType() + "/FubenBtnsAddListener()/Click fubenID : " + fubenID);
        // 体力校验
        int power = resSvc.GetMapCfgData(fubenID).power;
        if (power > playerData.power)
        {
            GameRoot.AddTips("体力值不足");
        }
        else {
            netSvc.SendMsg(new GameMsg {
                cmd= (int) CMD.ReqFBFight,
                reqFBFight = new ReqFBFight {
                    fbId = fubenID
                }
            });
        }
    }

    private void FubenBtnsAddListener() {
        for (int i = 0; i < fbBtnArr.Length; i++)
        {
            int len = fbBtnArr[i].gameObject.name.Length;
            int index = int.Parse(fbBtnArr[i].gameObject.name.Substring(len - 1));
            Debug.Log(GetType()+ "/FubenBtnsAddListener()/name index : "+ index);
            fbBtnArr[i].onClick.AddListener(()=>{

                ClickFubenBtn(10000+index);
            });
        }
    }

    #endregion
}
