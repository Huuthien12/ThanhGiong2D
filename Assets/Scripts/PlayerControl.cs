using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Bắt buộc phải có để điều khiển giao diện chữ hiển thị số cơm

public class PlayerControl : MonoBehaviour
{
    // Thêm dạng ArmoredWeapon (Mặc giáp có vũ khí) vào cuối danh sách Enum
    public enum PlayerForm { Baby, Adult, Armored, ArmoredWeapon }
    public static PlayerControl Instance { get; private set; }

    [Header("=== FORM SCALE CONFIGURATIONS ===")]
    public Vector3 scaleBaby = new Vector3(0.6f, 0.6f, 1f);
    public Vector3 scaleAdult = new Vector3(1.5f, 1.5f, 1f);
    public Vector3 scaleArmored = new Vector3(2.0f, 2.0f, 1f);
    public Vector3 scaleArmoredWeapon = new Vector3(2.0f, 2.0f, 1f); // Scale cho dạng có vũ khí

    [Header("=== FORM SETTINGS ===")]
    public PlayerForm currentForm = PlayerForm.Baby;

    public RuntimeAnimatorController babyAnimator;
    public RuntimeAnimatorController adultAnimator;
    public RuntimeAnimatorController armoredAnimator;
    public RuntimeAnimatorController armoredWeaponAnimator; // Kéo file Animator bộ vũ khí vào đây

    [Header("=== MOVEMENT SPEEDS PER FORM ===")]
    public float babySpeed = 3f;
    public float adultSpeed = 6f;
    public float armoredSpeed = 5.5f;
    public float armoredWeaponSpeed = 6f; // Thêm tốc độ dạng vũ khí (có thể cho chạy nhanh hơn)

    [Header("=== JUMP FORCES PER FORM ===")]
    public float babyJumpForce = 0f;
    public float adultJumpForce = 6f;
    public float armoredJumpForce = 7f;
    public float armoredWeaponJumpForce = 7f;

    [Header("=== CLIMBING SETTINGS (LEO THANG) ===")]
    public float climbSpeed = 4f;
    private float verticalInput;
    private bool isNearStairs = false;
    private bool isClimbing = false;

    [Header("=== RICE & EVOLUTION MECHANICS (ĂN CƠM & BIẾN HÌNH) ===")]
    public int riceCount = 0;              // Số cơm hiện tại đã ăn
    public bool canUnlockArmor = false;   // Trạng thái đã đủ điều kiện lấy giáp chưa

    [Header("=== UI & MAP BLOCK REFERENCES ===")]
    public TextMeshProUGUI notificationText; // Kéo object "ThongBao (2)" vào đây (hoặc code tự tìm)
    private GameObject mapBlocker;            // Tự động tìm bức tường "Block" chặn lối vào bệ giáp

    [Header("=== BASE MECHANICS ===")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    private float groundCheckRadius = 0.2f;   // Bán kính quét mặt đất tự động co giãn

    [Header("=== ATTACK MECHANICS (CHIẾN ĐẤU VẬT LÝ) ===")]
    public Transform attackPoint;       // Kéo một Object con đặt ở vị trí nắm đấm/vũ khí vào đây
    public float attackRange = 0.6f;    // Bán kính vùng đánh trúng quái (căn chỉnh tùy ý)
    public LayerMask enemyLayers;       // Chọn duy nhất Layer chứa quái vật (ví dụ: Layer "Enemies")

    private float currentSpeed;
    private float currentJumpForce;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private float moveInput;
    private bool facingRight = true;
    private float originalGravity;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rb = GetComponent<Rigidbody2D>();

        // Lấy Animator ở Object đồ họa con (Sprite_Render) để lật mặt không lỗi hoạt ảnh
        anim = GetComponentInChildren<Animator>();

        if (rb != null)
        {
            originalGravity = rb.gravityScale;
        }
    }

