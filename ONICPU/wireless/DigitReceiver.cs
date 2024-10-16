using KSerialization;

namespace ONICPU.wireless
{
  [SerializationConfig(MemberSerialization.OptIn)]
  public class DigitReceiver : Switch, ISaveLoadable, ISim200ms
  {
    public static readonly HashedString WRITE_PORT_ID = new HashedString("LogicReceiverOut");

    [MyCmpReq]
    private Operational operational;
    private LogicPorts ports;
    private int value = 0;
    private KBatchedAnimController kbac;
    private DigitBroadcastManager manager;
    [Serialize]
    public int Channel = 0;

    private static readonly EventSystem.IntraObjectHandler<DigitReceiver> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<DigitReceiver>(delegate (DigitReceiver component, object data)
    {
      component.OnOperationalChanged(data);
    });

    protected override void OnSpawn()
    {
      base.OnSpawn();
      OnToggle += OnSwitchToggled;
      ports = GetComponent<LogicPorts>();
      kbac = GetComponent<KBatchedAnimController>();
      kbac.Play("on", KAnim.PlayMode.Loop);
      manager = DigitBroadcastManager.GetInstancee();
      Subscribe(-592767678, OnOperationalChangedDelegate);
    }
    protected override void OnCleanUp()
    {
      Unsubscribe(-592767678, OnOperationalChangedDelegate);
      base.OnCleanUp();
    }

    private void OnOperationalChanged(object data)
    {
      OnSwitchToggled(operational.IsOperational);
    }
    private void OnSwitchToggled(bool toggled_on)
    {
      if (toggled_on && operational.IsOperational)
      {
        kbac.Play("on", KAnim.PlayMode.Loop);
        ports.SendSignal(WRITE_PORT_ID, (int)value);
      }
      else
      {
        kbac.Play("off");
        ports.SendSignal(WRITE_PORT_ID, 0);
      }
    }

    public void Sim200ms(float dt)
    {
      if (switchedOn && operational.IsOperational && value != manager.Channels[Channel].Value)
      {
        value = manager.Channels[Channel].Value;
        ports.SendSignal(WRITE_PORT_ID, (int)value);
      }
    }
  }
}
