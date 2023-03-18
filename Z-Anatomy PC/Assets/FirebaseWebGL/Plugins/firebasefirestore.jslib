mergeInto(LibraryManager.library, {

    GetDocument: function (collectionPath, documentId, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedId = Pointer_stringify(documentId);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {
            firebase.firestore().collection(parsedPath).doc(parsedId).get().then(function (doc) {

                if (doc.exists) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(doc.data()));
                } else {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "null");
                }
            }).catch(function(error) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    GetDocumentsInCollection: function (collectionPath, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {
            firebase.firestore().collection(parsedPath).get().then(function (querySnapshot) {

                var docs = {};
                querySnapshot.forEach(function(doc) {
                    docs[doc.id] = doc.data();
                });

                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(docs));
            }).catch(function(error) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    SetDocument: function (collectionPath, documentId, value, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedId = Pointer_stringify(documentId);
        var parsedValue = Pointer_stringify(value);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            firebase.firestore().collection(parsedPath).doc(parsedId).set(JSON.parse(parsedValue)).then(function() {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: document " + parsedId + " was set");
            })
                .catch(function(error) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    AddDocument: function (collectionPath, value, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedValue = Pointer_stringify(value);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            firebase.firestore().collection(parsedPath).add(JSON.parse(parsedValue)).then(function(unused) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: document added in collection " + parsedPath);
            })
                .catch(function(error) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    UpdateDocument: function (collectionPath, documentId, value, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedId = Pointer_stringify(documentId);
        var parsedValue = Pointer_stringify(value);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            firebase.firestore().collection(parsedPath).doc(parsedId).update(JSON.parse(parsedValue)).then(function() {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: document " + parsedId + " was updated");
            })
                .catch(function(error) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    DeleteDocument: function (collectionPath, documentId, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedId = Pointer_stringify(documentId);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            firebase.firestore().collection(parsedPath).doc(parsedId).delete().then(function() {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: document " + parsedId + " was deleted");
            })
                .catch(function(error) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    DeleteField: function (collectionPath, documentId, field, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedId = Pointer_stringify(documentId);
        var parsedField = Pointer_stringify(field);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            var value = {};
            value[parsedField] = firebase.firestore.FieldValue.delete();

            firebase.firestore().collection(parsedPath).doc(parsedId).update(value).then(function() {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: field " + parsedField + " was deleted");
            })
                .catch(function(error) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    AddElementInArrayField: function (collectionPath, documentId, field, value, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedId = Pointer_stringify(documentId);
        var parsedField = Pointer_stringify(field);
        var parsedValue = Pointer_stringify(value);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            var value = {};
            value[parsedField] = firebase.firestore.FieldValue.arrayUnion(JSON.parse(parsedValue));

            firebase.firestore().collection(parsedPath).doc(parsedId).update(value).then(function() {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: element " + parsedValue + " was added in " + parsedField);
            })
                .catch(function(error) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    RemoveElementInArrayField: function (collectionPath, documentId, field, value, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedId = Pointer_stringify(documentId);
        var parsedField = Pointer_stringify(field);
        var parsedValue = Pointer_stringify(value);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            var value = {};
            value[parsedField] = firebase.firestore.FieldValue.arrayRemove(JSON.parse(parsedValue));

            firebase.firestore().collection(parsedPath).doc(parsedId).update(value).then(function() {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: element " + parsedValue + " was removed in " + parsedField);
            })
                .catch(function(error) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    IncrementFieldValue: function (collectionPath, documentId, field, increment, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedId = Pointer_stringify(documentId);
        var parsedField = Pointer_stringify(field);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            var value = {};
            value[parsedField] = firebase.firestore.FieldValue.increment(increment);

            firebase.firestore().collection(parsedPath).doc(parsedId).update(value).then(function() {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: incremented " + parsedField + " by " + increment);
            })
                .catch(function(error) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    ListenForDocumentChange: function (collectionPath, documentId, includeMetadataChanges, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedId = Pointer_stringify(documentId);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            if (typeof firestorelisteners === 'undefined') firestorelisteners = {};

            this.firestorelisteners[parsedPath + "/" + parsedId] = firebase.firestore().collection(parsedPath).doc(parsedId)
                .onSnapshot({
                    includeMetadataChanges: (includeMetadataChanges == 1)
                }, function(doc) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(doc.data()));
                }, function(error) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    StopListeningForDocumentChange: function (collectionPath, documentId, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedId = Pointer_stringify(documentId);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            if (typeof firestorelisteners === 'undefined') firestorelisteners = {};

            this.firestorelisteners[parsedPath + "/" + parsedId]();
            unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: listener was removed");
        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    ListenForCollectionChange: function (collectionPath, includeMetadataChanges, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            if (typeof firestorelisteners === 'undefined') firestorelisteners = {};

            this.firestorelisteners[parsedPath + "/collection/"] = firebase.firestore().collection(parsedPath)
                .onSnapshot({
                    includeMetadataChanges: (includeMetadataChanges == 1)
                }, function(querySnapshot) {

                    var docs = {};
                    querySnapshot.forEach(function(doc) {
                        docs[doc.id] = doc.data();
                    });

                    unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(docs));

                }, function(error) {
                    unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    StopListeningForCollectionChange: function (collectionPath, objectName, callback, fallback) {
        var parsedPath = Pointer_stringify(collectionPath);
        var parsedObjectName = Pointer_stringify(objectName);
        var parsedCallback = Pointer_stringify(callback);
        var parsedFallback = Pointer_stringify(fallback);

        try {

            if (typeof firestorelisteners === 'undefined') firestorelisteners = {};

            this.firestorelisteners[parsedPath + "/collection/"]();
            unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: listener was removed");
        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    }

});