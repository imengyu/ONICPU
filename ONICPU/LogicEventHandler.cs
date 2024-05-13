using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

namespace ONICPU
{
  internal class LogicEventHandler : ILogicEventReceiver, ILogicNetworkConnection, ILogicUIElement, IUniformGridObject
  {
    private int cell;

    private int value;

    private Action<int, int> onValueChanged;

    private Action<int, bool> onConnectionChanged;

    private LogicPortSpriteType spriteType;

    public int Value => value;

    public LogicEventHandler(int cell, Action<int, int> on_value_changed, Action<int, bool> on_connection_changed, LogicPortSpriteType sprite_type)
    {
      this.cell = cell;
      onValueChanged = on_value_changed;
      onConnectionChanged = on_connection_changed;
      spriteType = sprite_type;
    }

    public void ReceiveLogicEvent(int value)
    {
      TriggerAudio(value);
      int arg = this.value;
      this.value = value;
      onValueChanged(value, arg);
    }

    public int GetLogicUICell()
    {
      return cell;
    }

    public LogicPortSpriteType GetLogicPortSpriteType()
    {
      return spriteType;
    }

    public Vector2 PosMin()
    {
      return Grid.CellToPos2D(cell);
    }

    public Vector2 PosMax()
    {
      return Grid.CellToPos2D(cell);
    }

    public int GetLogicCell()
    {
      return cell;
    }

    private void TriggerAudio(int new_value)
    {
      LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(cell);
      SpeedControlScreen instance = SpeedControlScreen.Instance;
      if (networkForCell == null || new_value == value || !(instance != null) || instance.IsPaused || (KPlayerPrefs.HasKey(AudioOptionsScreen.AlwaysPlayAutomation) && KPlayerPrefs.GetInt(AudioOptionsScreen.AlwaysPlayAutomation) != 1 && OverlayScreen.Instance.GetMode() != OverlayModes.Logic.ID))
      {
        return;
      }
      string name = "Logic_Building_Toggle";
      if (CameraController.Instance.IsAudibleSound(Grid.CellToPosCCC(cell, Grid.SceneLayer.BuildingFront)))
      {
        LogicCircuitNetwork.LogicSoundPair logicSoundPair = new LogicCircuitNetwork.LogicSoundPair();
        Dictionary<int, LogicCircuitNetwork.LogicSoundPair> logicSoundRegister = LogicCircuitNetwork.logicSoundRegister;
        int id = networkForCell.id;
        if (!logicSoundRegister.ContainsKey(id))
        {
          logicSoundRegister.Add(id, logicSoundPair);
        }
        else
        {
          logicSoundPair.playedIndex = logicSoundRegister[id].playedIndex;
          logicSoundPair.lastPlayed = logicSoundRegister[id].lastPlayed;
        }
        if (logicSoundPair.playedIndex < 2)
        {
          logicSoundRegister[id].playedIndex = logicSoundPair.playedIndex + 1;
        }
        else
        {
          logicSoundRegister[id].playedIndex = 0;
          logicSoundRegister[id].lastPlayed = Time.time;
        }
        float num = (Time.time - logicSoundPair.lastPlayed) / 3f;
        EventInstance instance2 = KFMOD.BeginOneShot(GlobalAssets.GetSound(name), Grid.CellToPos(cell));
        instance2.setParameterByName("logic_volumeModifer", num);
        instance2.setParameterByName("wireCount", networkForCell.WireCount % 24);
        instance2.setParameterByName("enabled", new_value);
        KFMOD.EndOneShot(instance2);
      }
    }

    public void OnLogicNetworkConnectionChanged(bool connected)
    {
      if (onConnectionChanged != null)
      {
        onConnectionChanged(cell, connected);
      }
    }
  }

}
