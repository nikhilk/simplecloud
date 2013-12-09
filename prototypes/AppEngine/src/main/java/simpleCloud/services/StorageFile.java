// StorageFile.java
//

package simpleCloud.services;

import java.io.*;
import java.util.*;

public abstract class StorageFile {

    private final String _path;

    protected StorageFile(String path) {
        _path = path;
    }

    public abstract boolean exists();

    public final String getFullName() {
        String path = _path;
        if (path.endsWith("/")) {
            path = path.substring(0, path.length() - 1);
        }

        int index = path.lastIndexOf('/');
        return path.substring(index + 1);
    }

    public final String getName() {
        String fullName = getFullName();

        int index = fullName.lastIndexOf('.');
        if (index > 0) {
            return fullName.substring(0, index);
        }
        else {
            return fullName;
        }
    }

    public final String getPath() {
        return _path;
    }

    public abstract boolean isDirectory();

    public String getContent() throws IOException {
        if (isDirectory()) {
            throw new UnsupportedOperationException("Content cannot be retrieved for directories.");
        }

        StringBuilder sb = new StringBuilder();

        InputStream stream = openForRead();
        try {
            Reader reader = new InputStreamReader(stream, "UTF-8");

            char[] buffer = new char[1024];
            int readCount;

            while((readCount = reader.read(buffer)) > 0) {
                sb.append(buffer, 0, readCount);
            }
        }
        finally {
            stream.close();
        }

        return sb.toString();
    }

    public abstract StorageFile getFile(String path);

    public abstract List<StorageFile> getFiles();

    public abstract StorageFile getParent();

    public abstract InputStream openForRead() throws IOException;
}
