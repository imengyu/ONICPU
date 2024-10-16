using KSerialization;

namespace ONICPU.wireless
{
  [SerializationConfig(MemberSerialization.OptIn)]
  public class DigitBroadcast : Switch, ISaveLoadable
  {
    public static readonly HashedString READ_PORT_ID = new HashedString("LogicRead");

    [MyCmpReq]
    private Operational operational;
    private LogicPorts ports;
    private KBatchedAnimController kbac;
    private DigitBroadcastManager manager;
    [Serialize]
    private int channel = 0;

    public int Channel
    {
      get => channel;
      set {
        if (channel != value)
        {
          ClearBroadcast();
          channel = value;
          UpdateBroadcast(true);
        }
      }
    }

    private static readonly EventSystem.IntraObjectHandler<DigitBroadcast> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<DigitBroadcast>(delegate (DigitBroadcast component, object data)
    {
      component.OnLogicValueChanged(data);
    });
    private static readonly EventSystem.IntraObjectHandler<DigitBroadcast> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<DigitBroadcast>(delegate (DigitBroadcast component, object data)
    {
      component.OnOperationalChanged(data);
    });

    protected override void OnSpawn()
    {
      base.OnSpawn();
      OnToggle += OnSwitchToggled;
      Subscribe(-801688580, OnLogicValueChangedDelegate);
      ports = GetComponent<LogicPorts>();
      kbac = GetComponent<KBatchedAnimController>();
      kbac.Play("on", KAnim.PlayMode.Loop);
      manager = DigitBroadcastManager.GetInstancee();
      Subscribe(-592767678, OnOperationalChangedDelegate);
      UpdateBroadcast();
    }
    protected override void OnCleanUp()
    {
      ClearBroadcast();
      Unsubscribe(-801688580, OnLogicValueChangedDelegate);
      Unsubscribe(-592767678, OnOperationalChangedDelegate);
      base.OnCleanUp();
    }
    private void UpdateBroadcast(bool addRef = false)
    {
      var channelIns = manager.Channels[channel];
      var value = ports.GetInputValue(READ_PORT_ID);
      channelIns.Value = operational.IsOperational ? value : 0;
      if (addRef)
        channelIns.RefCount++;
    }
    private void ClearBroadcast()
    {
      var channelIns = manager.Channels[channel];
      channelIns.RefCount--;
      if (channelIns.RefCount <= 0)
      {
        channelIns.Value = 0;
        channelIns.RefCount = 0;
      }
    }

    private void OnOperationalChanged(object data)
    {
      OnSwitchToggled(operational.IsOperational);
      UpdateBroadcast();
    }
    private void OnSwitchToggled(bool toggled_on)
    {
      if (toggled_on)
        kbac.Play("on", KAnim.PlayMode.Loop);
      else
        kbac.Play("off");
    }
    public void OnLogicValueChanged(object data)
    {
      var logicValueChanged = (LogicValueChanged)data;
      if (logicValueChanged.portID == READ_PORT_ID)
      {
        UpdateBroadcast();
      }
    }
  }
}
