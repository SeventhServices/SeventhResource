using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Seventh.Resource.Database.Serializer
{
    public class SqlRegexParser
    {
        //[Obsolete]
        //public IEnumerable<T> Parse(string sqlString)
        //{
        //    var pType = typeof(Card);
        //    var pPropertyInfoArr = pType.GetProperties();//
        //    var SQLReadReg = new Regex(@"('(?<card_id>.+?)','(?<character_id>.+?)','(?<potential_group>.+?)','(?<rarity_id>.+?)','(?<card_type_id>.+?)','(?<facial_image_id>.+?)','(?<card_message_id>.+?)','(?<cost>.+?)','(?<card_name>.+?)','(?<description>.+?)','(?<max_level>.+?)','(?<break_max_level>.+?)','(?<hp_grow_type>.+?)','(?<attack_grow_type>.+?)','(?<default_hp>.+?)','(?<default_attack>.+?)','(?<max_hp>.+?)','(?<max_attack>.+?)','(?<break_max_hp>.+?)','(?<break_max_attack>.+?)','(?<member_skill_id>.+?)','(?<leader_skill_id>.+?)','(?<basic_exp>.+?)','(?<payback_7thpt>.+?)','(?<type_potential>.+?)','(?<skill_potential>.+?)','(?<combo_ids>.+?)','(?<live_leader_skill_id>.+?)','(?<live_member_skill_id>.+?)','(?<max_intimate>.+?)','(?<stock_flg>.+?)','(?<sign_flg>.+?)','(?<role>.+?)','(?<limited_flg>.+?)','(?<start_time>.+?)','(?<delete_flg>.+?)')");//
        //    var pMatch = SQLReadReg.Match(sqlString);
        //    if (!SQLReadReg.IsMatch(sqlString)) yield break;

        //    while (pMatch.Success)
        //    {
        //        var t = Activator.CreateInstance<T>();
        //        foreach (var pPropertyInfo in pPropertyInfoArr)
        //        {
        //            if (SQLReadReg.GroupNumberFromName(pPropertyInfo.Name) >= 0)
        //            {
        //                if (pPropertyInfo.PropertyType == typeof(bool))
        //                {
        //                    bool trgValue = pMatch.Groups[pPropertyInfo.Name].Value.ToLower() == "1"
        //                                    || pMatch.Groups[pPropertyInfo.Name].Value.ToLower() == "true";
        //                    pPropertyInfo.SetValue(t, trgValue);
        //                }
        //                else
        //                {
        //                    object pObjVal = Convert.ChangeType(pMatch.Groups[pPropertyInfo.Name].Value, pPropertyInfo.PropertyType);//
        //                    pPropertyInfo.SetValue(t, pObjVal);
        //                }
        //            }
        //        }

        //        yield return t;
        //        pMatch = pMatch.NextMatch();
        //    }
        //}


        //[Obsolete]

        public SqlRegexParser()
        {

        }

        private readonly Regex _sqlDataRegex = new Regex("\\((?<sqlData>'.+?')\\)");
        private readonly Regex _sqlFieldRegex = new Regex("(^|,)'(?<field>.+?)'");

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
                            if (propertyInfos[fieldId].PropertyType == typeof(bool))
                            {
                                bool trgValue = sqlFieldMatch.Groups["field"].Value.ToLower() == "1"
                                    || sqlFieldMatch.Groups["field"].Value.ToLower() == "true";
                                propertyInfos[fieldId].SetValue(instance, trgValue);
                            }
                            else
                            {
                                object fieldObject = Convert.ChangeType(sqlFieldMatch.Groups["field"].Value, propertyInfos[fieldId].PropertyType);
                                propertyInfos[fieldId].SetValue(instance, fieldObject);
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