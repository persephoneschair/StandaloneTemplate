using Hackbox;
using Hackbox.Parameters;
using Hackbox.UI;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

public class HackboxManager : SingletonMonoBehaviour<HackboxManager>
{


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
    public Hackbox.UI.Preset[] Presets;

    #endregion

    #region Default Events

    public void OnRoomCreated(string roomCode)
    {
        DebugLog.Print($"CREATED ROOM {roomCode}", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
    }

    public void OnRoomConnected(string roomCode)
    {
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

    private UIComponent Choices(string eventName, string[] choiceOptions)
    {
        return Choices(eventName, choiceOptions, PersistenceManager.HackboxConfig.DefaultChoicesSize);
    }

    private UIComponent Choices(string eventName, string[] choiceOptions, int fontSize)
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
        choices.SetStyleParameterValue("fontSize", $"{fontSize}px");

        return choices;
    }

    private UIComponent Grid(string eventName, string[] gridOptions)
    {
        return Grid(eventName, gridOptions, PersistenceManager.HackboxConfig.DefaultGridColumns, PersistenceManager.HackboxConfig.DefaultGridGap, PersistenceManager.HackboxConfig.DefaultChoicesSize);
    }

    private UIComponent Grid(string eventName, string[] gridOptions, int gridColumns, int gridGap, int fontSize)
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
    ///Not all enums require I bespoke state (see Generic welcome)
    ///Overloads are available in all default presets to change font sizes etc.
    ///An example of a grid is given
    /// </Summary>

    private State ExampleGrid(string header, string questionLabel, string eventName, string[] options)
    {
        State s = GenerateDefaultState(header);
        s.Add(Label(questionLabel));
        s.Add(Grid(eventName, options));
        return s;
    }

    private State GenericWelcome(string header, string displayMessage)
    {
        State s = GenerateDefaultState(header);
        s.Add(Label(displayMessage));
        return s;
    }

    private State WipeoutGrid(string header, Question q, string eventName)
    {
        State s = GenerateDefaultState(header);
        s.Add(Label(q.QuestionText));
        s.Add(Grid(eventName, q.Answers.Select(x => x.AnswerText).ToArray()));
        return s;
    }

    #endregion

    #region State Deployment

    /// <Summary>
    ///Publicly called functions to deploy a state
    /// Best practise would be to pre-determine states, add them to the enum in the State Build section
    ///And then bind data to some global value in Question Manager
    ///This will need to be fine-tuned for games where there are inconsistent states between players
    ///But in theory, this allows for states to be sent from anywhere with a simple Member and enum
    /// </Summary>
    /// 
    public enum GameplayState
    {
        ExampleGrid,
        MemberWelcome,
        OperatorWelcome,
        WipeoutGrid
    };

    public void DeployState(Member mem, GameplayState stateToDeploy)
    {
        State s = null;
        switch (stateToDeploy)
        {
            case GameplayState.ExampleGrid:
                s = ExampleGrid
                    ("Example Grid",
                    "Some question label binded to CurrentQuestion.string",
                    stateToDeploy.ToString(),
                    new string[] { "Drawn", "From", "Question", "Data" });
                break;

            case GameplayState.WipeoutGrid:
                s = WipeoutGrid("q.CategoryText", new Question(), "wipeoutGrid");
                break;

            case GameplayState.MemberWelcome:
                s = GenericWelcome("WELCOME", "Welcome to the game!");
                break;

            case GameplayState.OperatorWelcome:
                s = GenericWelcome("OPERATOR", "You can control the game from this device");
                break;

        }
        Host.UpdateMemberState(mem, s);
    }

    #endregion

    #region Response Handling

    public void HandleMemberJoins(Member mem)
    {
        if(mem.Name == PersistenceManager.HackboxConfig.OperatorName)
        {
            DeployState(mem, GameplayState.OperatorWelcome);
        }
        else
        {
            DeployState(mem, GameplayState.MemberWelcome);
        }            
    }

    public void HandleResponse(Message msg)
    {
        GameplayState st;
        if (Enum.TryParse(msg.Event, out st))
        {
            switch (st)
            {
                case GameplayState.ExampleGrid:
                    Debug.Log(msg.Value);
                    break;
            }
        }
        else
        {
            // Handle parsing failure
            Debug.LogError("Failed to parse enum value.");
        }
    }

    #endregion

    #region Testing

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void TestDeploy()
    {
        foreach (Member m in Host.AllMembers)
            DeployState(m, GameplayState.ExampleGrid);
    }

    #endregion
}
