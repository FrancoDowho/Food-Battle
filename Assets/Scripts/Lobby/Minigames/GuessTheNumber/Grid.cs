using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Grid : MonoBehaviour
{
    [SerializeField] ButtonsGuessTheNumber _buttonPrefab;
    public List<ButtonsGuessTheNumber> buttons = new List<ButtonsGuessTheNumber>();
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void GenerateGridWithNumbers(int amount)
    {
        ClearGrid();
        buttons = new List<ButtonsGuessTheNumber>();
        for (int i = 0; i < amount; i++)
        {
            ButtonsGuessTheNumber newButton = Instantiate(_buttonPrefab, transform);
            newButton.Set(i + 1);
            buttons.Add(newButton);
        }
    }

    public void RemoveUnusedButtons()
    {
        for (int i = buttons.Count - 1; i >= 0; i--)
        {
            ButtonsGuessTheNumber button = buttons[i];

            if (button == null)
            {
                buttons.RemoveAt(i);
                continue;
            }

            if (!button.buttonComponent.interactable)
            {
                buttons.RemoveAt(i);
                Destroy(button.gameObject);
            }
        }
    }

    public void RemoveButtonsByNumber(int numberA, int numberB)
    {
        for (int i = buttons.Count - 1; i >= 0; i--)
        {
            ButtonsGuessTheNumber button = buttons[i];

            if (button == null)
            {
                buttons.RemoveAt(i);
                continue;
            }

            if (button.number == numberA || button.number == numberB)
            {
                buttons.RemoveAt(i);
                Destroy(button.gameObject);
            }
        }
    }

    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        buttons.Clear();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
