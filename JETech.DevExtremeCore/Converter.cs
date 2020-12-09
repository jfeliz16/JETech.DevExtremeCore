using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace JETech.DevExtremeCore
{
    public class Converter
    {
        public static string FilterToSql(string filter)
        {
            string filterSql = string.Empty;

            if (!string.IsNullOrEmpty(filter))
            {
                string[] filterJson =(string[]) Newtonsoft.Json.JsonConvert.DeserializeObject(filter);

                string col,exp,value;
                string condiction = string.Empty;

                for (int i = 0; i < filterJson.Length; i=+3)
                {
                    col = filterJson[i];
                    value = filterJson[i + 2];
                    exp = filterJson[i + 1];

                    switch (exp)
                    {   
                        case "constains":
                            condiction = $"{col} like '%{value}%'";
                            break;
                        default:
                            break;
                    }
                    if (string.IsNullOrEmpty(filterSql))
                    {
                        filterSql = condiction;
                    }
                    else
                    {
                        filterSql = String.Concat(filterSql, " and ", condiction);
                    }
                }
            }            

            return filterSql;
        }

        public static Expression<Func<t, bool>> FilterToExpresion<t>(string filter)
        {
            string filterSql = string.Empty;

            var expre = JETech.NetCoreWeb.Helper.ConverterExpression.GetContainsExpression<t>("Name", "jose");

            Expression.Add(expre, expre);

            if (!string.IsNullOrEmpty(filter))
            {
                string[] filterJson = (string[])Newtonsoft.Json.JsonConvert.DeserializeObject(filter);

                string col, exp, value;
                string condiction = string.Empty;

                for (int i = 0; i < filterJson.Length; i = +3)
                {
                    col = filterJson[i];
                    value = filterJson[i + 2];
                    exp = filterJson[i + 1];

                    switch (exp)
                    {
                        case "constains":
                            condiction = $"{col} like '%{value}%'";
                            break;
                        default:
                            break;
                    }
                    if (string.IsNullOrEmpty(filterSql))
                    {
                        filterSql = condiction;
                    }
                    else
                    {
                        filterSql = String.Concat(filterSql, " and ", condiction);
                    }
                }
            }

            return expre;
        }
    }
}
