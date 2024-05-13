namespace ONICPU
{
  public class FCPU4JSConfig : FCPU4Config
  {
    public new const string ID = "FCPU4JS";

    protected override string GetID()
    {
      return ID;
    }
    protected override string GetAnim()
    {
      return "fcpu_kanim";
    }
    protected override void OnFCpuAdded(FCPU component)
    {
      component.portCount = 4;
      component.CPUType = FCPU.FCPUType.JavaScript;
    }
  }
}
