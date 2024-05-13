using KSerialization;
using static ONICPU.STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class DigitTimeOfDaySensor : DigitCommonSensor
{
	private int tick = 0;

  protected override float simValue(out bool skip)
  {
		if (tick < 4) {
      tick++;
			skip = true;
			return 0;
    }

    tick = 0;
    skip = false;
    float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
    return currentCycleAsPercentage * 100;
  }
  public override LocString ThresholdValueName => UI.UISIDESCREENS.DIGITTIMEOFDAYSENSOR.PRECENT;
  public override string Format(float value, bool units)
  {
    return value + "%";
  }
}
