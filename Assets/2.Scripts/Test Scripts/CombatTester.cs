using UnityEngine;

public class CombatTester : MonoBehaviour
{
    [Header("타격 대상 (적)")]
    public Rigidbody2D enemyRb;

    [Header("타격감 셋팅")]
    public float knockbackForce = 35f; // 15 -> 35로 대폭 상향 (홈런 셋팅)
    public float upwardLiftForce = 15f; // 공중으로 띄우는 힘 상향

    [Header("데미지 셋팅")]
    public float baseDamage = 10f;
    public float airComboMultiplier = 1.3f; // 공중 콤보 보정

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (enemyRb != null)
            {
                // 1. 타격 방향 계산
                Vector2 hitDirection = (enemyRb.transform.position - transform.position).normalized;
                
                // 2. 대각선 위로 띄우는 힘 추가 (격투 게임의 찰진 맛)
                hitDirection.y += (upwardLiftForce / knockbackForce); 
                hitDirection = hitDirection.normalized;

                // 3. 공중 콤보 데미지 판정 (적이 공중에 떠 있는지 체크)
                float finalDamage = baseDamage;
                bool isAirborne = enemyRb.position.y > (transform.position.y + 0.5f);

                if (isAirborne)
                {
                    finalDamage *= airComboMultiplier;
                    Debug.Log($"<color=cyan>[공중 콤보!] 데미지 폭발: {finalDamage}</color>");
                }
                else
                {
                    Debug.Log($"[기본 타격] 데미지: {finalDamage}");
                }

                // 4. 타격 매니저 호출 (히트 스톱 + 카메라 흔들림 + 넉백 쾅!)
                CombatJuiceManager.Instance.TriggerImpact(enemyRb, hitDirection, knockbackForce);
            }
        }
    }
}