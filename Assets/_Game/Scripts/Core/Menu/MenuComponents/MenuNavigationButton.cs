using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigationButton : MonoBehaviour
{
    public MainMenuManager.NavigationButton buttonType;
    public Button button;

    public void OnClickButton()
    {
        MainMenuManager.Get.OnClickMenuButton(this.buttonType);
    }

    private void Update()
    {
        AutoEnableButton();
    }

    private void AutoEnableButton()
    {
        var hb = HackboxManager.Get;
        switch (buttonType)
        {
            case MainMenuManager.NavigationButton.PlayGame:
                button.interactable = hb.Operator != null;

                //If implementing a database-based game that requires a lower limit of available questions, use this code
                //button.interactable = (MainMenuManager.Get.GetDatabaseManager() as DatabaseManager).GetAvailableQCount() >= 25;
                break;


            /*case MainMenuManager.NavigationButton.HackboxConfig:
            //case MainMenuManager.NavigationButton.GameplayConfig:
                //button.interactable = hb.Host.Connected;
                break;*/

            default:
                button.interactable = true;
                break;
        }
    }
}
