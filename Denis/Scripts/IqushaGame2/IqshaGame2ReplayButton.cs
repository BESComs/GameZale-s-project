using UnityEngine;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts.IqushaGame2
{
    public class IqshaGame2ReplayButton : MonoBehaviour
    {
        //кнопка рестарта игры
        private Button button;
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if (!GameLearnFigures.Instance.inTask)
                    GameLearnFigures.Instance.RestartGame();
                
            });
        }
    }
}
