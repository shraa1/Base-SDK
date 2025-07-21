#if ENABLE_INPUT_SYSTEM
using BaseSDK.Controllers;

namespace BaseSDK {
    /// <summary>
    /// InputMaster is the New InputSystem asset, and class that is generated.
    /// We create an instance of this class to use. Could do more functionality here.
    /// </summary>
    public class InputMasterController : Singleton<InputMasterController> {
        private static InputMaster _inputMaster;
        /// <summary>
        /// The InputMaster asset file's class instance.
        /// </summary>
        public static InputMaster InputMaster => _inputMaster ??= new InputMaster();
    }
}
#endif