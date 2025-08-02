using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BouncingUIElement : MonoBehaviour
{
    [SerializeField] private float speed = 200f; // 移动速度
    [SerializeField] private Vector2 direction = Vector2.one.normalized; // 初始移动方向

    private RectTransform rectTransform;
    private Canvas canvas;
    private Rect canvasRect;
    private Vector2 canvasSize;
    private Vector2 elementSize;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        // 确保方向是单位向量
        direction = direction.normalized;
        
        // 获取画布和元素尺寸
        UpdateSizes();
    }

    private void Update()
    {
        UpdateSizes();
        
        // 移动UI元素
        rectTransform.anchoredPosition += direction * speed * Time.deltaTime;

        // 检测碰撞并反弹
        CheckBoundaryCollision();
    }

    private void UpdateSizes()
    {
        if (canvas == null) return;

        // 获取画布实际尺寸
        canvasRect = canvas.GetComponent<RectTransform>().rect;
        canvasSize = new Vector2(canvasRect.width, canvasRect.height);
        
        // 获取UI元素尺寸
        elementSize = rectTransform.rect.size;
    }

    private void CheckBoundaryCollision()
    {
        Vector2 currentPosition = rectTransform.anchoredPosition;
        bool bounced = false;

        // 检查左右边界
        if (currentPosition.x + elementSize.x / 2 > canvasSize.x / 2)
        {
            direction.x = -Mathf.Abs(direction.x);
            bounced = true;
        }
        else if (currentPosition.x - elementSize.x / 2 < -canvasSize.x / 2)
        {
            direction.x = Mathf.Abs(direction.x);
            bounced = true;
        }

        // 检查上下边界
        if (currentPosition.y + elementSize.y / 2 > canvasSize.y / 2)
        {
            direction.y = -Mathf.Abs(direction.y);
            bounced = true;
        }
        else if (currentPosition.y - elementSize.y / 2 < -canvasSize.y / 2)
        {
            direction.y = Mathf.Abs(direction.y);
            bounced = true;
        }

        // 如果发生反弹，重新归一化方向向量以保持速度一致
        if (bounced)
        {
            direction = direction.normalized;
        }
    }

    // 在编辑器中重置方向按钮
    [ContextMenu("Reset Direction")]
    private void ResetDirection()
    {
        direction = Vector2.one.normalized;
    }
}