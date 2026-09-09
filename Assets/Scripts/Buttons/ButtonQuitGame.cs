using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonQuitGame : MonoBehaviour
{
    public void CallPopUpManager()
    {
        PopUpManager.instance.ActivePopUp().DefaultButtonsText().SetTitle("Adios!").SetDesc("¿Seguro que quieres salir de Food Battle?")
            .SetButtonAction(0, PopUpManager.instance.ClosePopUp)
            .SetButtonAction(1, () => {
                PopUpManager.instance.ClosePopUp();
                Application.Quit();

            }); ;
    }
}
