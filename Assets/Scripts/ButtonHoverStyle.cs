using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonHoverStyle : MonoBehaviour
{
    [Header("Màu sắc")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 0.8f, 0.2f);  // Màu vàng cam
    public Color clickColor = new Color(0.7f, 0.7f, 0.7f);

    private Button button;
    private TextMeshProUGUI buttonText;

    void Start()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (button != null && buttonText != null)
        {
            // Đặt màu chữ bình thường
            buttonText.color = normalColor;

            // Tạo màu cho các trạng thái của button
            ColorBlock colors = button.colors;
            colors.normalColor = Color.clear;  // Nền trong suốt
            colors.highlightedColor = new Color(0.2f, 0.2f, 0.2f, 0.3f);  // Nền mờ khi hover
            colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);       // Nền khi click
            button.colors = colors;
        }
    }

    // Cập nhật màu chữ khi hover (cách khác đơn giản hơn)
    public void OnPointerEnter()
    {
        if (buttonText != null) buttonText.color = hoverColor;
    }

    public void OnPointerExit()
    {
        if (buttonText != null) buttonText.color = normalColor;
    }
}