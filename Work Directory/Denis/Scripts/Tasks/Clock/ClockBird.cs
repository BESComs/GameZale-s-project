using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Clock
{
	public class ClockBird : MonoBehaviour
	{
		private Animate _anim;
		
		private void Awake()
		{
			_anim = new Animate(this, true);
			_anim.SetScale(0);
		}
		
		public void Tweet()
		{
			_anim.Scale(1, 1.5f, _anim.BounceCurve);
		}
	}
}
