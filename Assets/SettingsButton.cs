using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    public GameObject settingsPanel;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Call);
    }

    private void Call()
    {
        settingsPanel.SetActive(true);
    }
}