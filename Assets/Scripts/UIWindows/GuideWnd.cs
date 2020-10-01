/****************************************************
文件：GuideWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/30 08:56:08
功能：引导对话界面
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideWnd : WindowRoot
{
    public Image imgIcon;
    public Text txtName;
    public Text txtContent;
    public Button btnNext;

    private PlayerData playerData;
    private AutoGuideCfg curTaskData;
    private string[] dialogArr;
    private int dialogIndex;

    protected override void InitWnd()
    {
        base.InitWnd();
        btnNext.onClick.AddListener(ClickNextBtn);

        playerData = GameRoot.Instance.PlayerData;
        curTaskData = MainCitySys.Instance.GetAutoGuideCfg();
        dialogArr = curTaskData.dialogArr.Split('#');
        dialogIndex = 1;
        SetTalk();
    }

    void SetTalk() {
        string[] talkArr = dialogArr[dialogIndex].Split('|');
        if (talkArr[0]=="0")
        {
            //自己

            SetSprite(imgIcon,PathDefine.SelfDialogIcon);
            SetText(txtName, playerData.name);
            
        }
        else
        {
            // 对话NPC
            switch (curTaskData.npcID)
            {
                case 0:
                    SetSprite(imgIcon, PathDefine.WiseManDialogIcon);
                    SetText(txtName, "智者");
                    break;
                case 1:
                    SetSprite(imgIcon, PathDefine.GeneralDialogIcon);
                    SetText(txtName, "将军");
                    break;
                case 2:
                    SetSprite(imgIcon, PathDefine.ArtisanDialogIcon);
                    SetText(txtName, "工匠");
                    break;
                case 3:
                    SetSprite(imgIcon, PathDefine.TraderDialogIcon);
                    SetText(txtName, "商人");
                    break;

                default:
                    SetSprite(imgIcon,PathDefine.GuideDialogIcon);
                    SetText(txtName,"香伴");
                    break;
            }
        }

        imgIcon.SetNativeSize();
        SetText(txtContent, talkArr[1].Replace("$name",playerData.name));
    }

    private void ClickNextBtn() {
        dialogIndex++;
        if (dialogIndex == dialogArr.Length)
        {
            // TODO 向服务器发送该自动任务完成，得到奖励
            GameMsg msg = new GameMsg {
                cmd= (int)CMD.ReqGuide,
                reqGuide = new ReqGuide {
                    guideid = curTaskData.ID
                }
            };
            netSvc.SendMsg(msg);

            SetWndState(false);
        }
        else
        {
            SetTalk();
        }
    }
}
