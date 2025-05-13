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
                if (str.StartsWith(path))
                {
                    AssetBundle b = null;
                    using (var stream = CurrentAssembly.GetManifestResourceStream(str))
                    {
                        try
                        {
                            b = AssetBundle.LoadFromStream(stream);
                        }
                        catch { }
                    }
                    if(b != null)
                    {
                        bundles.Add(b);
                    }
                }
            }
            audio = bundles.ConvertAll(x => x.LoadAllAssets<AudioClip>()).SelectMany(x => x).ToList();
            Bundles = new(bundles);
            Audio = new(audio);
        }

        public Object LoadAsset(string name)
        {
            foreach(var b in bundles)
            {
                try
                {
                    var a = b.LoadAsset(name);
                    if(a != null) { return a; }
                }
                catch { }
            }
            return null;
        }

        public T LoadAsset<T>(string name) where T : Object
        {
            foreach (var b in bundles)
            {
                try
                {
                    var a = b.LoadAsset<T>(name);
                    if (a != null) { return a; }
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

        private readonly List<AssetBundle> bundles = new();
        private readonly List<AudioClip> audio = new();
        public readonly ReadOnlyCollection<AssetBundle> Bundles;
        public readonly ReadOnlyCollection<AudioClip> Audio;
    }
}
