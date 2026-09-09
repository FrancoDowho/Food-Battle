using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject _currentMenuDisplay;
    [SerializeField] GameObject _completeMenu;
    [SerializeField] GameObject _menuStart;
    public static MenuManager instance;
    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        ShowThisMenuOnly(_menuStart);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowThisMenuOnly(GameObject menuToShow)
    {
        _currentMenuDisplay.SetActive(false);
        menuToShow.SetActive(true);
        _currentMenuDisplay = menuToShow;
    }

    public void HideMenu(GameObject menuToHide)
    {
        menuToHide.SetActive(false);
    }

    public void HideEverythingInMenu(bool hide = false)
    {
        _completeMenu.SetActive(hide);
    }
}
