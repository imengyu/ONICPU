namespace ONICPU.digit
{
  public class DigitSeg16Config : DigitSegBaseConfig
  {
    public override int GetBits()
    {
      return 16;
    }
    public override LocString GetPortString()
    {
      return STRINGS.BUILDINGS.PREFABS.DIGITSEG16.LOGIC_PORT;
    }
  }
}
