/****************************************************
文件：PlayerCtrlWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/05 09:16:11
功能：战场玩家操作界面
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrlWnd : WindowRoot
{
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;

    public Text txtLevel;
    public Text txtName;
    public Text txtExpPrg;
    public Transform expPrgTrans;

    public Button btnNomral;
    public Button btnSkill1;
    public Button btnSkill2;
    public Button btnSkill3;

    public Button btnHead;

    private bool menuState = true;
    private float pointDis;
    private Vector2 dirPointStartPos = Vector2.zero;
    private Vector2 dirBgDefaultPos = Vector2.zero;

    // 当前的方向（避免技能完成后方向数据的使用）
    [HideInInspector]
    public Vector2 currentDir;

    private void Start()
    {
        btnNomral.onClick.AddListener(ClickNormalAtkBtn);
        btnSkill1.onClick.AddListener(ClickSkill1Btn);
        btnSkill2.onClick.AddListener(ClickSkill2Btn);
        btnSkill3.onClick.AddListener(ClickSkill3Btn);
        btnHead.onClick.AddListener(ResetSkillCfg);
    }

    protected override void InitWnd()
    {
        base.InitWnd();

        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
        dirBgDefaultPos = imgDirBg.transform.position;
        SetActive(imgDirPoint, false);
        RefreshUI();
        RegisterTouchEvt();
    }

    public void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;

       
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
            }
            else
            {
                img.fillAmount = 0;
            }
        }

        #endregion

        


    }

    #region Click Events

    public void ClickNormalAtkBtn()
    {
        BattleSys.Instance.ReqReleaseSkill(0);

    }

    public void ClickSkill1Btn()
    {
        BattleSys.Instance.ReqReleaseSkill(1);
    }

    public void ClickSkill2Btn()
    {
        BattleSys.Instance.ReqReleaseSkill(2);
    }
    public void ClickSkill3Btn()
    {
        BattleSys.Instance.ReqReleaseSkill(3);
    }

    #endregion


    #region Ctrl



    public void RegisterTouchEvt()
    {

        OnClickDown(imgTouch.gameObject, (pointerEventData) =>
        {
            dirPointStartPos = pointerEventData.position;
            SetActive(imgDirPoint);
            imgDirBg.transform.position = pointerEventData.position;
        });

        OnClickUp(imgTouch.gameObject, (pointerEventData) =>
        {
            imgDirBg.transform.position = dirBgDefaultPos;
            SetActive(imgDirPoint, false);
            imgDirPoint.transform.localPosition = Vector2.zero;

            // TODO 传递方向信息
            Debug.Log(GetType() + "/OnClickUp()/dir：" + Vector2.zero);
            currentDir = Vector2.zero;
            BattleSys.Instance.SetSelfPlayerMoveDir(currentDir);
        });

        OnDrag(imgTouch.gameObject, (pointerEventData) =>
        {
            Vector2 dir = pointerEventData.position - dirPointStartPos;
            float len = dir.magnitude;
            if (len > pointDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.transform.position = dirPointStartPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = pointerEventData.position;
            }

            // TODO 传递方向信息
            Debug.Log(GetType() + "/OnClickUp()/dir：" + dir.normalized);
            currentDir = dir.normalized;
            BattleSys.Instance.SetSelfPlayerMoveDir(currentDir);
        });
    }
    #endregion


    #region 测试方法

    private void ResetSkillCfg() {
        resSvc.ResetSkillCfg();
    }

    #endregion
}
