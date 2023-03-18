using System;
using System.Collections.Generic;

namespace FirebaseWebGL.Scripts.Objects
{
    [Serializable]
    public class FirebaseUser
    {
        public string displayName;
        
        public string email;
        
        public bool isAnonymous;
        
        public bool isEmailVerified;
        
        public FirebaseUserMetadata metadata;
        
        public string phoneNumber;
        
        public FirebaseUserProvider[] providerData;
        
        public string providerId;
        
        public string uid;
    }
}