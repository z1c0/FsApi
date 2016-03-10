using System;

namespace FsApi
{
  public class FsException : Exception
  {
    internal FsException(string status)
    {
      Status = status;
    }

    public string Status { get; private set; }
  }
}
