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

    public Button btnHead;
    public Button btnGuide;
    public Button btnStrong;
    public Button btnChat;

    #endregion

    private bool menuState = true;
    private float pointDis;
    private Vector2 dirPointStartPos = Vector2.zero;
    private Vector2 dirBgDefaultPos = Vector2.zero;

    private AutoGuideCfg curTaskData = null;

    #region MainFUnctions
    protected override void InitWnd()
    {
        base.InitWnd();

        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
        dirBgDefaultPos = imgDirBg.transform.position;

        btnHead.onClick.AddListener(ClickHeadBtn);

        btnMenu.onClick.AddListener(ClickMenuBtn);
        btnGuide.onClick.AddListener(ClickGuideBtn);
        btnStrong.onClick.AddListener(ClickStrongBtn);
        btnChat.onClick.AddListener(ClickChatBtn);
        SetActive(imgDirPoint, false);

        RefreshUI();

        RegisterTouchEvt();
    }

    public void RefreshUI() {
        PlayerData pd = GameRoot.Instance.PlayerData;

        SetText(txtFight, Common.GetFightByProps(pd));
        SetText(txtPower, "体力:" + pd.power + "/" + Common.GetPowerLimit(pd.lv));
        imgPowerPrg.fillAmount = pd.power * 1.0f / Common.GetPowerLimit(pd.lv);
        SetText(txtLevel, pd.lv);
        SetText(txtName, pd.name);

        #region expprg


        // expprg
        int expPrgVal = (int)(pd.exp * 1.0f / Common.GetExpUpValByLv(pd.lv) * 100);
        SetText(txtExpPrg, expPrgVal + "%");
        int index = expPrgVal / 10;

        for (int i = 0; i < expPrgTrans.childCount; i++)
        {
            Image img = expPrgTrans.GetChild(i).GetComponent<Image>();
            if (i < index)
            {
                img.fillAmount = 1;
            }
            else if (i == index)
            {
                img.fillAmount = expPrgVal % 10 * 1.0f / 10;
            } else
            {
                img.fillAmount = 0;
            }
        }

        #endregion

        // 设置自动任务的图标
        curTaskData = resSvc.GetAutoGuideCfg(pd.guideid);
        if (curTaskData != null)
        {
            SetGuideBtnIcon(curTaskData.npcID);
        }
        else {
            SetGuideBtnIcon(-1);
        }


    }

    private void SetGuideBtnIcon(int npcID) {
        string spPath = "";
        Image img = btnGuide.GetComponent<Image>();
        switch (npcID)
        {
            case Constants.NPCWiseMan:
                spPath = PathDefine.WiseManHead;
                break;
            case Constants.NPCGeneral:
                spPath = PathDefine.GeneralHead;
                break;
            case Constants.NPCArtisan:
                spPath = PathDefine.ArtisanHead;
                break;
            case Constants.NPCTrader:
                spPath = PathDefine.TraderHead;
                break;
            default:
                spPath = PathDefine.TaskHead;
                break;
        }

        SetSprite(img, spPath);
    }
    #endregion

    #region ClickEvts

    public void ClickChatBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenChatWnd();
    }

    public void ClickStrongBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenStrongWnd();
    }

    public void ClickGuideBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        if (curTaskData != null)
        {
            MainCitySys.Instance.RunTask(curTaskData);
        }
        else {
            GameRoot.AddTips("更多引导任务，正在开发中...");
        }
    }

    public void ClickMenuBtn() {
        audioSvc.PlayUIAudio(Constants.UIExtenBtn);

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

    public void ClickHeadBtn() {
        audioSvc.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.Instance.OpenInfoWnd();
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
