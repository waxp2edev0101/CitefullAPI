using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CitefullAPI.Classes
{
    [BsonIgnoreExtraElements]
    public class QueryParams
    {
        [BsonElement("key")]
        public string key { get; set; }
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("description")]
        public string description;
        [BsonElement("vartype")]
        public string vartype { get; set; }
        [BsonElement("vardefault")]
        public string vardefault { get; set; }
        [BsonElement("required")]
        public bool required { get; set; }
        public string currentvalue { get; set; }

        public string TranslateDefault()
        {
            if (currentvalue == null)
            {
                currentvalue = vardefault;
                if (vartype == "DateTime") // Translate DateTime defaults into a date offset
                {
                    int offsetValue;
                    Char varConversionType = Convert.ToChar(currentvalue[^1..].ToUpper());
                    if (Char.IsDigit(varConversionType))
                    {
                        varConversionType = 'D';
                        offsetValue = Convert.ToInt16(currentvalue);
                    }
                    else
                        offsetValue = Convert.ToInt16(currentvalue[0..^1]);
                    TimeSpan t = TimeSpan.Zero;
                    if (offsetValue != 0)
                    {
                        switch (varConversionType)
                        {
                            case 'D':
                                t = TimeSpan.FromDays(offsetValue);
                                break;
                            case 'H':
                                t = TimeSpan.FromHours(offsetValue);
                                break;
                            case 'S':
                                t = TimeSpan.FromSeconds(offsetValue);
                                break;
                            case 'M':
                                t = TimeSpan.FromMilliseconds(offsetValue);
                                break;
                        }
                    }
                    currentvalue = $"{DateTime.UtcNow - t:O}";
                }
            }

            return currentvalue;
        }
    }
}
