using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;
using Work_Directory.Denis.Scripts.Tasks.Tangram;

namespace Tasks.Tangram
{
    [RequireComponent((typeof(Rigidbody2D)))]
    public class TangramPiece : MonoBehaviour
    {
        public TangramField requiredField;
        public Vector2 rayCastOffset;
        public bool opacity = true;
        [Space] public Transform setTransformWhenDrop;
        public Vector3 curveTrajectory = Vector3.zero;
        public GameObject activate;
        private TaskTangram _taskTangram;
        private Camera _cameraMain;
        public bool IsPaired { get; private set; }
        private Animate _anim;
        private Vector3 _initialPosition;
        private Vector3 _mouseOffset;
        private float _order;
        private SpriteRenderer _spriteRenderer1, _spriteRenderer2;
        private int _spriteSo1, _spriteSo2;
        private Collider2D collider;
        private Collider2D socketCollider;
        private ContactFilter2D contactFilter = new ContactFilter2D {useTriggers = true};

        private void Awake()
        {
            collider = GetComponent<Collider2D>();
            socketCollider = requiredField?.GetComponent<Collider2D>();
            _spriteRenderer1 = GetComponent<SpriteRenderer>();
            if (_spriteRenderer1 != null)
                _spriteSo1 = _spriteRenderer1.sortingOrder;
            _spriteRenderer2 = (transform.childCount == 0)
                ? null
                : transform.GetChild(0).GetComponent<SpriteRenderer>();
            if (_spriteRenderer2 != null)
                _spriteSo2 = _spriteRenderer2.sortingOrder;
            _taskTangram = GetComponentInParent<TaskTangram>();
            _cameraMain = Camera.main;
            _anim = new Animate(this, false);
            _initialPosition = transform.position;
        }

        private void Update()
        {
            if (_anim == null)
            {
                _anim = new Animate(this, false);
            }
        }

        private void OnMouseEnter()
        {
            if (IsPaired) return;

            _order = -0.25f;
            _anim.Scale(1.05f, 0.1f);
        }

        private void OnMouseExit()
        {
            if (IsPaired) return;

            _order = 0;
            _anim.Scale(1f, 0.1f);
        }

        private List<int> _sortOrd = new List<int>();

        private void OnMouseDown()
        {
            if (IsPaired) return;
            if (_spriteRenderer1 != null)
                _spriteRenderer1.sortingOrder = 20;
            if (_spriteRenderer2 != null)
                _spriteRenderer2.sortingOrder = 20;
            _mouseOffset = transform.position - _cameraMain.ScreenToWorldPoint(Input.mousePosition);
            if (opacity)
                _anim.Colorize(0.75f, 1f);
            OnMouseDrag();
        }

        private void OnMouseDrag()
        {
            if (IsPaired) return;

            var pointer = _cameraMain.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(pointer.x + _mouseOffset.x, pointer.y + _mouseOffset.y,
                _initialPosition.z + _order);
        }


        private void OnMouseUp()
        {
            if (IsPaired) return;
            if (_spriteRenderer1 != null)
                _spriteRenderer1.sortingOrder = _spriteSo1;
            if (_spriteRenderer2 != null)
                _spriteRenderer2.sortingOrder = _spriteSo2;
            if (opacity)
                _anim.Colorize(1f, 0.2f);

            var colliders = new List<Collider2D>();
            collider.OverlapCollider(contactFilter.NoFilter(), colliders);

            if (colliders.Contains(socketCollider))
            {
                _order = 0;
                var toScale = Vector3.one;
                IsPaired = true;
                requiredField.pairedPieces.Add(this);
                if (setTransformWhenDrop != null)
                {
                    if (curveTrajectory.Equals(Vector3.zero))
                    {
                        var position1 = setTransformWhenDrop.position;
                        _anim.SetPositionZ(position1.z);
                        _anim.Move(transform.position, position1);
                    }
                    else
                    {
                        var position = transform.position;
                        _anim.MoveArc(position, position + curveTrajectory, setTransformWhenDrop.position, 1f);
                        Timeout.Set(this, 0.5f, () => { _anim.SetPositionZ(setTransformWhenDrop.position.z); });
                    }

                    var delta = setTransformWhenDrop.eulerAngles.z - transform.eulerAngles.z;
                    if (delta > 180f)
                        delta = 360f - delta;
                    _anim.Rotate(delta);
                    toScale = setTransformWhenDrop.localScale;
                }
                else if (requiredField.alignPieces)
                {
                    _anim.SetPositionZ(_initialPosition.z);
                    _anim.Move(new Vector3(requiredField.transform.position.x,
                        requiredField.transform.position.y, _initialPosition.z), 0.25f, _anim.SpringCurve);
                }
                else
                    _anim.SetPositionZ(_initialPosition.z);

                if (requiredField.parentPieces)
                    Timeout.Set(this, 0.2f, () => { transform.SetParent(requiredField.transform); });
                _anim.Scale(toScale, 0.2f);
                _taskTangram.CheckTangramPieces();
                if (activate != null)
                    activate.SetActive(true);
            }

            if (!IsPaired)
                _anim.Move(_initialPosition, 0.5f, _anim.SpringCurve);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Selection.activeObject != gameObject) return;
            Gizmos.color = Color.black;
            Gizmos.DrawRay(
                transform.position + Quaternion.Euler(transform.eulerAngles) *
                new Vector3(rayCastOffset.x, rayCastOffset.y), Vector3.forward);
        }

#endif
    }
}