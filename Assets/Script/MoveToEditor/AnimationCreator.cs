using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Create an animation clip with a customizable frame rate
public class AnimationCreator : MonoBehaviour
{
    public string fileName;
    public Sprite[] sprites;
    public float fps = 5f; // Public property to adjust the frame rate dynamically

    [ContextMenu("Create Animation")]
    private void CreateAni()
    {
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogError("File name cannot be empty.");
            return;
        }

        string animationPath = "Assets/Animation/Enemy/" + fileName + ".anim";

        // Check if the animation file already exists
        if (AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath) != null)
        {
            Debug.LogError($"Animation file with the name '{fileName}' already exists at {animationPath}.");
            return;
        }

        AnimationClip clip = new AnimationClip();
        clip.frameRate = fps; // Use the user-defined frame rate

        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            spriteKeyFrames[i] = new ObjectReferenceKeyframe
            {
                time = (float)i / fps,
                value = sprites[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

        AssetDatabase.CreateAsset(clip, animationPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Animation '{fileName}.anim' created successfully at {animationPath}.");

        // Reset the input fields
        fileName = "";
        sprites = new Sprite[0];
    }

    [ContextMenu("Create UI Animation")]
    private void CreateUIAni()
    {
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogError("File name cannot be empty.");
            return;
        }

        string animationPath = "Assets/Animation/" + fileName + ".anim";

        // Check if the animation file already exists
        if (AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath) != null)
        {
            Debug.LogError($"Animation file with the name '{fileName}' already exists at {animationPath}.");
            return;
        }

        AnimationClip clip = new AnimationClip();
        clip.frameRate = fps;

        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(UnityEngine.UI.Image),
            path = "",
            propertyName = "m_Sprite"
        };

        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            spriteKeyFrames[i] = new ObjectReferenceKeyframe
            {
                time = (float)i / fps,
                value = sprites[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

        AssetDatabase.CreateAsset(clip, animationPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"UI Animation '{fileName}.anim' created successfully at {animationPath}.");

        // Reset the input fields
        fileName = "";
        sprites = new Sprite[0];
    }
}
