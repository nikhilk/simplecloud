// LocalStorage.java
//

package simpleCloud.core;

import java.io.*;
import java.util.*;
import simpleCloud.services.*;

public final class LocalStorage implements StorageService {

    private StorageFile _root;

    public LocalStorage() {
        File root = new File(new File("app").getAbsolutePath());
        _root = new LocalFile("/", root, root);
    }

    @Override
    public StorageFile getPath(String path) {
        return _root.getFile(path);
    }

    @Override
    public StorageFile getRoot() {
        return _root;
    }


    final class LocalFile extends StorageFile {

        private final File _file;
        private final File _root;

        public LocalFile(String path, File file, File root) {
            super(path);
            _file = file;
            _root = root;
        }

        @Override
        public boolean exists() {
            return _file.exists();
        }

        @Override
        public boolean isDirectory() {
            return _file.isDirectory();
        }

        @Override
        public StorageFile getFile(String path) {
            File file = new File(_file, path);

            try {
                path = file.getCanonicalPath();
                file = new File(path);

                String absolutePath = file.getAbsolutePath();
                if (absolutePath.startsWith(_root.getAbsolutePath())) {
                    String relativePath = _root.toURI().relativize(file.toURI()).getPath();
                    return new LocalFile(relativePath, file, _root);
                }

                return null;
            }
            catch (IOException e) {
                throw new IllegalArgumentException("Invalid path '" + path + "'");
            }
        }

        @Override
        public List<StorageFile> getFiles() {
            if (!isDirectory()) {
                throw new UnsupportedOperationException("Files can only be retrieved from directories.");
            }

            ArrayList<StorageFile> files = new ArrayList<StorageFile>();

            for (File file : _file.listFiles()) {
                String relativePath = _root.toURI().relativize(file.toURI()).getPath();
                files.add(new LocalFile(relativePath, file, _root));
            }

            return files;
        }

        @Override
        public StorageFile getParent() {
            if (_file != _root) {
                File file = _file.getParentFile();

                String relativePath = _root.toURI().relativize(file.toURI()).getPath();
                return new LocalFile(relativePath, file, _root);
            }

            return null;
        }

        @Override
        public InputStream openForRead() throws IOException {
            return new FileInputStream(_file);
        }
    }
}
