using System.Collections;
using System.Collections.Generic;

namespace WinMD52
{
    public class KnowledgeBase
    {
        private readonly Hashtable _hashtable = Hashtable.Synchronized(new Hashtable());
        private readonly List<string> _found = new List<string>();
        private readonly object _myLock = new object();

        /** Return just the file name, removing any directory information. **/
        public static string JustTheFileName(string path)
        {
            int lastdelim = path.LastIndexOf("\\");
            string filename;

            if (lastdelim >= 0)
                filename = path.Substring(lastdelim + 1);
            else
                filename = path;

            return filename;
        }

        public int Count { get { return _hashtable.Count; } }

        public int UnfoundCount { get
        {
            var keys = new ArrayList(_hashtable.Keys);
            foreach (var key in _found)
            {
                keys.Remove(key);
            }
            return keys.Count;
        }}

        public void AddRecord(string path, string hash)
        {
            lock (_myLock)
            {
                if (!_hashtable.ContainsKey(hash))
                {
                    _hashtable[hash] = new List<string> {path};
                }
                else
                {
                    // if this were a threadsafe list you could move this call to after the lock is released.
                    ((List<string>)_hashtable[hash]).Add(path);
                }
            }
        }

        public void Clear()
        {
            _hashtable.Clear();
            _found.Clear();
        }

        public bool? CheckFile(string partialpath, string hash)
        {
            _found.Add(hash);
            if (!_hashtable.ContainsKey(hash)) return null;
            var paths = (List<string>)_hashtable[hash];
            if (paths.Contains(partialpath)) return true;
            if (paths.Contains(JustTheFileName(partialpath))) return true;
            if (paths.Find(partialpath.EndsWith) != null) return true;
            return false;
        }
    }
}
