using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class DigitLightSensor : DigitCommonSensor
{
	private int simUpdateCounter;
	private float[] levels = new float[4];
	private float averageBrightness;

  protected override float simValue(out bool skip)
  {
    if (simUpdateCounter < 4)
    {
      levels[simUpdateCounter] = Grid.LightIntensity[Grid.PosToCell(this)];
      simUpdateCounter++;
      skip = true;
      return 0;
    }
    simUpdateCounter = 0;
    averageBrightness = 0f;
    for (int i = 0; i < 4; i++)
    {
      averageBrightness += levels[i];
    }
    averageBrightness /= 4f;
    skip = false;
    return averageBrightness;
  }

  public override LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.BRIGHTNESS;
  public override string Format(float value, bool units)
  {
    if (units)
    {
      return GameUtil.GetFormattedLux((int)value);
    }
    return $"{(int)value}";
  }
}
