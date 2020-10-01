/****************************************************
文件：CreateWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/24 08:39:12
功能：角色创建界面
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateWnd : WindowRoot
{
    public InputField iptName;
    public Button btnRand;
    public Button btnEnter;

    protected override void InitWnd()
    {
        base.InitWnd();

        // 随机名字生成
        iptName.text = resSvc.GetRDNameData(false);

        // 按钮绑定事件
        btnRand.onClick.AddListener(ClickRandBtn);
        btnEnter.onClick.AddListener(ClickEnterBtn);
    }

    public void ClickRandBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        string rdName = resSvc.GetRDNameData(false);
        iptName.text = rdName;
    }

    public void ClickEnterBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        if (iptName.text != "")
        {
            // 发送名字数据到服务器，登陆主城
            GameMsg msg = new GameMsg {
                cmd = (int)CMD.ReqRename,
                reqRename = new ReqRename {
                    name = iptName.text
                }
            };
            
            netSvc.SendMsg(msg);
        }
        else {
            GameRoot.AddTips("当前名字不合法");
        }
    }
}
