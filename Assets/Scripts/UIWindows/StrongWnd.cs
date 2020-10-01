/****************************************************
文件：StrongWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/01 10:29:07
功能：强化装备的界面
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrongWnd : WindowRoot
{

    #region UI Define    
    public Transform posBtnTrans;
    public Button btnClose;

    public Image imgCurPos;
    public Text txtStarLv;
    public Transform starTransGrid;

    public Text propHP1;
    public Text propHurt1;
    public Text propDef1;
    public Text propHP2;
    public Text propHurt2;
    public Text propDef2;

    public Image propArr1;
    public Image propArr2;
    public Image propArr3;

    public Text txtNeedLv;
    public Text txtCostCoin;
    public Text txtCostCrystal;

    public Transform costTransRoot;
    public Text txtCoin;

    public Button btnStrong;
    #endregion

    private Image[] imgs = new Image [6];
    private PlayerData playerData;
    private int currentIndex;
    private StrongCfg nextSc;

    protected override void InitWnd()
    {
        base.InitWnd();
        btnClose.onClick.AddListener(ClickCloseBtn);
        btnStrong.onClick.AddListener(ClickStrongBtn);
        playerData = GameRoot.Instance.PlayerData;
        RegClickEvts();
        ClickPosItem(0);

    }

    private void RegClickEvts() {
        for (int i = 0; i < posBtnTrans.childCount; i++)
        {
            Transform img = posBtnTrans.GetChild(i);
            OnClick(img.gameObject, (args) => {
                ClickPosItem((int)args);
                audioSvc.PlayUIAudio(Constants.UIClickBtn);
            },i);

            imgs[i] = img.GetComponent<Image>();
        }   
    }

    private void ClickPosItem(int index) {
        Debug.Log(GetType()+ "/ClickPosItem()/Click Item Index:"+index);
        currentIndex = index;
        for (int i = 0; i < imgs.Length; i++)
        {
            Transform trans = imgs[i].transform;
            if (i==index)
            {
                SetSprite(imgs[i], PathDefine.ItemArrowBg);
                trans.localPosition = new Vector3(10,trans.localPosition.y,0);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(250,95);
            }
            else
            {
                SetSprite(imgs[i], PathDefine.ItemPlatBg);
                trans.localPosition = new Vector3(0, trans.localPosition.y, 0);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(225,85);
            }
        }

        RefreshItem();
    }

    private void RefreshItem() {
        // 金币
        SetText(txtCoin,playerData.coin);
        switch (currentIndex)
        {
            case 0:
                SetSprite(imgCurPos,PathDefine.ItemToukui);
                break;

            case 1:
                SetSprite(imgCurPos, PathDefine.ItemBody);
                break;

            case 2:
                SetSprite(imgCurPos, PathDefine.ItemYaobu);
                break;

            case 3:
                SetSprite(imgCurPos, PathDefine.ItemHand);
                break;

            case 4:
                SetSprite(imgCurPos, PathDefine.ItemLeg);
                break;

            case 5:
                SetSprite(imgCurPos, PathDefine.ItemFoot);
                break;

            
        }
        int curStarLv = playerData.strongArr[currentIndex];
        SetText(txtStarLv, "- "+curStarLv+"星级");
        for (int i = 0; i < starTransGrid.childCount; i++)
        {
            Image img = starTransGrid.GetChild(i).GetComponent<Image>();
            if (i < curStarLv) {
                SetSprite(img,PathDefine.SpStar2);
            }
            else
            {
                SetSprite(img, PathDefine.SpStar1);
            }
        }

        int nextStarLv = curStarLv + 1;
        int sumAddHp = resSvc.GetPropAddValPreLv(currentIndex, nextStarLv, 1);
        int sumAddHurt = resSvc.GetPropAddValPreLv(currentIndex, nextStarLv, 2);
        int sumAddDef = resSvc.GetPropAddValPreLv(currentIndex, nextStarLv, 3);

        SetText(propHP1,"+"+sumAddHp);
        SetText(propHurt1,"+"+sumAddHurt);
        SetText(propDef1,"+"+sumAddDef);


        
        nextSc = resSvc.GetStrongCfg(currentIndex,nextStarLv);
        if (nextSc != null)
        {
            SetActive(propHP2, true);
            SetActive(propHurt2, true);
            SetActive(propDef2, true);

            SetActive(costTransRoot, true);
            SetActive(propArr1, true);
            SetActive(propArr2, true);
            SetActive(propArr3, true);

            SetText(propHP2,"强化后 +"+nextSc.addhp);
            SetText(propHurt2,"强化后 +"+nextSc.addhurt);
            SetText(propDef2,"强化后 +"+nextSc.adddef);

            SetText(txtNeedLv,"- 需要等级 ："+nextSc.minlv);
            SetText(txtCostCoin, nextSc.coin);
            SetText(txtCostCrystal,nextSc.crystal+"/"+playerData.crystal);
        }
        else {
            SetActive(propHP2,false);
            SetActive(propHurt2,false);
            SetActive(propDef2,false);

            SetActive(costTransRoot,false);
            SetActive(propArr1,false);
            SetActive(propArr2,false);
            SetActive(propArr3,false);
        }
    }

    private void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetWndState(false);
    }

    private void ClickStrongBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        if (playerData.strongArr[currentIndex] < 10)
        {
            if (playerData.lv < nextSc.minlv)
            {
                GameRoot.AddTips("当前等级不够");
                return;
            }
            if (playerData.coin < nextSc.coin)
            {
                GameRoot.AddTips("当前金币不够");
                return;
            }
            if (playerData.crystal < nextSc.crystal)
            {
                GameRoot.AddTips("当前水晶不够");
                return;
            }

            netSvc.SendMsg(new GameMsg {
                cmd= (int)CMD.ReqStrong,
                reqStrong = new ReqStrong{
                    pos=currentIndex
                }
            });
        }
        else {
            GameRoot.AddTips("星级已经升满");
        }
    }

    public void UpdateUI() {
        audioSvc.PlayUIAudio(Constants.FBItemEnter);

        ClickPosItem(currentIndex);
    }

}
