using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("=== UI REFERENCES ===")]
    public Image fillImage;           // Image có Image Type = Filled
    public Gradient colorGradient;    // Gradient màu theo % máu

    [Header("=== TÙY CHỈNH ===")]
    public bool fillHorizontal = true;  // Fill theo chiều ngang
    public float fillSpeed = 0.5f;      // Tốc độ thay đổi (tạo hiệu ứng mượt)

    private float targetFillAmount = 1f;
    private float currentFillAmount = 1f;

    void Start()
    {
        // Tìm Image nếu chưa gán
        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }

        // Cấu hình Image type là Filled
        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            if (fillHorizontal)
                fillImage.fillMethod = Image.FillMethod.Horizontal;
            else
                fillImage.fillMethod = Image.FillMethod.Vertical;
        }

        currentFillAmount = 1f;
        fillImage.fillAmount = 1f;
    }

    void Update()
    {
        // Hiệu ứng mượt khi thay đổi máu
        if (currentFillAmount != targetFillAmount)
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, fillSpeed * Time.deltaTime * 10f);
            if (fillImage != null)
            {
                fillImage.fillAmount = currentFillAmount;
            }
        }
    }

    public void UpdateBar(float currentHealth, float maxHealth)
    {
        if (fillImage == null) return;

        float healthPercent = currentHealth / maxHealth;
        targetFillAmount = Mathf.Clamp01(healthPercent);

        // Đổi màu theo % máu
        if (colorGradient != null)
        {
            fillImage.color = colorGradient.Evaluate(healthPercent);
        }
        else
        {
            // Mặc định: Xanh > 60%, Vàng > 30%, Đỏ < 30%
            if (healthPercent > 0.6f)
                fillImage.color = Color.green;
            else if (healthPercent > 0.3f)
                fillImage.color = Color.yellow;
            else
                fillImage.color = Color.red;
        }
    }

    // Cập nhật ngay lập tức (không hiệu ứng mượt)
    public void UpdateBarImmediate(float currentHealth, float maxHealth)
    {
        if (fillImage == null) return;

        float healthPercent = currentHealth / maxHealth;
        targetFillAmount = Mathf.Clamp01(healthPercent);
        currentFillAmount = targetFillAmount;
        fillImage.fillAmount = targetFillAmount;

        if (colorGradient != null)
        {
            fillImage.color = colorGradient.Evaluate(healthPercent);
        }
    }
}