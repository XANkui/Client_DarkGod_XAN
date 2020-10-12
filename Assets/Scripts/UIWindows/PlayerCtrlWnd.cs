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

    [Header("HP")]
    public Image imgPlayerHp;
    public Text txtHp;
    int HPSum;

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

    private void Update()
    {
        #region skill CD

        float delta = Time.deltaTime;
        if (isSk1CD == true)
        {
            // 图片
            sk1FillCount += delta;
            if (sk1FillCount >= sk1CDTime)
            {
                isSk1CD = false;
                SetActive(imgSk1CD, false);
                sk1FillCount = 0;
            }
            else
            {
                imgSk1CD.fillAmount = 1 - sk1FillCount / sk1CDTime;
            }

            // 文字
            sk1NumCount += delta;
            if (sk1NumCount >= 1)
            {
                sk1NumCount -= 1;
                sk1Num -= 1;
                SetText(txtSk1CD,sk1Num);
            }
        }

        if (isSk2CD == true)
        {
            // 图片
            sk2FillCount += delta;
            if (sk2FillCount >= sk2CDTime)
            {
                isSk2CD = false;
                SetActive(imgSk2CD, false);
                sk2FillCount = 0;
            }
            else
            {
                imgSk2CD.fillAmount = 1 - sk2FillCount / sk2CDTime;
            }

            // 文字
            sk2NumCount += delta;
            if (sk2NumCount >= 1)
            {
                sk2NumCount -= 1;
                sk2Num -= 1;
                SetText(txtSk2CD, sk2Num);
            }
        }

        if (isSk3CD == true)
        {
            // 图片
            sk3FillCount += delta;
            if (sk3FillCount >= sk3CDTime)
            {
                isSk3CD = false;
                SetActive(imgSk3CD, false);
                sk3FillCount = 0;
            }
            else
            {
                imgSk3CD.fillAmount = 1 - sk3FillCount / sk3CDTime;
            }

            // 文字
            sk3NumCount += delta;
            if (sk3NumCount >= 1)
            {
                sk3NumCount -= 1;
                sk3Num -= 1;
                SetText(txtSk3CD, sk3Num);
            }
        }
#endregion

        #region Boss Hp

        if (transBossHPBar.gameObject.activeSelf == true)
        {
            BlendBossHp();
            ImgYellow.fillAmount = currentPrg;
        }

        #endregion
    }

    protected override void InitWnd()
    {
        base.InitWnd();

        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
        dirBgDefaultPos = imgDirBg.transform.position;
        SetActive(imgDirPoint, false);

        HPSum = GameRoot.Instance.PlayerData.hp;
        SetText(txtHp, HPSum + "/"+ HPSum);
        imgPlayerHp.fillAmount = 1;

        sk1CDTime = resSvc.GetSkillCfg(101).cdTime /1000.0f;
        sk2CDTime = resSvc.GetSkillCfg(102).cdTime /1000.0f;
        sk3CDTime = resSvc.GetSkillCfg(103).cdTime /1000.0f;

        // 隐藏 Boss 血条
        SetBossHpBarState(false);

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

    public void SetPlayerHpBarVal(int val) {
        SetText(txtHp, val + "/" + HPSum);
        imgPlayerHp.fillAmount = val * 1.0f / HPSum;
    }

    #region Click Events

    public void ClickNormalAtkBtn()
    {
        BattleSys.Instance.ReqReleaseSkill(0);

    }


    #region Skill 1
    [Header("技能 1 CD")]
    public Image imgSk1CD;
    public Text txtSk1CD;

    private bool isSk1CD = false;
    private float sk1CDTime;

    private int sk1Num;

    private float sk1FillCount = 0;
    private float sk1NumCount = 0;

    public void ClickSkill1Btn()
    {
        if (isSk1CD == false && GetCanRlsSkill()==true)
        {
            BattleSys.Instance.ReqReleaseSkill(1);
            isSk1CD = true;
            SetActive(imgSk1CD);
            imgSk1CD.fillAmount = 1;
            sk1Num = (int)sk1CDTime;
            SetText(txtSk1CD, sk1Num);
        }
    }

    #endregion


    #region skill 2

    [Header("技能 2 CD")]
    public Image imgSk2CD;
    public Text txtSk2CD;

    private bool isSk2CD = false;
    private float sk2CDTime;

    private int sk2Num;

    private float sk2FillCount = 0;
    private float sk2NumCount = 0;

    public void ClickSkill2Btn()
    {
        if (isSk2CD == false && GetCanRlsSkill() == true)
        {
            BattleSys.Instance.ReqReleaseSkill(2);
            isSk2CD = true;
            SetActive(imgSk2CD);
            imgSk2CD.fillAmount = 1;
            sk2Num = (int)sk2CDTime;
            SetText(txtSk2CD, sk2Num);
        }
    }

    #endregion

    #region skill 3

    [Header("技能 3 CD")]
    public Image imgSk3CD;
    public Text txtSk3CD;

    private bool isSk3CD = false;
    private float sk3CDTime;

    private int sk3Num;

    private float sk3FillCount = 0;
    private float sk3NumCount = 0;
    
    public void ClickSkill3Btn()
    {
        if (isSk3CD == false && GetCanRlsSkill() == true)
        {
            BattleSys.Instance.ReqReleaseSkill(3);
            isSk3CD = true;
            SetActive(imgSk3CD);
            imgSk3CD.fillAmount = 1;
            sk3Num = (int)sk3CDTime;
            SetText(txtSk3CD, sk3Num);
        }
    }

    #endregion


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
            //Debug.Log(GetType() + "/OnClickUp()/dir：" + Vector2.zero);
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
            //Debug.Log(GetType() + "/OnClickUp()/dir：" + dir.normalized);
            currentDir = dir.normalized;
            BattleSys.Instance.SetSelfPlayerMoveDir(currentDir);
        });
    }
    #endregion



    public bool GetCanRlsSkill()
    {
        return BattleSys.Instance.battleMgr.CanRlsSkill();
    }


    #region boss 血条
    [Header("boss 血条")]
    public Transform transBossHPBar;
    public Image ImgRed;
    public Image ImgYellow;

   

    public void SetBossHpBarState(bool state, float prg = 1) {
        
        SetActive(transBossHPBar,state);
        ImgRed.fillAmount = prg;
        ImgYellow.fillAmount = prg;
    }


    private float currentPrg = 1;
    private float targetPrg = 1;
    public void SetBossHpBarVal(int oldVal,int newVal, int sumVal) {
        currentPrg = oldVal * 1.0f / sumVal;
        targetPrg = newVal * 1.0f / sumVal;
        ImgRed.fillAmount = targetPrg;
    }

    private void BlendBossHp() {
        if (Mathf.Abs(currentPrg - targetPrg) < Constants.AccelerHpSpeed * Time.deltaTime)
        {
            currentPrg = targetPrg;
        }
        else if (currentPrg > targetPrg)
        {
            currentPrg -= Constants.AccelerHpSpeed * Time.deltaTime;
        }
        else
        {
            currentPrg += Constants.AccelerHpSpeed * Time.deltaTime;
        }
    }

    #endregion

    #region 测试方法

    private void ResetSkillCfg() {
        resSvc.ResetSkillCfg();
    }

    #endregion
}
