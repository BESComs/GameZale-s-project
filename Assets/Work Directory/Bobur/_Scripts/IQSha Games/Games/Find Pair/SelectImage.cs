using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Find_Pair;

public class SelectImage : MonoBehaviour
{
    public Img CurImg;
    //основной класс
    public FindPair GameMainScript;
    private SpriteRenderer _backFon;
    private Color _color1, _color2;
    
    public void StartFunc()
    {
        //цвета спрайта если курсор находится на текущем элементе
        _backFon = CurImg.BackFon.GetComponent<SpriteRenderer>();
        _color1 = Color.white;
        _color1.a = .2f;
        _color2 = Color.white;
        _color2.a = .8f;
    }

    private void OnMouseOver()
    {
        if (_backFon.color != Color.green && _backFon.color != _color2)
            _backFon.color = _color2;
        //если игрок нажал на текущий элемент 
        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
        //помечает как выбранный/не выбранный 
        GameMainScript.ChildButton(CurImg);
        //соединяет с выбранной парой в другом столбце если есть
        //если состоит в паре удаляет из пары
        //иначе выделяет
        GameMainScript.CheckTransaction(CurImg);
    }

    private void OnMouseExit()
    {
        if (_backFon.color != Color.green && _backFon.color != _color1)
            _backFon.color = _color1;
    }
}
