using UnityEngine;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts.IqushaGame2
{
    public class IqushaGame2CheckButton : MonoBehaviour
    {
        //кнопка проверки игры
        private Button checkButton;
        private void Awake()
        {
            checkButton = GetComponent<Button>();
            checkButton.onClick.AddListener(() =>
            {
                if (!GameLearnFigures.Instance.inTask && !GameLearnFigures.Instance.gameIsEnd)
                    GameLearnFigures.Instance.CheckAnswer();
            });
        }
    }
}
