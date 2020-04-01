using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Seventh.Resource.Database.Serializer
{
    public class SqlRegexParser
    {
        private readonly Regex _sqlDataRegex = new Regex("\\((?<sqlData>'.+?')\\)");
        private readonly Regex _sqlFieldRegex = new Regex("(^|,)'(?<field>.+?)'");
        private readonly Regex _numRegex = new Regex(@"^\d+$");

        public IEnumerable<T> Parse<T>(string sqlString) where T : class
        {
            var type = typeof(T);
            var propertyInfos = type.GetProperties();

            var sqlDataMatch = _sqlDataRegex.Match(sqlString);
            if (_sqlDataRegex.IsMatch(sqlString))
            {
                while (sqlDataMatch.Success)
                {
                    var instance = Activator.CreateInstance<T>();
                    var sqlData = sqlDataMatch.Groups["sqlData"].Value;
                    var sqlFieldMatch = _sqlFieldRegex.Match(sqlData);
                    var fieldId = 0;
                    while (sqlFieldMatch != null && sqlFieldMatch.Success)
                    {
                        if (fieldId < propertyInfos.Length)
                        {
                            var propertyType = propertyInfos[fieldId].PropertyType;
                            switch (propertyType)
                            {
                                case Type t when t == typeof(bool):
                                    {
                                        bool value = sqlFieldMatch.Groups["field"].Value.ToLower() == "1"
                                            || sqlFieldMatch.Groups["field"].Value.ToLower() == "true";
                                        propertyInfos[fieldId].SetValue(instance, value);
                                    }
                                    break;
                                case Type t when t == typeof(string):
                                    {
                                        object fieldObject = Convert.ChangeType(sqlFieldMatch.Groups["field"].Value, t);
                                        propertyInfos[fieldId].SetValue(instance, fieldObject);
                                    }
                                    break;
                                case Type t when t.BaseType == typeof(Enum):
                                    {
                                        object fieldObject = Convert.ChangeType(Enum.ToObject(t, int.Parse(sqlFieldMatch.Groups["field"].Value)), t);
                                        propertyInfos[fieldId].SetValue(instance, fieldObject);
                                    }
                                    break;
                                case Type t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>):
                                    {
                                        var basicType = Nullable.GetUnderlyingType(t);
                                        var fieldString = sqlFieldMatch.Groups["field"].Value;
                                        if (_numRegex.IsMatch(fieldString))
                                        {
                                            object fieldObject = Convert.ChangeType(sqlFieldMatch.Groups["field"].Value, basicType);
                                            propertyInfos[fieldId].SetValue(instance, fieldObject);
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        object fieldObject = Convert.ChangeType(sqlFieldMatch.Groups["field"].Value, propertyInfos[fieldId].PropertyType);
                                        propertyInfos[fieldId].SetValue(instance, fieldObject);
                                    }
                                    break;
                            }
                        }

                        fieldId++;
                        sqlFieldMatch = sqlFieldMatch.NextMatch();
                    }
                    GC.ReRegisterForFinalize(sqlData);
                    yield return instance;
                    sqlDataMatch = sqlDataMatch.NextMatch();
                }
            }
        }

    }
}