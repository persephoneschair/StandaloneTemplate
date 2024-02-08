using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : SingletonMonoBehaviour<MainMenuManager>
{
    public enum NavigationButton
    {
        Home,
        PlayGame,
        ImportPack,
        QuestionDatabase,
        HackboxConfig,
        GameplayConfig,
        Quit,
    };

    public GameObject titleLogo;
    public GameObject[] subMenus;
    private List<SubMenuManager> subMenuManagers = new List<SubMenuManager>();

    private void Start()
    {
        foreach (GameObject go in subMenus.Where(x => x.GetComponent<SubMenuManager>() != null))
            subMenuManagers.Add(go.GetComponent<SubMenuManager>());
    }

    public void OnRoomConnected()
    {
        foreach (SubMenuManager smm in subMenuManagers)
            smm.OnRoomConnected();
    }

    public void OnClickMenuButton(NavigationButton buttonType)
    {
        KillAllMenus();
        switch (buttonType)
        {
            case NavigationButton.Home:
                subMenus[0].SetActive(true);
                //titleLogo.SetActive(true);
                break;

            case NavigationButton.PlayGame:
                ImportManager.Get.OnClickImportQuestions();
                break;

            case NavigationButton.HackboxConfig:
                (GetHackboxConfig() as HackboxConfigManager).correspondingMenu.SetActive(true);
                break;

            case NavigationButton.GameplayConfig:
                (GetGameplayConfig() as GameplayConfigManager).correspondingMenu.SetActive(true);
                break;

            case NavigationButton.Quit:
                DebugLog.Print("QUITTING GAME", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Red);
                Application.Quit();
                break;
        }
    }

    private void KillAllMenus()
    {
        subMenus[0].SetActive(false);
        foreach (SubMenuManager smm in subMenuManagers)
            smm.correspondingMenu.gameObject.SetActive(false);
    }

    /*public Object GetDatabaseManager()
    {
        return subMenus.FirstOrDefault(x => x.GetComponent<SubMenuManager>().GetType() == typeof(DatabaseManager));
    }*/

    public Object GetGameplayConfig()
    {
        return subMenuManagers.FirstOrDefault(x => x.GetType() == typeof(GameplayConfigManager));
    }

    public Object GetHackboxConfig()
    {
        return subMenuManagers.FirstOrDefault(x => x.GetType() == typeof(HackboxConfigManager));
    }
}
