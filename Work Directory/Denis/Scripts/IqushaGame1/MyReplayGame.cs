using UnityEngine;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts.IqushaGame1
{
    public class MyReplayGame : MonoBehaviour
    {
        //кнопка рестарта игры
        private Button button;
        private void Awake()
        {
            button = GetComponent<Button>();
            
            button.onClick.AddListener(() =>
            {
                if(!MyGameManager.Instance.inTask)
                    MyGameManager.Instance.RestartGame();
            });
        }
    }
}
