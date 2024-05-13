using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class DigitCritterCountSensor : DigitCommonSensor
{
	[Serialize]
	public bool countEggs = true;

	[Serialize]
	public bool countCritters = true;

	[Serialize]
	public int countThreshold;

	[Serialize]
	public bool activateOnGreaterThan = true;

	[Serialize]
	public int currentCount;

	private KSelectable selectable;

	private Guid roomStatusGUID;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<DigitCritterCountSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<DigitCritterCountSensor>(delegate(DigitCritterCountSensor component, object data)
	{
		component.OnCopySettings(data);
	});

  public override string Format(float value, bool units)
  {
    return value.ToString();
  }

  public float Threshold
	{
		get
		{
			return countThreshold;
		}
		set
		{
			countThreshold = (int)value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return activateOnGreaterThan;
		}
		set
		{
			activateOnGreaterThan = value;
		}
	}

	public new float CurrentValue => currentCount;

	public float RangeMin => 0f;

	public float RangeMax => 64f;

	public LocString Title => UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TITLE;

	public override LocString ThresholdValueName => UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.VALUE_NAME;

	public string AboveToolTip => UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_BELOW;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 1;

	public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(RangeMax);

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		selectable = GetComponent<KSelectable>();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		DigitCritterCountSensor component = ((GameObject)data).GetComponent<DigitCritterCountSensor>();
		if (component != null)
		{
			countThreshold = component.countThreshold;
			activateOnGreaterThan = component.activateOnGreaterThan;
			countCritters = component.countCritters;
			countEggs = component.countEggs;
		}
	}

  protected override float simValue(out bool skip)
  {
    Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
    if (roomOfGameObject != null)
    {
      currentCount = 0;
      if (countCritters)
      {
        currentCount += roomOfGameObject.cavity.creatures.Count;
      }
      if (countEggs)
      {
        currentCount += roomOfGameObject.cavity.eggs.Count;
      }
      bool state = (activateOnGreaterThan ? (currentCount > countThreshold) : (currentCount < countThreshold));
      SetState(state);
      if (selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
      {
        selectable.RemoveStatusItem(roomStatusGUID);
      }
      skip = false;
      return currentCount;
    }
    else
    {
      if (!selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
      {
        roomStatusGUID = selectable.AddStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom);
      }
      SetState(on: false);
			skip = false;
      return 0;
    }
  }

	public float GetRangeMinInputField()
	{
		return RangeMin;
	}
	public float GetRangeMaxInputField()
	{
		return RangeMax;
	}
	public float ProcessedSliderValue(float input)
	{
		return Mathf.Round(input);
	}
	public float ProcessedInputValue(float input)
	{
		return Mathf.Round(input);
	}
	public LocString ThresholdValueUnits()
	{
		return "";
	}
}
