using UnityEngine;
using UnityEngine.InputSystem;

public class ExpandController : MonoBehaviour
{
    public Transform gunBarrel;

    [SerializeField] private InputActionReference interactAction;
    private GameStateManager _gameStateManager;
    private MapGenerator _mapGenerator;
    private Transform previousHighlight;

    private void Start()
    {
        _mapGenerator = FindObjectOfType<MapGenerator>();
        _gameStateManager = FindObjectOfType<GameStateManager>();
    }

    private void Update()
    {
        CheckButton();
    }

    private void OnEnable()
    {
        interactAction.action.performed += Interact;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= Interact;
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        if (previousHighlight != null)
        {
            var pos = previousHighlight.GetComponentInParent<EntranceBehaviour>().NodePosition;
            _mapGenerator.ExpandNode(pos);
            _gameStateManager.PlayerExpandedMap();
        }
    }

    private void CheckButton()
    {
        if (previousHighlight != null)
        {
            previousHighlight.GetComponent<Highlight>().SetHighlight(false);
            previousHighlight = null;
        }

        if (Physics.Raycast(gunBarrel.position, gunBarrel.forward, out var hit, 100))
            // Debug.Log(hit.transform.name);
            if (hit.transform.CompareTag("Entrance"))
            {
                var button = hit.transform;

                if (button != null)
                {
                    button.GetComponent<Highlight>().SetHighlight(true);
                    previousHighlight = button;
                }
            }
    }
}