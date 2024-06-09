namespace ONICPU
{
  public class FCPU2JSConfig : FCPU2Config
  {
    public new const string ID = "FCPU2JS";

    protected override string GetID()
    {
      return ID;
    }
    protected override string GetAnim()
    {
      return "fcpu2_kanim";
    }
    protected override void OnFCpuAdded(FCPU component)
    {
      component.portCount = 2;
      component.CPUType = FCPU.FCPUType.JavaScript;
    }
  }
}
