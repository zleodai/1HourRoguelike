public class InputService : Service {
    public InputSystem_Actions Inputs { get; private set; }
    void OnDestroy() => Inputs?.Disable();
    public override void StartService() {
        Inputs = new InputSystem_Actions();
        Inputs.Enable();
    }
}