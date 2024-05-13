using KSerialization;
using ONICPU.screens;
using UnityEngine;
using static ONICPU.sensor.DigitPressureSensor;

[SerializationConfig(MemberSerialization.OptIn)]
public class DigitMassSensor : Switch, ISaveLoadable, IDigitCommonSensor, IDigitPressureSensor
{
  [MyCmpGet]
  private LogicPorts logicPorts;

  private bool was_pressed;

  private bool was_on;

  private float massLast;
  [Serialize]
  private float massSolid;
  [Serialize]
  private float massPickupables;
  [Serialize]
  private float massActivators;
  [Serialize]
  public Unit unit = Unit.kg;

  public Unit OutputUnit
  {
    get { return unit; }
    set { 
      unit = value;
      UpdateLogicCircuit();
    }
  }

  private float toggleCooldown = 0.15f;

  private HandleVector<int>.Handle solidChangedEntry;

  private HandleVector<int>.Handle pickupablesChangedEntry;

  private HandleVector<int>.Handle floorSwitchActivatorChangedEntry;
 
  public float CurrentValue => massSolid + massPickupables + massActivators;
  public LocString ThresholdValueName => STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE;
  public string Format(float value, bool units)
  {
    GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.Kilogram;
    bool includeSuffix = units;
    return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.None, massFormat, includeSuffix);
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
  }
  protected override void OnSpawn()
  {
    base.OnSpawn();
    UpdateVisualState(force: true);
    int cell = Grid.CellAbove(this.NaturalBuildingCell());
    solidChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.SolidChanged", base.gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
    pickupablesChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.PickupablesChanged", base.gameObject, cell, GameScenePartitioner.Instance.pickupablesChangedLayer, OnPickupablesChanged);
    floorSwitchActivatorChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.SwitchActivatorChanged", base.gameObject, cell, GameScenePartitioner.Instance.floorSwitchActivatorChangedLayer, OnActivatorsChanged);
    base.OnToggle += SwitchToggled;
  }
  protected override void OnCleanUp()
  {
    GameScenePartitioner.Instance.Free(ref solidChangedEntry);
    GameScenePartitioner.Instance.Free(ref pickupablesChangedEntry);
    GameScenePartitioner.Instance.Free(ref floorSwitchActivatorChangedEntry);
    base.OnCleanUp();
  }

  private void Update()
  {
    toggleCooldown = Mathf.Max(0f, toggleCooldown - Time.deltaTime);
    if (toggleCooldown == 0f)
    {
      float currentValue = CurrentValue;
      if (massLast != currentValue)
      {
        massLast = currentValue;
        UpdateLogicCircuit();
        UpdateVisualState();
      }
    }
  }

  private void OnSolidChanged(object data)
  {
    int i = Grid.CellAbove(this.NaturalBuildingCell());
    if (Grid.Solid[i])
    {
      massSolid = Grid.Mass[i];
    }
    else
    {
      massSolid = 0f;
    }
  }
  private void OnPickupablesChanged(object data)
  {
    float num = 0f;
    int cell = Grid.CellAbove(this.NaturalBuildingCell());
    ListPool<ScenePartitionerEntry, LogicMassSensor>.PooledList pooledList = ListPool<ScenePartitionerEntry, LogicMassSensor>.Allocate();
    GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(cell).x, Grid.CellToXY(cell).y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
    for (int i = 0; i < pooledList.Count; i++)
    {
      Pickupable pickupable = pooledList[i].obj as Pickupable;
      if (!(pickupable == null) && !pickupable.wasAbsorbed)
      {
        KPrefabID kPrefabID = pickupable.KPrefabID;
        if (!kPrefabID.HasTag(GameTags.Creature) || kPrefabID.HasTag(GameTags.Creatures.Walker) || kPrefabID.HasTag(GameTags.Creatures.Hoverer) || kPrefabID.HasTag(GameTags.Creatures.Flopping))
        {
          num += pickupable.PrimaryElement.Mass;
        }
      }
    }
    pooledList.Recycle();
    massPickupables = num;
  }
  private void OnActivatorsChanged(object data)
  {
    float num = 0f;
    int cell = Grid.CellAbove(this.NaturalBuildingCell());
    ListPool<ScenePartitionerEntry, LogicMassSensor>.PooledList pooledList = ListPool<ScenePartitionerEntry, LogicMassSensor>.Allocate();
    GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(cell).x, Grid.CellToXY(cell).y, 1, 1, GameScenePartitioner.Instance.floorSwitchActivatorLayer, pooledList);
    for (int i = 0; i < pooledList.Count; i++)
    {
      FloorSwitchActivator floorSwitchActivator = pooledList[i].obj as FloorSwitchActivator;
      if (!(floorSwitchActivator == null))
      {
        num += floorSwitchActivator.PrimaryElement.Mass;
      }
    }
    pooledList.Recycle();
    massActivators = num;
  }
 
  private void SwitchToggled(bool toggled_on)
  {
    UpdateLogicCircuit();
    UpdateVisualState();
  }
  private void UpdateLogicCircuit()
  {
    logicPorts.SendSignal(LogicSwitch.PORT_ID, switchedOn ? (int)transMassUnit(unit, CurrentValue) : 0);
  }
  private void UpdateVisualState(bool force = false)
  {
    bool flag = CurrentValue > 0;
    if (!(flag != was_pressed || was_on != base.IsSwitchedOn || force))
    {
      return;
    }
    KBatchedAnimController component = GetComponent<KBatchedAnimController>();
    if (flag)
    {
      if (force)
      {
        component.Play(base.IsSwitchedOn ? "on_down" : "off_down");
      }
      else
      {
        component.Play(base.IsSwitchedOn ? "on_down_pre" : "off_down_pre");
        component.Queue(base.IsSwitchedOn ? "on_down" : "off_down");
      }
    }
    else if (force)
    {
      component.Play(base.IsSwitchedOn ? "on_up" : "off_up");
    }
    else
    {
      component.Play(base.IsSwitchedOn ? "on_up_pre" : "off_up_pre");
      component.Queue(base.IsSwitchedOn ? "on_up" : "off_up");
    }
    was_pressed = flag;
    was_on = base.IsSwitchedOn;
  }
}