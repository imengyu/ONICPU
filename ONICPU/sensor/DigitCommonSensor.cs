using KSerialization;
using ONICPU.screens;

[SerializationConfig(MemberSerialization.OptIn)]
public class DigitCommonSensor : Switch, ISaveLoadable, ISim200ms, IDigitCommonSensor
{
  [MyCmpGet]
  private LogicPorts logicPorts;

  private float resultTemp = 0;

  public virtual LocString ThresholdValueName => "";
  public virtual string Format(float value, bool units)
  {
    return GameUtil.GetFormattedInt((int)value);
  }
  public virtual float CurrentValue { get { return resultTemp; } }

  protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += OnSwitchToggled;

    KBatchedAnimController component = GetComponent<KBatchedAnimController>();
    component.Play(switchedOn ? "on_pre" : "on_pst");
    component.Queue(switchedOn ? "on" : "off");

    UpdateLogicCircuit();
	}
	protected virtual float simValue(out bool skip)
	{
    skip = false;
    return 0;
	}

	public void Sim200ms(float dt)
	{
    bool skip;
    var tmp = simValue(out skip);
    if (skip)
      return;
    if (tmp != resultTemp)
    {
      resultTemp = tmp;
      UpdateLogicCircuit();
    }
  }
	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
	}
  protected void UpdateLogicCircuit()
  {
    logicPorts.SendSignal(LogicSwitch.PORT_ID, switchedOn ? (int)resultTemp : 0);
  }
}