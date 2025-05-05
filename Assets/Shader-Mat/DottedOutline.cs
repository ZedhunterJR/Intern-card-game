using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Graphic))]
public class DottedOutline : MonoBehaviour
{
    private RectTransform rectTransform;
    private Material materialInstance;
    private Graphic graphic;

    private float cAspect;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        graphic = GetComponent<Graphic>();

        // Make sure we are modifying a unique material instance
        materialInstance = Instantiate(graphic.material);
        graphic.material = materialInstance;
    }

    void Update()
    {
        if (materialInstance == null || rectTransform == null)
            return;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;
        if (width <= 0 || height <= 0) return;


        float aspect = width / height;

        // Check if the aspect ratio has changed more than ~5%
        if (Mathf.Abs(cAspect - aspect) / aspect > 0.05f)
        {
            cAspect = aspect;
            materialInstance.SetFloat("_AspectRatio", cAspect);
        }
    }
}
