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

public class MonsterData : BaseData<MonsterData> {
    public int mWave;   //批次
    public int mIndex;   //序號
    public MonsterCfg mCfg;
    public Vector3 mBornPos;
    public Vector3 mBornRot;
    public int mLevel;
}

public class MonsterCfg : BaseData<MonsterCfg> {
    public string mName;
    public string resPath;
    public MonsterType mType;       //1是普通怪物；2是Boss怪物
    public bool isStop;         // 怪物技能是否可以中断
    public int skillID;
    public float atkDis;
    public BattleProps bps;
}

public class SkillMoveCfg : BaseData<SkillCfg>
{
    public int delayTime;
    public int moveTime;
    public float moveDis;
    
}

public class SkillActionCfg : BaseData<SkillCfg>
{
    public int delayTime;        
    public float radius;         //伤害的有效半径
    public float angle;         // 伤害的有效角度
    

}

public class SkillCfg : BaseData<SkillCfg>
{
    public string skillName;
    public int cdTime;
    public int skillTime;
    public int aniAction;
    public string fx;
    public bool isCombo;
    public bool isCollide;
    public bool isBreak;
    public DamageType dmgType;
    public List<int> skillMoveLst;          // 释放技能，人物移动数据
    public List<int> skillActionLst;        // 释放技能，技能的距离的大小与方向
    public List<int> skillDamageLst;        // (这里可能解释不对)释放仅能，技能的伤害（技能可以随着玩家的成长而变化，或者技能是一个范围的随机值）
  
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
    public List<MonsterData> monsterLst;
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

public class BattleProps {
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int dodge;
    public int pierce;
    public int critical;
}
