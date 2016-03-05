namespace FsApi
{
  public abstract class KeyLabelItem
  {
    public int Key { get; internal set; }

    public string Label { get; internal set; }

    public override string ToString()
    {
      return Label;
    }
  }
}
