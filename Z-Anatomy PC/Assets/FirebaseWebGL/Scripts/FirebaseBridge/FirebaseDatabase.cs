using System.Runtime.InteropServices;

namespace FirebaseWebGL.Scripts.FirebaseBridge
{
    public static class FirebaseDatabase
    {
        /// <summary>
        /// Gets JSON from a specified path
        /// Will return a snapshot of the JSON in the callback output
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void GetJSON(string path, string objectName, string callback, string fallback);

        /// <summary>
        /// Posts JSON to a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="value"> JSON string to post to the specified path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void PostJSON(string path, string value, string objectName, string callback,
            string fallback);

        /// <summary>
        /// Pushes JSON to a specified path with a Firebase generated unique key
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="value"> JSON string to push to the specified path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void PushJSON(string path, string value, string objectName, string callback,
            string fallback);

        /// <summary>
        /// Updates JSON in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="value"> JSON string to update in the specified path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void UpdateJSON(string path, string value, string objectName, string callback,
            string fallback);

        /// <summary>
        /// Deletes JSON in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void DeleteJSON(string path, string objectName, string callback, string fallback);

        /// <summary>
        /// Listens for value changes in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the onValueChanged/fallback of </param>
        /// <param name="onValueChanged"> Name of the method to call when the listener is triggered. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void ListenForValueChanged(string path, string objectName, string onValueChanged,
            string fallback);

        /// <summary>
        /// Stops listening for value changed on a specific path
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void StopListeningForValueChanged(string path, string objectName, string callback,
            string fallback);

        /// <summary>
        /// Listens for value changes in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the onChildAdded/fallback of </param>
        /// <param name="onChildAdded"> Name of the method to call when the listener is triggered. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void ListenForChildAdded(string path, string objectName, string onChildAdded,
            string fallback);

        /// <summary>
        /// Stops listening for child added on a specific path
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void StopListeningForChildAdded(string path, string objectName, string callback,
            string fallback);

        /// <summary>
        /// Listens for value changes in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the onChildChanged/fallback of </param>
        /// <param name="onChildChanged"> Name of the method to call when the listener is triggered. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void ListenForChildChanged(string path, string objectName, string onChildChanged,
            string fallback);

        /// <summary>
        /// Stops listening for child changed on a specific path
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void StopListeningForChildChanged(string path, string objectName, string callback,
            string fallback);

        /// <summary>
        /// Listens for value changes in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the onChildRemoved/fallback of </param>
        /// <param name="onChildRemoved"> Name of the method to call when the listener is triggered. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void ListenForChildRemoved(string path, string objectName, string onChildRemoved,
            string fallback);

        /// <summary>
        /// Stops listening for child removed on a specific path
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void StopListeningForChildRemoved(string path, string objectName, string callback,
            string fallback);

        /// <summary>
        /// Adds a specified number to a numeric value in a specified path using race conditions safe transactions
        /// If the value is not numeric or doesn't exist it will be treated as 0
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="amount"> Number to add </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void ModifyNumberWithTransaction(string path, float amount, string objectName,
            string callback, string fallback);

        /// <summary>
        /// Toggles a boolean flag in a specified path using race conditions safe transactions
        /// If the value is not a boolean or doesn't exist it will be treated as false
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void ToggleBooleanWithTransaction(string path, string objectName, string callback,
            string fallback);
    }
}