using STRINGS;

namespace ONICPU.sensor
{
  public class DigitRadiationSensor : DigitCommonSensor
  {
    private int simUpdateCounter;
    private float[] radHistory = new float[8];
    private float averageRads;

    protected override float simValue(out bool skip)
    {
      if (simUpdateCounter < 8)
      {
        int i = Grid.PosToCell(this);
        radHistory[simUpdateCounter] = Grid.Radiation[i];
        simUpdateCounter++;
        skip = true;
        return 0;
      }
      simUpdateCounter = 0;
      averageRads = 0f;
      for (int j = 0; j < 8; j++)
      {
        averageRads += radHistory[j];
      }
      averageRads /= 8f;
      skip = false;
      return averageRads;
    }
    public override LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.RADIATION;
    public override string Format(float value, bool units)
    {
      return GameUtil.GetFormattedRads(value);
    }
  }
}
