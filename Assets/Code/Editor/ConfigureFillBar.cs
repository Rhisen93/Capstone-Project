using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Helper to configure Image as Filled type
/// Select the Image GameObject and click Tools > Configure Image as Filled
/// </summary>
public class ConfigureFillBar : MonoBehaviour
{
    [MenuItem("Tools/Configure Selected Image as Filled")]
    static void ConfigureImageAsFilled()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("No GameObject selected!");
            return;
        }

        Image image = Selection.activeGameObject.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Selected GameObject doesn't have an Image component!");
            return;
        }

        // Configure as Filled
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
        image.fillOrigin = (int)Image.OriginHorizontal.Left;
        image.fillAmount = 1f;

        Debug.Log($"Configured {Selection.activeGameObject.name} as Filled Image (Horizontal, Left, 1.0)");
        EditorUtility.SetDirty(image);
    }

    [MenuItem("Tools/Configure Selected Image as Filled", true)]
    static bool ValidateConfigureImageAsFilled()
    {
        return Selection.activeGameObject != null && 
               Selection.activeGameObject.GetComponent<Image>() != null;
    }
}
#endif
