using System.Collections;
using UnityEngine;

namespace Tasks.additional
{
	public class ElementVoice : MonoBehaviour
	{
		public AudioClip voice;
		public Vector2 randomDelay;
		private AudioSource _source;

		private void Start()
		{
			_source = gameObject.AddComponent<AudioSource>();
			StartCoroutine(MakeVoice());
		}

		private IEnumerator MakeVoice()
		{
			_source.clip = voice;
			_source.playOnAwake = false;

			while (true)
			{
				yield return new WaitForSeconds(Random.Range(randomDelay.x, randomDelay.y));
				_source.Play();
				yield return new WaitForSeconds(_source.clip.length);
			}
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}
