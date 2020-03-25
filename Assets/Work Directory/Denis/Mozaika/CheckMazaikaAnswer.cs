using UnityEngine;
using UnityEngine.UI;
using Work_Directory.Denis.Scripts;

namespace Work_Directory.Denis.Mozaika
{
    public class CheckMazaikaAnswer : MonoBehaviour
    {
        //Кнопка отвечающая за проверку задания
        private Button button;
        //true пока проигрываются все анимации при нажатии
        //используется для того чтобы избегать многократные нажатия
        private bool inTask;

        private async void OnEnable()
        {
            button = GetComponent<Button>();
            await new WaitForSeconds(1);
            await new WaitForUpdate();
            button.onClick.AddListener( async () =>
            {
                if (MazaikaManager.Instance.gameIsEnd || inTask) return;
                inTask = true;
                //проверка ответа игрока
                MazaikaManager.Instance.CheckAnswer();
                //закончить игру если проверка дала положительный результат
                //иначе начать проигрывание анимации
                if(MazaikaManager.Instance.gameIsEnd)
                {
                    MazaikaManager.Instance.GameEnd();
                    gameObject.SetActive(false);
                }
                else
                {
                    MazaikaManager.Instance.RegisterAnswer(false);
                    var tmp = MazaikaManager.Instance.wrong;
                    tmp.gameObject.SetActive(true);
                    await new Fade(tmp, Mode.In, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                    await new WaitForSeconds(1);
                    await new Fade(tmp, Mode.Out, AnimationCurve.EaseInOut(0, 0, 1, 1)).RunTask();
                    tmp.gameObject.SetActive(false);
                }
                inTask = false;
            });
        }
    }
}