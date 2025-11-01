using System;
using UnityEngine;
[CreateAssetMenu(menuName = "Events/SOEvent")]
public class SOEvent : ScriptableObject
{
    public event Action<object> OnRaised;
    public void Raise(object sender) => OnRaised?.Invoke(sender);
}
