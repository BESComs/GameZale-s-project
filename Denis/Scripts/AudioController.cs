using System.Threading.Tasks;
using UnityEngine;

namespace Work_Directory.Denis.Scripts
{
    public class AudioController : MonoBehaviour
    {
        /*
         * Script for audioPrefab
         * Use in Start Play Button to play audio in games with script StartPlay
         */
        [HideInInspector] public bool inGame; 
        [HideInInspector] public AudioClip introAudio;
        [HideInInspector] public AudioClip gameSounds;
        [HideInInspector] public AudioClip finalAudio;
        private AudioSource _audioSourceIntro;
        private AudioSource _audioSourceGame;
        private AudioSource _audioSourceFinal;
    
        public void OnInstantiate()
        {
            transform.localPosition = new Vector3(520.3125f * ((float)Screen.width / Screen.height +
                                                               16f / 9) / 2f, -400f, 0); 
            _audioSourceFinal = GetComponent<AudioSource>();
            _audioSourceGame = transform.GetChild(1).GetComponent<AudioSource>();
            _audioSourceIntro = transform.GetChild(0).GetComponent<AudioSource>();
            _audioSourceGame.clip = gameSounds;
            _audioSourceIntro.clip = introAudio;
            _audioSourceFinal.clip = finalAudio;
        }

        /*
         * Play final audio after game is end
         * 
         */
        public async Task StartFinalAudio()
        {
            _audioSourceFinal.Play();
            _audioSourceGame.Stop();
            await new WaitForSeconds(finalAudio.length);
        }

        /*
         * Play game audio after user pushed on playButton
         */
        public void StartGameSounds()
        {
            _audioSourceGame.Play();
            _audioSourceGame.loop = true;
        }

        /*
         * Play game game audio description inst
         */
        public async void StartIntroAudioClip()
        {
            if(introAudio == null) return;
            _audioSourceIntro.Play();
            await new Scaling(transform.GetChild(0), false, AnimationCurve.EaseInOut(0, 1, introAudio.length / 2f, .8f)).RunTask();
            if(this == null) return;
            _task = new Scaling(transform.GetChild(0), false, AnimationCurve.EaseInOut(0, 1, introAudio.length / 2f, 1.25f)).RunTask();
        }

        /*
         * Mute or unmute game sound after user pushed on "MuteUnmuteGameSounds" button
         */
        public void MuteUnmuteGameSounds()
        {
            _audioSourceGame.mute = !_audioSourceGame.mute;
        }

        private Task _task;
        
        /*
         * Play audio description of the game after user pushed on "RepeatAudioDescriptionButton" button
         */
        public async void RepeatIntroAudioClip()
        {
            if(!inGame || introAudio == null) return;
            _audioSourceIntro.Play();
            if (_task != null && !_task.IsCompleted)
                return;
        
            await new Scaling(transform.GetChild(0), false, AnimationCurve.EaseInOut(0, 1, introAudio.length / 2f, .8f)).RunTask();
            _task = new Scaling(transform.GetChild(0), false,
                AnimationCurve.EaseInOut(0, 1, introAudio.length / 2f,  1.25f)).RunTask();
        }
    }
}