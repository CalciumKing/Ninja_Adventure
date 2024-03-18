using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager i;
    [SerializeField] Transform cherryBar;
    [SerializeField] TMP_Text gemText;
    private void Awake()
    {
        if(i == null) i = this;
        else Destroy(this);
    }
    public void UpdateCherries(int amt)
    {
        for (int i = 0; i < cherryBar.childCount; i++)
        {
            var cherryImage = cherryBar.GetChild(i).GetComponent<Image>();
            cherryImage.enabled = amt > i;
        }
    }
    public void UpdateGems(int amt)
    {
        gemText.text = amt.ToString();
    }
}