using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using kooltool.Data;

public interface IResource : IDisposable
{
    byte[] Data { get; set; }

    IResource Clone();
}

public class Texture2DResource : IResource
{
    public Texture2D texture;

    byte[] IResource.Data
    {
        get
        {
            return texture.EncodeToPNG();
        }

        set
        {
            texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;
            texture.LoadImage(value);
            texture.Apply();
        }
    }

    IResource IResource.Clone()
    {
        var texture = new Texture2D(this.texture.width,
                                    this.texture.height, 
                                    TextureFormat.ARGB32, 
                                    false);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels32(this.texture.GetPixels32());
        texture.Apply();

        return new Texture2DResource
        {
            texture = texture,
        };
    }

    void IDisposable.Dispose()
    {
        UnityEngine.Object.Destroy(texture);
    }
}

public class Resource<T>
{
    public T instance;
    public string id;

    public static implicit operator T(Resource<T> resource)
    {
        return resource.instance;
    }
}

public class Context : IDisposable
{
    public Project project;
    public string source;

    private Dictionary<string, IResource> resources
        = new Dictionary<string, IResource>();

    public interface IResourceIndex
    {
        void SetResource(string path, IResource resource);

        TResource GetResource<TResource>(string path)
            where TResource : IResource, new();
    }

    private class Index : IResourceIndex
    {
        public readonly string folder;
        public readonly Dictionary<string, IResource> resources;

        public Index(string folder, Dictionary<string, IResource> resources)
        {
            this.folder = folder;
            this.resources = resources;
        }

        void IResourceIndex.SetResource(string path, 
                                        IResource resource)
        {
            resources[path] = resource;
        }

        TResource IResourceIndex.GetResource<TResource>(string path)
        {
            IResource resource;

            if (!resources.TryGetValue(path, out resource))
            {
                resource = new TResource();
                resource.Data = File.ReadAllBytes(folder + "/" + path);

                resources[path] = resource;
            }

            return (TResource) resource;
        }
    }

    public void SetResource(string path,
                            IResource resource)
    {
        resources[path] = resource;
    }

    public void Initialise(string source)
    {
        var index = new Index(source, resources);

        foreach (var real in project.index.resources)
        {
            real.Initialise(index);
        }

        project.index.context = this;
    }

    public void Read(string source)
    {
        project = JsonWrapper.Deserialise<Project>(File.ReadAllText(source + "/project.json"));

        Initialise(source);
    }

    // TODO: work out how to remember which things were already saved??
    // ALT: tracker dirty and fully dirty when not writing to the origin
    public void Write(string dest)
    {
        Directory.CreateDirectory(dest);

        foreach (var pair in resources)
        {
            File.WriteAllBytes(dest + "/" + pair.Key, pair.Value.Data);
        }

        File.WriteAllText(dest + "/project.json", JsonWrapper.Serialise(project));
    }

    // TODO: this.MemberwiseClone() is a thing hmm
    public Context Clone()
    {
        var clone = new Context
        {
            project = JsonWrapper.Copy(project),
            resources = resources.ToDictionary(pair => pair.Key,
                                               pair => pair.Value.Clone()),
        };

        clone.Initialise("");

        return clone;
    }

    public void Dispose()
    {
        foreach (var pair in resources)
        {
            pair.Value.Dispose();
        }
    }
}
