using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace Learn.Core.Util
{
    /// <summary>
    /// Json操作
    /// </summary>
    public static class Json
    {
        public static object ToJson(this string Json)
        {
            return Json == null ? null : JsonConvert.DeserializeObject(Json);
        }
        public static string ToJson(this object obj)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            var longConverter = new LongJsonConvert();
            return JsonConvert.SerializeObject(obj, timeConverter, longConverter);
        }
        public static string ToJson(this object obj, string datetimeformats)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeformats };
            var longConverter = new LongJsonConvert();
            return JsonConvert.SerializeObject(obj, timeConverter, longConverter);
        }
        public static T ToObject<T>(this string Json)
        {
            return Json == null ? default(T) : JsonConvert.DeserializeObject<T>(Json);
        }
        public static List<T> ToList<T>(this string Json)
        {
            return Json == null ? null : JsonConvert.DeserializeObject<List<T>>(Json);
        }
        public static DataTable ToTable(this string Json)
        {
            return Json == null ? null : JsonConvert.DeserializeObject<DataTable>(Json);
        }
        public static JObject ToJObject(this string Json)
        {
            return Json == null ? JObject.Parse("{}") : JObject.Parse(Json.Replace("&nbsp;", ""));
        }
    }

    /// <summary>
    /// 解决js不能解析Int64类型的问题  序列化时 转化为String类型
    /// </summary>
    public class LongJsonConvert : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //  Convert.ToInt64(reader.Value);
            return reader.Value;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());


        }
        public override bool CanConvert(Type objectType)
        {
            if ((objectType == typeof(Int64)) || (objectType == typeof(Int64?)))
            {
                return true;
            }
            return false;
        }
    }
}
