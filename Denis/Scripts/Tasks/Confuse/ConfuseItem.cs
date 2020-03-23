using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Confuse
{
	public class ConfuseItem : MonoBehaviour
	{
		public GameObject Object;
		public Vector3 LookMove;
		private TaskConfuse _taskConfuse;
		[HideInInspector]public bool Selected = false;
		[HideInInspector]public bool Looked = false;
		public Animate Anim;
		private Collider2D _collider;

		private void Awake()
		{
			_taskConfuse = GetComponentInParent<TaskConfuse>();
			Anim = new Animate(this, true);
			_collider = GetComponent<Collider2D>();
			Parented(true);
			Enable(false);
		}

		public void Enable(bool set)
		{
			if (_collider != null)
				_collider.enabled = set;
		}

		public void Parented(bool set)
		{
			if (Object == null) return;
			Object.transform.SetParent((set) ? transform : transform.parent);
		}
		
		private void OnMouseDown()
		{
			if (_taskConfuse.IsComplete || Selected) return;
			_taskConfuse.Selected(this);
		}
	}
}
