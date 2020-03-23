using UnityEngine;
using UnityEngine.EventSystems;

public class OpenCloseBook : MonoBehaviour, IPointerDownHandler
{
    public GameObject but;
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.parent.gameObject.SetActive(false);
        but.SetActive(true);
    }
}
