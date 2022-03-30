using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreProject.DataLayer.Helpers
{
    public class OrmHelper
    {
        Type T;
        string tablename;

        public OrmHelper(Type T)
        {
            this.T =T;
            this.tablename = T.Name;
        }

        public IEnumerable<PropertyInfo> GetProperties => T.GetProperties();

        public static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        }
       
        public string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {tablename} ");

            insertQuery.Append("(");

            var properties = GenerateListOfProperties(GetProperties);
            properties.ForEach(prop =>
            {
                if (!prop.Equals("Id"))
                    insertQuery.Append($"[{prop}],");
            });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            properties.ForEach(prop => { if (!prop.Equals("Id")) insertQuery.Append($"@{prop},"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                  .Append(");")            
                  .Append("SELECT CAST(SCOPE_IDENTITY() as int)");
            // .Append(" SELECT last_insert_rowid()") //For SQLite

            return insertQuery.ToString();
        }

        public string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {tablename} SET ");
            var properties = GenerateListOfProperties(GetProperties);

            properties.ForEach(property =>
            {
                if (!property.Equals("Id"))
                {
                    updateQuery.Append($"{property}=@{property},");
                }
            });

            updateQuery.Remove(updateQuery.Length - 1, 1);
            updateQuery.Append(" WHERE Id=@Id");

            return updateQuery.ToString();
        }
    }
}
