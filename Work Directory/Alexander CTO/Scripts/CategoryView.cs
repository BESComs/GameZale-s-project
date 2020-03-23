using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameLabel;

    public CategoryData categoryData;

    public void Init(CategoryData categoryData)
    {
        this.categoryData = categoryData;
        icon.sprite = categoryData.categoryIcon;
        nameLabel.text = categoryData.categoryName;   
    }
    
    public void OnButtonClicked()
    {
        LessonsDatabase.Instance.SetOpenedTheme(categoryData.categoryId);
        LauncherV2.Instance.OpenCategory(categoryData);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (categoryData != null) Init(categoryData);
    }
#endif
}