namespace ONICPU
{
  public class FCPU2Config : FCPU4Config
  {
    public new const string ID = "FCPU2";

    protected override string GetID()
    {
      return ID;
    }
    protected override string GetAnim()
    {
      return "fcpu2_kanim";
    }
    protected override int GetH()
    {
      return 2;
    }
    protected override void OnFCpuAdded(FCPU component)
    {
      component.portCount = 2;
      component.CPUType = FCPU.FCPUType.AssemblyCode;
    }
  }
}
