using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#region
//保持UTF-8
#endregion
public class IOUtil 
{
    public static void DeleteFileOrDire(string fileFullPath)
    {
        // 1、首先判断文件或者文件路径是否存在
        if (File.Exists(fileFullPath))
        {
            // 2、根据路径字符串判断是文件还是文件夹
            FileAttributes attr = File.GetAttributes(fileFullPath);
            // 3、根据具体类型进行删除
            if (attr == FileAttributes.Directory)
            {
                // 3.1、删除文件夹
                Directory.Delete(fileFullPath, true);
                Debug.Log($"删除文件夹{fileFullPath}");
            }
            else
            {
                // 3.2、删除文件
                File.Delete(fileFullPath);
                Debug.Log($"删除文件{fileFullPath}");
            }
            File.Delete(fileFullPath);
        }
        else
            Debug.LogError($"该路径不存在{fileFullPath}");
    }
}
