#if !AB_MODE && UNITY_EDITOR
#else
#define _AB_MODE_
#endif
using UnityEngine;
using System.Collections.Generic;
using CaomaoFramework;
using System;
using System.IO;
using System.Collections;
using Tangzx.ABSystem;
using CaomaoFramework.Data;
/*----------------------------------------------------------------
// 模块名：WWWResourceManager
// 创建者：chen
// 修改者列表：
// 创建日期：
// 模块描述：
//--------------------------------------------------------------*/
/// <summary>
/// 
/// </summary>

public class WWWResourceManager : MonoBehaviour
{
    public static Version version = new Version(0, 1, 0);
    public static WWWResourceManager m_oInstance;
    public static bool enableLog = true;
    public bool m_bLoadError = false;
    public delegate void LoadAssetCompleteHandler(AssetBundleInfo info);
    public delegate void LoaderCompleteHandler(AssetBundleLoader info);
    public delegate void LoadProgressHandler(AssetBundleLoadProgress progress);
    private Action<bool> m_allLoadFinishedEventHandler;
    private Action<bool> m_allUnLoadFinishedEventHandler;
    private Dictionary<string, AssetBundle> m_dicAtlasCache = new Dictionary<string, AssetBundle>();
    private Dictionary<string, Sprite> m_dicDynamicSpriteCache = new Dictionary<string, Sprite>();
    /// <summary>
    /// 同时最大的加载数
    /// </summary>
    private const int MAX_REQUEST = 100;
    /// <summary>
    /// 可再次申请的加载数
    /// </summary>
    private int _requestRemain = MAX_REQUEST;
    /// <summary>
    /// 当前申请要加载的队列
    /// </summary>
    private List<AssetBundleLoader> _requestQueue = new List<AssetBundleLoader>();

    /// <summary>
    /// 加载队列
    /// </summary>
    private List<AssetBundleLoader> _currentLoadQueue = new List<AssetBundleLoader>();
    /// <summary>
    /// 未完成的
    /// </summary>
    private HashSet<AssetBundleLoader> _nonCompleteLoaderSet = new HashSet<AssetBundleLoader>();
    /// <summary>
    /// 此时加载的所有Loader记录，(用于在全加载完成之后设置 minLifeTime)
    /// </summary>
    private HashSet<AssetBundleLoader> _thisTimeLoaderSet = new HashSet<AssetBundleLoader>();
    /// <summary>
    /// 已加载完成的缓存列表
    /// </summary>
    private Dictionary<string, AssetBundleInfo> _loadedAssetBundle = new Dictionary<string, AssetBundleInfo>();
    /// <summary>
    /// 已创建的所有Loader列表(包括加载完成和未完成的)
    /// </summary>
    private Dictionary<string, AssetBundleLoader> _loaderCache = new Dictionary<string, AssetBundleLoader>();
    /// <summary>
    /// 当前是否还在加载，如果加载，则暂时不回收
    /// </summary>
    private bool _isCurrentLoading;

    private AssetBundleLoadProgress _progress = new AssetBundleLoadProgress();
    /// <summary>
    /// 进度
    /// </summary>
    public LoadProgressHandler onProgress;

    public AssetBundlePathResolver pathResolver;

    private AssetBundleDataReader _depInfoReader;

    private Action _initCallback;

    /// <summary>
    /// 加载进度
    /// </summary>
    public float ProgressValue
    {
        get
        {
            if (this._progress == null)
            {
                Debug.LogError("null == m_resourceManager");
                return 0f;
            }
            if (this._progress.total > 0)
                return this._progress.complete / this._progress.total;
            else return 0;
        }
    }

    public WWWResourceManager()
    {
        pathResolver = new AssetBundlePathResolver();
    }

    public AssetBundleDataReader depInfoReader { get { return _depInfoReader; } }

    public static WWWResourceManager Instance
    {
        get
        {
            if (WWWResourceManager.m_oInstance == null)
            {
                WWWResourceManager.m_oInstance = UnityMonoDriver.s_instance.transform.Find("WWWResourceManager").GetComponent<WWWResourceManager>();
            }
            return WWWResourceManager.m_oInstance;
        }
    }

    protected void Awake()
    {
        InvokeRepeating("CheckUnusedBundle", 0, 5);
    }

    public void Init(Stream depStream, Action callback)
    {
        if (depStream.Length > 4)
        {
            BinaryReader br = new BinaryReader(depStream);
            if (br.ReadChar() == 'A' && br.ReadChar() == 'B' && br.ReadChar() == 'D')
            {
                if (br.ReadChar() == 'T')
                    _depInfoReader = new AssetBundleDataReader();
                else
                    _depInfoReader = new AssetBundleDataBinaryReader();

                depStream.Position = 0;
                _depInfoReader.Read(depStream);
            }
        }

        depStream.Close();

        if (callback != null)
            callback();
    }

