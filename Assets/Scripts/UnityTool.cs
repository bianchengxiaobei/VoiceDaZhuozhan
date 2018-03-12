using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework;
using System.IO;
using System.Security.Cryptography;
public class UnityTool : UnityToolBase
{
    private static Camera m_oUICamera = null;

    public static Camera UICamera
    {
        get
        {
            if (m_oUICamera == null)
            {
                m_oUICamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>(); 
            }
            return m_oUICamera;
        }
    }
    /// <summary>
    /// 返回所在时间内的帧数
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static int TimeToFrameCount(float time)
    {
        return (int)(time / Time.deltaTime);
    }
    /// <summary>
    /// 取得节点下的所有T组件,存到List中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="trans"></param>
    /// <param name="result"></param>
    /// <param name="includeInActive"></param>
    public static void GetComponentsInChildren<T>(Transform trans, List<T> result, bool includeInActive = false)
    {
        T[] components = trans.GetComponents<T>();
        for (int i = 0; i < components.Length; i++)
        {
            result.Add(components[i]);
        }
        for (int j = 0; j < trans.childCount; j++)
        {
            Transform child = trans.GetChild(j);
            if (includeInActive || child.gameObject.activeSelf)
            {
                UnityTool.GetComponentsInChildren<T>(child, result, includeInActive);
            }
        }
    }
    /// <summary>
    /// 两个碰撞器是否交互
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    public static bool IsColliderTounching(Collider2D c1, Collider2D c2)
    {
        return c1.bounds.min.x < c2.bounds.max.x && c1.bounds.max.x > c2.bounds.min.x && c1.bounds.min.y < c2.bounds.max.y && c1.bounds.max.y > c2.bounds.min.y;
    }

    public static Vector3 ConvertAngleToDir(int angle)
    {
        float angle1 = angle * 0.0001f;
        return new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1));
    }
    public static Vector3 ConvertPos(int x,int z,float y = 0)
    {
        return new Vector3(x * 0.001f, y, z * 0.001f);
    }
    public static float GetRotation(float x1, float y1, float x2, float y2)
    {
        float pi = 3.141593f;
        float diferenceX = x2 - x1;
        float diferenceY = y2 - y1;
        float Atan = (Mathf.Atan(diferenceY / diferenceX) * 180) / pi;
        if (diferenceX < 0)
        {
            Atan += 180;
        }
        return Atan;
    }
    public static float GetRotation(Vector2 left,Vector2 right)
    {
        Vector2 dir = right - left;
        return Vector2.SignedAngle(dir,Vector2.up);
    }
    public static Vector2 GetPivot(float h, float v, float size)
    {
        float horizontal = h - (UICamera.pixelWidth * 0.5f);
        float vertical = v - (UICamera.pixelHeight * 0.5f);

        float slope = vertical / horizontal;

        Vector2 vector = Vector2.zero;

        float num4;

        if ((slope > GetScreenSlope) || (slope < -GetScreenSlope))
        {
            num4 = (MidlleHeight - HalfSize(size)) / vertical;
            if (vertical < 0)
            {
                vector.y = HalfSize(size);
                num4 *= -1;
            }
            else
            {
                vector.y = UICamera.pixelHeight - HalfSize(size);
            }
            vector.x = MiddleWidth + (horizontal * num4);
            return vector;
        }
        num4 = (MiddleWidth - HalfSize(size)) / horizontal;
        if (horizontal < 0)
        {
            vector.x = HalfSize(size);
            num4 *= -1;
        }
        else
        {
            vector.x = UICamera.pixelWidth - HalfSize(size);
        }
        vector.y = MidlleHeight + (vertical * num4);
        return vector;
    }
    public static float MiddleWidth
    {
        get
        {
            return UICamera.pixelWidth / 2;
        }
    }
    public static float MidlleHeight
    {
        get
        {
            return UICamera.pixelHeight / 2;
        }
    }
    public static float HalfSize(float s)
    {
        return s / 2;
    }
    public static float GetScreenSlope
    {
        get
        {
            //Fix for Unity 5 
            float h = UICamera.pixelHeight;
            float w = UICamera.pixelWidth;

            float s = h / w;

            return s;
        }
    }
    public static GameObject AddChild(GameObject parent, GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }
        GameObject gameObject = UnityEngine.Object.Instantiate(prefab) as GameObject;
        if (gameObject != null && parent != null)
        {
            Transform transform = gameObject.transform;
            transform.parent = parent.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            if (transform is RectTransform)
            {
                (transform as RectTransform).sizeDelta = Vector2.zero;
            }
            gameObject.layer = parent.layer;
        }
        return gameObject;
    }
    /// <summary>
    /// 加载文本文件，返回字符串
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string LoadTxtFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            using (StreamReader sr = File.OpenText(filePath))
                return sr.ReadToEnd();
        }
        else
        {
            return string.Empty;
        }
    }
    /// <summary>
    /// 生成文件的md5
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string BuildFileMd5(string filename)
    {
        string filemd5 = null;
        try
        {
            using (var fileStream = File.OpenRead(filename))
            {
                //UnityEditor.AssetDatabase
                var md5 = MD5.Create();
                var fileMD5Bytes = md5.ComputeHash(fileStream);//计算指定Stream 对象的哈希值                            
                                                               //fileStream.Close();//流数据比较大，手动卸载 
                                                               //fileStream.Dispose();
                                                               //由以连字符分隔的十六进制对构成的string，其中每一对表示value 中对应的元素；例如“F-2C-4A”               
                filemd5 = FormatMD5(fileMD5Bytes);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
        return filemd5;
    }
    public static string FormatMD5(byte[] data)
    {
        return System.BitConverter.ToString(data).Replace("-", "").ToLower();
    }

    public static double ConvertMSToMis(long ms)
    {
        float secend = ms / 1000;
        return secend / 60;
    }
    public static List<T> RandomList<T>(List<T> sourceList)
    {
        if (sourceList == null || sourceList.Count == 0)
        {
            return sourceList;
        }
        List<T> random = new List<T>(sourceList.Count);
        do
        {
            int index = Random.Range(0, sourceList.Count);
            T target = sourceList[index];
            sourceList.Remove(target);
            random.Add(target);

        } while (sourceList.Count > 0);

        return random;

    }
}
