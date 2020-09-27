/****************************************************
文件：LoadingWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/23 09:55:05
功能：Loadding UI
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingWnd : WindowRoot
{
    public Text txtTips;
    public Image imgFG;
    public Image imgPonit;
    public Text txtPrg;

    private float fgWidth;

    protected override void InitWnd() {

        base.InitWnd();

        fgWidth = imgFG.GetComponent<RectTransform>().sizeDelta.x;

        SetText( txtTips, "这是一条游戏Tips...");
        SetText(txtPrg,"0%");
        imgFG.fillAmount = 0;
        imgPonit.transform.localPosition = new Vector3(-fgWidth/2,0,0);
    }

    public void SetProgress(float prg) {
        SetText(txtPrg,(int)(prg * 100) + "%");
        imgFG.fillAmount = prg;
        float posX = prg * fgWidth - fgWidth / 2;
        imgPonit.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX,0);
    }
}
