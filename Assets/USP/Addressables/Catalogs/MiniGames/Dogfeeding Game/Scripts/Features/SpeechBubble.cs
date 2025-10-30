using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpeechBubbleImage : MonoBehaviour
{
    [Header("Bubble Sprites")]
    [SerializeField] private Sprite[] bubbleSprites; // Assign all your bubble images here
    [SerializeField] private float showDuration = 2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private Camera gamecamera;

    private SpriteRenderer bubbleRenderer;
    private Transform target;
    private Coroutine activeRoutine;

    private void Awake()
    {
        bubbleRenderer = GetComponent<SpriteRenderer>();
        bubbleRenderer.enabled = false;
        transform.localScale = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + offset;

        if (faceCamera && gamecamera != null)
            transform.rotation = Quaternion.identity; // keep facing forward for 2D
    }

    public void AttachTo(Transform targetTransform)
    {
        target = targetTransform;
    }

    [ContextMenu("Show Bubble Image")]
    public void test1()
    {
        ShowBubble(1);
    }

    /// <summary>
    /// Show a specific speech bubble image by index.
    /// </summary>
    public void ShowBubble(int index, float duration = -1f)
    {
        if (index < 0 || index >= bubbleSprites.Length)
        {
            Debug.LogWarning("Invalid bubble index");
            return;
        }

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(ShowRoutine(bubbleSprites[index], duration > 0 ? duration : showDuration));
    }

    private IEnumerator ShowRoutine(Sprite sprite, float duration)
    {
        bubbleRenderer.sprite = sprite;
        bubbleRenderer.enabled = true;

        // Pop in
        float t = 0f;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / 0.2f);
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        // Pop out
        t = 0f;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / 0.2f);
            yield return null;
        }

        bubbleRenderer.enabled = false;
    }
}
