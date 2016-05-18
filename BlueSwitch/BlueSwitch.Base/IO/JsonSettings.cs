﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BlueSwitch.Base.Components.Switches.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace BlueSwitch.Base.IO
{
    //public class NamespaceMigration
    //{
    //    public string FromAssembly { get; set; }

    //    public string FromType { get; set; }

    //    public Type ToType { get; set; }
    //}

    //public class NamespaceMigrationSerializationBinder : DefaultSerializationBinder
    //{
    //    private readonly NamespaceMigration[] _migrations;

    //    public NamespaceMigrationSerializationBinder(params NamespaceMigration[] migrations)
    //    {
    //        _migrations = migrations;
    //    }

    //    public override Type BindToType(string assemblyName, string typeName)
    //    {
    //        var migration = _migrations.SingleOrDefault(p => p.FromAssembly == assemblyName && p.FromType == typeName);
    //        if (migration != null)
    //        {
    //            return migration.ToType;
    //        }
    //        return base.BindToType(assemblyName, typeName);
    //    }
    //}

    public class JsonSerializable
    {
        public JsonSerializable()
        {

        }

        private bool _isLoaded;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            protected set { _isLoaded = value; }
        }

        private String _filePath;

        [JsonIgnore]
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public void Load(String applicationName, String fileName)
        {
            FilePath = GetFilePath(applicationName, fileName);
            LoadFrom(FilePath);
        }

        public static T Load<T>(String path) where T : JsonSerializable
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            byte[] data = File.ReadAllBytes(path);

            using (var sr = new StreamReader(new MemoryStream(data)))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    serializer.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    serializer.TypeNameHandling = TypeNameHandling.Auto;
                    var jObject = serializer.Deserialize(jsonTextReader, typeof(T));
                    ((T) jObject).FilePath = path;
                    return (T)jObject;
                }
            }
        }

        public virtual void LoadFrom(string path)
        {
            IsLoaded = false;
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            byte[] data = File.ReadAllBytes(path);
            data = LoadFilter(data);

            using (var sr = new StreamReader(new MemoryStream(data)))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    serializer.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    serializer.TypeNameHandling = TypeNameHandling.Auto;
                    var jObject = (JObject)serializer.Deserialize(jsonTextReader);
                    Type type = this.GetType();
                    PropertyInfo[] properties = type.GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            var value = jObject.GetValue(property.Name);
                            if (value != null)
                            {
                                var result = value.ToObject(property.PropertyType);
                                property.SetValue(this, result, null);
                            }
                        }
                        catch
                        {
                            Debug.Write("Deserialization failed");
                        }
                    }
                }
            }

            IsLoaded = true;
        }

        public static String GetFilePath(String applicationName, String fileName)
        {
            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String folderPath = Path.Combine(appDataPath, applicationName);
            String filePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return filePath;
        }

        public static bool Exists(String filePath)
        {
            return File.Exists(filePath);
        }

        public virtual void Save(String path)
        {
            String result = JsonConvert.SerializeObject(this,new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Auto});
            var data = Encoding.UTF8.GetBytes(result);
            data = SaveFilter(data);
            File.WriteAllBytes(path, data);
        }

        protected virtual byte[] LoadFilter(byte[] data)
        {
            return data; // No filter in base class
        }

        protected virtual byte[] SaveFilter(byte[] data)
        {
            return data; // No filter in base class
        }
    }
}
