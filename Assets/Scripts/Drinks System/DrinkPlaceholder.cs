using UnityEngine;

public class DrinkPlaceholder : MonoBehaviour
{
    Renderer _renderer;
    Collider _collider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
        gameObject.name = "Serve";
    }

    // Update is called once per frame
    void Update()
    {
        //if (PlayerManager.HeldItem != null && PlayerManager.HeldItem.CompareTag("Drink")) Show();
        //else Hide();
        Hide();
    }
    
    void Show()
    {
        _renderer.enabled = true;
        _collider.enabled = true;
    }

    void Hide()
    {
        _renderer.enabled = false;
        _collider.enabled = false;
    }
}
