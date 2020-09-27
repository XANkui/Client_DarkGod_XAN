/****************************************************
文件：MainCityWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/27 10:05:18
功能：主城UI界面
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCityWnd : WindowRoot
{
    #region UIDefine
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;

    public Animation menuAni;
    public Button btnMenu;

    public Text txtFight;
    public Text txtPower;
    public Image imgPowerPrg;
    public Text txtLevel;
    public Text txtName;
    public Text txtExpPrg;
    public Transform expPrgTrans;

    #endregion

    private bool menuState = true;
    private float pointDis;
    private Vector2 dirPointStartPos = Vector2.zero;
    private Vector2 dirBgDefaultPos = Vector2.zero;

    #region MainFUnctions
    protected override void InitWnd()
    {
        base.InitWnd();

        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
        dirBgDefaultPos = imgDirBg.transform.position;

        btnMenu.onClick.AddListener(ClickMenuBtn);
        SetActive(imgDirPoint, false);
        
        RefreshUI();
        
        RegisterTouchEvt();
    }

    private void RefreshUI() {
        PlayerData pd = GameRoot.Instance.PlayerData;

        SetText(txtFight,Common.GetFightByProps(pd));
        SetText(txtPower,"体力:"+pd.power+"/"+Common.GetPowerLimit(pd.lv));
        imgPowerPrg.fillAmount = pd.power*1.0f/ Common.GetPowerLimit(pd.lv);
        SetText(txtLevel,pd.lv);
        SetText(txtName,pd.name);

        // expprg
        int expPrgVal = (int)(pd.exp * 1.0f / Common.GetExpUpValByLv(pd.lv) * 100);
        SetText(txtExpPrg,expPrgVal+"%");
        int index = expPrgVal / 10;

        for (int i = 0; i < expPrgTrans.childCount; i++)
        {
            Image img = expPrgTrans.GetChild(i).GetComponent<Image>();
            if (i<index)
            {
                img.fillAmount = 1;
            }
            else if (i==index)
            {
                img.fillAmount = expPrgVal % 10 * 1.0f / 10;
            }else
            {
                img.fillAmount = 0;
            }
        }
    }
    #endregion

    #region ClickEvts
    public void ClickMenuBtn() {
        audioSvc.PlauUIAudio(Constants.UIExtenBtn);

        menuState = !menuState;
        AnimationClip clip = null;
        if (menuState == true)
        {
            clip = menuAni.GetClip("OpenMCMenu");
        }
        else {
            clip = menuAni.GetClip("CloseMCMenu");
        }

        menuAni.Play(clip.name);
    }

    public void RegisterTouchEvt() {

        OnClickDown(imgTouch.gameObject, (pointerEventData) =>
        {
            dirPointStartPos = pointerEventData.position;
            SetActive(imgDirPoint);
            imgDirBg.transform.position = pointerEventData.position;
        });

        OnClickUp(imgTouch.gameObject, (pointerEventData) =>
        {
            imgDirBg.transform.position = dirBgDefaultPos;
            SetActive(imgDirPoint,false);
            imgDirPoint.transform.localPosition = Vector2.zero;

            // TODO 传递方向信息
            Debug.Log(GetType()+ "/OnClickUp()/dir："+Vector2.zero);
            MainCitySys.Instance.SetMoveDir(Vector2.zero);
        });

        OnDrag(imgTouch.gameObject, (pointerEventData) =>
        {
            Vector2 dir = pointerEventData.position - dirPointStartPos;
            float len = dir.magnitude;
            if (len>pointDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir,pointDis);
                imgDirPoint.transform.position = dirPointStartPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = pointerEventData.position;
            }

            // TODO 传递方向信息
            Debug.Log(GetType() + "/OnClickUp()/dir：" + dir.normalized);
            MainCitySys.Instance.SetMoveDir(dir.normalized);
        });
    }

    #endregion

}
