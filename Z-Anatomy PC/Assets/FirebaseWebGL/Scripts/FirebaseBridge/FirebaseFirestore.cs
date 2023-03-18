using System.Runtime.InteropServices;

namespace FirebaseWebGL.Scripts.FirebaseBridge
{
    public static class FirebaseFirestore
    {
        /// <summary>
        /// Gets a document from a specified collection path and id
        /// Will return the document in JSON form in the callback output
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="documentId"> Document id </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void GetDocument(string collectionPath, string documentId, string objectName,
            string callback, string fallback);

        /// <summary>
        /// Gets all documents from a specified collection path
        /// Will return the documents in JSON array form in the callback output
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void GetDocumentsInCollection(string collectionPath, string objectName, string callback,
            string fallback);

        /// <summary>
        /// Sets document content to a specified collection path and document id
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="documentId"> Document id </param>
        /// <param name="value"> JSON document content </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void SetDocument(string collectionPath, string documentId, string value, string objectName,
            string callback,
            string fallback);

        /// <summary>
        /// Adds a document to a specified collection path with a firebase generated id
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="value"> JSON document content </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void AddDocument(string collectionPath, string value, string objectName, string callback,
            string fallback);

        /// <summary>
        /// Updates document content in a specified collection path and document id
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="documentId"> Document id </param>
        /// <param name="value"> JSON document content </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void UpdateDocument(string collectionPath, string documentId, string value,
            string objectName, string callback,
            string fallback);

        /// <summary>
        /// Deletes document in a specified collection path and document id
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="documentId"> Document id </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void DeleteDocument(string collectionPath, string documentId, string objectName,
            string callback, string fallback);

        /// <summary>
        /// Deletes a field in a specified collection path and document id
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="documentId"> Document id </param>
        /// <param name="field"> Field to delete </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void DeleteField(string collectionPath, string documentId, string field, string objectName,
            string callback, string fallback);

        /// <summary>
        /// Adds an element in an array field in a specified collection path and document id
        /// Note: If the element is already in the array, it won't do anything
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="documentId"> Document id </param>
        /// <param name="field"> Array field </param>
        /// <param name="value"> Element to add to the array field </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void AddElementInArrayField(string collectionPath, string documentId, string field,
            string value, string objectName, string callback, string fallback);

        /// <summary>
        /// Removes an element in an array field in a specified collection path and document id
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="documentId"> Document id </param>
        /// <param name="field"> Array field </param>
        /// <param name="value"> Element to remove from the array field </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void RemoveElementInArrayField(string collectionPath, string documentId, string field,
            string value, string objectName, string callback, string fallback);

        /// <summary>
        /// Increments a numeric field in a specified collection path and document id by a certain amount
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="documentId"> Document id </param>
        /// <param name="field"> Field to increment </param>
        /// <param name="increment"> Increment amount </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void IncrementFieldValue(string collectionPath, string documentId, string field,
            int increment, string objectName, string callback, string fallback);

        /// <summary>
        /// Listens for document content changes in a specified collection path and document id
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="documentId"> Document id </param>
        /// <param name="includeMetadataChanges"> Whether the listener should trigger for metadata changes </param>
        /// <param name="objectName"> Name of the gameobject to call the onChildChanged/fallback of </param>
        /// <param name="onDocumentChange"> Name of the method to call when the listener is triggered. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void ListenForDocumentChange(string collectionPath, string documentId,
            bool includeMetadataChanges, string objectName, string onDocumentChange,
            string fallback);

        /// <summary>
        /// Stops listening for document changes
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="documentId"> Document id </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void StopListeningForDocumentChange(string collectionPath, string documentId,
            string objectName, string callback, string fallback);

        /// <summary>
        /// Listens for collection changes in a specified collection path
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="includeMetadataChanges"> Whether the listener should trigger for metadata changes </param>
        /// <param name="objectName"> Name of the gameobject to call the onChildChanged/fallback of </param>
        /// <param name="onCollectionChange"> Name of the method to call when the listener is triggered. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void ListenForCollectionChange(string collectionPath, bool includeMetadataChanges,
            string objectName, string onCollectionChange, string fallback);

        /// <summary>
        /// Stops listening for collection changes
        /// </summary>
        /// <param name="collectionPath"> Collection path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal")]
        public static extern void StopListeningForCollectionChange(string collectionPath, string objectName,
            string callback, string fallback);
    }
}