using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MoonSharp.Interpreter;
using System.IO;

namespace LUA
{
    public class ScriptLoader
    {
        protected static Dictionary<string, Script> script = new Dictionary<string, Script>();

        static private string directory = "Scripts";

        public ScriptLoader()
        {
            UserData.RegisterAssembly();

            LoadScript("test", "test.lua");

            Debug.Log("Test call to LUA.");
            DynValue ret = Call("test", "test");
            Debug.Assert(ret.CastToBool() == true);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public static void LoadScript(string category, string filename)
        {
            script[category] = new Script();

            script[category].Options.DebugPrint += (string str) =>
            {
                Debug.Log("LUA: " + str);
            };

            FileInfo info = new FileInfo(
                Path.Combine(Application.streamingAssetsPath,
                    Path.Combine(directory, filename)
                )
            );
            Load(category, info);
        }

        private static void Load(string category, FileInfo file)
        {
            StreamReader reader = new StreamReader(file.OpenRead());
            string textScript = reader.ReadToEnd();

            try
            {
                script[category].DoString(textScript);
            }
            catch (SyntaxErrorException e)
            {
                Debug.LogError("LUA error: " + e.DecoratedMessage);
            }
        }

        public static DynValue Call(string category, string function, params object[] args)
        {
            object func = script[category].Globals[function];
            try
            {
                return script[category].Call(func, args);
            }
            catch (ScriptRuntimeException e)
            {
                Debug.LogError("Script exception: " + e.DecoratedMessage);
                return null;
            }
            catch (ArgumentException e)
            {
                Debug.LogError("Script exception while running '" + function + "': " + e.Message);
                return null;
            }
        }

        public static void RegisterPlaceolder(string category, Type type) {
            string typeName = type.ToString().Split('.').Last().Split('+').Last();
            Debug.Log(String.Format("Registering placeholder for type {0} in category {1} as {2}", 
                type, category, typeName));
            script[category].Globals[typeName] = type;
        }
    }
}
