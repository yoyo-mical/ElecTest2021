using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Testplan {
    [DataContract] internal sealed class WebRequest {
        [DataMember] public string id;
        [DataMember] public string operation;
        [DataMember] public WebArguments args;
    }
    [DataContract] internal sealed class WebArguments {
        [DataMember] public Dictionary<string,string> fields;
        [DataMember] public string path;
        [DataMember] public bool savePath;
        [DataMember] public string pattern;
        [DataMember] public int selectionStart;
        [DataMember] public int selectionLength;
        [DataMember] public List<WebTextRun> recipeRuns;
    }
    [DataContract] internal sealed class WebTextRun {
        [DataMember] public string text;
        [DataMember] public bool bold;
        [DataMember] public bool italic;
        [DataMember] public string color;
    }
    [DataContract] internal sealed class WebPoint {
        [DataMember] public string x;
        [DataMember] public double y;
    }
    [DataContract] internal sealed class WebState {
        [DataMember] public bool authorized;
        [DataMember] public bool busy;
        [DataMember] public string account;
        [DataMember] public string edition;
        [DataMember] public string error;
        [DataMember] public string configuredPath;
        [DataMember] public bool serialConnected;
        [DataMember] public bool meterConnected;
        [DataMember] public string serialState;
        [DataMember] public string meterState;
        [DataMember] public string serialLog;
        [DataMember] public string script;
        [DataMember] public string recipe;
        [DataMember] public int currentLine;
        [DataMember] public Dictionary<string,string> fields=new Dictionary<string,string>();
        [DataMember] public Dictionary<string,bool> enabled=new Dictionary<string,bool>();
        [DataMember] public Dictionary<string,string[]> choices=new Dictionary<string,string[]>();
        [DataMember] public Dictionary<string,string> readings=new Dictionary<string,string>();
        [DataMember] public string test;
        [DataMember] public string metric;
        [DataMember] public string unit;
        [DataMember] public List<WebPoint> points=new List<WebPoint>();
        [DataMember] public string[] sheetColumns;
        [DataMember] public List<string[]> sheet=new List<string[]>();
    }
    [DataContract] internal sealed class WebReply {
        [DataMember] public string type="reply";
        [DataMember] public string id;
        [DataMember] public bool ok;
        [DataMember] public string error;
        [DataMember] public WebState state;
        [DataMember] public List<WebTextRun> recipeRuns;
    }
    internal static class WebJson {
        internal static string Encode<T>(T value) {
            using(var stream=new MemoryStream()){new DataContractJsonSerializer(typeof(T),new DataContractJsonSerializerSettings{UseSimpleDictionaryFormat=true}).WriteObject(stream,value);return Encoding.UTF8.GetString(stream.ToArray());}
        }
        internal static T Decode<T>(string value) {
            using(var stream=new MemoryStream(Encoding.UTF8.GetBytes(value))){return (T)new DataContractJsonSerializer(typeof(T),new DataContractJsonSerializerSettings{UseSimpleDictionaryFormat=true}).ReadObject(stream);}
        }
    }
}
