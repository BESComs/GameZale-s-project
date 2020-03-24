using UnityEngine;
using UnityEngine.UI;

public class CustomizationButton : MonoBehaviour
{
    public GameObject customizationStuff;
    public GameObject lessonsStuff;
    
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    
    private void OnClick()
    {
        customizationStuff.SetActive(true);
        lessonsStuff.SetActive(false);   
    }
    
}