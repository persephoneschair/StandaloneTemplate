using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class GameplayConfig
{
    public GameplayConfig()
    {
        ID = Guid.NewGuid();
        Epoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private Guid _id;
    public Guid ID
    {
        get { return _id; }
        set { _id = value; }
    }

    private long _epoch;
    public long Epoch
    {
        get { return _epoch; }
        set { _epoch = value; }
    }

    private bool _lockConfig = false;
    public bool LockConfig
    {
        get { return _lockConfig; }
        set
        {
            _lockConfig = value;
        }
    }

    private string _configName = "Default";
    public string ConfigName
    {
        get { return _configName; }
        set
        {
            _configName = value;
            //(MainMenuManager.Get.GetGameplayConfig() as GameplayConfigManager).BuildDropdown();
        }
    }

    private bool _isCurrent = true;
    public bool IsCurrent
    {
        get { return _isCurrent; }
        set
        {
            _isCurrent = value;
        }
    }

    private float _timeAvailable = 180f;
    public float TimeAvailable
    {
        get { return _timeAvailable; }
        set
        {
            _timeAvailable = value;
            OnPropertyChanged();
        }
    }


    public void OnPropertyChanged()
    {
        //PersistenceManager.CurrentGameplayConfig = this;
    }
}
