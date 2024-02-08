using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HackboxConfigManager : SubMenuManager
{
    private const string displayMessage = "The Hackbox room code is\n<size=150%><color=green>{0}</color></size>";
    public TextMeshProUGUI roomCodeMesh;
    public RectTransform mainWindowRT;

    public TMP_InputField operatorNameInput;
    public TextMeshProUGUI operatorDetectionMesh;
    public Button operatorTestButton;
    private bool opTestActive;

    public GenericSlider defaultLabelSizeSlider;
    public GenericSlider defaultTextInputSizeSlider;
    public GenericSlider defaultChoicesSizeSlider;
    public GenericSlider defaultGridColumnsSlider;
    public GenericSlider defaultGridGapSlider;

    public Toggle createOnStartUpToggle;
    public Toggle requireTwitchToggle;
    public Toggle reloadHostToggle;
    public Button initialConnectButton;

    public Button backButton;
    public Button forceNewRoomButton;

    private bool loaded = false;

    public override void Start()
    {
        base.Start();
        mainWindowRT.localPosition = new Vector3(0, -1500, 0);
        roomCodeMesh.text = "ROOM NOT CREATED";
        ApplyValues();
    }

    public override void OnRoomConnected()
    {
        base.OnRoomConnected();
        ApplyValues();
        roomCodeMesh.text = string.Format(displayMessage, HackboxManager.Get.Host.RoomCode);
    }

    private void ApplyValues()
    {
        if(PersistenceManager.HackboxConfig != null)
        {
            operatorNameInput.text = PersistenceManager.HackboxConfig.OperatorName;

            defaultLabelSizeSlider.slider.value = PersistenceManager.HackboxConfig.DefaultLabelSize;
            defaultTextInputSizeSlider.slider.value = PersistenceManager.HackboxConfig.DefaultTextInputSize;
            defaultChoicesSizeSlider.slider.value = PersistenceManager.HackboxConfig.DefaultChoicesSize;
            defaultGridColumnsSlider.slider.value = PersistenceManager.HackboxConfig.DefaultGridColumns;
            defaultGridGapSlider.slider.value = PersistenceManager.HackboxConfig.DefaultGridGap;

            createOnStartUpToggle.isOn = PersistenceManager.HackboxConfig.CreateRoomOnStartup;
            requireTwitchToggle.isOn = PersistenceManager.HackboxConfig.RequireTwitch;
            reloadHostToggle.isOn = PersistenceManager.HackboxConfig.ReloadHost;

            if (createOnStartUpToggle.isOn && !HackboxManager.Get.Host.Connected)
                OnInitialConnect();            

            loaded = true;
        }
    }

    private void SetConfigBack()
    {
        PersistenceManager.HackboxConfig.OperatorName = operatorNameInput.text;

        PersistenceManager.HackboxConfig.DefaultLabelSize = (int)defaultLabelSizeSlider.slider.value;
        PersistenceManager.HackboxConfig.DefaultTextInputSize = (int)defaultTextInputSizeSlider.slider.value;
        PersistenceManager.HackboxConfig.DefaultChoicesSize = (int)defaultChoicesSizeSlider.slider.value;
        PersistenceManager.HackboxConfig.DefaultGridColumns = (int)defaultGridColumnsSlider.slider.value;
        PersistenceManager.HackboxConfig.DefaultGridGap = (int)defaultGridGapSlider.slider.value;

        PersistenceManager.HackboxConfig.CreateRoomOnStartup = createOnStartUpToggle.isOn;
        PersistenceManager.HackboxConfig.RequireTwitch = requireTwitchToggle.isOn;
        PersistenceManager.HackboxConfig.ReloadHost = reloadHostToggle.isOn;

        PersistenceManager.WriteHackboxConfig();
    }

    public void OnConfigValueChanged()
    {
        if (!loaded)
            return;
        SetConfigBack();
    }

    public void Update()
    {
        CheckForConnection();
        CheckForOperator();
    }

    void CheckForConnection()
    {
        if (!HackboxManager.Get.Host.enabled)
            forceNewRoomButton.interactable = false;
        else
            forceNewRoomButton.interactable = true;
    }

    void CheckForOperator()
    {
        if(HackboxManager.Get.Operator != null && HackboxManager.Get.Operator.Name.ToUpperInvariant() == PersistenceManager.HackboxConfig.OperatorName.ToUpperInvariant())
        {
            operatorDetectionMesh.text = "<color=green>OPERATOR ONLINE";
            operatorNameInput.interactable = false;
            operatorTestButton.interactable = !opTestActive;
        }
        else
        {
            operatorDetectionMesh.text = "<color=red>NO OPERATOR DETECTED";
            operatorNameInput.interactable = true;
            operatorTestButton.interactable = false;
        }
    }

    public void OnInitialConnect()
    {
        HackboxManager.Get.Host.Connect();
        initialConnectButton.interactable = false;
    }

    public void OnForceNewRoom()
    {
        parentButton.GetComponent<Button>().interactable = false;
        HackboxManager.Get.ResetConnection();
        MainMenuManager.Get.OnClickMenuButton(MainMenuManager.NavigationButton.Home);
    }

    public void OnTestOperator()
    {
        opTestActive = true;
        HackboxManager.Get.DeployOperatorState(HackboxManager.OperatorState.OperatorTest);
    }

    public void OnCloseMenu()
    {
        PersistenceManager.WriteHackboxConfig();
        OnEndTest();
    }

    public void OnEndTest()
    {
        if (HackboxManager.Get.Operator != null)
            HackboxManager.Get.DeployInformationState(HackboxManager.Get.Operator, HackboxManager.InformationState.OperatorWelcome);
        opTestActive = false;
    }
}
