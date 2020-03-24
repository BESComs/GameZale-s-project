using UnityEngine;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts.IqushaGame1
{
    public class ButtonCheckAnswer : MonoBehaviour
    {
        //Кнопка проверки ответа
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if(!MyGameManager.Instance.inTask && !MyGameManager.Instance.gameEnd)
                    MyGameManager.Instance.CheckAnswer();
            });
        }
    }
}
