using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class BasicLogger {

    private static string META_PATH = "";
    private static string CURRENT_FILE = "";

    private static StreamWriter writer;

    public static string currentFolder() {
        return META_PATH;
    }
    public static void newFolder(string name) {
        DirectoryInfo di = System.IO.Directory.CreateDirectory(META_PATH + name);
    }

    public static void switchFolder(string path) {
        META_PATH = path + '/';
    }

    public static void newFile(string name) {
        FileStream fs = File.Create(META_PATH + name);
        fs.Close();
    }

    public static void openFile(string name) {
        CURRENT_FILE = name;
        writer = new StreamWriter(META_PATH + CURRENT_FILE, true);
    }

    public static void newLine(string line) {
        writer.WriteLine(line);
    }

    public static void closeFile() {
        if(writer != null) writer.Close();

    }

    public static void readLine(int line) {

    }
}
