using System;
using UnityEngine;

namespace FirebaseWebGL.Scripts.Objects
{
    [Serializable]
    public class FirebaseUserMetadata
    {
        public ulong lastSignInTimestamp;

        public ulong creationTimestamp;
    }
}
