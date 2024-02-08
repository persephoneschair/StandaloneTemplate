using Hackbox;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
    public Players Players = new Players();

    public void CreateNewPlayer(Member mem)
    {
        Players.Add(new Player(mem));
        //Any visual representation happens here
    }

    public Player GetPlayer(Member mem)
    {
        return Players.FirstOrDefault(x => x.Member == mem);
    }
}

public class Players : List<Player>
{
    public void DeployStateToAllMembers(HackboxManager.InformationState state)
    {
        foreach (Player pl in this)
            pl.DeployStateToMember(state);
    }

    public void ResetPlayerVariables()
    {
        foreach(Player pl in this)
            pl.ResetPlayerVariables();
    }
}

public class Player
{
    public Player(Member mem)
    {
        Member = mem;
        TwitchName = mem.Twitch == null ? "" : mem.Twitch.UserName;
        TwitchAvatarPath = mem.Twitch == null ? "" : mem.Twitch.AvatarURL;
    }

    //Player props go here

    private Member _member;
    public Member Member
    {
        get { return _member; }
        set
        {
            _member = value;
        }
    }

    private string _twitchName;
    public string TwitchName
    {
        get { return _twitchName; }
        set
        {
            _twitchName = value;
        }
    }

    private string _twitchAvatarPath;
    public string TwitchAvatarPath
    {
        get { return _twitchAvatarPath; }
        set
        {
            _twitchAvatarPath = value;
        }
    }

    private int _points = 0;
    public int Points
    {
        get { return _points; }
        set
        {
            _points = value;
        }
    }

    public void DeployStateToMember(HackboxManager.InformationState state)
    {
        HackboxManager.Get.DeployInformationState(Member, state);
    }

    public void ResetPlayerVariables()
    {

    }
}
