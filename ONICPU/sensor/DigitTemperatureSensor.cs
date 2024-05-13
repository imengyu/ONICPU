using STRINGS;

namespace ONICPU.sensor
{
  public class DigitTemperatureSensor : DigitCommonSensor
  {
    private HandleVector<int>.Handle structureTemperature;
    public float StructureTemperature => GameComps.StructureTemperatures.GetPayload(structureTemperature).Temperature;

    private float[] temperatures = new float[8];
    private float averageTemp;
    private int simUpdateCounter;

    protected override float simValue(out bool skip)
    {
      if (simUpdateCounter < 8)
      {
        int i = Grid.PosToCell(this);
        if (Grid.Mass[i] > 0f)
        {
          temperatures[simUpdateCounter] = Grid.Temperature[i];
          simUpdateCounter++;
        }
        skip = true;
        return 0;
      }
      simUpdateCounter = 0;
      averageTemp = 0f;
      for (int j = 0; j < 8; j++)
      {
        averageTemp += temperatures[j];
      }
      averageTemp /= 8f;
      skip = false;
      return GameUtil.GetConvertedTemperature(averageTemp);
    }
    public override LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;

    public override float CurrentValue => averageTemp;
    public override string Format(float value, bool units)
    {
      return GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, units, roundInDestinationFormat: true);
    }
  }
}