    void InitComplete()
    {
        if (_initCallback != null)
            _initCallback();
        _initCallback = null;
    }

    IEnumerator LoadDepInfo()
    {
        string depFile = string.Format("{0}/{1}", pathResolver.BundleCacheDir, pathResolver.DependFileName);
        //编辑器模式下测试AB_MODE，直接读取
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        depFile = pathResolver.GetBundleSourceFile(pathResolver.DependFileName, false);
#endif
        if (File.Exists(depFile))
        {
            FileStream fs = new FileStream(depFile, FileMode.Open, FileAccess.Read);
            Init(fs, null);
            fs.Close();
        }
        else
        {
            string srcURL = pathResolver.GetBundleSourceFile(pathResolver.DependFileName);;
            WWW w = new WWW(srcURL);
            yield return w;

            if (w.error == null)
            {
                Init(new MemoryStream(w.bytes), null);
                XMLParser.SaveBytes(depFile, w.bytes);
                //File.WriteAllBytes(depFile, w.bytes);
            }
            else
            {
                Debug.LogError(string.Format("{0} not exist!", depFile));
            }
        }
        LoadAtlas("common1.ab");
        this.InitComplete();
    }
    public void LoadAtlas(string atlaName)
    {
        string path = string.Format("{0}/{1}", AssetBundlePathResolver.instance.BundleCacheDir, atlaName);
        if (!File.Exists(path))
        {
            path = AssetBundlePathResolver.instance.GetBundleSourceFile(atlaName, false);
        }
        if (!this.m_dicAtlasCache.ContainsKey(atlaName))
        {
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            this.m_dicAtlasCache.Add(atlaName, ab);
        }
        else
        {
            Debug.Log("已经加载该图集:" + atlaName);
        }

    }
    /// <summary>
    /// 加载图集里面的Sprite
    /// </summary>
    /// <param name="atlaName"></param>
    /// <param name="sprite"></param>
    /// <returns></returns>
    public Sprite LoadSpriteFormAtla(string atlaName,string sprite)
    {
        Sprite result = null;
        //已经存在缓存
        if (this.m_dicDynamicSpriteCache.ContainsKey(sprite))
        {
            result = this.m_dicDynamicSpriteCache[sprite];
        }
        else
        {        
            //如果还没有加载该图集
            if (!this.m_dicAtlasCache.ContainsKey(atlaName))
            {
                Debug.LogError("No Atla:" + atlaName);
                this.LoadAtlas(atlaName);
            }
            Sprite[] temp = this.m_dicAtlasCache[atlaName].LoadAssetWithSubAssets<Sprite>(sprite);
            if (temp == null || temp.Length <= 0)
            {
                Debug.LogError("Sprite == null:"+sprite+"Atla="+atlaName);
                return null;
            }
            result = temp[0];
            if (!this.m_dicDynamicSpriteCache.ContainsKey(sprite))
            {
                this.m_dicDynamicSpriteCache.Add(sprite, result);
            }
        }
        return result;
    }
    void OnDestroy()
    {
        this.RemoveAll();
    }

    /// <summary>
    /// 通过ShortName获取FullName
    /// </summary>
    /// <param name="shortFileName"></param>
    /// <returns></returns>
    public string GetAssetBundleFullName(string shortFileName)
    {
        return _depInfoReader.GetFullName(shortFileName);
    }

    /// <summary>
    /// 用默认优先级为0的值加载
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="handler">回调</param>
    /// <returns></returns>
    public AssetBundleLoader Load(string path, LoadAssetCompleteHandler handler = null)
    {
        return Load(path, 0, handler);
    }

