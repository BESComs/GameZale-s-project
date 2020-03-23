using System.Collections;
using UnityEngine;

namespace Tasks.additional
{
	public class HideTimer : MonoBehaviour
	{
		public GameObject[] pieces;
		public TextMesh seconds;
		[Space]
		public GameObject[] disable;
		public GameObject[] enable;

		private void Awake()
		{
			foreach (var piece in pieces)
				piece.SetActive(false);
			foreach (var obj in enable)
				obj.SetActive(false);
		}
		
		private void Start ()
		{
			StartCoroutine(TimerCoroutine());
		}

		private IEnumerator TimerCoroutine()
		{
			var length = pieces.Length;
			seconds.text = length + "";
			for (var i = 0; i < length; i++)
			{
				yield return new WaitForSeconds(1);
				pieces[i].SetActive(true);
				seconds.text = length - i - 1 + "";
			}
			yield return new WaitForSeconds(0.5f);
			
			foreach (var obj in disable)
				obj.SetActive(false);
			yield return new WaitForSeconds(1f);
			foreach (var obj in enable)
				obj.SetActive(true);
		}
		
		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}