    void Start()
    {
        FindMapReferences();
        UpdateFormCapabilities();
        UpdateArmorUI();
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ========================================================
        // 🔄 CHỈ RESET CƠM, GIỮ NGUYÊN HÌNH DẠNG (FORM) TO LỚN
        // ========================================================
        if (scene.buildIndex != 0) // Bỏ qua màn hình Menu chính (Index 0)
        {
            riceCount = 0;                      // Reset số cơm về 0 để làm lại nhiệm vụ
            canUnlockArmor = false;             // Khóa trạng thái bệ giáp của Map mới lại

            Debug.Log($"🔄 Đã reset điểm cơm về 0. Giữ nguyên Form to lớn [{currentForm}] khi sang màn: {scene.name}");
        }
        // ========================================================

        // Khi sang Map mới, tự động quét tìm lại UI và tường Block của Map đó
        FindMapReferences();
        UpdateFormCapabilities(); // Cập nhật lại Animator, Tốc độ và Scale của Form hiện tại
        UpdateArmorUI();

        GameObject spawnPoint = null;
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject go in allObjects)
        {
            if (go.name == "SpawnPoint")
            {
                spawnPoint = go;
                break;
            }
        }

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            if (rb != null)
            {
#if UNITY_2023_1_OR_NEWER
                rb.linearVelocity = Vector2.zero;
#else
                rb.velocity = Vector2.zero;
#endif
            }
        }
    }

    private void FindMapReferences()
    {
        mapBlocker = GameObject.Find("Block");

        if (notificationText == null)
        {
            GameObject textObj = GameObject.Find("ThongBao (2)");
            if (textObj != null)
            {
                notificationText = textObj.GetComponent<TextMeshProUGUI>();
            }
        }

        if (canUnlockArmor && mapBlocker != null)
        {
            mapBlocker.SetActive(false);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // 🚫 KHÓA LEO THANG: Cả hai dạng giáp (Armored và ArmoredWeapon) đều không cần leo thang
        if (isNearStairs && Mathf.Abs(verticalInput) > 0.1f && !isClimbing && currentForm != PlayerForm.Armored && currentForm != PlayerForm.ArmoredWeapon)
        {
            isClimbing = true;
        }

        if (isClimbing)
        {
            if (rb != null) rb.gravityScale = 0f;
            isGrounded = false;
        }
        else
        {
            if (rb != null) rb.gravityScale = originalGravity;

            if (groundCheck != null)
            {
                // Sử dụng groundCheckRadius động để không bị lỗi vị trí khi phóng to Scale lên 2.0
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
#if UNITY_2023_1_OR_NEWER
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce);
#else
                rb.velocity = new Vector2(rb.velocity.x, currentJumpForce);
#endif
            }
        }

        if (!isClimbing)
        {
            if (moveInput > 0 && !facingRight) Flip();
            else if (moveInput < 0 && facingRight) Flip();
        }

        // ========================================================
        // ⚔️ KIỂM TRA BẤM NÚT TẤN CÔNG (PHÍM J) - HOÀN CHỈNH VẬT LÝ
        // ========================================================
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (anim != null)
            {
                // Kích hoạt Trigger "Attack" cho bất kỳ Animator Controller nào đang được gắn
                anim.SetTrigger("Attack");
                Debug.Log($"⚔️ Thánh Gióng ở dạng [{currentForm}] phát lệnh vung đòn!");
            }

            // Gọi hàm xử lý va chạm cơ học để đấm/vụt trúng quái vật
            PerformAttackPhysics();
        }
        // ========================================================

        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveInput));
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetBool("IsClimbing", isClimbing);
        }
    }

    void FixedUpdate()
    {
        if (isClimbing)
        {
#if UNITY_2023_1_OR_NEWER
            rb.linearVelocity = new Vector2(moveInput * (currentSpeed * 0.5f), verticalInput * climbSpeed);
#else
            rb.velocity = new Vector2(moveInput * (currentSpeed * 0.5f), verticalInput * climbSpeed);
#endif
        }
        else
        {
#if UNITY_2023_1_OR_NEWER
            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
#else
            rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
#endif
        }
    }

    // Hàm thực hiện quét va chạm tròn xung quanh điểm AttackPoint
    private void PerformAttackPhysics()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("⚠️ Bạn chưa tạo hoặc chưa kéo Object 'AttackPoint' vào bảng Inspector của Player!");
            return;
        }

        // Quét radar vật lý tất cả các Collider nằm trong tầm đánh và thuộc Layer quái vật
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Duyệt qua từng đối tượng quái dính đòn
        foreach (Collider2D enemy in hitEnemies)
        {
            // Kiểm tra xem con quái đó có bộ nhận diện máu EnemyHealth không
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Gây 1 điểm sát thương và truyền vị trí của người chơi để tính toán hướng đẩy quái ra xa
                enemyHealth.TakeDamage(1, transform.position);
            }
        }
    }

    public void AddRice()
    {
        riceCount++;
        if (riceCount >= 10)
        {
            canUnlockArmor = true;
            if (currentForm == PlayerForm.Baby)
            {
                ChangeForm(PlayerForm.Adult);
            }
            if (mapBlocker != null)
            {
                mapBlocker.SetActive(false);
            }
        }
        UpdateArmorUI();
    }

    private void UpdateArmorUI()
    {
        if (notificationText == null) return;

        if (!canUnlockArmor)
        {
            notificationText.text = $"Ăn đủ 10 cơm nắm để mở khóa giáp ({riceCount}/10)";
            notificationText.color = Color.white;
        }
        else
        {
            if (currentForm == PlayerForm.Armored || currentForm == PlayerForm.ArmoredWeapon)
            {
                notificationText.text = "";
            }
            else
            {
                notificationText.text = "🌟 Đã mở khóa! Hãy tiến vào bệ đá để mặc giáp.";
                notificationText.color = Color.yellow;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stairs"))
        {
            isNearStairs = true;
        }

        if (collision.CompareTag("BeGiap"))
        {
            if (canUnlockArmor && currentForm == PlayerForm.Adult)
            {
                ChangeForm(PlayerForm.Armored);
                Destroy(collision.gameObject);
                if (notificationText != null) notificationText.text = "";
            }
        }

        // 🌟 ĐÃ THÊM TRẠNG THÁI: Nhặt vũ khí tại bệ thờ ở map2_2
        if (collision.CompareTag("VuKhi"))
        {
            // Chỉ cho phép dạng mặc giáp thường nhặt để tiến hóa tiếp
            if (currentForm == PlayerForm.Armored)
            {
                ChangeForm(PlayerForm.ArmoredWeapon); // Biến hình thành dạng giáp có vũ khí!
                Destroy(collision.gameObject);        // Biến mất bệ thờ/vũ khí trên Map
                Debug.Log("🔥 Tuyệt vời! Thánh Gióng đã lấy được vũ khí thần thánh!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Stairs"))
        {
            isNearStairs = false;
            if (rb != null) rb.gravityScale = originalGravity;

            if (isClimbing)
            {
                isClimbing = false;
#if UNITY_2023_1_OR_NEWER
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
#else
                rb.velocity = new Vector2(rb.velocity.x, 0f);
#endif
                if (verticalInput > 0.1f)
                {
                    transform.position += new Vector3(0f, 0.2f, 0f);
                }
            }
        }
    }

    public void ChangeForm(PlayerForm newForm)
    {
        currentForm = newForm;
        if (anim == null) anim = GetComponentInChildren<Animator>();
        UpdateFormCapabilities();
    }

    private void UpdateFormCapabilities()
    {
        switch (currentForm)
        {
            case PlayerForm.Baby:
                currentSpeed = babySpeed;
                currentJumpForce = babyJumpForce;
                groundCheckRadius = 0.15f;
                if (babyAnimator != null && anim != null) { anim.runtimeAnimatorController = babyAnimator; anim.Rebind(); }
                if (groundCheck != null) groundCheck.localPosition = new Vector3(0f, -0.45f, 0f);
                break;

            case PlayerForm.Adult:
                currentSpeed = adultSpeed;
                currentJumpForce = adultJumpForce;
                groundCheckRadius = 0.25f;
                if (adultAnimator != null && anim != null) { anim.runtimeAnimatorController = adultAnimator; anim.Rebind(); }
                if (groundCheck != null) groundCheck.localPosition = new Vector3(0f, -0.55f, 0f);
                break;

            case PlayerForm.Armored:
                currentSpeed = armoredSpeed;
                currentJumpForce = armoredJumpForce;
                groundCheckRadius = 0.4f;
                if (armoredAnimator != null && anim != null) { anim.runtimeAnimatorController = armoredAnimator; anim.Rebind(); }
                if (groundCheck != null) groundCheck.localPosition = new Vector3(0f, -0.6f, 0f);
                break;

            case PlayerForm.ArmoredWeapon:
                currentSpeed = armoredWeaponSpeed;
                currentJumpForce = armoredWeaponJumpForce;
                groundCheckRadius = 0.4f;
                if (armoredWeaponAnimator != null && anim != null) { anim.runtimeAnimatorController = armoredWeaponAnimator; anim.Rebind(); }
                if (groundCheck != null) groundCheck.localPosition = new Vector3(0f, -0.6f, 0f);
                break;
        }
        ApplyScaleAndFlip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        ApplyScaleAndFlip();
    }

    private void ApplyScaleAndFlip()
    {
        Vector3 baseScale = scaleBaby;
        if (currentForm == PlayerForm.Adult) baseScale = scaleAdult;
        if (currentForm == PlayerForm.Armored) baseScale = scaleArmored;
        if (currentForm == PlayerForm.ArmoredWeapon) baseScale = scaleArmoredWeapon;

        baseScale.x = Mathf.Abs(baseScale.x) * (facingRight ? 1f : -1f);
        transform.localScale = baseScale;
    }

    // Vẽ vòng tròn xanh lam hỗ trợ căn khoảng cách đánh ngoài màn hình thiết kế Scene
    private void OnDrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}