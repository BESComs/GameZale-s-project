using UnityEngine;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts.IqushaGame3
{
    public class PositionInSpaceCheckButton : MonoBehaviour
    {
        
        //кнопка проверки ответа
        private Button checkButton;
        private void Awake()
        {
            checkButton = GetComponent<Button>();
            
            checkButton.onClick.AddListener(() =>
            {
                if (!PositionInSpaceGameManager.Instance.inTask && !PositionInSpaceGameManager.Instance.gameIsEnd)
                {
                    PositionInSpaceGameManager.Instance.CheckAnswer();
                }
            });
        }
    }
}
