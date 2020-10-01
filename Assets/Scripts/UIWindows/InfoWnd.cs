/****************************************************
文件：InfoWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/29 08:25:04
功能：角色信息展示界面
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoWnd : WindowRoot
{
    public Transform transDetail;
    #region UI Define
    public Text txtInfo;
    public Text txtExp;
    public Image imgExpPrg;
    public Text txtPower;
    public Image imgPowerPrg;

    public Text txtJob;
    public Text txtFight;
    public Text txtHp;
    public Text txtHurt;
    public Text txtDef;

    public Text txtDetailHp;
    public Text txtAd;
    public Text txtAp;
    public Text txtAddef;
    public Text txtApdef;
    public Text txtDodge;
    public Text txtPierce;
    public Text txtCritical;

    public Button btnClose;
    public Button btnDetailClose;
    public Button btnDetail;

    public RawImage imgCharShow;
    #endregion

    private Vector2 startPos;

    protected override void InitWnd()
    {
        base.InitWnd();

        btnClose.onClick.AddListener(ClickCloseBtn);
        btnDetail.onClick.AddListener(ClickDetailBtn);
        btnDetailClose.onClick.AddListener(ClickDetailCloseBtn);
        SetActive(transDetail,false);
        RegTouchEvts();

        RefreshUI();
    }

    private void RegTouchEvts() {
        OnClickDown(imgCharShow.gameObject,(evtData)=> {
            startPos = evtData.position;
            MainCitySys.Instance.SetStartRotate();
        });

        OnDrag(imgCharShow.gameObject, (evtData) => {
            float rotate = -(evtData.position.x - startPos.x)*0.4f;
            MainCitySys.Instance.SetPlayerRotate(rotate);
        });
    }

    void RefreshUI() {
        PlayerData pd = GameRoot.Instance.PlayerData;

        SetText(txtInfo,pd.name+" LV."+pd.lv);
        SetText(txtExp,pd.exp+"/"+Common.GetExpUpValByLv(pd.lv));
        imgExpPrg.fillAmount = pd.exp * 1.0f / Common.GetExpUpValByLv(pd.lv);
        SetText(txtPower,pd.power+"/"+Common.GetPowerLimit(pd.lv));
        imgPowerPrg.fillAmount = pd.power * 1.0f / Common.GetPowerLimit(pd.lv);

        SetText(txtJob,"暗夜精灵");
        SetText(txtFight,Common.GetFightByProps(pd));
        SetText(txtHp,pd.hp);
        SetText(txtHurt,pd.ap+pd.ad);
        SetText(txtDef,pd.apdef+pd.addef);

        // detail 
        SetText(txtDetailHp, pd.hp);
        SetText(txtAd, pd.ad);
        SetText(txtAp, pd.ap);
        SetText(txtAddef, pd.addef);
        SetText(txtApdef, pd.apdef);
        SetText(txtDodge, pd.dodge +"%");
        SetText(txtPierce, pd.pierce+"%");
        SetText(txtCritical, pd.critical + "%");
    }

    public void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        MainCitySys.Instance.CloseInfoWnd();
    }

    public void ClickDetailBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetActive(transDetail);
    }

    public void ClickDetailCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetActive(transDetail,false);
    }
}
