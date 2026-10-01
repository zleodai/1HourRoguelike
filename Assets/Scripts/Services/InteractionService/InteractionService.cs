using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionService : Service
{
    [SerializeField] private Camera interactionCamera;

    private InteractionData _hoveredData;

    public delegate void InformString(string data);
    public InformString StartViewingInteractionObject;
    public InformString EndViewingInteractionObject;

    public override void StartService()
    {
        if (interactionCamera == null)
        {
            interactionCamera = Camera.main;
        }

        if (interactionCamera == null)
        {
            Debug.LogWarning("[InteractionService] No interaction camera assigned and no MainCamera found.");
        }
    }

    private void Update()
    {
        if (interactionCamera == null)
        {
            interactionCamera = Camera.main;
            if (interactionCamera == null)
            {
                SetHoveredData(null);
                return;
            }
        }

        if (!interactionCamera.isActiveAndEnabled || Mouse.current == null)
        {
            SetHoveredData(null);
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // ScreenToWorldPoint's z value is distance from the camera. Find the
        // screen-space depth of the game's z=0 plane so the result lands there.
        float planeDepth = interactionCamera.WorldToScreenPoint(Vector3.zero).z;
        Vector3 mouseWorldPosition = interactionCamera.ScreenToWorldPoint(
            new Vector3(mousePosition.x, mousePosition.y, planeDepth));

        Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPosition);
        InteractionData hoveredData = hitCollider != null
            ? hitCollider.GetComponentInParent<InteractionData>()
            : null;

        SetHoveredData(hoveredData);
    }

    private void SetHoveredData(InteractionData nextData)
    {
        if (_hoveredData == nextData)
        {
            return;
        }

        if (_hoveredData != null)
        {
            EndViewingInteractionObject?.Invoke(_hoveredData.data);
        }

        _hoveredData = nextData;

        if (_hoveredData != null)
        {
            StartViewingInteractionObject?.Invoke(_hoveredData.data);
        }
    }

    private void OnDisable()
    {
        SetHoveredData(null);
    }
}
