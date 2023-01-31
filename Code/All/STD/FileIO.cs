using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

public class FileIO
{
    public static void save(string path, byte[] bytes)
    {
        FileStream fs = File.Open(pathForDocumentsFile(path), FileMode.OpenOrCreate);
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
    }

    public static byte[] load(string path)
    {
        FileInfo fileInfo = new FileInfo(pathForDocumentsFile(path));
        if(!fileInfo.Exists)
            return null;
        
        FileStream fs = File.Open(pathForDocumentsFile(path), FileMode.Open);

        int len = (int)fs.Length;
        byte[] bytes = new byte[len];
        fs.Read(bytes, 0, len);
        fs.Close();

        return bytes;
    }

    public static string bytes2string(byte[] bytes)
    {
        return Encoding.Default.GetString(bytes);
    }

    public static byte[] string2bytes(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    public static byte[] struct2bytes(object obj)
    {
        int len = Marshal.SizeOf(obj);
        byte[] bytes = new byte[len];

        IntPtr ptr = Marshal.AllocHGlobal(len);
        Marshal.StructureToPtr(obj, ptr, false);
        Marshal.Copy(ptr, bytes, 0, len);
        Marshal.FreeHGlobal(ptr);

        return bytes;
    }

    public static T bytes2struct<T>(byte[] bytes) where T : struct
    {
        int len = Marshal.SizeOf(typeof(T));
        if(len > bytes.Length)
        {
            throw new Exception();
        }

        IntPtr ptr = Marshal.AllocHGlobal(len);
        Marshal.Copy(bytes, 0, ptr, len);
        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);

        return obj;
    }

    public static string pathForDocumentsFile(string filename)
    {
        string path;

        if (Application.platform == RuntimePlatform.Android)
            path = Application.persistentDataPath;
        else
            path = Application.dataPath;

        path = path.Substring(0, path.LastIndexOf('/'));

        return Path.Combine(path, filename);
    }
}
