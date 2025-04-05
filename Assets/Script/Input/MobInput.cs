using UnityEngine;
using UnityEngine.EventSystems;

public class MobInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Car _car;
    [SerializeField] private float _inputValue;
    public void OnPointerDown(PointerEventData eventData)
    {
        _car.SetMoveInput(_inputValue);
    }

    public void OnPointerUp(PointerEventData eventData)
    {   
        _car.SetMoveInput(0f);
    }
}
