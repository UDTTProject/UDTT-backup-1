using UnityEngine;

[CreateAssetMenu(fileName = "BaseStatusData", menuName = "ScriptableObjects/BaseStatusData", order = 1)]
public class BaseStatusData : ScriptableObject
{
    /// <summary>
    /// 최대 체력
    /// </summary>
    public float maxHp;
    /// <summary>
    /// 현재 체력
    /// </summary>
    public float currentHp;
    /// <summary>
    /// 방어력
    /// </summary>
    public float defence;
    /// <summary>
    /// 공격 속도 배율 (1이 기본)
    /// </summary>
    public float preAttackDelay;
    /// <summary>
    /// 공격 선딜레이
    /// </summary>
    public float postAttackDelay;
    /// <summary>
    /// 공격력
    /// </summary>
    public float attackDamage;
    /// <summary>
    /// 공격 후딜레이
    /// </summary>
    public float attackSpeed;
    /// <summary>
    /// 공격 사거리
    /// </summary>
    public float attackRange;
    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed;
    /// <summary>
    /// 사망 시 페이드 아웃 시간
    /// </summary>
    public float deathFadeTime;
    /// <summary>
    /// 사망 애니메이션 사용 여부
    /// </summary>
    public bool useDeathAnimation;
}