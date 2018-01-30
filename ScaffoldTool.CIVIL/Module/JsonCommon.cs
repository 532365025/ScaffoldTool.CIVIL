using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldTool.Module
{
    public class JsonCommon
    {
        /// <summary>
        /// JSON序列化
        /// 2017-8-16
        /// Don
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// JSON反序列化
        /// 2017-8-16
        /// Don
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
    }
}
