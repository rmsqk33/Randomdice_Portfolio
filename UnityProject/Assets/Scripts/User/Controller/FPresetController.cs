using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPresetController : FControllerBase
{
    static readonly int MAX_PRESET_PAGE = 5;
    static readonly int MAX_PRESET = 5;
    int[] battleFieldPresetIDList = new int[MAX_PRESET];
    int[,] dicePresetIDList = new int[MAX_PRESET_PAGE, MAX_PRESET];

    public int SelectedPresetIndex { get; private set; } = -1;

    public FPresetController(FLocalPlayer InOwner) : base(InOwner)
    {
    }

    public void Handle_S_USER_DATA(in S_USER_DATA InPacket)
    {
        SelectedPresetIndex = InPacket.selectedPresetIndex;

        Array.Copy(InPacket.battleFieldPreset, battleFieldPresetIDList, InPacket.battleFieldPreset.Length);
        Array.Copy(InPacket.dicePreset, dicePresetIDList, InPacket.dicePreset.Length);

        FDiceInventory diceInventory = FindDiceInventory();
        if (diceInventory != null)
        {
            diceInventory.SetPresetTab(SelectedPresetIndex);
        }

        FBattleFieldInventory battlefieldInventory = FindBattlefieldInventory();
        if (battlefieldInventory != null)
        {
            battlefieldInventory.SetPresetTab(SelectedPresetIndex);
        }
    }

    public void SetPreset(int InIndex)
    {
        if (SelectedPresetIndex == InIndex)
            return;

        SelectedPresetIndex = InIndex;

        FDiceInventory diceInventory = FindDiceInventory();
        if (diceInventory != null)
        {
            diceInventory.SetPresetTab(InIndex);
        }

        FBattleFieldInventory battlefieldInventory = FindBattlefieldInventory();
        if (battlefieldInventory != null)
        {
            battlefieldInventory.SetPresetTab(InIndex);
        }

        C_CHANGE_PRESET packet = new C_CHANGE_PRESET();
        packet.presetIndex = InIndex;
        FServerManager.Instance.SendMessage(packet);
    }

    public void SetBattleFieldPreset(int InID)
    {
        battleFieldPresetIDList[SelectedPresetIndex] = InID;

        FBattleFieldInventory battlefieldInventory = FindBattlefieldInventory();
        if (battlefieldInventory != null)
            battlefieldInventory.SetBattleFieldPreset(InID);

        C_CHANGE_PRESET_BATTLEFIELD packet = new C_CHANGE_PRESET_BATTLEFIELD();
        packet.battlefieldId = InID;
        packet.presetIndex = SelectedPresetIndex;
        FServerManager.Instance.SendMessage(packet);
    }

    public int GetBattleFieldPresetID(int InIndex)
    {
        if (InIndex < 0 || MAX_PRESET <= InIndex)
            return 0;

        return battleFieldPresetIDList[InIndex];
    }

    FBattleFieldInventory FindBattlefieldInventory()
    {
        return FUIManager.Instance.FindUI<FBattleFieldInventory>();
    }

    public void SetDicePreset(int InID, int InIndex)
    {
        C_CHANGE_PRESET_DICE packet = new C_CHANGE_PRESET_DICE();

        FDiceInventory diceInventory = FindDiceInventory();
        int prevIndex = GetDicePresetIndex(InID, SelectedPresetIndex);
        if (prevIndex != -1)
        {
            int prevDiceID = dicePresetIDList[SelectedPresetIndex, InIndex];
            dicePresetIDList[SelectedPresetIndex, prevIndex] = prevDiceID;
            diceInventory.SetDicePreset(prevDiceID, prevIndex);

            packet.diceId = prevDiceID;
            packet.slotIndex = prevIndex;
            packet.presetIndex = SelectedPresetIndex;
            FServerManager.Instance.SendMessage(packet);
        }

        dicePresetIDList[SelectedPresetIndex, InIndex] = InID;
        diceInventory.SetDicePreset(InID, InIndex);

        packet.diceId = InID;
        packet.slotIndex = InIndex;
        packet.presetIndex = SelectedPresetIndex;
        FServerManager.Instance.SendMessage(packet);
    }

    int GetDicePresetIndex(int InID, int InIndex)
    {
        if (0 <= InIndex && InIndex < MAX_PRESET_PAGE)
        {
            for (int i = 0; i < MAX_PRESET; ++i)
            {
                if (dicePresetIDList[InIndex, i] == InID)
                    return i;
            }
        }
        return -1;
    }

    public delegate void ForeachDicePresetHandle(int InID);
    public void ForeachDicePreset(int InIndex, in ForeachDicePresetHandle InFunc)
    {
        if (0 <= InIndex && InIndex < MAX_PRESET_PAGE)
        {
            for (int i = 0; i < MAX_PRESET; ++i)
            {
                InFunc(dicePresetIDList[InIndex, i]);
            }
        }
    }

    FDiceInventory FindDiceInventory()
    {
        return FUIManager.Instance.FindUI<FDiceInventory>();
    }
}
