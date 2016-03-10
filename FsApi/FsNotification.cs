namespace FsApi
{
  public class FsNotification
  {
    public string Name { get; internal set; }

    internal object Value { private get; set; }

    public T GetValue<T>()
    {
      return (T)Value;
    }
  }
}
