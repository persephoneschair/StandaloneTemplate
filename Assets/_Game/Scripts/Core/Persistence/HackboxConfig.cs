using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackboxConfig
{
    public HackboxConfig()
    {

    }

    private string _operatorName = "Operator";
    public string OperatorName
    {
        get { return _operatorName; }
        set
        {
            _operatorName = value;
            OnPropertyChanged();
        }
    }

    private int _defaultLabelSize = 18;
    public int DefaultLabelSize
    {
        get { return _defaultLabelSize; }
        set
        {
            _defaultLabelSize = value;
            OnPropertyChanged();
        }
    }

    private int _defaultTextInputSize = 18;
    public int DefaultTextInputSize
    {
        get { return _defaultTextInputSize; }
        set
        {
            _defaultTextInputSize = value;
            OnPropertyChanged();
        }
    }

    private int _defaultChoicesSize = 18;
    public int DefaultChoicesSize
    {
        get { return _defaultChoicesSize; }
        set
        {
            _defaultChoicesSize = value;
            OnPropertyChanged();
        }
    }

    private int _defaultGridColumns = 4;
    public int DefaultGridColumns
    {
        get { return _defaultGridColumns; }
        set
        {
            _defaultGridColumns = value;
            OnPropertyChanged();
        }
    }

    private int _defaultGridGap = 20;
    public int DefaultGridGap
    {
        get { return _defaultGridGap; }
        set
        {
            _defaultGridGap = value;
            OnPropertyChanged();
        }
    }

    private bool _requireTwitch = false;
    public bool RequireTwitch
    {
        get { return _requireTwitch; }
        set
        {
            _requireTwitch = value;
            OnPropertyChanged();
        }
    }

    private bool _reloadHost = false;
    public bool ReloadHost
    {
        get { return _reloadHost; }
        set
        {
            _reloadHost = value;
            HackboxManager.Get.Host.ReloadHost = value;
            OnPropertyChanged();
        }
    }



    public void OnPropertyChanged()
    {
        PersistenceManager.HackboxConfig = this;
    }
}
