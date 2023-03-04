using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

public class FDataCenter : FNonObjectSingleton<FDataCenter>
{
    FDataNode m_RootNode = new FDataNode();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize() 
    {
        string dataPath = Application.dataPath + "/Resources/Data/";
        Instance.ParseXML(dataPath);

        FDiceDataManager.Instance.Initialize();
        FBattleFieldDataManager.Instance.Initialize();
        FStoreDataManager.Instance.Initialize();
    }

    void ParseXML(in string InPath)
    {
        if (!Directory.Exists(InPath))
            return;

        string[] allFiles = Directory.GetFiles(InPath, "*.xml", SearchOption.AllDirectories);
        foreach(string file in allFiles)
        {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(file);

                FDataNode newNode = new FDataNode();
                newNode.ParseXmlData(xmlDocument.FirstChild);
                m_RootNode.AddChild(newNode);
        }
    }

    public FDataNode GetDataNodeWithQuery(in string InQuery)
    {
        return m_RootNode.GetDataNodeWithQuery(InQuery);
    }

    public List<FDataNode> GetDataNodesWithQuery(in string InQuery)
    {
        return m_RootNode.GetDataNodesWithQuery(InQuery);
    }

    public int GetIntAttribute(in string InQuery)
    {
        FDataNode node = GetDataNodeWithQuery(InQuery);
        if (node == null)
            return 0;

        string attrName = GetAttrNameInQuery(InQuery);
        return node.GetIntAttr(attrName);
    }

    public bool GetBoolAttribute(in string InQuery)
    {
        FDataNode node = GetDataNodeWithQuery(InQuery);
        if (node == null)
            return false;

        string attrName = GetAttrNameInQuery(InQuery);
        return node.GetBoolAttr(attrName);
    }

    public double GetDoubleAttribute(in string InQuery)
    {
        FDataNode node = GetDataNodeWithQuery(InQuery);
        if (node == null)
            return 0f;

        string attrName = GetAttrNameInQuery(InQuery);
        return node.GetDoubleAttr(attrName);
    }

    public string GetStringAttribute(in string InQuery)
    {
        FDataNode node = GetDataNodeWithQuery(InQuery);
        if (node == null)
            return "";

        string attrName = GetAttrNameInQuery(InQuery);
        return node.GetStringAttr(attrName);
    }

    string GetAttrNameInQuery(in string InQuery)
    {
        int attrIndex = InQuery.LastIndexOf('@');
        return InQuery.Substring(attrIndex + 1, InQuery.Length - attrIndex - 1);
    }
}
