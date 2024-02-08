using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayConfigManager : SubMenuManager
{
    public TMP_Dropdown profileDropdown;
    public TMP_InputField profileNameInput;
    public GenericSlider playerLimitSlider;
    public Toggle shuffleQOrderToggle;
    public GenericSlider gameTimeSlider;

    public RectTransform mainWindowRT;

    public Button backButton;
    public Button newConfigButton;
    public Button deleteButton;

    private bool loaded = false;

    public override void Start()
    {
        base.Start();
        mainWindowRT.localPosition = new Vector3(0, -1500, 0);
        BuildDropdown();
    }

    public void BuildDropdown()
    {
        loaded = false;
        var cgc = PersistenceManager.CurrentGameplayConfig;
        profileDropdown.ClearOptions();
        profileDropdown.AddOptions(PersistenceManager.storedGameplayConfigs.Select(x => x.ConfigName).ToList());
        int index = Array.IndexOf(PersistenceManager.storedGameplayConfigs.ToArray(), cgc);
        profileDropdown.value = index;
        OnChangeProfile(index);
    }

    public void OnChangeProfile(int index)
    {
        loaded = false;
        PersistenceManager.CurrentGameplayConfig = PersistenceManager.storedGameplayConfigs[index];
        ApplyValues();
    }

    private void ApplyValues()
    {
        var cgc = PersistenceManager.CurrentGameplayConfig;
        profileNameInput.text = cgc.ConfigName;
        playerLimitSlider.slider.value = cgc.PlayerLimit;
        shuffleQOrderToggle.isOn = cgc.ShuffleQuestionOrder;
        gameTimeSlider.slider.value = cgc.TimeAvailable;

        if (PersistenceManager.CurrentGameplayConfig != null)
        {
            if (cgc.LockConfig)
            {
                profileNameInput.interactable = false;
                playerLimitSlider.slider.interactable = false;
                shuffleQOrderToggle.interactable = false;
                gameTimeSlider.slider.interactable = false;
                deleteButton.interactable = false;
            }
            else
            {
                profileNameInput.interactable = true;
                playerLimitSlider.slider.interactable = true;
                shuffleQOrderToggle.interactable = true;
                gameTimeSlider.slider.interactable = true;
                deleteButton.interactable = true;
            }
        }
        loaded = true;
    }

    private void SetConfigBack()
    {
        if (!loaded)
            return;

        var cgc = PersistenceManager.CurrentGameplayConfig;
        cgc.ConfigName = profileNameInput.text;
        cgc.PlayerLimit = (int)playerLimitSlider.slider.value;
        cgc.ShuffleQuestionOrder = shuffleQOrderToggle.isOn;
        cgc.TimeAvailable = gameTimeSlider.slider.value;
    }

    public void OnChangeProfileName(string s)
    {
        SetConfigBack();
        BuildDropdown();
    }

    public void OnConfigValuesChanged()
    {
        SetConfigBack();
    }

    public void OnNewConfig()
    {
        loaded = false;
        var def = new GameplayConfig()
        {
            ConfigName = "New Config",
        };
        PersistenceManager.storedGameplayConfigs.Add(def);
        PersistenceManager.CurrentGameplayConfig = def;
        BuildDropdown();
    }

    public void OnDeleteConfig()
    {
        loaded = false;
        PersistenceManager.OnDeleteConfig();
        BuildDropdown();
    }

    public void OnCloseMenu()
    {
        PersistenceManager.WriteGameplayConfigs();
    }
}
