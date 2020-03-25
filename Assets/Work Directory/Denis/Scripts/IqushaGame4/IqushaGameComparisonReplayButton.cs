using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Work_Directory.Denis.Scripts.IqushaGame4
{
    public class IqushaGameComparisonReplayButton : MonoBehaviour
    {
        //кнопка рестарта игры
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(delegate {
                if (!IqushaGameComparisonManager.Instance.inTask)
                {
                    IqushaGameComparisonManager.Instance.RestartGame();
                } 
            });
        }
    }
}
