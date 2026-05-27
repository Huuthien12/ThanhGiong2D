using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public enum PlayerForm { Baby, Adult, Armored, ArmoredWeapon, NguaGiao, NguaTre }
    public static PlayerControl Instance { get; private set; }

    [Header("=== FORM SCALE CONFIGURATIONS ===")]
    public Vector3 scaleBaby = new Vector3(0.6f, 0.6f, 1f);
    public Vector3 scaleAdult = new Vector3(1.5f, 1.5f, 1f);
    public Vector3 scaleArmored = new Vector3(2.0f, 2.0f, 1f);
    public Vector3 scaleArmoredWeapon = new Vector3(2.0f, 2.0f, 1f);
    public Vector3 scaleNguaGiao = new Vector3(1.8f, 1.8f, 1f);
    public Vector3 scaleNguaTre = new Vector3(2.2f, 2.2f, 1f);

    [Header("=== FORM SETTINGS ===")]
    public PlayerForm currentForm = PlayerForm.Baby;
    public RuntimeAnimatorController babyAnimator;
    public RuntimeAnimatorController adultAnimator;
    public RuntimeAnimatorController armoredAnimator;
    public RuntimeAnimatorController armoredWeaponAnimator;
    public RuntimeAnimatorController nguaGiaoAnimator;
    public RuntimeAnimatorController nguaTreAnimator;

    [Header("=== MOVEMENT SPEEDS PER FORM ===")]
    public float babySpeed = 3f;
    public float adultSpeed = 6f;
    public float armoredSpeed = 5.5f;
    public float armoredWeaponSpeed = 6f;
    public float nguaGiaoSpeed = 7f;
    public float nguaTreSpeed = 9f;

    [Header("=== JUMP FORCES PER FORM ===")]
    public float babyJumpForce = 0f;
    public float adultJumpForce = 6f;
    public float armoredJumpForce = 7f;
    public float armoredWeaponJumpForce = 7f;
    public float nguaGiaoJumpForce = 8f;
    public float nguaTreJumpForce = 10f;

    [Header("=== CLIMBING SETTINGS ===")]
    public float climbSpeed = 4f;
    private float verticalInput;
    private bool isNearStairs = false;
    private bool isClimbing = false;

    [Header("=== RICE & EVOLUTION MECHANICS ===")]
    public int riceCount = 0;
    public bool canUnlockArmor = false;

    [Header("=== UI & MAP BLOCK REFERENCES ===")]
    public TextMeshProUGUI notificationText;
    private GameObject mapBlocker;

    [Header("=== BASE MECHANICS ===")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    private float groundCheckRadius = 0.2f;

    [Header("=== ATTACK MECHANICS ===")]
    public Transform attackPoint;
    public float attackRange = 0.6f;
    public LayerMask enemyLayers;

    [Header("=== PLAYER HEALTH ===")]
    public int maxHealth = 5;
    private int currentHealth;
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

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
        currentHealth = maxHealth;
        Debug.Log($"❤️ Player khởi tạo với {currentHealth}/{maxHealth} máu");
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Map2_1" && GameManager.Instance != null && !GameManager.Instance.daQuaMapTruoc)
        {
            riceCount = 0;
            canUnlockArmor = false;
            Debug.Log("🔄 Reset cơm khi vào Map1 từ menu");
        }

        FindMapReferences();
        UpdateFormCapabilities();
        UpdateArmorUI();

        GameObject spawnPoint = null;
        spawnPoint = GameObject.Find("SpawnPoint");

        if (spawnPoint == null)
        {
            spawnPoint = GameObject.FindWithTag("SpawnPoint");
        }

        if (spawnPoint == null)
        {
            GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject go in allObjects)
            {
                if (go.name.ToLower() == "spawnpoint")
                {
                    spawnPoint = go;
                    break;
                }
            }
        }

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            Debug.Log($"📍 Teleport về SpawnPoint: {spawnPoint.name} tại {spawnPoint.transform.position}");

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy SpawnPoint trong scene!");
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

        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
                Debug.Log("🛡️ Hết thời gian bất tử");
            }
        }

        moveInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (isNearStairs && Mathf.Abs(verticalInput) > 0.1f && !isClimbing &&
            currentForm != PlayerForm.Armored && currentForm != PlayerForm.ArmoredWeapon)
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
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && currentJumpForce > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce);
            }
        }

        if (!isClimbing)
        {
            if (moveInput > 0 && !facingRight) Flip();
            else if (moveInput < 0 && facingRight) Flip();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (anim != null)
            {
                anim.SetTrigger("Attack");
                Debug.Log($"⚔️ Tấn công ở dạng [{currentForm}]!");
            }
            PerformAttackPhysics();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (anim != null)
            {
                anim.SetTrigger("Skill");
                Debug.Log($"✨ Dùng skill ở dạng [{currentForm}]!");
            }
            PerformAttackPhysics();
        }

        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveInput));
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetBool("IsClimbing", isClimbing);
        }

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
            rb.linearVelocity = new Vector2(moveInput * (currentSpeed * 0.5f), verticalInput * climbSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        }
    }

    private void PerformAttackPhysics()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("⚠️ AttackPoint chưa được gán!");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1, transform.position);
                Debug.Log($"⚡ Đánh trúng {enemy.name}!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            Debug.Log("🛡️ Đang bất tử, không nhận sát thương!");
            return;
        }

        Debug.Log($"🔥 Player nhận {damage} sát thương!");

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;

        StartCoroutine(HurtFlash());

        if (anim != null)
            anim.SetTrigger("Hurt");

        currentHealth -= damage;
        Debug.Log($"❤️ Máu còn: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator HurtFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float elapsed = 0f;
            while (elapsed < invincibilityDuration)
            {
                sr.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                sr.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.2f;
            }
            sr.color = Color.white;
        }
    }

    void Die()
    {
        Debug.Log("💀 Player đã chết!");
        enabled = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (anim != null)
            anim.SetTrigger("Die");

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.PlayerDied();
        }
    }

    public void AddRice()
    {
        riceCount++;
        UpdateArmorUI();

        // Ở Map 2_1, chỉ cần mở khóa giáp, KHÔNG biến hình
        if (riceCount >= 10)
        {
            canUnlockArmor = true;  // Mở khóa giáp cho Map 2_1
                                    // ❌ Xóa dòng ChangeForm ở đây
                                    // if (currentForm == PlayerForm.Baby)
                                    // {
                                    //     ChangeForm(PlayerForm.Adult);
                                    // }

            if (mapBlocker != null)
            {
                mapBlocker.SetActive(false);
            }
        }
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
            if (currentForm == PlayerForm.Armored || currentForm == PlayerForm.ArmoredWeapon ||
                currentForm == PlayerForm.NguaGiao || currentForm == PlayerForm.NguaTre)
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

        if (collision.CompareTag("VuKhi"))
        {
            if (currentForm == PlayerForm.Armored)
            {
                ChangeForm(PlayerForm.ArmoredWeapon);
                Destroy(collision.gameObject);
                Debug.Log("🔥 Thánh Gióng đã lấy được vũ khí thần thánh!");
            }
        }

        // Các vật phẩm để biến hình thành Ngựa Giao và Ngựa Trẻ
        if (collision.CompareTag("NguaGiaoItem"))
        {
            if (currentForm == PlayerForm.ArmoredWeapon)
            {
                ChangeForm(PlayerForm.NguaGiao);
                Destroy(collision.gameObject);
                Debug.Log("🐴 Biến hình thành Ngựa Giao!");
            }
        }

        if (collision.CompareTag("NguaTreItem"))
        {
            if (currentForm == PlayerForm.NguaGiao)
            {
                ChangeForm(PlayerForm.NguaTre);
                Destroy(collision.gameObject);
                Debug.Log("🦄 Biến hình thành Ngựa Trẻ - Hình thái tối thượng!");
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
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
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
        Debug.Log($"🦄 Đã biến hình thành: {newForm}");
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

            case PlayerForm.NguaGiao:
                currentSpeed = nguaGiaoSpeed;
                currentJumpForce = nguaGiaoJumpForce;
                groundCheckRadius = 0.35f;
                if (nguaGiaoAnimator != null && anim != null) { anim.runtimeAnimatorController = nguaGiaoAnimator; anim.Rebind(); }
                if (groundCheck != null) groundCheck.localPosition = new Vector3(0f, -0.65f, 0f);
                break;

            case PlayerForm.NguaTre:
                currentSpeed = nguaTreSpeed;
                currentJumpForce = nguaTreJumpForce;
                groundCheckRadius = 0.45f;
                if (nguaTreAnimator != null && anim != null) { anim.runtimeAnimatorController = nguaTreAnimator; anim.Rebind(); }
                if (groundCheck != null) groundCheck.localPosition = new Vector3(0f, -0.7f, 0f);
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
        if (currentForm == PlayerForm.NguaGiao) baseScale = scaleNguaGiao;
        if (currentForm == PlayerForm.NguaTre) baseScale = scaleNguaTre;

        baseScale.x = Mathf.Abs(baseScale.x) * (facingRight ? 1f : -1f);
        transform.localScale = baseScale;
    }

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