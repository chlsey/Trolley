using UnityEngine;
using System;
using TMPro;

public class GetRidofUI : MonoBehaviour
{
    private DateTime time;
    public TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        if ((DateTime.Now - time).TotalSeconds > 7)
        {
            text.enabled = false;
        }
    }
}
