/****************************************************
文件：LoginWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/23 20:46:29
功能：登陆界面
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginWnd : WindowRoot
{
    public InputField iptAcct;
    public InputField iptPswd;
    public Button btnNotice;
    public Button btnEnter;

    protected override void InitWnd()
    {
        base.InitWnd();

        // 获取本地存储的账号密码
        if (PlayerPrefs.HasKey(Acct) && PlayerPrefs.HasKey(Pass))
        {
            iptAcct.text = PlayerPrefs.GetString(Acct);
            iptPswd.text = PlayerPrefs.GetString(Pass);
        }
        else {
            iptAcct.text = "";
            iptPswd.text = "";
        }

        // 按钮事件
        btnEnter.onClick.AddListener(ClickEnterBtn);
        btnNotice.onClick.AddListener(ClickNoticeBtn);
    }

    

    /// <summary>
    /// 进入游戏
    /// 更新/存储本地账号密码
    /// </summary>
    public void ClickEnterBtn() {
        audioSvc.PlayUIAudio(Constants.UILoginBtn);

        string acct = iptAcct.text;
        string pass = iptPswd.text;
        if (acct != "" && pass != "")
        {
            // 更新本地保存
            PlayerPrefs.SetString(Acct, acct);
            PlayerPrefs.SetString(Pass, pass);

            // TODO 发送网络消息，请求登录
            GameMsg gameMsg = new GameMsg {
                cmd = (int)CMD.ReqLogin,
                reqLogin = new ReqLogin {
                    acct = acct,
                    pass = pass
                }
            };
            netSvc.SendMsg(gameMsg);
                       
        }
        else {
            GameRoot.AddTips("账号密码不能为空");
        }
    }
    /// <summary>
    /// 公告按钮
    /// </summary>
    public void ClickNoticeBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);

        GameRoot.AddTips("正在研发中...");
    }

    private const string Acct = "Acct";
    private const string Pass = "Pass";
}
