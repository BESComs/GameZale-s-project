using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Select
{
	public class SelectItem : MonoBehaviour
	{
		public bool childSetActive;
		public bool setTopOnSelect;
		public bool Required;
		[Space]
		public bool Scaling = true;
		public Vector3 Moving;
		public Color Colorize;
		public Sprite ChangeSprite;
		[Space]
		public GameObject Activate;
		public bool OnlyIfRequired = true;
		public float ActivateDuration = 0f;
		[Space]
		public SelectItem SelectToo;

		private TaskSelect _taskSelect;
		[HideInInspector] public bool Success = false;
		private SpriteRenderer _spriteRenderer;
		private Animate _anim;

		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			Moving = Vector3.zero;
			Colorize = Color.white;
			_taskSelect = GetComponentInParent<TaskSelect>();
			_anim = new Animate(this, true);
			if (Required)
			{
				if (Activate != null)
					Activate.SetActive(false);
			}
			else
				Success = true;
		}
		
		private void OnMouseDown()
		{
			if (OnlyIfRequired && Required || !OnlyIfRequired)
				Activates();
			if (Required)
			{
				if (Success) return;
				if (setTopOnSelect)
					if (_spriteRenderer != null)
						_spriteRenderer.sortingOrder = 2;
				
				Success = true;
				Selected();
				if (SelectToo != null)
				{
					SelectToo.Success = true;
					if(childSetActive)
						SelectToo.transform.GetChild(0).gameObject.SetActive(true);
					SelectToo.Selected();
				}
				_taskSelect.CheckObjects();
			}
			else
				_anim.Scale(1f, 0.9f, 0.2f, _anim.BounceCurve);
			
		}

		private void Selected()
		{
			if (Scaling)
				_anim.Scale(1,1.2f, 0.2f);
			
			_anim.Colorize(Colorize,0.5f);
			
			if (!Moving.Equals(Vector3.zero))
				_anim.MoveAdd(Moving);
			
			if (ChangeSprite != null)
				Timeout.Set(this, 0.5f, () => { GetComponent<SpriteRenderer>().sprite = 
					ChangeSprite; });
		}

		private void Activates()
		{
			if(Activate == null) return;
			Activate.SetActive(true);
			if (!ActivateDuration.Equals(0))
				Timeout.Set(this, ActivateDuration, () => { Activate.SetActive(false); });
		}
	}
}
