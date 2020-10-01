
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
        Debug.Log(GetType() + "/InitSvc()/Init ResSvc...");

        Instance = this;

        InitRDNameCfg(PathDefine.RDNameCfg);
        InitMapCfg(PathDefine.MapCfg);
        InitGuideCfg(PathDefine.GuideCfg);
        InitStrongCfg(PathDefine.StrongCfg);
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
                    ID = ID
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
    #endregion
}
   

