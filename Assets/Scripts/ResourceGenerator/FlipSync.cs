using UnityEngine;

public class FlipSync : MonoBehaviour
{
    private SpriteRenderer parentRenderer;
    private SpriteRenderer myRenderer;

    void Awake()
    {
        if (transform.parent != null)
            parentRenderer = transform.parent.GetComponent<SpriteRenderer>();
        else
            parentRenderer = null;

        myRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (parentRenderer != null && myRenderer != null)
        {
            myRenderer.flipX = parentRenderer.flipX;
        }
    }
}
