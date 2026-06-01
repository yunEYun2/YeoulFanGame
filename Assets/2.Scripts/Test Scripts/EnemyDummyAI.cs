using UnityEngine;

public class EnemyDummyAI : MonoBehaviour
{
    [Header("추적할 대상 (플레이어)")]
    public Transform player;
    
    [Header("이동 속도")]
    public float moveSpeed = 5f; 
    
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > 100f) return;

        // 경직(넉백)이 끝났는지 체크: 속도가 1 미만으로 멈췄을 때만 행동 개시
        if (Mathf.Abs(rb.linearVelocity.x) < 1f && Mathf.Abs(rb.linearVelocity.y) < 1f)
        {
            // [수정된 부분] X, Y 양방향 모두를 포함하여 플레이어의 '정중앙'을 향하는 방향 계산
            Vector2 direction = (player.position - transform.position).normalized;
            
            // X축, Y축 모두 플레이어를 향해 둥둥 떠서 다가오게 만듭니다.
            rb.linearVelocity = direction * moveSpeed;
        }
    }
}