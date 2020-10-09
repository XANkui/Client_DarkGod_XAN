
/****************************************************
文件：ResSvc.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/22 22:32:52
功能：资源管理服务
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResSvc : MonoBehaviour
{
    public static ResSvc Instance = null;

    public void InitSvc()
    {
        

        Instance = this;

        InitRDNameCfg(PathDefine.RDNameCfg);

        InitMonsterCfg(PathDefine.MonsterCfg);
        InitMapCfg(PathDefine.MapCfg);

        InitGuideCfg(PathDefine.GuideCfg);
        InitStrongCfg(PathDefine.StrongCfg);
        InitTaskRewardCfg(PathDefine.TaskRewardCfg);

        InitSkillCfg(PathDefine.SkillCfg);
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);
        InitSkillActionCfg(PathDefine.SkillActionCfg);        

        Debug.Log(GetType() + "/InitSvc()/Init ResSvc...");
    }

    public void ResetSkillCfg() {
        skillDic.Clear();
        skillMoveDic.Clear();

        InitSkillCfg(PathDefine.SkillCfg);
        InitSkillMoveCfg(PathDefine.SkillMoveCfg);

        Debug.Log(GetType() + "/ResetSkillCfg()/...");
    }

    #region 異步加載場景
    /// <summary>
    /// 異步加載場景
    /// </summary>
    /// <param name="sceneName"></param>
    public void AsyncLoadScene(string sceneName, Action loaded)
    {


        GameRoot.Instance.loadingWnd.SetWndState();

        // 异步加载场景
        AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
        prgCB = () =>
        {
            float prg = sceneAsync.progress;
            GameRoot.Instance.loadingWnd.SetProgress(prg);

            //Debug.Log(GetType()+ "/AsyncLoadScene()/prg = "+prg);

            if (prg == 1)
            {


                if (loaded != null)
                {
                    loaded();
                }

                prgCB = null;
                sceneAsync = null;
                GameRoot.Instance.loadingWnd.SetWndState(false);
            }
        };

    }

    private void Update()
    {
        if (prgCB != null)
        {
            prgCB();
        }
    }
    #endregion

    private Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
    public GameObject LoadPrefab(string path, bool cache = false)
    {
        GameObject prefab = null;
        if (goDic.TryGetValue(path,out prefab) ==false)
        {
            prefab = Resources.Load<GameObject>(path);
            if (cache ==true)
            {
                goDic.Add(path, prefab);
            }
        }

        GameObject go = null;
        if (prefab != null)
        {
            go = Instantiate(prefab);
        }

        return go;
    }

    #region 音频加载
    /// <summary>
    /// 加载音频文件
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="cache">是否缓存</param>
    /// <returns></returns>
    public AudioClip LoadAudio(string path, bool cache = false)
    {

        AudioClip au = null;
        if (adDic.TryGetValue(path, out au) == false)
        {
            au = Resources.Load<AudioClip>(path);

            if (cache == true)
            {
                adDic.Add(path, au);
            }
        }

        return au;
    }

    private Action prgCB = null;
    private Dictionary<string, AudioClip> adDic = new Dictionary<string, AudioClip>();

    #endregion

    #region 图片加载
    private Dictionary<string, Sprite> spDic = new Dictionary<string, Sprite>();
    public Sprite LoadSprite(string path, bool cache = false) {
        Sprite sp = null;
        if (spDic.TryGetValue(path,out sp) == false)
        {
            sp = Resources.Load<Sprite>(path);
            if (cache == true)
            {
                spDic.Add(path,sp);
            }
        }

        return sp;
    }

    #endregion

    #region 随机名字 InitCfgs
    private List<string> surnameLst = new List<string>();
    private List<string> manLst = new List<string>();
    private List<string> womanLst = new List<string>();

    private void InitRDNameCfg(string path) {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == false)
        {
            Debug.LogError(GetType() + "/InitRDNameCfg()/xml file:" + path + " not exist");
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "surname":
                            surnameLst.Add(e.InnerText);
                            break;
                        case "man":
                            manLst.Add(e.InnerText);
                            break;
                        case "woman":
                            womanLst.Add(e.InnerText);
                            break;
                        default:
                            break;
                    }
                }
            }

        }
    }

    public string GetRDNameData(bool man =true) {

        System.Random rd = new System.Random();
        string rdName = surnameLst[PETools.RDInt(0,surnameLst.Count-1,rd)];
        if (man == true)
        {
            rdName += manLst[PETools.RDInt(0, manLst.Count - 1, rd)];
        }
        else {
            rdName += womanLst[PETools.RDInt(0, womanLst.Count - 1, rd)];
        }

        return rdName;
    }
    #endregion

    #region 怪物配置
    private Dictionary<int, MonsterCfg> monsterDic = new Dictionary<int, MonsterCfg>();

    private void InitMonsterCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == false)
        {
            Debug.LogError(GetType() + "/InitCityMapCfg()/xml file:" + path + " not exist");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

                MonsterCfg mc = new MonsterCfg
                {
                    ID = ID,
                    bps = new BattleProps()
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "mName":
                            mc.mName = e.InnerText;
                            break;

                        case "resPath":
                            mc.resPath = e.InnerText;
                            break;

                        case "hp":
                            mc.bps.hp = int.Parse(e.InnerText);
                            break;

                        case "ad":
                            mc.bps.ad = int.Parse(e.InnerText);
                            break;

                        case "ap":
                            mc.bps.ap = int.Parse(e.InnerText);
                            break;

                        case "addef":
                            mc.bps.addef = int.Parse(e.InnerText);
                            break;

                        case "apdef":
                            mc.bps.apdef = int.Parse(e.InnerText);
                            break;

                        case "dodge":
                            mc.bps.dodge = int.Parse(e.InnerText);
                            break;

                        case "pierce":
                            mc.bps.pierce = int.Parse(e.InnerText);
                            break;

                        case "critical":
                            mc.bps.critical = int.Parse(e.InnerText);
                            break;

                    }
                }

                monsterDic.Add(ID, mc);
            }

        }
    }

    public MonsterCfg GetMonsterCfg(int id)
    {
        MonsterCfg data = null;
        monsterDic.TryGetValue(id, out data);

        return data;
    }
    #endregion

    #region 主城配置
    private Dictionary<int, MapCfg> mapCfgDataDic = new Dictionary<int, MapCfg>();

    private void InitMapCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == false)
        {
            Debug.LogError(GetType() + "/InitCityMapCfg()/xml file:" + path + " not exist");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

                MapCfg mc = new MapCfg
                {
                    ID = ID,
                    monsterLst = new List<MonsterData>()
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "mapName":
                            mc.mapName = e.InnerText;
                            break;

                        case "sceneName":
                            mc.sceneName = e.InnerText;
                            break;

                        case "power":
                            mc.power = int.Parse(e.InnerText);
                            break;

                        case "mainCamPos":
                            string[] valAttr = e.InnerText.Split(',');
                            mc.mainCameraPos = new Vector3(float.Parse(valAttr[0]),
                                float.Parse(valAttr[1]),
                                float.Parse(valAttr[2]));
                            break;
                        case "mainCamRote":
                            string[] valAttr1 = e.InnerText.Split(',');
                            mc.mainCameraRot = new Vector3(float.Parse(valAttr1[0]),
                                float.Parse(valAttr1[1]),
                                float.Parse(valAttr1[2]));
                            break;

                        case "playerBornPos":
                            string[] valAttr2 = e.InnerText.Split(',');
                            mc.playerBornPos = new Vector3(float.Parse(valAttr2[0]),
                                float.Parse(valAttr2[1]),
                                float.Parse(valAttr2[2]));
                            break;
                        case "playerBornRote":
                            string[] valAttr3 = e.InnerText.Split(',');
                            mc.playerBornRot = new Vector3(float.Parse(valAttr3[0]),
                                float.Parse(valAttr3[1]),
                                float.Parse(valAttr3[2]));
                            break;

                        case "monsterLst":
                            {
                                string[] valArr = e.InnerText.Split('#');
                                for (int waveIndex = 0; waveIndex < valArr.Length; waveIndex++)
                                {
                                    if (waveIndex == 0)
                                    {
                                        continue;
                                    }
                                    string[] tmpArr = valArr[waveIndex].Split('|');
                                    for (int j = 0; j < tmpArr.Length; j++)
                                    {
                                        if (j==0)
                                        {
                                            continue;
                                        }

                                        string[] arr = tmpArr[j].Split(',');
                                        MonsterData md = new MonsterData {
                                            ID = int.Parse(arr[0]),
                                            mWave = waveIndex,
                                            mIndex = j,
                                            mCfg = GetMonsterCfg(int.Parse(arr[0])),
                                            mBornPos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3])),
                                            mBornRot = new Vector3(0, float.Parse(arr[4]), 0),
                                            mLevel = int.Parse(arr[5])
                                        };

                                        mc.monsterLst.Add( md);
                                    }
                                }
                            }
                            break;

                    }
                }

                mapCfgDataDic.Add(ID, mc);
            }

        }
    }

    public MapCfg GetMapCfgData(int id)
    {
        MapCfg data = null;
        mapCfgDataDic.TryGetValue(id, out data);

        return data;
    }
    #endregion

    #region 自动引导配置
    private Dictionary<int, AutoGuideCfg> guideTaskDic = new Dictionary<int, AutoGuideCfg>();
    private void InitGuideCfg(string path) {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == false)
        {
            Debug.LogError(GetType() + "/InitCityMapCfg()/xml file:" + path + " not exist");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

                AutoGuideCfg agc = new AutoGuideCfg
                {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "npcID":
                            agc.npcID = int.Parse( e.InnerText);
                            break;

                        case "dilogArr":
                            agc.dialogArr = (e.InnerText);
                            break;
                        case "actID":
                            agc.actID = int.Parse(e.InnerText);
                            break;

                        case "coin":
                            agc.coin = int.Parse(e.InnerText);
                            break;

                        case "exp":
                            agc.exp = int.Parse(e.InnerText);
                            break;


                    }
                }

                guideTaskDic.Add(ID, agc);
            }

        }
    }

    public AutoGuideCfg GetAutoGuideCfg(int id) {
        AutoGuideCfg agc = null;
        guideTaskDic.TryGetValue(id, out agc);

        return agc;
    }

    #endregion

    #region 强化升级配置

    // 位置（星级，配置）
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();

    private void InitStrongCfg(string path) {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == false)
        {
            Debug.LogError(GetType() + "/InitCityMapCfg()/xml file:" + path + " not exist");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

                StrongCfg sc = new StrongCfg
                {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    int val = int.Parse(e.InnerText); ;
                    switch (e.Name)
                    {
                        case "pos":
                            sc.pos = val;
                            break;
                        case "starlv":
                            sc.starlv = val;
                            break;
                        case "adddef":
                            sc.adddef = val;
                            break;
                        case "addhp":
                            sc.addhp = val;
                            break;
                        case "addhurt":
                            sc.addhurt = val;
                            break;
                        case "minlv":
                            sc.minlv = val;
                            break;
                        case "coin":
                            sc.coin = val;
                            break;
                        case "crystal":
                            sc.crystal = val;
                            break;


                    }
                }

                Dictionary<int, StrongCfg> dic = null;
                if (strongDic.TryGetValue(sc.pos,out dic) == true)
                {
                    dic.Add(sc.starlv,sc);
                }
                else
                {
                    dic = new Dictionary<int, StrongCfg>();
                    dic.Add(sc.starlv,sc);
                    strongDic.Add(sc.pos, dic);
                }

                
            }

        }
    }

    public StrongCfg GetStrongCfg(int pos,int starlv)
    {
        StrongCfg sc = null;
        Dictionary<int, StrongCfg> dic = null;
        
        if (strongDic.TryGetValue(pos, out dic) == true)
        {
            if (dic.ContainsKey(starlv))
            {
                sc = dic[starlv];
            }
        }

        return sc;
    }

    /// <summary>
    /// 获取之前等级的加成属性
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="starlv"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetPropAddValPreLv(int pos,int starlv,int type) {
        Dictionary<int, StrongCfg> posDic = null;
        int val = 0;
        if (strongDic.TryGetValue(pos,out posDic) == true)
        {
            for (int i = 0; i < starlv; i++)
            {
                StrongCfg sc;
                if (posDic.TryGetValue(i,out sc)==true)
                {
                    switch (type)
                    {
                        case 1://hp
                            val += sc.addhp;
                            break;

                        case 2://hurt
                            val += sc.addhurt;
                            break;

                        case 3://def
                            val += sc.adddef;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        return val;
    }

    #endregion

    #region 任务奖励配置
    private Dictionary<int, TaskRewardCfg> taskRwdDic = new Dictionary<int, TaskRewardCfg>();
    private void InitTaskRewardCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == false)
        {
            Debug.LogError(GetType() + "/InitTaskRewardCfg()/xml file:" + path + " not exist");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

                TaskRewardCfg trc = new TaskRewardCfg
                {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "taskName":
                            trc.taskName = e.InnerText;
                            break;

                        case "coin":
                            trc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            trc.exp = int.Parse(e.InnerText);
                            break;

                        case "count":
                            trc.count = int.Parse(e.InnerText);
                            break;

                        


                    }
                }

                taskRwdDic.Add(ID, trc);
            }

        }
    }

    public TaskRewardCfg GetTaskRewardCfg(int id)
    {
        TaskRewardCfg trc = null;
        taskRwdDic.TryGetValue(id, out trc);

        return trc;
    }

    #endregion

    #region 技能配置
    private Dictionary<int, SkillCfg> skillDic = new Dictionary<int, SkillCfg>();
    private void InitSkillCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == false)
        {
            Debug.LogError(GetType() + "/InitTaskRewardCfg()/xml file:" + path + " not exist");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

                SkillCfg sc = new SkillCfg
                {
                    ID = ID,
                    skillMoveLst = new List<int>(),
                    skillActionLst = new List<int>(),
                    skillDamageLst = new List<int>()
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "skillName":
                            sc.skillName = e.InnerText;
                            break;

                        case "skillTime":
                            sc.skillTime = int.Parse(e.InnerText);
                            break;
                        case "aniAction":
                            sc.aniAction = int.Parse(e.InnerText);
                            break;

                        case "fx":
                            sc.fx = (e.InnerText);
                            break;


                        case "dmgType":
                            if (e.InnerText.Equals("1"))
                            {
                                sc.dmgType = DamageType.AD;
                            }
                            else if (e.InnerText.Equals("2"))
                            {
                                sc.dmgType = DamageType.AP;
                            }
                            else {
                                Debug.Log(GetType()+ "/InitSkillCfg()/dmagType value data Error");
                            }
                    
                            
                            break;

                        case "skillMoveLst":
                            string[] skMoveArr = e.InnerText.Split('|');
                            for (int j = 0; j < skMoveArr.Length; j++)
                            {
                                if (skMoveArr[j] != "")
                                {
                                    sc.skillMoveLst.Add( int.Parse(skMoveArr[j]));
                                }
                            }
                            
                            break;

                        case "skillActionLst":
                            string[] skActionArr = e.InnerText.Split('|');
                            for (int j = 0; j < skActionArr.Length; j++)
                            {
                                if (skActionArr[j] != "")
                                {
                                    sc.skillActionLst.Add(int.Parse(skActionArr[j]));
                                }
                            }

                            break;

                        case "skillDamageLst":
                            string[] skDamageArr = e.InnerText.Split('|');
                            for (int j = 0; j < skDamageArr.Length; j++)
                            {
                                if (skDamageArr[j] != "")
                                {
                                    sc.skillDamageLst.Add(int.Parse(skDamageArr[j]));
                                }
                            }

                            break;

                    }
                }

                skillDic.Add(ID, sc);
            }

        }
    }

    public SkillCfg GetSkillCfg(int id)
    {
        SkillCfg sc = null;
        skillDic.TryGetValue(id, out sc);

        return sc;
    }

    #endregion

    #region 技能移动配置
    private Dictionary<int, SkillMoveCfg> skillMoveDic = new Dictionary<int, SkillMoveCfg>();
    private void InitSkillMoveCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == false)
        {
            Debug.LogError(GetType() + "/InitTaskRewardCfg()/xml file:" + path + " not exist");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

                SkillMoveCfg smc = new SkillMoveCfg
                {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "moveDis":
                            smc.moveDis = float.Parse( e.InnerText);
                            break;

                        case "moveTime":
                            smc.moveTime = int.Parse(e.InnerText);
                            break;

                        case "delayTime":
                            smc.delayTime = int.Parse(e.InnerText);
                            break;


                    }
                }

                skillMoveDic.Add(ID, smc);
            }

        }
    }

    public SkillMoveCfg GetSkillMoveCfg(int id)
    {
        SkillMoveCfg smc = null;
        skillMoveDic.TryGetValue(id, out smc);

        return smc;
    }

    #endregion

    #region 技能Action配置
    private Dictionary<int, SkillActionCfg> skillActionDic = new Dictionary<int, SkillActionCfg>();
    private void InitSkillActionCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (xml == false)
        {
            Debug.LogError(GetType() + "/InitTaskRewardCfg()/xml file:" + path + " not exist");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }

                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

                SkillActionCfg sac = new SkillActionCfg
                {
                    ID = ID
                    
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "delayTime":
                            sac.delayTime = int.Parse(e.InnerText);
                            break;

                        case "radius":
                            sac.radius = float.Parse(e.InnerText);
                            break;
                        case "angle":
                            sac.angle = float.Parse(e.InnerText);
                            break;

                        

                    }
                }

                skillActionDic.Add(ID, sac);
            }

        }
    }

    public SkillActionCfg GetSkillActionCfg(int id)
    {
        SkillActionCfg sac = null;
        skillActionDic.TryGetValue(id, out sac);

        return sac;
    }

    #endregion
}


