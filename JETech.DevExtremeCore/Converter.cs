using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using JETech.NetCoreWeb.Helper;

namespace JETech.DevExtremeCore
{
    internal class FilterDevExtremeObjet 
    {
        public string Column { get; set; }
        public string Condiction { get; set; }
        public string Value { get; set; }
    }

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
            Expression<Func<t, bool>> expResult = null;

            if (!string.IsNullOrEmpty(filter))
            {
                string col, exp, value;

                filter = filter
                    .Replace(",\"and\",", ",")
                    .Replace("[","")
                    .Replace("]","");

                filter = string.Concat("[", filter, "]");

                var filterJson = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(filter);
                Expression<Func<t, bool>> expItem = null;                

                for (int i = 0; i < filterJson.Length; i +=3)
                {
                    col = filterJson[i];
                    value = filterJson[i + 2];
                    exp = filterJson[i + 1];
                    expItem = null;

                    switch (exp)
                    {
                        case "contains":
                            expItem = ConverterExpression.GetContainsExpression<t>(col, value);                           
                            break;
                        default:
                            break;
                    }
                    if (expResult != null && expItem != null)
                    {
                        ParameterExpression param = expResult.Parameters[0];

                        if (ReferenceEquals(param, expItem.Parameters[0]))
                        {
                            // simple version
                            expResult = Expression.Lambda<Func<t, bool>>(
                                Expression.AndAlso(expItem.Body, expResult.Body), param);
                        }
                        else 
                        {
                            // otherwise, keep expr1 "as is" and invoke expr2
                            expResult = Expression.Lambda<Func<t, bool>>(
                                Expression.AndAlso(
                                    expResult.Body,
                                    Expression.Invoke(expItem, param)), param);
                        }          
                    }
                    else if (expItem != null) 
                    {
                        expResult = expItem;
                    }
                }
            }

            return expResult;
        }
    }
}
