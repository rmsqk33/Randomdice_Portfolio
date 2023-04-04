using FEnum;
using System;
using System.IO;
using UnityEngine;
using Packet;

public class FAccountMananger : FNonObjectSingleton<FAccountMananger>
{
    string ACCOUNT_DATA_PATH = Application.dataPath + "account_data.json";

    [Serializable]
    public class AccountData
    {
        public string id;
    }

#if DEBUG
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Test()
    {
        Instance.CreateLocalPlayer();
    }
#endif

    public bool TryLogin()
    {
        if (File.Exists(ACCOUNT_DATA_PATH))
            RequestLogin();
        else
            CreateAccount();

        return true;
    }

    public void Handle_S_GUEST_LOGIN(bool InResult)
    {
        if(InResult)
        {
            Instance.CreateLocalPlayer();

            if(FSceneManager.Instance.CurrentSceneType == SceneType.Login)
            {
                FSceneManager.Instance.ChangeScene(SceneType.Lobby);
            }
        }
        else
        {
            CreateAccount();
        }
    }

    public void Handle_S_CREATE_GUEST_ACCOUNT(string InID)
    {
        SaveAccountData(InID);
        RequestLogin();
    }

    private void RequestLogin()
    {
        AccountData accountData = new AccountData();

        string loadJsonStr = File.ReadAllText(ACCOUNT_DATA_PATH);
        accountData = JsonUtility.FromJson<AccountData>(loadJsonStr);

        C_GUEST_LOGIN pkt = new C_GUEST_LOGIN();
        pkt.id = accountData.id;

        FServerManager.Instance.SendMessage(pkt);
    }

    private void SaveAccountData(string InID)
    {
        AccountData accountData = new AccountData();
        accountData.id = InID;

        string saveJsonStr = JsonUtility.ToJson(accountData);
        File.WriteAllText(ACCOUNT_DATA_PATH, saveJsonStr);
    }

    private void CreateAccount()
    {
        C_CREATE_GUEST_ACCOUNT pkt = new C_CREATE_GUEST_ACCOUNT();

        FServerManager.Instance.SendMessage(pkt);
    }

    public void CreateLocalPlayer()
    {
        if (FGlobal.localPlayer == null)
        {
            GameObject gameObject = new GameObject("localPlayer");
            FGlobal.localPlayer = gameObject.AddComponent<FLocalPlayer>();
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }
}
