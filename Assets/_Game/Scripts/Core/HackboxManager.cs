using Hackbox;
using Hackbox.Parameters;
using Hackbox.UI;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QuestionManagement;

public class HackboxManager : SingletonMonoBehaviour<HackboxManager>
{
    public Member Operator;

    #region Presets & Themes

    public Host Host;    

    public enum ThemeType
    {
        Default
    }
    public Theme[] Themes;

    public enum PresetType
    {
        Label,
        TextInput,
        Choices,
        Grid
    }
    public Preset[] Presets;

    #endregion

    #region Default Events

    public void OnRoomCreated(string roomCode)
    {
        DebugLog.Print($"CREATED ROOM {roomCode}", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
    }

    public void OnRoomConnected(string roomCode)
    {
        MainMenuManager.Get.OnRoomConnected();
        DebugLog.Print($"CONNECTED TO ROOM {roomCode}", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Green);
    }

    public void OnRoomDisconnected(string roomCode)
    {
        DebugLog.Print($"DISCONNECTED FROM ROOM {roomCode}", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Red);
    }

    public void OnRoomReconnecting(string roomCode)
    {
        DebugLog.Print($"RECONNECTING TO ROOM {roomCode}", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
    }

    public void OnRoomReconnectFailed(string roomCode)
    {
        DebugLog.Print($"FAILED TO RECONNECT TO ROOM {roomCode}", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Red);
    }

    public void OnMemberJoined(Member mem)
    {
        HandleMemberJoins(mem);
        DebugLog.Print($"{mem.Name} JOINED", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Yellow);
    }

    public void OnMemberKicked(Member mem)
    {
        DebugLog.Print($"{mem.Name} KICKED", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Red);
    }

    public void OnMessage(Message msg)
    {
        HandleResponse(msg);
        //DebugLog.Print($"MESSAGE FROM {msg.Member.Name}", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Blue);
    }

    public void OnPingPong()
    {

    }

    public void ResetConnection()
    {
        foreach (Member mem in Host.AllMembers)
            DeployInformationState(mem, InformationState.RoomDestroyed);

        Invoke("InvokeReset", 0.1f);
    }

    private void InvokeReset()
    {
        Operator = null;
        Host.Disconnect();
        Host.Connect(true);
    }


    #endregion

    #region Theme Builder

    private State GenerateDefaultState(string headerText)
    {
        State newState = new State() { Theme = Themes[(int)ThemeType.Default] };
        newState.SetHeaderText(headerText);
        newState.SetHeaderParameter("align", "center");
        return newState;
    }

    #endregion

    #region Preset Builder

    private UIComponent Label(string labelText)
    {
        return Label(labelText, PersistenceManager.HackboxConfig.DefaultLabelSize);
    }

    private UIComponent Label(string labelText, int fontSize)
    {
        UIComponent label = new UIComponent()
        {
            Name = "labelText",
            Preset = Presets[(int)PresetType.Label],
        };
        label.SetParameterValue("text", labelText);
        label.SetStyleParameterValue("fontSize", $"{fontSize}px");

        return label;
    }

    private UIComponent TextInput(string eventName)
    {
        return TextInput(eventName, PersistenceManager.HackboxConfig.DefaultTextInputSize);
    }

    private UIComponent TextInput(string eventName, int fontSize)
    {
        UIComponent textInput = new UIComponent()
        {
            Name = "textInput",
            Preset = Presets[(int)PresetType.TextInput],
        };
        textInput.SetParameterValue("event", eventName);
        textInput.SetStyleParameterValue("fontSize", $"{fontSize}px");

        return textInput;
    }

    private UIComponent Choices(string eventName, string[] choiceOptions, bool multiSelect = false)
    {
        return Choices(eventName, choiceOptions, PersistenceManager.HackboxConfig.DefaultChoicesSize, multiSelect);
    }

    private UIComponent Choices(string eventName, string[] choiceOptions, int fontSize, bool multiSelect = false)
    {
        if (choiceOptions == null || choiceOptions.Length == 0)
            return null;

        UIComponent choices = new UIComponent()
        {
            Name = "choices",
            Preset = Presets[(int)PresetType.Choices],
        };

        ChoicesParameter ch = new ChoicesParameter(Presets[(int)PresetType.Choices].GetParameter<ChoicesParameter>("choices"));
        var newOption = ch.Value[0];
        for (int i = 0; i < choiceOptions.Length; i++)
        {
            if(i == 0)
            {
                ch.Value[i].Label = choiceOptions[i];
                ch.Value[i].Value = choiceOptions[i];
            }
            else
            {
                var x = new ChoicesParameter.Choice(newOption);
                x.Label = choiceOptions[i];
                x.Value = choiceOptions[i];
                ch.Value.Add(x);
            }
        }

        choices["choices"] = ch;
        choices.SetParameterValue("event", eventName);
        choices.SetParameterValue("multiSelect", multiSelect);
        choices.SetStyleParameterValue("fontSize", $"{fontSize}px");

        return choices;
    }

    private UIComponent Grid(string eventName, string[] gridOptions, bool multiSelect = false)
    {
        return Grid(eventName, gridOptions, PersistenceManager.HackboxConfig.DefaultGridColumns, PersistenceManager.HackboxConfig.DefaultGridGap, PersistenceManager.HackboxConfig.DefaultChoicesSize, multiSelect);
    }

    private UIComponent Grid(string eventName, string[] gridOptions, int gridColumns, int gridGap, int fontSize, bool multiSelect)
    {
        if (gridOptions == null || gridOptions.Length == 0)
            return null;

        UIComponent grid = new UIComponent()
        {
            Name = "grid",
            Preset = Presets[(int)PresetType.Grid],
        };

        ChoicesParameter ch = new ChoicesParameter(Presets[(int)PresetType.Grid].GetParameter<ChoicesParameter>("choices"));
        var newOption = ch.Value[0];
        for (int i = 0; i < gridOptions.Length; i++)
        {
            if (i == 0)
            {
                ch.Value[i].Label = gridOptions[i];
                ch.Value[i].Value = gridOptions[i];
            }
            else
            {
                var x = new ChoicesParameter.Choice(newOption);
                x.Label = gridOptions[i];
                x.Value = gridOptions[i];
                ch.Value.Add(x);
            }
        }

        grid["choices"] = ch;
        grid.SetParameterValue("event", eventName);
        grid.SetParameterValue("multiSelect", multiSelect);
        grid.SetStyleParameterValue("fontSize", $"{fontSize}px");
        grid.SetStyleParameterValue("gridGap", $"{gridGap}px");
        grid.SetStyleParameterValue("gridColumns", gridColumns);

        return grid;
    }

    #endregion

    #region State Builder

    /// <Summary>
    ///Build bespoke game states in here
    ///Set the enum in the State Deployment section
    ///Not all enums require a bespoke state (see Member/Operator Welcome)
    ///Overloads are available in all default presets to change font sizes etc.
    ///Examples are provided that cover Simple Question, Choices, Grid, Multi-select Choices and Multi-select Grid
    /// </Summary>
    
    private State SimpleQuestion(string header, string questionlabel, string eventName)
    {
        State s = GenerateDefaultState(header);
        s.Add(Label(questionlabel));
        s.Add(TextInput(eventName));
        return s;
    }

    private State Choices(string header, string questionLabel, string eventName, string[] options, bool multiSelect = false)
    {
        State s = GenerateDefaultState(header);
        s.Add(Label(questionLabel));
        s.Add(Choices(eventName, options, multiSelect));
        return s;
    }

    private State GridOrMultiGrid(string header, string questionLabel, string eventName, string[] options, bool multiSelect = false)
    {
        State s = GenerateDefaultState(header);
        s.Add(Label(questionLabel));
        s.Add(Grid(eventName, options, multiSelect));
        return s;
    }

    private State GenericSingleMessage(string header, string displayMessage)
    {
        State s = GenerateDefaultState(header);
        s.Add(Label(displayMessage));
        return s;
    }

    private State GenericMultiMessage(string header, string[] displayMessages)
    {
        State s = GenerateDefaultState(header);
        foreach(string str in displayMessages)
            s.Add(Label(str));
        return s;
    }

    private State OperatorTestState(string header, string eventName)
    {
        State s = GenerateDefaultState(header);
        s.Add(Label("This is a test state<br>Press the button below to return to standby mode"));
        s.Add(Choices(eventName, new string[] { "End Test" }));
        return s;
    }

    private State OperatorProgressGameplay(string header, string stateToProgressTo, string eventName)
    {
        State s = GenerateDefaultState(header);
        s.Add(Label(stateToProgressTo));
        s.Add(Choices(eventName, new string[] { "Progress Gameplay" }));
        return s;
    }

    #endregion

    #region State Deployment

    /// <Summary>
    ///Publicly called functions to deploy a state
    ///This will need to be fine-tuned for games where there are inconsistent states between players
    ///But in theory, this allows for states to be sent from anywhere with just a Member, enum argument and some data
    /// </Summary>
    
    public enum InformationState
    {
        RoomDestroyed,
        MemberWelcome,
        OperatorWelcome,
        GameIsFull,
        GenericMessage
    };

    public enum GameplayState
    {
        SimpleQ,
        Choices,
        MultiSelectChoices,
        Grid,
        MultiSelectGrid
    };

    public enum OperatorState
    {
        OperatorTest,
        ProgressGameplay
    };

    public void DeployInformationState(Member mem, InformationState stateToDeploy, string message, string customHeader = "")
    {
        DeployInformationState(mem, stateToDeploy, new string[1] { message }, customHeader);
    }

    public void DeployInformationState(Member mem, InformationState stateToDeploy, string[] messages = null, string header = "")
    {
        State s = null;
        switch (stateToDeploy)
        {

            case InformationState.RoomDestroyed:
                s = GenericSingleMessage("ROOM DESTROYED", "ROOM DESTROYED<br>PLEASE REFRESH YOUR BROWSER AND RECONNECT");
                break;

            case InformationState.MemberWelcome:
                s = GenericSingleMessage("WELCOME", "Welcome to the game!");
                break;

            case InformationState.OperatorWelcome:
                s = GenericSingleMessage("OPERATOR", "You can control the game from this device");
                break;

            case InformationState.GameIsFull:
                s = GenericSingleMessage("GAME FULL", "The game has reached maximum capacity");
                break;

            case InformationState.GenericMessage:
                s = GenericMultiMessage(string.IsNullOrEmpty(header) ? "MESSAGE" : header, messages);
                break;
        }
        Host.UpdateMemberState(mem, s);
    }

    public void DeployGameplayState(Member mem, GameplayState stateToDeploy, Question question, string customHeader = "")
    {
        State s = null;

        switch (stateToDeploy)
        {
            case GameplayState.SimpleQ:
                s = SimpleQuestion
                    ("Example Question",
                    question.QuestionText,
                    stateToDeploy.ToString());
                break;

            case GameplayState.Choices:
                s = Choices
                    ("Example Choices",
                    question.QuestionText,
                    stateToDeploy.ToString(),
                    question.Answers.Select(x => x.AnswerText).ToArray());
                break;

            case GameplayState.MultiSelectChoices:
                s = Choices
                    ("Example Choices",
                    question.QuestionText,
                    stateToDeploy.ToString(),
                    question.Answers.Select(x => x.AnswerText).ToArray(),
                    true);
                break;

            case GameplayState.Grid:
                s = GridOrMultiGrid
                    ("Example Grid",
                    question.QuestionText,
                    stateToDeploy.ToString(),
                    question.Answers.Select(x => x.AnswerText).ToArray());
                break;

            case GameplayState.MultiSelectGrid:
                s = GridOrMultiGrid
                    ("Example Grid",
                    question.QuestionText,
                    stateToDeploy.ToString(),
                    question.Answers.Select(x => x.AnswerText).ToArray(),
                    true);
                break;
        }
        Host.UpdateMemberState(mem, s);
    }

    public void DeployOperatorState(OperatorState stateToDeploy, string customHeader = "")
    {
        State s = null;
        switch (stateToDeploy)
        {
            case OperatorState.OperatorTest:
                s = OperatorTestState("OPERATOR TEST", stateToDeploy.ToString());
                break;

            case OperatorState.ProgressGameplay:
                s = OperatorProgressGameplay("OPERATOR", "BIND THIS INFORMATION BOX TO THE ACTION BUTTON ENUM", stateToDeploy.ToString());
                break;
        }
        Host.UpdateMemberState(Operator, s);
    }

    #endregion

    #region Response Handling

    private void HandleMemberJoins(Member mem)
    {
        if(mem.Name.ToUpperInvariant() == PersistenceManager.HackboxConfig.OperatorName.ToUpperInvariant() && Operator == null)
        {
            Operator = mem;
            DeployInformationState(Operator, InformationState.OperatorWelcome);
        }
        else
        {
            PlayerManager.Get.CreateNewPlayer(mem);
            DeployInformationState(mem, PlayerManager.Get.Players.Count >= PersistenceManager.CurrentGameplayConfig.PlayerLimit ? InformationState.GameIsFull : InformationState.MemberWelcome);
        }            
    }

    private void HandleResponse(Message msg)
    {
        if (msg.Member == Operator)
            HandleOperatorResponse(msg);
        else
            HandleGameplayResponse(msg);
    }

    private void HandleGameplayResponse(Message msg)
    {
        if (Enum.TryParse(msg.Event, out GameplayState st))
        {
            //Player response handling logic goes in here
            switch (st)
            {
                case GameplayState.SimpleQ:
                    break;

                case GameplayState.Choices:
                case GameplayState.Grid:
                    break;

                case GameplayState.MultiSelectChoices:
                case GameplayState.MultiSelectGrid:
                    break;                
            }
        }
        else
            // Handle parsing failure
            Debug.LogError("Failed to parse enum value.");
    }

    private void HandleOperatorResponse(Message msg)
    {
        if (Enum.TryParse(msg.Event, out OperatorState st))
        {
            //Operator response handling logic goes in here
            switch (st)
            {
                case OperatorState.OperatorTest:
                    (MainMenuManager.Get.GetHackboxConfig() as HackboxConfigManager).OnEndTest();
                    break;

                case OperatorState.ProgressGameplay:
                    break;
            }
        }
        else
            // Handle parsing failure
            Debug.LogError("Failed to parse enum value.");
    }

    #endregion

    #region Testing

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void TestDeploy()
    {
        /*foreach (Member m in Host.AllMembers)
            DeployGameplayState(m, GameplayState.ExampleGridMulti);*/
    }

    #endregion
}
