using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Resalta claramente el botón que tiene el foco (navegación con mando): se agranda y muestra un contorno.
[RequireComponent(typeof(Button))]
public class ButtonSelectionFeedback : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float selectedScale = 1.12f;
    [SerializeField] private Color outlineColor = new Color(1f, 0.85f, 0.1f, 1f);
    [SerializeField] private Vector2 outlineDistance = new Vector2(4f, -4f);

    private Outline outline;
    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale;

        outline = GetComponent<Outline>();
        if (outline == null) outline = gameObject.AddComponent<Outline>();
        outline.effectColor = outlineColor;
        outline.effectDistance = outlineDistance;
        outline.enabled = false;
    }

    public void OnSelect(BaseEventData eventData) => SetSelected(true);
    public void OnDeselect(BaseEventData eventData) => SetSelected(false);

    // Si el panel se oculta con el boton seleccionado, no queda agrandado al volver a mostrarse.
    private void OnDisable() => SetSelected(false);

    private void SetSelected(bool selected)
    {
        transform.localScale = selected ? baseScale * selectedScale : baseScale;
        outline.enabled = selected;
    }
}