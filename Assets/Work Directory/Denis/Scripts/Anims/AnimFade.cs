using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Work_Directory.Denis.Scripts.Anims
{
	//
	public class AnimFade : MonoBehaviour
	{
		public float duration = 0.5f;
		
		private Animate _anim;
		private readonly AnimationCurve _curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

		private void Awake()
		{
			_anim = new Animate(this, true);
			_anim.SetColor(1);
		}

		public void In()
		{
			gameObject.SetActive(true);
			_anim.Colorize(1, duration, _curve);
		}
		
		public void Out()
		{
			gameObject.SetActive(true);
			_anim.Colorize(0, duration, _curve);
			Timeout.Set(this, duration, () =>
			{
				gameObject.SetActive(false);
			});
		}
	}
}
