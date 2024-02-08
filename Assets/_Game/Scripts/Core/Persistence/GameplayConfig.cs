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

    #region Uneditable

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

    private bool _isCurrent = true;
    public bool IsCurrent
    {
        get { return _isCurrent; }
        set
        {
            _isCurrent = value;
        }
    }

    #endregion

    #region Editable

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

    private int _playerLimit = 40;
    public int PlayerLimit
    {
        get { return _playerLimit; }
        set
        {
            _playerLimit = value;
            OnPropertyChanged();
        }
    }

    private bool _shuffleQuestionOrder = false;
    public bool ShuffleQuestionOrder
    {
        get { return _shuffleQuestionOrder; }
        set
        {
            _shuffleQuestionOrder = value;
        }
    }

    private float _timeAvailable = 15f;
    public float TimeAvailable
    {
        get { return _timeAvailable; }
        set
        {
            _timeAvailable = value;
            OnPropertyChanged();
        }
    }

    #endregion


    public void OnPropertyChanged()
    {
        //PersistenceManager.CurrentGameplayConfig = this;
    }
}
