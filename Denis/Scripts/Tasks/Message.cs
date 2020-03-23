using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks
{
	public class Message : MonoBehaviour
	{
		public float Fading = 1f;
		[Space]
		public bool AutoStart = true;
		public float AutoStartDelay = 1f;
		
		private Animate _anim;
		private TextMesh _text;
		private AudioSource _play;
		
		private void Awake()
		{
			_anim = new Animate(this, false);
			_anim.SetColor(0);
			_text = GetComponent<TextMesh>();
			_play = GetComponent<AudioSource>();
			_play.playOnAwake = false;
		}

		private void Start()
		{
			if (AutoStart)
				Timeout.Set(this, AutoStartDelay, Show);
		}
		
		private void Show()
		{
			gameObject.SetActive(true);
			_anim.Scale(0, 1, Fading);
			_anim.Colorize(1, Fading);
			if (_play.clip != null)
				_play.Play();
		}
		private void Hide()
		{
			_anim.Scale(0, Fading);
			_anim.Colorize(0, Fading);
		}
		
		public void Show(string text)
		{
			_anim.Colorize(0, 0.2f);
			Timeout.Set(this, 0.2f, () =>
			{
				_text.text = text;
				_play.Stop();
				_play.clip = null;
				Show();
			});
		}
		public void Show(string text, float delay)
		{
			_anim.Colorize(0, 0.2f);
			Timeout.Set(this, 0.2f, () =>
			{
				_text.text = text;
				_play.Stop();
				_play.clip = null;
				Show();
				Timeout.Set(this, delay, Hide);
			});
		}
		public void Show(string text, AudioClip clip)
		{
			_anim.Colorize(0, 0.2f);
			Timeout.Set(this, 0.2f, () =>
			{
				_text.text = text;
				_play.Stop();
				_play.clip = clip;
				Show();
			});
		}
	}
}