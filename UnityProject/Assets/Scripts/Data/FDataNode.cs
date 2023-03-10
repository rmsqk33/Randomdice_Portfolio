using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class FDataNode
{
    struct FDataAttribute
    {
        public string StrValue;
        public int IntValue;
        public double DoubleValue;
        public bool BoolValue;
        public Color ColorValue;
    }

    Dictionary<string, List<FDataNode>> childNodes = new Dictionary<string, List<FDataNode>>();
    Dictionary<string, FDataAttribute> dataAttributes = new Dictionary<string, FDataAttribute>();

    string name;

    public string Name { get { return name; } } 

    public string GetStringAttr(in string InName)
    {
        if(dataAttributes.ContainsKey(InName))
            return dataAttributes[InName].StrValue;

        return "";
    }

    public int GetIntAttr(in string InName)
    {
        if (dataAttributes.ContainsKey(InName))
            return dataAttributes[InName].IntValue;

        return 0;
    }

    public double GetDoubleAttr(in string InName)
    {
        if (dataAttributes.ContainsKey(InName))
            return dataAttributes[InName].DoubleValue;

        return 0f;
    }

    public bool GetBoolAttr(in string InName)
    {
        if (dataAttributes.ContainsKey(InName))
            return dataAttributes[InName].BoolValue;

        return false;
    }

    public Color GetColorAttr(in string InName)
    {
        Color color = new Color();
        if (dataAttributes.ContainsKey(InName))
            color = dataAttributes[InName].ColorValue;

        return color;
    }

    public FDataNode GetDataNodeWithQuery(in string InQuery)
    {
        int nextQueryIndex = InQuery.IndexOf('.');
        string query = nextQueryIndex == -1 ? InQuery : InQuery.Substring(0, nextQueryIndex);

        char[] delimeters = {'[', '@', '=', ']'};
        string[] nodeQuerys = query.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
        string nodeName = nodeQuerys[0];
        string attrName = 1 < nodeQuerys.Length ? nodeQuerys[1] : null;
        string attrValue = 2 < nodeQuerys.Length ? nodeQuerys[2] : null;

        FDataNode retNode = null;

        if (childNodes.ContainsKey(nodeName))
        {
            foreach (FDataNode node in childNodes[nodeName])
            {
                if (attrName != null && attrValue != null)
                {
                    if (node.GetStringAttr(attrName) != attrValue)
                        continue;
                }

                if (nextQueryIndex == -1)
                {
                    retNode = node;
                    break;
                }

                retNode = node.GetDataNodeWithQuery(InQuery.Substring(nextQueryIndex + 1, InQuery.Length - nextQueryIndex - 1));
                if (retNode != null)
                    break;
            }
        }

        return retNode;
    }

    public List<FDataNode> GetDataNodesWithQuery(in string InQuery)
    {
        int nextQueryIndex = InQuery.IndexOf('.');
        string query = nextQueryIndex == -1 ? InQuery : InQuery.Substring(0, nextQueryIndex);

        char[] delimeters = { '@', '=' };
        string[] nodeQuerys = query.Split(delimeters);
        string nodeName = nodeQuerys[0];
        string attrName = 1 < nodeQuerys.Length ? nodeQuerys[1] : null;
        string attrValue = 2 < nodeQuerys.Length ? nodeQuerys[2] : null;

        List<FDataNode> retNodeList = new List<FDataNode>();

        if (childNodes.ContainsKey(nodeName))
        {
            foreach (FDataNode node in childNodes[nodeName])
            {
                if (attrName != null)
                {
                    if (node.GetStringAttr(attrName) != attrValue)
                        continue;
                }

                if (nextQueryIndex == -1)
                    retNodeList.Add(node);

                retNodeList.AddRange(node.GetDataNodesWithQuery(InQuery.Substring(nextQueryIndex + 1, InQuery.Length - nextQueryIndex - 1)));
            }
        }

        return retNodeList;
    }

    public delegate void ForeachChildNodesFunc(in FDataNode InNode);
    public void ForeachChildNodes(in string InName, in ForeachChildNodesFunc InFunc)
    {
        if (childNodes.ContainsKey(InName) == false)
            return;

        foreach (FDataNode node in childNodes[InName])
        {
            if(node.Name == InName)
            {
                InFunc(node);
            }
        }
    }

    public void AddChild(in FDataNode InChild)
    {
        if (childNodes.ContainsKey(InChild.Name))
            childNodes[InChild.Name].Add(InChild);
        else
        {
            List<FDataNode> newDataNodes = new List<FDataNode>();
            newDataNodes.Add(InChild);
            childNodes.Add(InChild.Name, newDataNodes);
        }
    }

    public void ParseXmlData(in XmlNode InNode)
    {
        name = InNode.Name;

        ParseChild(InNode);
        ParseAttribute(InNode);
    }

    void ParseChild(in XmlNode InNode)
    {
        foreach (XmlNode childNode in InNode.ChildNodes)
        {
            FDataNode newChild = new FDataNode();
            newChild.ParseXmlData(childNode);

            AddChild(newChild);
        }
    }

    void ParseAttribute(in XmlNode InNode)
    {
        foreach (XmlAttribute attr in InNode.Attributes)
        {
            FDataAttribute newAttr = new FDataAttribute();

            bool color = attr.Name == "color";
            if (bool.TryParse(attr.Value, out newAttr.BoolValue)) { }
            else if (int.TryParse(attr.Value, out newAttr.IntValue)) { }
            else if (double.TryParse(attr.Value, out newAttr.DoubleValue)) { }
            else if (ColorUtility.TryParseHtmlString(attr.Value, out newAttr.ColorValue)) { }
            
            newAttr.StrValue = attr.Value;

            dataAttributes.Add(attr.Name, newAttr);
        }
    }
}
