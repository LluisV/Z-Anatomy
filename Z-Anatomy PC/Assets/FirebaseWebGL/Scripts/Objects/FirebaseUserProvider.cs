using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseWebGL.Scripts.Objects
{
    [Serializable]
    public class FirebaseUserProvider
    {
        public string displayName;

        public string email;

        public string photoUrl;

        public string providerId;

        public string userId;
    }
}