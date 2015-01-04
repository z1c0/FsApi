using System;

namespace FsApi
{
  public class FsResult
  {
    public Exception Error { get; internal set; }
    public bool Succeeded { get { return Error != null; } }
  }

  public class FsResult<T> : FsResult
  {
    public T Value { get; set; }
  }
}