    /// <summary>
    /// 通过一个路径加载ab
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="prority">优先级</param>
    /// <param name="handler">回调</param>
    /// <returns></returns>
    public AssetBundleLoader Load(string path, int prority, LoadAssetCompleteHandler handler = null)
    {
#if _AB_MODE_
        AssetBundleLoader loader = this.CreateLoader(HashUtil.Get(path.ToLower()) + ".ab", path);
#else
        AssetBundleLoader loader = this.CreateLoader(path);
#endif
        loader.prority = prority;
        loader.onComplete += handler;

        _isCurrentLoading = true;
        _nonCompleteLoaderSet.Add(loader);
        _thisTimeLoaderSet.Add(loader);

        return loader;
    }
    /// <summary>
    /// 通过Resources加载资源
    /// </summary>
    /// <param name="strAssetFile"></param>
    /// <returns></returns>
    public UnityEngine.Object Load(string strAssetFile)
    {
        UnityEngine.Object @object = null;
        if (!string.IsNullOrEmpty(strAssetFile))
        {
            @object = Resources.Load(strAssetFile);
        }
        if (null == @object)
        {
            Debug.LogError("null == obj: " + strAssetFile);
        }
        return @object;
    }
    internal AssetBundleLoader CreateLoader(string abFileName, string oriName = null)
    {
        AssetBundleLoader loader = null;

        if (_loaderCache.ContainsKey(abFileName))
        {
            loader = _loaderCache[abFileName];
        }
        else
        {
#if _AB_MODE_
            AssetBundleData data = _depInfoReader.GetAssetBundleInfo(abFileName);
                if (data == null && oriName != null)
                {               
                    data = _depInfoReader.GetAssetBundleInfoByShortName(oriName.ToLower());
                }
                if (data == null)
                {
                Debug.Log("MIssLoad:" + oriName);
                    MissAssetBundleLoader missLoader = new MissAssetBundleLoader();
                    missLoader.bundleManager = this;
                    return missLoader;
                }

                loader = this.CreateLoader();
                loader.bundleManager = this;
                loader.bundleData = data;
                loader.bundleName = data.fullName;
#else
            loader = this.CreateLoader();
            loader.bundleManager = this;
            loader.bundleName = abFileName;
#endif
            _loaderCache[abFileName] = loader;
        }

        return loader;
    }

