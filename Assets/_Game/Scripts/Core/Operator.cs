using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using Newtonsoft.Json;

public class Operator : SingletonMonoBehaviour<Operator>
{
    [Header("Game Settings")]
    [Tooltip("Limits the number of accounts that may connect to the room (set to 0 for infinite)")]
    [Range(0, 100)] public int playerLimit;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        PersistenceManager.OnStartup();
    }

    public string playerOutput;
}
