
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
}
