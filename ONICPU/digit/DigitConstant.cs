using KSerialization;

namespace ONICPU.digit
{
  [SerializationConfig(MemberSerialization.OptIn)]
  public class DigitConstant : Switch, ISaveLoadable
  {
    private LogicPorts ports;

    [Serialize]
    private int value = 0;

    public int Value
    {
      get { return value; } 
      set {
        this.value = value;
        UpdateLogicCircuit();
      }
    }


    private KBatchedAnimController kbac;
  
    protected override void OnSpawn()
    {
      base.OnSpawn();
      OnToggle += OnSwitchToggled;
      ports = GetComponent<LogicPorts>();
      kbac = GetComponent<KBatchedAnimController>();
      kbac.Play("on");
      UpdateLogicCircuit();
    }

    private void OnSwitchToggled(bool toggled_on)
    {
      UpdateLogicCircuit();
      kbac.Play(toggled_on ? "on" : "off");
    }
    protected void UpdateLogicCircuit()
    {
      if (value > int.MaxValue)
        value = int.MaxValue;
      if (value < int.MinValue)
        value = int.MinValue;
      ports.SendSignal(LogicSwitch.PORT_ID, switchedOn ? (int)value : 0);
    }
  }
}
