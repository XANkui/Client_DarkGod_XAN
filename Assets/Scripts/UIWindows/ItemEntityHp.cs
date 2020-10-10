/****************************************************
文件：ItemEntityHp.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/10 11:19:35
功能：血条特效UI相关
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEntityHp : MonoBehaviour
{
    #region UI Define

    public Image imgHpYellow;
    public Image imgHpRed;

    public Animation criticalAni;
    public Text txtCritical;


    public Animation dodgeAni;
    public Text txtDodge;

    public Animation hpAni;
    public Text txtHp;

    #endregion

    private int hpVal;
    private RectTransform rect;
    private Transform rootTrans;
    // 发屏幕与实际屏幕开的比例
    private float scaleRate = 1.0f * Constants.ScreenStandardHeight / Screen.height;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        #region Test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetCiritical(988);
            SetHurt(456);
            SetDodge();
        }
        #endregion

        // 血条的位置更新
        Vector3 screenPos = Camera.main.WorldToScreenPoint(rootTrans.position);
        rect.anchoredPosition = screenPos * scaleRate;

    }

    public void InitItemInfo(Transform trans,int hp) {
        rootTrans = trans;
        rect = transform.GetComponent<RectTransform>();
        hpVal = hp;
        imgHpYellow.fillAmount = 1;
        imgHpRed.fillAmount = 1;
    }

    public void SetCiritical(int critical) {
        criticalAni.Stop();
        txtCritical.text = "暴击 " + critical;
        criticalAni.Play();
    }

    public void SetDodge()
    {
        dodgeAni.Stop();
        txtDodge.text = "闪避 " ;
        dodgeAni.Play();
    }

    public void SetHurt(int hurt)
    {
        hpAni.Stop();
        txtHp.text = "-" + hurt;
        hpAni.Play();
    }
}
