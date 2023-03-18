using System.Runtime.InteropServices;

namespace FirebaseWebGL.Scripts.FirebaseBridge
{
    public static class FirebaseStorage
    {
        /// <summary>
        /// Uploads a byte array to storage
        /// </summary>
        /// <param name="path"> Storage path </param>
        /// <param name="data"> Bytes to upload encoded in a base 64 string </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void UploadFile(string path, string data, string objectName, string callback, string fallback);

        /// <summary>
        /// Downloads a byte array from storage
        /// </summary>
        /// <param name="path"> Storage path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output). Will return a base 64 encoded string </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void DownloadFile(string path, string objectName, string callback, string fallback);
    }
}