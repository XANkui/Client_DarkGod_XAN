/****************************************************
文件：ChatWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/02 08:59:37
功能：聊天界面
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ChatType {
    World=0,        //世界
    Guild=1,        //公会
    Friend=2,        //好友
}

public class ChatWnd : WindowRoot
{
    public Button btnClose;
    public Button btnWorld;
    public Button btnGuild;
    public Button btnFriend;
    public Text txtChat;

    public InputField iptChat;
    public Button btnSend;

    private Image imgWorld;
    private Image imgGuild;
    private Image imgFriend;

    private ChatType chatType;
    private List<string> chatLst = new List<string>();

    protected override void InitWnd()
    {
        base.InitWnd();
        imgWorld = btnWorld.GetComponent<Image>();
        imgGuild = btnGuild.GetComponent<Image>();
        imgFriend = btnFriend.GetComponent<Image>();

        btnClose.onClick.AddListener(ClickCloseBtn);
        btnWorld.onClick.AddListener(ClickWorldBtn);
        btnGuild.onClick.AddListener(ClicGuildBtn);
        btnFriend.onClick.AddListener(ClickFriendBtn);

        btnSend.onClick.AddListener(ClickSendBtn);
        
        chatType = ChatType.World;

        RefreshUI();
    }

    public void AddChatMsg(string name, string chat) {
        if (name.Equals(GameRoot.Instance.PlayerData.name))
        {
            chatLst.Add(Constants.Color(name + ":", TxtColor.Red) + chat);

        }
        else {
            chatLst.Add(Constants.Color(name + ":", TxtColor.Blue) + chat);

        }
        if (chatLst.Count > 12)
        {
            chatLst.RemoveAt(0);
        }

        if (GetWndState() == true)
        {
            RefreshUI();
        }
        
    }

    private void RefreshUI() {
        switch (chatType)
        {
            case ChatType.World:
                string chatMsg = "";
                for (int i = 0; i < chatLst.Count; i++)
                {
                    chatMsg += chatLst[i] + "\n";
                }
                SetText(txtChat,chatMsg);
                SetSprite(imgWorld,PathDefine.SpBtnType1);
                SetSprite(imgGuild,PathDefine.SpBtnType2);
                SetSprite(imgFriend,PathDefine.SpBtnType2);
                break;
            case ChatType.Guild:

           
                SetText(txtChat, "尚未加入公会");
                SetSprite(imgWorld, PathDefine.SpBtnType2);
                SetSprite(imgGuild, PathDefine.SpBtnType1);
                SetSprite(imgFriend, PathDefine.SpBtnType2);
                break;
            case ChatType.Friend:

                SetText(txtChat, "暂无好友信息");
                SetSprite(imgWorld, PathDefine.SpBtnType2);
                SetSprite(imgGuild, PathDefine.SpBtnType2);
                SetSprite(imgFriend, PathDefine.SpBtnType1);
                break;
            default:
                break;
        }
    }

    #region Click Events

    public void ClickSendBtn() {
        if (iptChat.text != null && iptChat.text.Trim() != "") {
            if (iptChat.text.Length > 12)
            {
                GameRoot.AddTips("输入不能超过12个字");
            }
            else {
                // 发送信息给服务器
                GameMsg msg = new GameMsg {
                    cmd = (int)CMD.SndChat,
                    sndChat = new SndChat {
                        chat = iptChat.text
                    }
                };

                iptChat.text = "";
                netSvc.SendMsg(msg);
            }
        }
        else{
            GameRoot.AddTips("尚未输入聊天信息");
        }
    }

    private void ClickWorldBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = ChatType.World;
        RefreshUI();
    }

    private void ClicGuildBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = ChatType.Guild;
        RefreshUI();

    }

    private void ClickFriendBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = ChatType.Friend;
        RefreshUI();

    }

    public void ClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        chatType = ChatType.World;
        SetWndState(false);
    }

    #endregion


}
