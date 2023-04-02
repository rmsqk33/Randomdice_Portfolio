using FEnum;
using Packet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FBattleWaveController : FControllerBase
{
    int life;
    int totalCard;
    int cardIncrease;
    int wave;
    int summonCount;
    bool startedWave;

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
        }
    }

    public int Life
    {
        get { return life; }
        set
        {
            life = value;
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

    public void StartBattle(int InID)
    {
        battleData = FBattleDataManager.Instance.FindBattleData(InID);
        if (battleData == null)
            return;

        enemySummonTimer.Interval = battleData.summonInterval;

        StartNextWaveAlarm();
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
            StartNextWaveAlarm();
        }
    }

    private void EndBattle()
    {
        startedWave = false;

        FServerManager.Instance.StopP2PServer();
        FServerManager.Instance.ConnectMainServer();
        FAccountMananger.Instance.TryLogin();

        FPopupManager.Instance.OpenBattleResultPopup();

        C_BATTLE_RESULT packet = new C_BATTLE_RESULT();
        packet.battleId = battleData.id;
        packet.clearWave = wave - 1;

        FServerManager.Instance.SendMessage(packet);
    }

    private void StartNextWaveAlarm()
    {
        startedWave = false;

        TotalCard += CardIncrease;
        waveData = battleData.FindWaveData(Wave % battleData.maxWave + 1);
        ++Wave;

        CardIncrease = waveData.card;

        FBattlePanelUI battleUI = FindBattlePanelUI();
        if (battleUI != null)
        {
            battleUI.StartWaveAlarm(Wave);
        }
    }

    private FBattlePanelUI FindBattlePanelUI()
    {
        return FUIManager.Instance.FindUI<FBattlePanelUI>();
    }
}

