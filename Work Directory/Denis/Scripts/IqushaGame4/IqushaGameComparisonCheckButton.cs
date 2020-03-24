using UnityEngine;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts.IqushaGame4
{
    public class IqushaGameComparisonCheckButton : MonoBehaviour
    {
        //кнопка проверки ответа
        private Button _button;
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                if (!IqushaGameComparisonManager.Instance.inTask && !IqushaGameComparisonManager.Instance.gameIsEnd)
                {
                    IqushaGameComparisonManager.Instance.CheckAnswer();
                }
            });
        }
    }
}
