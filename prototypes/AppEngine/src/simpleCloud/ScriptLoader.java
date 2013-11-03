package simpleCloud;

import java.io.*;

final class ScriptLoader {

    public String loadScript(String filePath) throws IOException {
        BufferedReader reader = new BufferedReader(new FileReader(filePath));
        try {
            StringBuffer buffer = new StringBuffer();
            char[] data = new char[1024];
            
            int readCount = 0;
            while((readCount = reader.read(data)) != -1){
                String s = String.valueOf(data, 0, readCount);
                buffer.append(s);
            }            
            
            return buffer.toString();
        }
        finally {
            reader.close();
        }
    }
}
