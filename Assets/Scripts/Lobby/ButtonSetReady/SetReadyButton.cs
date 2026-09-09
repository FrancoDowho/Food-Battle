using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetReadyButton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMP_Text _buttonTXT;
    bool _isReady;
    public void SetPlayerReady()
    {
       if(BasicSpawner.instance.playersControlls.Count <= 1)
        {
            PopUpManager.instance.ActivePopUp()
          .SetAmountOfButtons(1)
          .SetTitle("Faltan comidas en el plato")
          .SetDesc("Aun no hay suficientes amigos para comenzar la partida!")
          .SetButtonText(0, "Ok")
          .SetButtonAction(0, () =>
          {
              PopUpManager.instance.ClosePopUp();

          });
            return;
        }

        BasicSpawner.instance.localPlayer.ChangeReadyState();
        if (_isReady)
        {
            _isReady = true;
            _buttonTXT.text = "Cancelar";
        }
        else
        {
            _buttonTXT.text = "Listo";
            _isReady = false;


        }
    }
}
