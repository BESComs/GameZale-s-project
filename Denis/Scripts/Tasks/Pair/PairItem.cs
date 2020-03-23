using System.Collections;
using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Pair
{
	public class PairItem : MonoBehaviour
	{
		public PairItem PairingWith;
		public bool Outro = true;
		public bool Colorize = false;
		public Color ChangeColor = Color.white;
		[HideInInspector] public bool Paired = false;
		private TaskPair _taskPair;
		private Animate _anim;

		private void Awake()
		{
			_taskPair = GetComponentInParent<TaskPair>();
			_anim = new Animate(this, true);
		}

		private void OnMouseDown()
		{
			if (Paired) return;
				
			var wait = _taskPair.WaitPairing;
			if (wait == null)
			{
				_taskPair.WaitPairing = this;
				Pulsing(true);
			}
			else if (!Paired)
			{
				if (PairingWith == wait)
				{
					Out();
					wait.Out();
					Paired = true;
					wait.Paired = true;
					_taskPair.CheckObjects();
				}
				else
					wait.Pulsing(false);
				_taskPair.WaitPairing = null;
			}
		}

		private Coroutine _pulsing;
		private void Pulsing(bool value)
		{
			if (value)
				_pulsing = StartCoroutine(PulsingCoroutine());
			else if (_pulsing != null)
			{
				StopCoroutine(_pulsing);
				_anim.StopAll();
				_anim.SetScale(1);
			}		
		}
		IEnumerator PulsingCoroutine()
		{
			while (true)
			{
				_anim.Scale(1f,1.1f, 1f, _anim.BounceCurve);
				yield return new WaitForSeconds(1);
			}
		}

		public void Out()
		{
			Pulsing(false);
			if (Outro)
			{
				Timeout.Set(this, 0.2f, () =>
				{
					if (_pulsing != null)
						StopCoroutine(_pulsing);
					_anim.Scale(1.5f);
					_anim.Colorize(0);
				});
			}
			else
				_anim.Scale(1.2f);
			if (Colorize)
				_anim.Colorize(ChangeColor);
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}
