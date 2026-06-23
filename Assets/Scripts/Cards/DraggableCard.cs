using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Aggiunta l'interfaccia IDragHandler qui sotto
public class DraggableCardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform; // Più efficiente per muovere la UI rispetto al Transform classico
    private Image imageComponent;
    private Vector2 targetPosition; // Memorizza dove deve andare la carta
    private bool isDragging;
    
    [SerializeField] private float moveSpeedLimit = 50f;
    [HideInInspector] public bool wasDragged;
    
    [HideInInspector] public UnityEvent<DraggableCardController> BeginDragEvent;
    [HideInInspector] public UnityEvent<DraggableCardController> EndDragEvent;
    
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        imageComponent = GetComponent<Image>();
    }

    void Update()
    {
        if (isDragging)
        {
            // Movimento fluido verso la posizione del mouse
            Vector2 currentPos = rectTransform.anchoredPosition;
            Vector2 direction = (targetPosition - currentPos).normalized;
            float distance = Vector2.Distance(currentPos, targetPosition);
            
            // Calcola la velocità limitata
            float step = Mathf.Min(moveSpeedLimit * 100f, distance / Time.deltaTime); 
            rectTransform.anchoredPosition += direction * step * Time.deltaTime;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent?.Invoke(this);
        isDragging = true;
        wasDragged = true;

        // Disattiva il raycast SOLO sulla carta, così le zone di "Drop" sotto di essa possono rilevarla
        if (imageComponent != null) imageComponent.raycastTarget = false;

        SetTargetPosition(eventData);
    }

    // Questo metodo DEVE esserci, anche se aggiorna solo la posizione target
    public void OnDrag(PointerEventData eventData)
    {
        SetTargetPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent?.Invoke(this);
        isDragging = false;

        // Riattiva il raycast sulla carta
        if (imageComponent != null) imageComponent.raycastTarget = true;

        StartCoroutine(FrameWait());
    }

    private void SetTargetPosition(PointerEventData eventData)
    {
        // Questo metodo converte la posizione del mouse sullo schermo nella corretta posizione locale del Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, 
            eventData.position, 
            canvas.worldCamera, 
            out Vector2 localPoint
        );
        targetPosition = localPoint;
    }

    IEnumerator FrameWait()
    {
        yield return new WaitForEndOfFrame();
        wasDragged = false;
    }
}