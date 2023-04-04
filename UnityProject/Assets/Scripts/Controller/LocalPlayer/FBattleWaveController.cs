using Packet;

public class FBattleWaveController : FControllerBase, FServerStateObserver
{
    int life = 0;
    int totalCard = 0;
    int cardIncrease = 0;
    int wave = 0;
    int summonCount = 0;
    bool startedWave = false;

    FTimer enemySummonTimer = new FTimer();
    FTimer waveEndCheckTimer = new FTimer(FBattleDataManager.Instance.WaveEndInterval);

    FBattleData battleData;
    FWaveData waveData;

    public FBattleWaveController(FLocalPlayer InOwner) : base(InOwner)
    {
    }

    public int Wave
    {
        get { return wave; }
        set
        {
            wave = value;

            FBattlePanelUI ui = FindBattlePanelUI();
            if (ui != null)
            {
                ui.SetWave(wave);
            }

            if (FGlobal.localPlayer.IsHost)
            {
                P2P_CHANGE_WAVE pkt = new P2P_CHANGE_WAVE();
                pkt.wave = value;
                FServerManager.Instance.SendMessage(pkt);
            }
        }
    }

    public int Life
    {
        get { return life; }
        set
        {
            life = value;

            if (FGlobal.localPlayer.IsHost)
            {
                P2P_CHANGE_LIFE pkt = new P2P_CHANGE_LIFE();
                pkt.life = value;
                FServerManager.Instance.SendMessage(pkt);
            }

            if (life <= 0)
            {
                EndBattle();
            }
        }
    }

    public int TotalCard
    {
        get { return totalCard; }
        set
        {
            totalCard = value;

            FBattlePanelUI ui = FindBattlePanelUI();
            if (ui != null)
            {
                ui.SetTotalCard(totalCard);
            }
        }
    }

    public int CardIncrease
    {
        get { return cardIncrease; }
        set
        {
            cardIncrease = value;

            FBattlePanelUI ui = FindBattlePanelUI();
            if (ui != null)
            {
                ui.SetCardIncrease(cardIncrease);
            }
        }
    }

    public override void Initialize() 
    {
        if (FServerManager.Instance.IsConnected)
        {
            FServerManager.Instance.AddObserver(this);
        }
        else
        {
            FGlobal.localPlayer.IsHost = true;
        }
    }

    public override void Release() 
    {
        FServerManager.Instance.RemoveObserver(this);
    }

    public void OnDisconnectServer()
    {
        FGlobal.localPlayer.IsHost = true;
    }

    public void StartBattle(int InID)
    {
        battleData = FBattleDataManager.Instance.FindBattleData(InID);
        if (battleData == null)
            return;

        enemySummonTimer.Interval = battleData.summonInterval;

        SetNextWave(1);
    }

    public void StartWave()
    {
        summonCount = 0;

        enemySummonTimer.Start();
        waveEndCheckTimer.Start();

        startedWave = true;
    }

    public override void Tick(float InDeltaTime)
    {
        if (FGlobal.localPlayer.IsHost == false)
            return;

        if (startedWave == false)
            return;

        CreateEnemyProcess(InDeltaTime);
        CheckEndWaveProcess(InDeltaTime);
    }

    private void CreateEnemyProcess(float InDeltaTime)
    {
        enemySummonTimer.Tick(InDeltaTime);
        if (enemySummonTimer.IsElapsedCheckTime() == false)
            return;

        int enemyID = waveData.GetEnemyID(summonCount);
        FObjectManager.Instance.CreateEnemy(enemyID);
        ++summonCount;

        if (waveData.SummonCount <= summonCount)
        {
            enemySummonTimer.Stop();
        }
    }

    private void CheckEndWaveProcess(float InDeltaTime)
    {
        if (summonCount < waveData.SummonCount)
            return;

        if (0 < FObjectManager.Instance.EnemyCount)
            return;

        waveEndCheckTimer.Tick(InDeltaTime);
        if (waveEndCheckTimer.IsElapsedCheckTime())
        {
            waveEndCheckTimer.Stop();
            SetNextWave(Wave + 1);
        }
    }

    private void EndBattle()
    {
        startedWave = false;
        
        FServerManager.Instance.CloseP2PServer();
        FServerManager.Instance.DisconnectServer();
        FServerManager.Instance.ConnectMainServer();
        FAccountMananger.Instance.TryLogin();

        C_BATTLE_RESULT packet = new C_BATTLE_RESULT();
        packet.battleId = battleData.id;
        packet.clearWave = wave - 1;

        FServerManager.Instance.SendMessage(packet);

        FPopupManager.Instance.OpenBattleResultPopup();
    }

    public void SetNextWave(int InWave)
    {
        if (Wave == InWave)
            return;

        startedWave = false;

        TotalCard += cardIncrease;
        waveData = battleData.FindWaveData(wave % battleData.maxWave + 1);
        Wave = InWave;

        CardIncrease = waveData.card;

        FBattlePanelUI battleUI = FindBattlePanelUI();
        if (battleUI != null)
        {
            battleUI.StartWaveAlarm(wave);
        }
    }

    private FBattlePanelUI FindBattlePanelUI()
    {
        return FUIManager.Instance.FindUI<FBattlePanelUI>();
    }
}

