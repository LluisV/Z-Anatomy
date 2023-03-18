mergeInto(LibraryManager.library, {

    GetCurrentProjectId: function () {
        var projectId = firebaseConfig.projectId;
        var bufferSize = lengthBytesUTF8(projectId) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(projectId, buffer, bufferSize);
        return buffer;
    }

});