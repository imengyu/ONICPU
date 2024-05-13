using System.Diagnostics;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{name}")]
public class DigitBattery : Battery
{
	public static readonly HashedString PORT_ID = "BatterySmartLogicPort";

	[MyCmpGet]
	private LogicPorts logicPorts;

	private MeterController logicMeter;

	private static readonly EventSystem.IntraObjectHandler<DigitBattery> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<DigitBattery>(delegate(DigitBattery component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<DigitBattery> UpdateLogicCircuitDelegate = new EventSystem.IntraObjectHandler<DigitBattery>(delegate(DigitBattery component, object data)
	{
		component.UpdateLogicCircuit(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		CreateLogicMeter();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		Subscribe(-592767678, UpdateLogicCircuitDelegate);
	}

	private void CreateLogicMeter()
	{
		logicMeter = new MeterController(GetComponent<KBatchedAnimController>(), "logicmeter_target", "logicmeter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		UpdateLogicCircuit(null);
	}

	private void UpdateLogicCircuit(object data)
	{
		float num = Mathf.RoundToInt(base.PercentFull * 100f);
		logicPorts.SendSignal(PORT_ID, (int)num);
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == PORT_ID)
		{
			SetLogicMeter(LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue));
		}
	}

	public void SetLogicMeter(bool on)
	{
		if (logicMeter != null)
		{
			logicMeter.SetPositionPercent(on ? 1f : 0f);
		}
	}
}
