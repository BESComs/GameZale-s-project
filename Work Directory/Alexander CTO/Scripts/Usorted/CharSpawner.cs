using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSpawner : MonoBehaviour
{
    public string prevLocationName;
    public bool forceSpawn;

    public GameObject teenMale;
    public GameObject teenFemale;

    public GameObject attachedCam;
    
    // Start is called before the first frame update
    void Start()
    {
        teenMale.SetActive(false);
        teenFemale.SetActive(false);
        if (forceSpawn || Bootstraper.Instance.PrevSceneName == prevLocationName)
        {
            attachedCam?.SetActive(true);
            var charData = CharCustomizationSaver.Instance.LoadData();
            if (charData.genderData == GenderData.Male)
            {
                teenMale.SetActive(true);
            }
            else
            {
                teenFemale.SetActive(true);
            }
        }
    }

}
