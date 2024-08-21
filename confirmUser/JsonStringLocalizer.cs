using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace confirmUser
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly JsonSerializer _serializer=new();
        public LocalizedString this[string name] 
        {
            get
            {
                //var value=GetString(name);
                //return new LocalizedString(name, value);
                var value = GetString(name);
                if (string.IsNullOrEmpty(value))
                {
                    // Handle the empty string case by providing a meaningful fallback
                    value = $"[{name}]"; // Optionally, use the key itself or another fallback message
                }
                return new LocalizedString(name, value);
            }
        }

        public LocalizedString this[string name, params object[] arguments] 
        {
            get
            { 
                var actualValue = this[name];
                return !actualValue.ResourceNotFound
                    ?new LocalizedString(name, string.Format(actualValue.Value,arguments)) 
                    :actualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";

            using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamReader = new(stream);
            using JsonTextReader reader = new(streamReader);
            while(reader.Read())
            {
                if(reader.TokenType != JsonToken.PropertyName)
                    continue;
                var key=reader.Value as string;
                reader.Read();
                var value = _serializer.Deserialize<string>(reader);
                yield return new LocalizedString(key, value);
            }
        }

        private string GetString(string key)
        {
            var filePath=$"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            var fullFilePath=Path.GetFullPath(filePath);
            if(File.Exists(fullFilePath) )
            {
                var result=GetValueFromJson(key, fullFilePath);
                return result;

            }
            return string.Empty;
        }
        private string GetValueFromJson(string proertyName,string filePath)
        {
            if (string.IsNullOrEmpty(proertyName)|| string.IsNullOrEmpty(filePath)) 
           return string.Empty;

            using FileStream stream= new (filePath,FileMode.Open, FileAccess.Read,FileShare.Read);
            using StreamReader streamReader= new (stream);
            using JsonTextReader reader = new (streamReader);
            while(reader.Read())
            {
                if(reader.TokenType == JsonToken.PropertyName&& reader.Value as string ==proertyName)
                {
                    reader.Read();
                    return _serializer.Deserialize<string>(reader);
                }
            }
            return string.Empty;
        }
    }
}