    protected virtual AssetBundleLoader CreateLoader()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return new EditorModeAssetBundleLoader();
#elif UNITY_IOS
            return new IOSAssetBundleLoader();
#elif UNITY_ANDROID
            return new AndroidAssetBundleLoader();
#else
            return new MobileAssetBundleLoader();
#endif
    }

    void CheckNewLoaders()
    {
        if (_nonCompleteLoaderSet.Count > 0)
        {
            List<AssetBundleLoader> loaders = ListPool<AssetBundleLoader>.Get();
            loaders.AddRange(_nonCompleteLoaderSet);
            _nonCompleteLoaderSet.Clear();

            var e = loaders.GetEnumerator();
            while (e.MoveNext())
            {
                _currentLoadQueue.Add(e.Current);
            }

            _progress = new AssetBundleLoadProgress();
            _progress.total = _currentLoadQueue.Count;

            e = loaders.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Start();
            }
            ListPool<AssetBundleLoader>.Release(loaders);
        }
    }

    public void RemoveAll()
    {
        this.StopAllCoroutines();

        _currentLoadQueue.Clear();
        _requestQueue.Clear();
        foreach (AssetBundleInfo abi in _loadedAssetBundle.Values)
        {
            abi.Dispose();
        }
        _loadedAssetBundle.Clear();
        _loaderCache.Clear();
    }

    public AssetBundleInfo GetBundleInfo(string key)
    {
        key = key.ToLower();
#if _AB_MODE_
            key = HashUtil.Get(key) + ".ab";
#endif
        var e = _loadedAssetBundle.GetEnumerator();
        while (e.MoveNext())
        {
            AssetBundleInfo abi = e.Current.Value;
            if (abi.bundleName == key)
                return abi;
        }
        return null;
    }

    /// <summary>
    /// 请求加载Bundle，这里统一分配加载时机，防止加载太卡
    /// </summary>
    /// <param name="loader"></param>
    internal void Enqueue(AssetBundleLoader loader)
    {
        if (_requestRemain < 0)
            _requestRemain = 0;
        _requestQueue.Add(loader);
    }

    void CheckQueue()
    {
        if (_requestRemain > 0 && _requestQueue.Count > 0)
            _requestQueue.Sort();

        while (_requestRemain > 0 && _requestQueue.Count > 0)
        {
            AssetBundleLoader loader = _requestQueue[0];
            _requestQueue.RemoveAt(0);
            LoadBundle(loader);
        }
    }

    void LoadBundle(AssetBundleLoader loader)
    {
        if (!loader.isComplete)
        {
            loader.LoadBundle();
            _requestRemain--;
        }
    }

    internal void LoadError(AssetBundleLoader loader)
    {
        Debug.LogError("Cant load AB : " + loader.bundleName, this);
        this.m_bLoadError = true;
        LoadComplete(loader);
    }

    internal void LoadComplete(AssetBundleLoader loader)
    {
        _requestRemain++;
        _currentLoadQueue.Remove(loader);
        _progress.loader = loader;
        _progress.complete = _progress.total - _currentLoadQueue.Count;
        if (onProgress != null)
        {
            onProgress(_progress);
        }

        //all complete
        if (_currentLoadQueue.Count == 0 && _nonCompleteLoaderSet.Count == 0)
        {
            //Action<bool> allLoadFinishedEventHandler = this.m_allLoadFinishedEventHandler;
            //this.m_allLoadFinishedEventHandler = null;
            //try
            //{
            //    allLoadFinishedEventHandler(!this.m_bLoadError);
            //}
            //catch (Exception exception)
            //{
            //    Debug.LogException(exception);
            //}
            //Debug.Log("AllFinish");
            _isCurrentLoading = false;

            var e = _thisTimeLoaderSet.GetEnumerator();
            while (e.MoveNext())
            {
                AssetBundleLoader cur = e.Current;
                if (cur.bundleInfo != null)
                    cur.bundleInfo.ResetLifeTime();
            }
            _thisTimeLoaderSet.Clear();
        }
    }

    internal AssetBundleInfo CreateBundleInfo(AssetBundleLoader loader, AssetBundleInfo abi = null, AssetBundle assetBundle = null)
    {
        if (abi == null)
            abi = new AssetBundleInfo();
        abi.bundleName = loader.bundleName.ToLower();
        abi.bundle = assetBundle;
        abi.data = loader.bundleData;

        _loadedAssetBundle[abi.bundleName] = abi;
        return abi;
    }

    internal void RemoveBundleInfo(AssetBundleInfo abi)
    {
        abi.Dispose();
        _loadedAssetBundle.Remove(abi.bundleName);
    }

    /// <summary>
    /// 当前是否在加载状态
    /// </summary>
    public bool isCurrentLoading { get { return _isCurrentLoading; } }

    void CheckUnusedBundle()
    {
        this.UnloadUnusedBundle();
    }

    /// <summary>
    /// 卸载不用的
    /// </summary>
    public void UnloadUnusedBundle(bool force = false)
    {
        if (_isCurrentLoading == false || force)
        {
            List<string> keys = ListPool<string>.Get();
            keys.AddRange(_loadedAssetBundle.Keys);

            bool hasUnusedBundle = false;
            //一次最多卸载的个数，防止卸载过多太卡
            int unloadLimit = 20;
            int unloadCount = 0;

            do
            {
                hasUnusedBundle = false;
                for (int i = 0; i < keys.Count && !_isCurrentLoading && unloadCount < unloadLimit; i++)
                {
                    if (_isCurrentLoading && !force)
                        break;

                    string key = keys[i];
                    AssetBundleInfo abi = _loadedAssetBundle[key];
                    if (abi.isUnused)
                    {
                        hasUnusedBundle = true;
                        unloadCount++;

                        this.RemoveBundleInfo(abi);

                        keys.RemoveAt(i);
                        i--;
                    }
                }
            } while (hasUnusedBundle && !_isCurrentLoading && unloadCount < unloadLimit);

            ListPool<string>.Release(keys);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (unloadCount > 0 && enableLog)
            {
                Debug.Log("===>> Unload Count: " + unloadCount);
            }
#endif
        }
    }

    public void RemoveBundle(string key)
    {
        AssetBundleInfo abi = this.GetBundleInfo(key);
        if (abi != null)
        {
            this.RemoveBundleInfo(abi);
        }
    }

    public void Init(Action action)
    {
        _initCallback = action;
#if _AB_MODE_
            this.StartCoroutine(LoadDepInfo());
#else
        this.InitComplete();
#endif
    }
    public void Update()
    {
        if (this.m_allUnLoadFinishedEventHandler != null)
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
            Action<bool> temp = this.m_allUnLoadFinishedEventHandler;
            this.m_allUnLoadFinishedEventHandler = null;
            temp(true);
        }
        if (_isCurrentLoading)
        {
            CheckNewLoaders();
            CheckQueue();
        }
        if (_currentLoadQueue.Count == 0 && _nonCompleteLoaderSet.Count == 0)
        {
            if (this.m_allLoadFinishedEventHandler != null)
            {
                Action<bool> allLoadFinishedEventHandler = this.m_allLoadFinishedEventHandler;
                this.m_allLoadFinishedEventHandler = null;
                try
                {
                    allLoadFinishedEventHandler(!this.m_bLoadError);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
                Debug.Log("AllFinish");
                //this._progress.complete = 0;
                //this._progress.total = 0;
            }
        }
    }
    public void SetAllLoadFinishedEventHandler(Action<bool> eventHandler)
    {
        this.m_allLoadFinishedEventHandler += eventHandler;
    }
    public void SetAllUnLoadFinishedEventHandler(Action<bool> eventHandler)
    {
        this.m_allUnLoadFinishedEventHandler += eventHandler;
    }
}
