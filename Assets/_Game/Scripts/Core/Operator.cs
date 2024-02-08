using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public class Operator : SingletonMonoBehaviour<Operator>
{

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        PersistenceManager.OnStartup();
    }
}
