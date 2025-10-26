using System;
using UnityEngine;
[RequireComponent(typeof(Canvas))]
public class SetUICam : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Canvas>().worldCamera = UICam.Instance.GetComponent<Camera>();
    }
}