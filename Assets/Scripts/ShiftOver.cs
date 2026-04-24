using System;
using System.Collections;
using UnityEngine;

public class ShiftOver : MonoBehaviour
{

    public GameObject ShiftOverPanel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (TimeManager.Instance.currentTime.Hours >= 10)
        //{
        //    PlayerManager.FirstPersonController.enabled = false;
        //    StartCoroutine(PanelFadeOut(3.5f));
        //    Cursor.lockState = CursorLockMode.None;
        //}
    }

    IEnumerator PanelFadeOut(float time)
    {
        CanvasGroup panelTransparency = ShiftOverPanel.GetComponent<CanvasGroup>();
        panelTransparency.alpha = 0;
        ShiftOverPanel.SetActive(true);
        
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            panelTransparency.alpha = Mathf.Lerp(0, 1, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
