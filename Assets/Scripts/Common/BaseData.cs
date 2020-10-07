/****************************************************
文件：BaseData.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/27 17:26:18
功能：配置数据
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMoveCfg : BaseData<SkillCfg>
{
    public int delayTime;
    public int moveTime;
    public float moveDis;
    
}

public class SkillCfg : BaseData<SkillCfg>
{
    public string skillName;
    public int skillTime;
    public int aniAction;
    public string fx;
    public List<int> skillMoveLst;
  
}

public class StrongCfg : BaseData<StrongCfg>
{
    public int pos;   
    public int starlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int coin;
    public int crystal;
}

public class AutoGuideCfg : BaseData<AutoGuideCfg> {
    public int npcID;   // 触发任务目标NPC
    public string dialogArr;
    public int actID;
    public int coin;
    public int exp;
}

public class MapCfg : BaseData<MapCfg> {
    public string mapName;
    public string sceneName;
    public int power;
    public Vector3 mainCameraPos;
    public Vector3 mainCameraRot;
    public Vector3 playerBornPos;
    public Vector3 playerBornRot;
}

public class TaskRewardCfg:BaseData<TaskRewardCfg> {
    public string taskName;
    public int count;
    public int exp;
    public int coin;
}

public class TaskRewardData : BaseData<TaskRewardCfg>
{
    
    public int prgs;
    public bool isTakenReward;      // 是否领取奖励

}

public class BaseData<T>
{
    public int ID;
}
