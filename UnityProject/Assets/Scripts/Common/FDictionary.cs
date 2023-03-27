using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keyList;
    [SerializeField]
    List<TValue> valueList;

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        SyncDictionaryFromInspector();
    }

    public void SyncDictionaryFromInspector()
    {
        Clear();
        for (int i = 0; i < keyList.Count; ++i)
        {
            if(base.ContainsKey(keyList[i]) == false)
            {
                base.Add(keyList[i], i < valueList.Count ? valueList[i] : default(TValue));
            }
        }
    }
}
