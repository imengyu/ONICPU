namespace ONICPU.digit
{
  public class DigitSeg32Config : DigitSegBaseConfig
  {
    public override int GetBits()
    {
      return 32;
    }
    public override LocString GetPortString()
    {
      return STRINGS.BUILDINGS.PREFABS.DIGITSEG32.LOGIC_PORT;
    }
  }
}
