using UnityEngine;

namespace Work_Directory.Denis.Scripts.Tasks
{
    public  class TaskParent : MonoBehaviour
    {
        //Родительский класс ко всем играм написанными шохжахон для повтора игры
        //загрузка финальной сцены
        public bool fadeGame;
        [HideInInspector] public Transform replayButton;
        [HideInInspector] public Transform finalScene;
        [HideInInspector] public Transform audioPrefab;
        
        //анимированное появление всех объектов на сцену после прохождения игры
        public async void LoadFinalScene()
        {
            if(!fadeGame)
                await new Fade(transform).RunTask();
            if(this == null) return;
            gameObject.SetActive(false);
            finalScene.gameObject.SetActive(true);
            var ac = audioPrefab.GetComponent<AudioController>();
            ac.inGame = false;
            await new Fade(finalScene, Mode.In).RunTask();
            if(this == null) return;
            await ac.StartFinalAudio();
            if(this == null) return;
            replayButton.gameObject.SetActive(true);
            new Fade(replayButton, Mode.In).RunTask();
            
        }
        public void SetButton()
        {
            replayButton.GetComponent<ReplayGame>().cacheGamePrefab = this;
        }
    }
}