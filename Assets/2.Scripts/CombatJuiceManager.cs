using UnityEngine;
using System.Collections;
using Unity.Cinemachine; // cinemachine namespace

public class CombatJuiceManager : MonoBehaviour
{
    // SingleTone
    public static CombatJuiceManager Instance;

    [Header("카메라 흔들림 설정")]
    [Tooltip("Cinemachine Impulse Source 컴포넌트를 연결해주세요.")]
    public CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// 공격 적중 시 호출하는 통합 타격감 함수
    /// </summary>
    /// <param name="targetRb">맞은 적의 Rigidbody2D</param>
    /// <param name="knockbackDir">날아갈 방향</param>
    /// <param name="knockbackForce">날아갈 힘(속도)</param>
    public void TriggerImpact(Rigidbody2D targetRb, Vector2 knockbackDir, float knockbackForce)
    {
        // 1. 히트 스톱 (0.1초 동안 시간을 0.05배속으로 느리게)
        StartCoroutine(HitStopRoutine(0.1f, 0.05f));

        // 2. 카메라 흔들림 발생
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        // 3. 물리 넉백 (선형 속도 직접 제어)
        ApplyKnockback(targetRb, knockbackDir, knockbackForce);
    }

    // 히트 스톱 코루틴
    private IEnumerator HitStopRoutine(float duration, float slowTimeScale)
    {
        Time.timeScale = slowTimeScale;
        // Time.timeScale이 변경되었으므로 실제 시간을 기준으로 대기해야 합니다.
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    // 넉백 적용 로직
    private void ApplyKnockback(Rigidbody2D rb, Vector2 dir, float force)
    {
        // 기존에 남아있던 물리력을 초기화한 뒤, 새롭게 강력한 선형 속도를 주입합니다.
        rb.linearVelocity = Vector2.zero; 
        rb.linearVelocity = dir.normalized * force;
    }
}