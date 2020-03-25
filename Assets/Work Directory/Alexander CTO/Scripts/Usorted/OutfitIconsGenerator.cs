using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class OutfitIconsGenerator : MonoBehaviour
{
    public CameraImageCapture imageCapture;
    public OutfitPartsContainer outfitContainer;

    [FolderPath(ParentFolder = "Assets")] public string saveFolderPath;

    public int textureSize;
    public int iconSize;

    [PreviewField(200)] public Texture2D iconsAtlas;

    [Button]
    public void GenerateIcons()
    {
        iconsAtlas = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false);

        foreach (var outfitPart in outfitContainer.outfitParts)
        {
            outfitPart.gameObject.SetActive(false);
        }

        var camTransform = imageCapture.cam.transform;

        int iconNumber = 0;
        foreach (var outfitPart in outfitContainer.outfitParts)
        {
            outfitPart.gameObject.SetActive(true);

            var offsetChangers = new List<TextureOffsetChanger>();
            foreach (var linkedMesh in outfitPart.linkedMeshes)
            {
                linkedMesh.gameObject.SetActive(true);
                var linkedOffsetChanger = linkedMesh.GetComponent<TextureOffsetChanger>();
                if (linkedOffsetChanger != null) offsetChangers.Add(linkedOffsetChanger);
            }
            offsetChangers.Add(outfitPart.GetComponent<TextureOffsetChanger>());

            var camPoint = outfitPart.transform.parent.Find("CamPoint");
            camTransform.position = camPoint.position;
            camTransform.rotation = camPoint.rotation;

            foreach (var textureVariant in outfitPart.textureVariants)
            {
                foreach (var offsetChanger in offsetChangers)
                {
                    offsetChanger.Awake();
                    offsetChanger.SetOffset(textureVariant.textureOffset);
                }


                var icon = imageCapture.CaptureImage(iconSize, iconSize);
                IconsProcessor.AddIconToTexture(iconsAtlas, icon, iconNumber);
                iconNumber++;
            }


            outfitPart.gameObject.SetActive(false);
            foreach (var linkedMesh in outfitPart.linkedMeshes)
                linkedMesh.gameObject.SetActive(false);
        }

        iconsAtlas.Apply();
    }

    [Button]
    public void SaveImage()
    {
        IconsProcessor.SaveImageAsPNG(iconsAtlas, $"{Application.dataPath}/{saveFolderPath}", "iconsPack");
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}