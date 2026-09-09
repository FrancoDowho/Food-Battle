using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ProfilePicMark : MonoBehaviour
{
    public TMP_Text txtname;
    public Image pfp;
    public CanvasGroup cg;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Appear()
    {
        cg.alpha = 0;
        cg.DOFade(1, 0.5f);
    }

    public void Disappear()
    {
        cg.DOFade(0, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
