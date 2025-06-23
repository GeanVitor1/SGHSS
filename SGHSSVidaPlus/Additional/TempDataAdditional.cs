using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace SGHSSVidaPlus.MVC.Additional // Namespace atualizado (e padronizando casing para "Additional")
{
    public static class TempDataAdditional
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            if (!tempData.ContainsKey(key))
                return null;

            var retorno = tempData[key].ToString();
            return JsonConvert.DeserializeObject<T>(retorno);
        }
    }
}