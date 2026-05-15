using System.Collections;
using TMPro;
using UnityEngine;

public class Floating3DText : MonoBehaviour
{
    public float floatSpeed = 0.3f;
    public float lifeTime = 1.5f;

    private TextMeshPro textMeshPro;  

    void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        if (textMeshPro == null)
        {
            Debug.LogError("Không tìm thấy TextMeshPro!");
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, lifeTime);
        StartCoroutine(FadeOut());
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color originalColor = textMeshPro.color;

        while (elapsed < lifeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / lifeTime);
            textMeshPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }

    public void SetText(string text)
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = text;
        }
    }
}