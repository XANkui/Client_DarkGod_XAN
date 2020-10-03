/****************************************************
文件：TaskWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/03 08:52:34
功能：任务奖励系统
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskWnd : WindowRoot
{
    public Transform scrollTrans;
    public Button btnClose;

    private PlayerData playerData = null;
    private List<TaskRewardData> trdLst = new List<TaskRewardData>();

    private void Start()
    {
        btnClose.onClick.AddListener(ClickCloseBtn);
    }

    protected override void InitWnd()
    {
        base.InitWnd();

        playerData = GameRoot.Instance.PlayerData;
        RefreshUI();
    }

    public void RefreshUI() {
        trdLst.Clear();

        List<TaskRewardData> todoLst = new List<TaskRewardData>();
        List<TaskRewardData> DoneLst = new List<TaskRewardData>();

        // 1|0|0#2|0|0#
        for (int i = 0; i < playerData.taskArr.Length; i++)
        {
            string[] taskInfo = playerData.taskArr[i].Split('|');
            TaskRewardData trd = new TaskRewardData {
                ID = int.Parse(taskInfo[0]),
                prgs = int.Parse(taskInfo[1]),
                isTakenReward = taskInfo[2].Equals("1")
            };

            if (trd.isTakenReward == true)
            {
                DoneLst.Add(trd);
            }
            else {
                todoLst.Add(trd);
            }
        }

        trdLst.AddRange(todoLst);
        trdLst.AddRange(DoneLst);

        //  清空所有子物体
        for (int i = 0; i < scrollTrans.childCount; i++)
        {
            Destroy(scrollTrans.GetChild(i).gameObject);
        }

        // 实例化任务列表
        for (int i = 0; i < trdLst.Count; i++)
        {
            GameObject go = resSvc.LoadPrefab(PathDefine.TaskItemPrefab);
            go.transform.SetParent(scrollTrans);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.name = "taskItem_"+i;

            TaskRewardData trd = trdLst[i];
            TaskRewardCfg trc = resSvc.GetTaskRewardCfg(trd.ID);

            SetText(GetTrans(go.transform,"txtName"),trc.taskName);
            SetText(GetTrans(go.transform,"txtPrg"),trd.prgs+"/"+ trc.count);
            SetText(GetTrans(go.transform,"txtExp"),"经验"+trc.exp);
            SetText(GetTrans(go.transform,"txtCoin"),"金币"+trc.coin);

            Image imgPrg = GetTrans(go.transform, "prgBar/prgVal").GetComponent<Image>();
            float val = trd.prgs * 1.0f / trc.count;
            imgPrg.fillAmount = val;

            Button btnTake = GetTrans(go.transform, "btnTake").GetComponent<Button>();
            btnTake.onClick.AddListener(()=> {
                ClickTakeBtn(go.name);
            });

            Transform transComp = GetTrans(go.transform,"imgComp");
            if (trd.isTakenReward == true)
            {
                btnTake.interactable = false;
                SetActive(transComp);
            }
            else
            {
                SetActive(transComp,false);
                if (trd.prgs == trc.count)
                {
                    btnTake.interactable = true;
                }
                else
                {
                    btnTake.interactable = false;

                }
            }

        }
    }

    #region Click Events

    private void ClickTakeBtn(string name)
    {
        Debug.Log(GetType() + "/()/ Click Take Button :" + name);

        string[] nameArr = name.Split('_');
        int index = int.Parse(nameArr[1]);

        GameMsg msg = new GameMsg {
            cmd = (int)CMD.ReqTakeTaskReward,
            reqTakeTaskReward = new ReqTakeTaskReward {
                rid = trdLst[index].ID
            }
        };

        netSvc.SendMsg(msg);

        TaskRewardCfg trc = resSvc.GetTaskRewardCfg(trdLst[index].ID);
        int coin = trc.coin;
        int exp = trc.exp;
        GameRoot.AddTips(Constants.Color("获得奖励 ",TxtColor.Blue)+ Constants.Color("金币 +"+coin +" 经验 +"+exp, TxtColor.Red));
    }

    private void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        SetWndState(false);
    }

    #endregion




}
