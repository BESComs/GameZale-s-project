using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamDemo : MonoBehaviour
{
    private Animator annn;
    
    public float prog;

    public List<GameObject> objectsToEnable;
    

    private void Awake()
    {
        annn = GetComponent<Animator>();
    }

    private void Start()
    {
        if (PlayerPrefs.GetString("qwe") == "qwe")
        {
            DisableShit();
        }
    }


    private void Update()
    {
        prog = annn.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (prog >= 1f && !flag || Input.GetKeyDown(KeyCode.Escape))
        {
            DisableShit();
        }
    }

    private bool flag;
    
    public void DisableShit()
    {
        flag = true;
        PlayerPrefs.SetString("qwe", "qwe");
        PlayerPrefs.Save();
        foreach (var obj in objectsToEnable)
        {
            obj.SetActive(true);            
        }
        gameObject.SetActive(false);
    }
}
