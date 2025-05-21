using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SquirrelBombMod
{
    public class AssetCollection
    {
        public AssetCollection(string path)
        {
            foreach(string str in CurrentAssembly.GetManifestResourceNames())
            {
                if (!str.StartsWith(path))
                    continue;

                using var stream = CurrentAssembly.GetManifestResourceStream(str);
                try
                {
                    var b = AssetBundle.LoadFromStream(stream);
                    if (b)
                        bundles.Add(b);
                }
                catch { }
            }

            audio.AddRange(bundles.ConvertAll(x => x.LoadAllAssets<AudioClip>()).SelectMany(x => x));
            Audio = new(audio);
        }

        public Object LoadAsset(string name)
        {
            var key = $"Object_{name}";
            if(loadedObjects.TryGetValue(key, out var obj))
                return obj;

            foreach(var b in bundles)
            {
                try
                {
                    var a = b.LoadAsset(name);
                    if (a == null)
                        continue;

                    loadedObjects[key] = a;
                    return a;
                }
                catch { }
            }
            return null;
        }

        public T LoadAsset<T>(string name) where T : Object
        {
            var key = $"{typeof(T).Name}_{name}";
            if(loadedObjects.TryGetValue(key, out var obj) && obj is T t)
                return t;

            foreach (var b in bundles)
            {
                try
                {
                    var a = b.LoadAsset<T>(name);
                    if (a == null)
                        continue;

                    loadedObjects[key] = a;
                    return a;
                }
                catch { }
            }
            return default;
        }

        public void UnloadAll(bool unloadLoadedObjects)
        {
            bundles.ForEach(x => x.Unload(unloadLoadedObjects));
            bundles.Clear();
        }

        private readonly Dictionary<string, Object> loadedObjects = [];
        private readonly List<AssetBundle> bundles = [];
        private readonly List<AudioClip> audio = [];
        public readonly ReadOnlyCollection<AudioClip> Audio;
    }
}
