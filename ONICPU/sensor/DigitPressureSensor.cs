using KSerialization;
using ONICPU.screens;
using STRINGS;

namespace ONICPU.sensor
{
  [SerializationConfig(MemberSerialization.OptIn)]
  public class DigitPressureSensor : DigitCommonSensor, IDigitPressureSensor
  {
    private float[] samples = new float[8];
    private int sampleIdx;

    public Element.State desiredState = Element.State.Gas;
    [Serialize]
    public Unit unit = Unit.kg;

    public Unit OutputUnit
    {
      get { return unit; }
      set { 
        unit = value;
        UpdateLogicCircuit();
      }
    }

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      if (unit == Unit.none)
        unit = GameUtil.massUnit == GameUtil.MassUnit.Kilograms ? Unit.kg : Unit.pounds;
    }

    public enum Unit
    {
      none,
      μg,
      mg,
      g,
      kg,
      t,
      pounds,
      gr,
      dr,
    }

    public static float transMassUnit(Unit unit, float kg)
    {
      switch (unit)
      {
        case Unit.t: return kg / 1000;
        case Unit.kg: return kg;
        case Unit.g: return kg * 1000;
        case Unit.mg: return kg * 1000000;
        case Unit.μg: return kg * 1000000000;
        case Unit.pounds: return kg / 2.2f;
        case Unit.dr: return kg / 2.2f * 256f;
        case Unit.gr: return kg / 2.2f * 7000f;
      }
      return kg;
    }

    protected override float simValue(out bool skip)
    {
      int num = Grid.PosToCell(this);
      if (sampleIdx < 8)
      {
        float num2 = (Grid.Element[num].IsState(desiredState) ? Grid.Mass[num] : 0f);
        samples[sampleIdx] = num2;
        sampleIdx++;
        skip = true;
        return 0;
      }
      sampleIdx = 0;
      skip = false;
      return transMassUnit(unit, CurrentValue);
    }

    public override float CurrentValue
    {
      get
      {
        float num = 0f;
        for (int i = 0; i < 8; i++)
        {
          num += samples[i];
        }
        return num / 8f;
      }
    }
    public override LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE;
    public override string Format(float value, bool units)
    {
      GameUtil.MetricMassFormat massFormat = ((desiredState != Element.State.Gas) ? GameUtil.MetricMassFormat.Kilogram : GameUtil.MetricMassFormat.Gram);
      bool includeSuffix = units;
      return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.None, massFormat, includeSuffix);
    }
  }
}
