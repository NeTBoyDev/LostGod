using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInvokable
{
    public void Invoke();
}


public interface IInvokable<T>
{
    public void Invoke(T parameter);
}

public enum InvokeType{Event,Aggressive,Quest,Stop}