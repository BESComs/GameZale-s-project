using Tasks.Select;
using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Collect
{
	public class CollectItem : MonoBehaviour
	{
		public bool Required = false;
		[Space]
		public bool Scaling = true;
		public GameObject Activate;
		
		[HideInInspector] public bool Success;

		private Animate _anim;

		private void Awake()
		{
			_anim = new Animate(this, true);
			if (Required)
				if (Activate != null)
					Activate.SetActive(false);
		}
		
		public void OnMouseDown()
		{
			Success = !Success;
			Selected(Success);
		}

		public void Selected(bool val)
		{
			if (val)
			{
				if (Scaling)
					_anim.Scale(1,1.2f, 0.2f);
				if (Activate != null)
					Activate.SetActive(true);
			}
			else
			{
				if (Scaling)
					_anim.Scale(1.2f,1f, 0.2f);
				if (Activate != null)
					Activate.SetActive(false);
			}
		}
	}
}
