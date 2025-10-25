using System;

public interface ISelfDestructs
{
    public Action OnSelfDestruct { get; set; }
}