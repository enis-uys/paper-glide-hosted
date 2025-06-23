using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TouchDetection : MonoBehaviour
{
    [SerializeField]
    private Text messageText;
    public static TouchDetection instance;
    public delegate void Swipe(Vector2 direction);
    public event Swipe OnSwipe;
    public delegate void Press();
    public event Press OnPress;

    private Vector2 initialPos;

    private PlayerControlls playerControlls;

    private Vector2 CurrentPos => playerControlls.Player.TouchPosition.ReadValue<Vector2>();
    private float touchStartTime;

    [SerializeField]
    private float pressTime = 0.2f;

    [SerializeField]
    private float swipeResistance = 400f;

    private InputAction touchPos,
        touchPress;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        playerControlls = new PlayerControlls();
    }

    private void OnEnable()
    {
        touchPos = playerControlls.Player.TouchPosition;
        touchPress = playerControlls.Player.TouchPress;
        touchPos.Enable();
        touchPress.Enable();
        touchPress.performed += _ =>
        {
            initialPos = CurrentPos;
            touchStartTime = Time.time; // Store the start time
        };
        touchPress.canceled += _ => DetectSwipe();
    }

    private void DetectSwipe()
    {
        messageText.text = "Swipe detected";
        // Berechnet die Differenz zwischen der aktuellen Berührungsposition und der Anfangsposition.
        Vector2 delta = CurrentPos - initialPos;

        // Initialisiert einen Vektor, um die Richtung des Swipes zu bestimmen (standardmäßig keine Bewegung).
        Vector2 swipeDirection = Vector2.zero;

        // Prüft, ob die horizontale Bewegung groß genug ist, um als Swipe betrachtet zu werden.
        if (Mathf.Abs(delta.x) > swipeResistance)
        {
            // Setzt die x-Komponente des swipeDirection-Vektors auf -1, 0 oder 1, je nach Richtung des Swipes.
            swipeDirection.x = Mathf.Sign(delta.x);
        }
        // Prüft, ob die vertikale Bewegung groß genug ist, um als Swipe betrachtet zu werden.
        else if (Mathf.Abs(delta.y) > swipeResistance)
        {
            // Setzt die y-Komponente des swipeDirection-Vektors auf -1, 0 oder 1, je nach Richtung des Swipes.
            swipeDirection.y = Mathf.Sign(delta.y);
        }

        // Berechnet die Dauer der Berührung.
        float touchDuration = Time.time - touchStartTime;

        // Überprüft, ob die Berührungsdauer kurz genug und die Bewegung klein genug ist, um als Druck (und nicht als Swipe) zu gelten.
        if (touchDuration < pressTime && delta.magnitude < swipeResistance)
        {
            // Zeigt eine Nachricht an und löst das OnPress-Ereignis aus.
            messageText.text = "Press detected";
            OnPress();
        }
        // Überprüft, ob ein Swipe-Ereignis vorhanden und die Swipe-Bewegung nicht Null ist.
        else if (OnSwipe != null && swipeDirection != Vector2.zero)
        {
            // Löst das OnSwipe-Ereignis mit der ermittelten Swipe-Richtung aus.
            OnSwipe(swipeDirection);
        }
    }
}
