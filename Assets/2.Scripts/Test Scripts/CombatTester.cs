using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필수

public class CombatTester : MonoBehaviour
{
    [Header("타격 대상 (적)")]
    public Rigidbody2D enemyRb;

    [Header("타격감 셋팅")]
    public float knockbackForce = 35f; 
    public float upwardLiftForce = 15f; 

    [Header("데미지 셋팅")]
    public float baseDamage = 10f;
    public float airComboMultiplier = 1.3f; 

    [Header("시각 효과 (VFX & 돌진)")]
    [Tooltip("타격 시 터질 파티클(VFX) 프리팹을 넣어주세요.")]
    public GameObject hitVFXPrefab; 
    [Tooltip("적에게 다가가는 돌진 속도")]
    public float dashSpeed = 80f; // 눈에 안 보일 정도로 빠르게 셋팅

    // 내부 연산용 변수
    private Vector3 originalPosition;
    private bool isAttacking = false; 

    void Update()
    {
        // 마우스 클릭 시, 현재 공격 중이 아닐 때만 실행
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            if (enemyRb != null)
            {
                StartCoroutine(AttackSequence());
            }
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        originalPosition = transform.position; // 원래 내 위치 기억

        // 1. [돌진] 적의 코앞(X축)까지 목표 지점 계산
        float dir = Mathf.Sign(enemyRb.position.x - transform.position.x);
        Vector3 targetPos = new Vector3(enemyRb.position.x - (dir * 1.5f), transform.position.y, transform.position.z);

        // 목표 지점에 도달할 때까지 초고속으로 이동 (Dash)
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, dashSpeed * Time.deltaTime);
            yield return null; 
        }

        // 2. [VFX 폭발] 타격 순간 이펙트 생성
        if (hitVFXPrefab != null)
        {
            // 적과 플레이어 사이의 타격 지점에 파티클 생성
            Vector3 vfxPos = (transform.position + new Vector3(enemyRb.position.x, enemyRb.position.y, 0)) / 2f;
            GameObject spawnedVFX = Instantiate(hitVFXPrefab, vfxPos, Quaternion.identity);
            
            // 메모리 누수 방지: 1초 뒤 파티클 자동 삭제
            Destroy(spawnedVFX, 1f);
        }

        // 3. [물리 타격] 넉백 및 히트 스톱, 화면 흔들림 발생
        Vector2 hitDirection = (enemyRb.transform.position - transform.position).normalized;
        hitDirection.y += (upwardLiftForce / knockbackForce); 
        hitDirection = hitDirection.normalized;

        float finalDamage = baseDamage;
        bool isAirborne = enemyRb.position.y > (transform.position.y + 0.5f);
        if (isAirborne) finalDamage *= airComboMultiplier;

        // 전투 매니저 호출! (히트스톱 + 카메라 쉐이크 + 넉백)
        CombatJuiceManager.Instance.TriggerImpact(enemyRb, hitDirection, knockbackForce);

        // 4. [복귀] 타격 포즈를 아주 잠깐(0.15초) 멈춰서 보여준 뒤 원래 자리로 돌아옴
        yield return new WaitForSeconds(0.15f); 
        
        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, (dashSpeed / 1.5f) * Time.deltaTime);
            yield return null;
        }

        isAttacking = false; // 공격 종료
    }
}