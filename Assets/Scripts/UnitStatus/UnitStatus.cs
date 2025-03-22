using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 유닛의 기본 능력치와 상태를 관리하는 클래스
/// </summary>
public class UnitStatus : MonoBehaviour
{
    [Header("Base Status Data")]
    [SerializeField] private BaseStatusData baseStatusData;  // BaseStatusData 참조

    [Header("Attack Timing")]
    [SerializeField] private float preAttackDelay;   // 공격 선딜레이
    [SerializeField] private float postAttackDelay;  // 공격 후딜레이
    [SerializeField] private float attackSpeed;      // 공격 속도 배율 (1이 기본)

    [Header("Death Effect")]
    [SerializeField] private float deathFadeTime;    // 사망 시 페이드 아웃 시간
    [SerializeField] private bool useDeathAnimation = true; // 사망 애니메이션 사용 여부

    // 체력 변경 이벤트
    public UnityEvent<float> onHealthChanged;
    public UnityEvent onUnitDeath;
    
    private bool isAttacking = false;                       // 현재 공격 중인지 여부
    private float lastAttackTime;                           // 마지막 공격 시간
    private bool isDead = false;                           // 사망 상태
    private SpriteRenderer spriteRenderer;                 // 스프라이트 렌더러 (2D)
    private Collider2D collider12D;                        // 2D 콜라이더
    private UnitMovement movement;                        // 이동 컴포넌트
    private UnitHealthBar healthBar;                       // 체력바 참조

    // 프로퍼티들
    public float MaxHealth => baseStatusData.maxHp;
    public float CurrentHealth 
    { 
        get => baseStatusData.currentHp;
        private set
        {
            baseStatusData.currentHp = Mathf.Clamp(value, 0f, baseStatusData.maxHp);
            onHealthChanged?.Invoke(baseStatusData.currentHp / baseStatusData.maxHp);
            
            if (baseStatusData.currentHp <= 0 && !isDead)
            {
                Die();
            }
        }
    }
    public float AttackDamage => baseStatusData.attackDamage;
    public float Defense => baseStatusData.defence;
    public float AttackRange => baseStatusData.attackRange;
    public bool IsAttacking => isAttacking;

    private void Start()
    {
        CurrentHealth = baseStatusData.maxHp;  // 프로퍼티 사용
        lastAttackTime = -999f;
        
        // 컴포넌트 참조
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider12D = GetComponent<Collider2D>();
        movement = GetComponent<UnitMovement>();
        healthBar = GetComponent<UnitHealthBar>();
        
        // 이벤트 초기화
        if (onHealthChanged == null)
            onHealthChanged = new UnityEvent<float>();
        if (onUnitDeath == null)
            onUnitDeath = new UnityEvent();
    }

    /// <summary>
    /// 데미지를 받는 메서드
    /// </summary>
    /// <param name="damage">받은 데미지</param>
    /// <returns>실제로 적용된 데미지</returns>
    public float TakeDamage(float damage)
    {
        if (isDead) return 0;

        float actualDamage = Mathf.Max(1, damage - baseStatusData.defence);
        CurrentHealth = CurrentHealth - actualDamage;  // 프로퍼티 사용
        return actualDamage;
    }

    /// <summary>
    /// 체력을 회복하는 메서드
    /// </summary>
    public void Heal(float amount)
    {
        if (isDead) return;
        CurrentHealth = CurrentHealth + amount;  // 프로퍼티 사용
    }

    /// <summary>
    /// 공격이 가능한 상태인지 확인하는 메서드
    /// </summary>
    public bool CanAttack()
    {
        return !isAttacking && Time.time >= lastAttackTime + (preAttackDelay + postAttackDelay) / attackSpeed;
    }

    /// <summary>
    /// 공격을 시도하는 메서드
    /// </summary>
    /// <returns>공격 시도 성공 여부</returns>
    public bool TryAttack()
    {
        if (!CanAttack()) return false;

        StartAttack();
        return true;
    }

    /// <summary>
    /// 공격을 시작하는 메서드
    /// </summary>
    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        // 선딜레이 후 공격 판정
        Invoke(nameof(ExecuteAttack), preAttackDelay);
    }

    /// <summary>
    /// 실제 공격 판정을 처리하는 메서드
    /// </summary>
    private void ExecuteAttack()
    {
        // TODO: 여기에 실제 공격 로직 구현
        // 예: 범위 내의 적 탐지 및 데미지 처리
        
        // 후딜레이 후 공격 상태 종료
        Invoke(nameof(EndAttack), postAttackDelay);
    }

    /// <summary>
    /// 공격 상태를 종료하는 메서드
    /// </summary>
    private void EndAttack()
    {
        isAttacking = false;
    }

    /// <summary>
    /// 유닛이 사망할 때 호출되는 메서드
    /// </summary>
    private void Die()
    {
        if (isDead) return; 
        isDead = true;

        // 사망 이벤트 발생
        onUnitDeath.Invoke();

        // 컴포넌트 비활성화
        if (movement != null)
            movement.enabled = false;
        if (collider12D != null)
            collider12D.enabled = false;

        if (useDeathAnimation && spriteRenderer != null)
        {
            // 페이드 아웃 효과 시작
            StartCoroutine(DeathFadeOut());
        }
        else
        {
            // 즉시 비활성화
            gameObject.SetActive(false);
        }

        // 체력바 비활성화
        if (healthBar != null)
        {
            healthBar.SetHealthBarVisible(false);
        }

        Debug.Log($"{gameObject.name} 유닛 사망");
    }

    /// <summary>
    /// 사망 시 페이드 아웃 효과
    /// </summary>
    private System.Collections.IEnumerator DeathFadeOut()
    {
        float elapsedTime = 0;
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsedTime < deathFadeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / deathFadeTime;
            
            // 알파값 감소
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            
            yield return null;
        }

        // 페이드 아웃 완료 후 오브젝트 비활성화
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 능력치를 수정하는 메서드들
    /// </summary>
    public void ModifyAttackDamage(float amount) => baseStatusData.attackDamage = Mathf.Max(0, baseStatusData.attackDamage + amount);
    public void ModifyDefense(float amount) => baseStatusData.defence = Mathf.Max(0, baseStatusData.defence + amount);
    public void ModifyAttackSpeed(float multiplier) => baseStatusData.attackSpeed = Mathf.Max(0.1f, baseStatusData.attackSpeed * multiplier);
    public void ModifyAttackRange(float amount) => baseStatusData.attackRange = Mathf.Max(0.1f, baseStatusData.attackRange + amount);

    // Update 메서드 추가
    private void Update()
    {
        // Inspector에서 currentHealth가 직접 수정되었을 때를 감지
        if (baseStatusData.currentHp <= 0 && !isDead)
        {
            Die();
        }
    }
}
