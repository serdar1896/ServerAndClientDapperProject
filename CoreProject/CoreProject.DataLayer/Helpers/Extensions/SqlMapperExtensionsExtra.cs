using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Contrib.Extensions
{
    // ReSharper disable once PartialTypeWithSinglePart
    public static partial class SqlMapperExtensions
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetQueriesAll =
            new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ConcurrentDictionary<int, SqlWhereOrderCache> GetQueriesWhereOrder =
            new ConcurrentDictionary<int, SqlWhereOrderCache>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName =
            new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties =
           new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        #region Extra Select

        /// <summary>
        /// </summary>
        /// <typeparam name="T">Interface type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <returns>Entity of T</returns>
        public static async Task<IEnumerable<T>> GetByAsync<T>(this IDbConnection connection, object where = null, object order = null,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            Type type = typeof(T);
            bool isUseWhere = where != null;
            bool isUseOrder = order != null;
            if (!isUseWhere && !isUseOrder)
            {
                return await connection.GetAllAsync<T>(transaction, commandTimeout);
            }
            Type whereType = isUseWhere ? where.GetType() : null;
            Type orderType = isUseOrder ? order.GetType() : null;
            SqlWhereOrderCache cache;
            int key = GetKeyTypeWhereOrder(type, whereType, orderType);
            if (!GetQueriesWhereOrder.TryGetValue(key, out cache))
            {
                cache = new SqlWhereOrderCache();
                if (isUseWhere)
                {
                    cache.Where = GetListOfNames(whereType.GetProperties());
                }
                if (isUseOrder)
                {
                    cache.Order = GetListOfNames(orderType.GetProperties());
                }
                string name = GetTableName(type);
                var sb = new StringBuilder();

                sb.AppendFormat("select * from {0} (nolock)", name);
                int cnt, last, i;
                if (isUseWhere)
                {
                    sb.Append(" where ");
                    cnt = cache.Where.Count();
                    last = cnt - 1;
                    for (i = 0; i < cnt; i++)
                    {
                        string prop = cache.Where.ElementAt(i);
                        sb.AppendFormat("[{0}]=@{1}", prop, prop);
                        if (i != last)
                        {
                            sb.Append(" and ");
                        }
                    }
                }
                if (isUseOrder)
                {
                    sb.Append(" order by ");
                    cnt = cache.Order.Count();
                    last = cnt - 1;
                    for (i = 0; i < cnt; i++)
                    {
                        string prop = cache.Order.ElementAt(i);
                        sb.AppendFormat("[{0}] #{1}", prop, prop);
                        if (i != last)
                        {
                            sb.Append(", ");
                        }
                    }
                }

                // TODO: pluralizer 
                // TODO: query information schema and only select fields that are both in information schema and underlying class / interface 
                cache.Sql = sb.ToString();
                GetQueriesWhereOrder[key] = cache;
            }

            IEnumerable<T> obj = null;
            var dynParms = new DynamicParameters();
            if (isUseWhere)
            {
                foreach (string name in cache.Where)
                {
                    dynParms.Add(name, whereType.GetProperty(name).GetValue(where, null));
                }
            }
            if (isUseOrder)
            {
                foreach (string name in cache.Order)
                {
                    var enumVal = (SortAs)orderType.GetProperty(name).GetValue(order, null);
                    switch (enumVal)
                    {
                        case SortAs.Asc:
                            cache.Sql = cache.Sql.Replace("#" + name, "ASC");
                            break;
                        case SortAs.Desc:
                            cache.Sql = cache.Sql.Replace("#" + name, "DESC");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            if (type.IsInterface)
            {
                IEnumerable<dynamic> res = connection.Query(cache.Sql);
                if (!res.Any())
                    return (IEnumerable<T>)null;
                var objList = new List<T>();
                foreach (dynamic item in res)
                {
                    var objItem = ProxyGenerator.GetInterfaceProxy<T>();

                    foreach (PropertyInfo property in TypePropertiesCache(type))
                    {
                        dynamic val = item[property.Name];
                        property.SetValue(objItem, val, null);
                    }

                    ((IProxy)objItem).IsDirty = false; //reset change tracking and return   
                    objList.Add(objItem);
                }
                obj = objList.AsEnumerable();
            }
            else
            {
                obj = await connection.QueryAsync<T>(cache.Sql, dynParms, transaction, commandTimeout: commandTimeout);
            }
            return obj;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">Interface type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <returns>Entity of T</returns>
        public static IEnumerable<T> GetByTop<T>(this IDbConnection connection, object where = null, object order = null, int top = 1,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            Type type = typeof(T);
            bool isUseWhere = where != null;
            bool isUseOrder = order != null;
            if (!isUseWhere && !isUseOrder)
            {
                return connection.GetAll<T>(transaction, commandTimeout);
            }
            Type whereType = isUseWhere ? where.GetType() : null;
            Type orderType = isUseOrder ? order.GetType() : null;
            int key = GetKeyTypeWhereOrder(type, whereType, orderType);
            if (!GetQueriesWhereOrder.TryGetValue(key, out SqlWhereOrderCache cache))
            {
                cache = new SqlWhereOrderCache();
                if (isUseWhere)
                {
                    cache.Where = GetListOfNames(whereType.GetProperties());
                }
                if (isUseOrder)
                {
                    cache.Order = GetListOfNames(orderType.GetProperties());
                }
                string name = GetTableName(type);
                var sb = new StringBuilder();

                sb.AppendFormat("select TOP {0} * from {1} (nolock)", top, name);
                int cnt, last, i;
                if (isUseWhere)
                {
                    sb.Append(" where ");
                    cnt = cache.Where.Count();
                    last = cnt - 1;
                    for (i = 0; i < cnt; i++)
                    {
                        string prop = cache.Where.ElementAt(i);
                        sb.AppendFormat("[{0}]=@{1}", prop, prop);
                        if (i != last)
                        {
                            sb.Append(" and ");
                        }
                    }
                }
                if (isUseOrder)
                {
                    sb.Append(" order by ");
                    cnt = cache.Order.Count();
                    last = cnt - 1;
                    for (i = 0; i < cnt; i++)
                    {
                        string prop = cache.Order.ElementAt(i);
                        sb.AppendFormat("[{0}] #{1}", prop, prop);
                        if (i != last)
                        {
                            sb.Append(", ");
                        }
                    }
                }

                // TODO: pluralizer 
                // TODO: query information schema and only select fields that are both in information schema and underlying class / interface 
                cache.Sql = sb.ToString();
                GetQueriesWhereOrder[key] = cache;
            }

            IEnumerable<T> obj = null;
            var dynParms = new DynamicParameters();
            if (isUseWhere)
            {
                foreach (string name in cache.Where)
                {
                    dynParms.Add(name, whereType.GetProperty(name).GetValue(where, null));
                }
            }
            if (isUseOrder)
            {
                foreach (string name in cache.Order)
                {
                    var enumVal = (string)orderType.GetProperty(name).GetValue(order, null);
                    switch (enumVal)
                    {
                        case "ASC":
                            cache.Sql = cache.Sql.Replace("#" + name, "ASC");
                            break;
                        case "DESC":
                            cache.Sql = cache.Sql.Replace("#" + name, "DESC");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            if (type.IsInterface)
            {
                IEnumerable<dynamic> res = connection.Query(cache.Sql);
                if (!res.Any())
                    return (IEnumerable<T>)null;
                var objList = new List<T>();
                foreach (dynamic item in res)
                {
                    var objItem = ProxyGenerator.GetInterfaceProxy<T>();

                    foreach (PropertyInfo property in TypePropertiesCache(type))
                    {
                        dynamic val = item[property.Name];
                        property.SetValue(objItem, val, null);
                    }

                    ((IProxy)objItem).IsDirty = false; //reset change tracking and return   
                    objList.Add(objItem);
                }
                obj = objList.AsEnumerable();
            }
            else
            {
                obj = connection.Query<T>(cache.Sql, dynParms, transaction, commandTimeout: commandTimeout);
            }
            return obj;
        }

        #endregion

        private static int GetKeyTypeWhereOrder(Type type, Type where, Type order)
        {
            RuntimeTypeHandle handler = type.TypeHandle;
            string whereCondition = @where != null ? @where.TypeHandle.Value.ToString() : string.Empty;
            string orderCondition = order != null ? order.TypeHandle.Value.ToString() : string.Empty;
            ;
            string str = string.Format("{0}{1}{2}", handler.Value, whereCondition, orderCondition);
            return str.GetHashCode();
        }

        private static IEnumerable<string> GetListOfNames(PropertyInfo[] list)
        {
            var lst = new List<string>();
            foreach (PropertyInfo info in list)
            {
                lst.Add(info.Name);
            }
            return lst.AsEnumerable();
        }

        private static string GetTableName(Type type)
        {
            string name;
            if (!TypeTableName.TryGetValue(type.TypeHandle, out name))
            {
                name = type.Name; // + "s";
                if (type.IsInterface && name.StartsWith("I"))
                    name = name.Substring(1);

                //NOTE: This as dynamic trick should be able to handle both our own Table-attribute as well as the one in EntityFramework 
                var tableattr =
                    type.GetCustomAttributes(false)
                        .Where(attr => attr.GetType().Name == "TableAttribute")
                        .SingleOrDefault() as
                        dynamic;
                if (tableattr != null)
                    name = tableattr.Name;
                TypeTableName[type.TypeHandle] = name;
            }
            return name;
        }

        public static bool IsWriteable(PropertyInfo pi)
        {
            object[] attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false);
            if (attributes.Length == 1)
            {
                var write = (WriteAttribute)attributes[0];
                return write.Write;
            }
            return true;
        }

        private static IEnumerable<PropertyInfo> TypePropertiesCache(Type type)
        {
            if (TypeProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pis))
            {
                return pis;
            }

            IEnumerable<PropertyInfo> properties = type.GetProperties().Where(IsWriteable);
            TypeProperties[type.TypeHandle] = properties;
            return properties;
        }

        private class ProxyGenerator
        {
            private static readonly Dictionary<Type, object> TypeCache = new Dictionary<Type, object>();

            private static AssemblyBuilder GetAsmBuilder(string name)
            {
                return AssemblyBuilder.DefineDynamicAssembly(new AssemblyName { Name = name },
                        AssemblyBuilderAccess.Run); //NOTE: to save, use RunAndSave
            }

            public static T GetClassProxy<T>()
            {
                // A class proxy could be implemented if all properties are virtual
                //  otherwise there is a pretty dangerous case where internal actions will not update dirty tracking
                throw new NotImplementedException();
            }


            public static T GetInterfaceProxy<T>()
            {
                Type typeOfT = typeof(T);

                object k;
                if (TypeCache.TryGetValue(typeOfT, out k))
                {
                    return (T)k;
                }
                AssemblyBuilder assemblyBuilder = GetAsmBuilder(typeOfT.Name);

                ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("SqlMapperExtensions." + typeOfT.Name);
                //NOTE: to save, add "asdasd.dll" parameter

                Type interfaceType = typeof(IProxy);
                TypeBuilder typeBuilder = moduleBuilder.DefineType(typeOfT.Name + "_" + Guid.NewGuid(),
                    TypeAttributes.Public | TypeAttributes.Class);
                typeBuilder.AddInterfaceImplementation(typeOfT);
                typeBuilder.AddInterfaceImplementation(interfaceType);

                //create our _isDirty field, which implements IProxy
                MethodInfo setIsDirtyMethod = CreateIsDirtyProperty(typeBuilder);

                // Generate a field for each property, which implements the T
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    bool isId = property.GetCustomAttributes(true).Any(a => a is KeyAttribute);
                    CreateProperty<T>(typeBuilder, property.Name, property.PropertyType, setIsDirtyMethod, isId);
                }

                Type generatedType = typeBuilder.CreateType();

                //assemblyBuilder.Save(name + ".dll");  //NOTE: to save, uncomment

                object generatedObject = Activator.CreateInstance(generatedType);

                TypeCache.Add(typeOfT, generatedObject);
                return (T)generatedObject;
            }


            private static MethodInfo CreateIsDirtyProperty(TypeBuilder typeBuilder)
            {
                Type propType = typeof(bool);
                FieldBuilder field = typeBuilder.DefineField("_" + "IsDirty", propType, FieldAttributes.Private);
                PropertyBuilder property = typeBuilder.DefineProperty("IsDirty",
                    System.Reflection.PropertyAttributes.None,
                    propType,
                    new[] { propType });

                const MethodAttributes getSetAttr =
                    MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.SpecialName |
                    MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                MethodBuilder currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + "IsDirty",
                    getSetAttr,
                    propType,
                    Type.EmptyTypes);
                ILGenerator currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);
                MethodBuilder currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + "IsDirty",
                    getSetAttr,
                    null,
                    new[] { propType });
                ILGenerator currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ret);

                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                MethodInfo getMethod = typeof(IProxy).GetMethod("get_" + "IsDirty");
                MethodInfo setMethod = typeof(IProxy).GetMethod("set_" + "IsDirty");
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);

                return currSetPropMthdBldr;
            }

            private static void CreateProperty<T>(TypeBuilder typeBuilder, string propertyName, Type propType,
                MethodInfo setIsDirtyMethod, bool isIdentity)
            {
                //Define the field and the property 
                FieldBuilder field = typeBuilder.DefineField("_" + propertyName, propType, FieldAttributes.Private);
                PropertyBuilder property = typeBuilder.DefineProperty(propertyName,
                    System.Reflection.PropertyAttributes.None,
                    propType,
                    new[] { propType });

                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.Virtual |
                                                    MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                MethodBuilder currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName,
                    getSetAttr,
                    propType,
                    Type.EmptyTypes);

                ILGenerator currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);

                MethodBuilder currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                    getSetAttr,
                    null,
                    new[] { propType });

                //store value in private field and set the isdirty flag
                ILGenerator currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldc_I4_1);
                currSetIL.Emit(OpCodes.Call, setIsDirtyMethod);
                currSetIL.Emit(OpCodes.Ret);

                //TODO: Should copy all attributes defined by the interface?
                if (isIdentity)
                {
                    Type keyAttribute = typeof(KeyAttribute);
                    ConstructorInfo myConstructorInfo = keyAttribute.GetConstructor(new Type[] { });
                    var attributeBuilder = new CustomAttributeBuilder(myConstructorInfo, new object[] { });
                    property.SetCustomAttribute(attributeBuilder);
                }

                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                MethodInfo getMethod = typeof(T).GetMethod("get_" + propertyName);
                MethodInfo setMethod = typeof(T).GetMethod("set_" + propertyName);
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);
            }
        }

        public interface IProxy
        {
            bool IsDirty { get; set; }
        }
    }

    public class SqlWhereOrderCache
    {
        public string Sql { get; set; }
        public IEnumerable<string> Where { get; set; }
        public IEnumerable<string> Order { get; set; }
    }




}


namespace Dapper.Contrib.Extensions
{
    public static partial class SqlMapperExtensions
    {
        #region Delegate
        public delegate string GetDatabaseTypeDelegate(IDbConnection connection);
        #endregion
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ExplicitKeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        //private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ComputedProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        public static GetDatabaseTypeDelegate GetDatabaseType;
        private static readonly ISqlAdapter DefaultAdapter = (ISqlAdapter)new SqlServerAdapter();

        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary = new Dictionary<string, ISqlAdapter>()
        {
            {
                "sqlconnection",
                (ISqlAdapter) new SqlServerAdapter()
            },
            {
                "sqlceconnection",
                (ISqlAdapter) new SqlCeServerAdapter()
            },
            {
                "npgsqlconnection",
                (ISqlAdapter) new PostgresAdapter()
            },
            {
                "sqliteconnection",
                (ISqlAdapter) new SQLiteAdapter()
            },
            {
                "mysqlconnection",
                (ISqlAdapter) new MySqlAdapter()
            }
        };

        public static long InsertWithParam<T>(this IDbConnection connection, T source, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType())
            {
                type = type.GetGenericArguments()[0];
            }
            var tableName = GetTableName(type);
            var sb = new StringBuilder((string)null);
            var stringBuilder = new StringBuilder((string)null);
            var first1 = TypePropertiesCache(type).ToList();
            var first2 = KeyPropertiesCache(type);
            var propertyInfoList = ComputedPropertiesCache(type);
            var list = first1.Except<PropertyInfo>(first2.Union<PropertyInfo>((IEnumerable<PropertyInfo>)propertyInfoList)).ToList<PropertyInfo>();
            ISqlAdapter formatter = GetFormatter(connection);
            var parameters = new DynamicParameters();

            sb.Append(string.Join(",", list.Select(p => p.Name)));
            stringBuilder.Append(string.Format("@{0}", string.Join(", @", list.Select(p => p.Name))));

            for (int index = 0; index < list.Count; ++index)
            {
                var propertyInfo = list.ElementAt<PropertyInfo>(index);
                var attribute = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true);
                // Just in case you have a property without this annotation
                if (attribute != null && attribute.Length > 0)
                {
                    var _attribute = (ColumnAttribute)attribute[0];
                    //var param = new dyn
                    if (_attribute.Size > 0)
                        parameters.Add(name: string.Format("{0}", propertyInfo.Name),
                            value: type.GetProperty(propertyInfo.Name).GetValue(source),
                            dbType: _attribute.dbType, size: _attribute.Size);
                    else
                        parameters.Add(name: string.Format("{0}", propertyInfo.Name),
                            value: type.GetProperty(propertyInfo.Name).GetValue(source),
                            dbType: _attribute.dbType);
                }
                else
                {
                    if (index == 0)
                    {
                        sb = new StringBuilder();
                        stringBuilder = new StringBuilder();
                    }

                    formatter.AppendColumnName(sb, propertyInfo.Name);
                    stringBuilder.AppendFormat("@{0}", (object)propertyInfo.Name);
                    if (index < list.Count - 1)
                    {
                        sb.Append(", ");
                        stringBuilder.Append(", ");

                    }
                }
            }

            int num1 = connection.State == ConnectionState.Closed ? 1 : 0;
            if (num1 != 0)
                connection.Open();
            long num2 = 0;
            if (!parameters.ParameterNames.Any())
            {
                num2 = formatter.Insert(connection, transaction, commandTimeout, tableName, sb.ToString(), stringBuilder.ToString(), (IEnumerable<PropertyInfo>)first2, (object)source);
            }
            else
            {
                var sql = string.Format("INSERT INTO {0} ({1}) VALUES({2}) SELECT @@IDENTITY", (object)tableName, (object)sb,
                    (object)stringBuilder);

                long.TryParse(connection.ExecuteScalar(sql, parameters, transaction, commandTimeout, CommandType.Text).ToString(), out num2);

            }
            if (num1 != 0)
                connection.Close();
            return num2;
        }
        public static bool UpdateWithParam<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var proxy = (object)entity as IProxy;
            if (proxy != null && !proxy.IsDirty)
                return false;
            var type = typeof(T);
            if (type.IsArray)
                type = type.GetElementType();
            else if (TypeExtensions.IsGenericType(type))
                type = type.GetGenericArguments()[0];
            var list1 = KeyPropertiesCache(type).ToList<PropertyInfo>();
            var source = ExplicitKeyPropertiesCache(type);
            if (!list1.Any<PropertyInfo>() && !source.Any<PropertyInfo>())
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");
            var tableName = GetTableName(type);
            var sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET ", (object)tableName);
            var first = TypePropertiesCache(type).ToList();
            list1.AddRange((IEnumerable<PropertyInfo>)source);
            var propertyInfoList = ComputedPropertiesCache(type);
            var second = list1.Union<PropertyInfo>((IEnumerable<PropertyInfo>)propertyInfoList);
            var list2 = first.Except<PropertyInfo>(second).ToList<PropertyInfo>();
            var parameters = new DynamicParameters();

            sb.Append(string.Join(",", list2.Select(x => string.Format("{0} = @{0}", x.Name))));

            for (int index = 0; index < list2.Count; ++index)
            {
                PropertyInfo propertyInfo = list2.ElementAt<PropertyInfo>(index);
                object[] attribute = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true);
                // Just in case you have a property without this annotation
                if (attribute != null && attribute.Length > 0)
                {
                    var _attribute = (ColumnAttribute)attribute[0];
                    //var param = new dyn
                    if (_attribute.Size > 0)
                        parameters.Add(name: string.Format("{0}", propertyInfo.Name),
                            value: type.GetProperty(propertyInfo.Name).GetValue(source),
                            dbType: _attribute.dbType, size: _attribute.Size);
                    else
                        parameters.Add(name: string.Format("{0}", propertyInfo.Name),
                            value: type.GetProperty(propertyInfo.Name).GetValue(source),
                            dbType: _attribute.dbType);
                }
            }

            sb.Append(" WHERE ");

            sb.Append(string.Join(" AND ", list1.Select(x => string.Format("{0} = @{0}", x.Name))));

            for (int index = 0; index < list1.Count; ++index)
            {
                PropertyInfo propertyInfo = list1.ElementAt<PropertyInfo>(index);
                object[] attribute = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true);
                // Just in case you have a property without this annotation
                if (attribute != null && attribute.Length > 0)
                {
                    var _attribute = (ColumnAttribute)attribute[0];
                    //var param = new dyn
                    if (_attribute.Size > 0)
                        parameters.Add(name: string.Format("{0}", propertyInfo.Name),
                            value: type.GetProperty(propertyInfo.Name).GetValue(source),
                            dbType: _attribute.dbType, size: _attribute.Size);
                    else
                        parameters.Add(name: string.Format("{0}", propertyInfo.Name),
                            value: type.GetProperty(propertyInfo.Name).GetValue(source),
                            dbType: _attribute.dbType);
                }
            }

            string sql = sb.ToString();
            // ISSUE: variable of a boxed type

            int num1 = connection.State == ConnectionState.Closed ? 1 : 0;
            if (num1 != 0)
                connection.Open();

            return connection.Execute(sql, (object)parameters, transaction, commandTimeout, CommandType.Text) > 0;
        }

        public static IEnumerable<T> GetByParam<T>(this IDbConnection connection, object where = null, object order = null,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            Type type = typeof(T);
            var properties = TypePropertiesCache(type).ToList();
            bool isUseWhere = where != null;
            bool isUseOrder = order != null;
            if (!isUseWhere && !isUseOrder)
            {
                return connection.GetAll<T>(transaction, commandTimeout);
            }
            Type whereType = isUseWhere ? where.GetType() : null;
            Type orderType = isUseOrder ? order.GetType() : null;
            SqlWhereOrderCache cache;
            var parameters = new DynamicParameters();
            int key = GetKeyTypeWhereOrder(type, whereType, orderType);
            if (!GetQueriesWhereOrder.TryGetValue(key, out cache))
            {
                cache = new SqlWhereOrderCache();
                if (isUseWhere)
                {
                    cache.Where = GetListOfNames(whereType.GetProperties());
                }
                if (isUseOrder)
                {
                    cache.Order = GetListOfNames(orderType.GetProperties());
                }
                string name = GetTableName(type);
                var sb = new StringBuilder();

                sb.AppendFormat("SELECT * FROM {0} (NOLOCK)", name);
                int cnt, last, i;
                if (isUseWhere)
                {
                    sb.Append(" WHERE ");
                    cnt = cache.Where.Count();
                    last = cnt - 1;
                    for (i = 0; i < cnt; i++)
                    {
                        var prop = cache.Where.ElementAt(i);
                        sb.AppendFormat("[{0}] = @{1}", prop, prop);
                        if (i != last)
                        {
                            sb.Append(" AND ");
                        }
                    }
                }
                if (isUseOrder)
                {
                    sb.Append(" ORDER BY ");
                    cnt = cache.Order.Count();
                    last = cnt - 1;
                    for (i = 0; i < cnt; i++)
                    {
                        string prop = cache.Order.ElementAt(i);
                        sb.AppendFormat("[{0}] #{1}", prop, prop);
                        if (i != last)
                        {
                            sb.Append(", ");
                        }
                    }
                }

                // TODO: pluralizer 
                // TODO: query information schema and only select fields that are both in information schema and underlying class / interface 
                cache.Sql = sb.ToString();
                GetQueriesWhereOrder[key] = cache;
            }

            IEnumerable<T> obj = null;

            if (isUseWhere)
            {
                foreach (string name in cache.Where)
                {
                    var property = properties.FirstOrDefault(x => x.Name == name);
                    if (property != null)
                    {
                        object[] attribute = property.GetCustomAttributes(typeof(ColumnAttribute), true);
                        // Just in case you have a property without this annotation
                        if (attribute != null && attribute.Length > 0)
                        {
                            var _attribute = (ColumnAttribute)attribute[0];
                            //var param = new dyn
                            if (_attribute.Size > 0)
                                parameters.Add(name: string.Format("{0}", property.Name),
                                    value: whereType.GetProperty(property.Name).GetValue(where, null),
                                    dbType: _attribute.dbType, size: _attribute.Size);
                            else
                                parameters.Add(name: string.Format("{0}", property.Name),
                                    value: whereType.GetProperty(property.Name).GetValue(where, null),
                                    dbType: _attribute.dbType);
                        }
                        else
                        {
                            parameters.Add(property.Name, whereType.GetProperty(property.Name).GetValue(where, null));
                        }
                    }
                    else
                    {
                        parameters.Add(name, whereType.GetProperty(name).GetValue(where, null));
                    }
                }
            }
            if (isUseOrder)
            {
                foreach (string name in cache.Order)
                {
                    var enumVal = (SortAs)orderType.GetProperty(name).GetValue(order, null);
                    switch (enumVal)
                    {
                        case SortAs.Asc:
                            cache.Sql = cache.Sql.Replace("#" + name, "ASC");
                            break;
                        case SortAs.Desc:
                            cache.Sql = cache.Sql.Replace("#" + name, "DESC");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            if (type.IsInterface)
            {
                IEnumerable<dynamic> res = connection.Query(cache.Sql);
                if (!res.Any())
                    return (IEnumerable<T>)null;
                var objList = new List<T>();
                foreach (dynamic item in res)
                {
                    var objItem = ProxyGenerator.GetInterfaceProxy<T>();

                    foreach (PropertyInfo property in TypePropertiesCache(type))
                    {
                        dynamic val = item[property.Name];
                        property.SetValue(objItem, val, null);
                    }

                    ((IProxy)objItem).IsDirty = false; //reset change tracking and return   
                    objList.Add(objItem);
                }
                obj = objList.AsEnumerable();
            }
            else
            {
                obj = connection.Query<T>(cache.Sql, parameters, transaction, commandTimeout: commandTimeout,
                    commandType: CommandType.Text);
            }
            return obj;
        }

        private static List<PropertyInfo> KeyPropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> source1;
            if (KeyProperties.TryGetValue(type.TypeHandle, out source1))
                return source1.ToList<PropertyInfo>();
            List<PropertyInfo> source2 = TypePropertiesCache(type).ToList();
            List<PropertyInfo> list = source2.Where<PropertyInfo>((Func<PropertyInfo, bool>)(p => ((IEnumerable<object>)p.GetCustomAttributes(true)).Any<object>((Func<object, bool>)(a => a is KeyAttribute)))).ToList<PropertyInfo>();
            if (list.Count == 0)
            {
                PropertyInfo propertyInfo = source2.FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>)(p => p.Name.ToLower() == "id"));
                if (propertyInfo != (PropertyInfo)null && !((IEnumerable<object>)propertyInfo.GetCustomAttributes(true)).Any<object>((Func<object, bool>)(a => a is ExplicitKeyAttribute)))
                    list.Add(propertyInfo);
            }
            KeyProperties[type.TypeHandle] = (IEnumerable<PropertyInfo>)list;
            return list;
        }
        private static List<PropertyInfo> ExplicitKeyPropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> source;
            if (ExplicitKeyProperties.TryGetValue(type.TypeHandle, out source))
                return source.ToList<PropertyInfo>();
            List<PropertyInfo> list = TypePropertiesCache(type).Where<PropertyInfo>((Func<PropertyInfo, bool>)(p => ((IEnumerable<object>)p.GetCustomAttributes(true)).Any<object>((Func<object, bool>)(a => a is ExplicitKeyAttribute)))).ToList<PropertyInfo>();
            ExplicitKeyProperties[type.TypeHandle] = (IEnumerable<PropertyInfo>)list;
            return list;
        }
        private static ISqlAdapter GetFormatter(IDbConnection connection)
        {
            GetDatabaseTypeDelegate getDatabaseType = GetDatabaseType;
            string key = getDatabaseType?.Invoke(connection).ToLower() ?? connection.GetType().Name.ToLower();
            if (AdapterDictionary.ContainsKey(key))
                return AdapterDictionary[key];
            return DefaultAdapter;
        }

        private static List<PropertyInfo> ComputedPropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> source;
            if (ComputedProperties.TryGetValue(type.TypeHandle, out source))
                return source.ToList<PropertyInfo>();
            List<PropertyInfo> list = TypePropertiesCache(type).Where<PropertyInfo>((Func<PropertyInfo, bool>)(p => ((IEnumerable<object>)p.GetCustomAttributes(true)).Any<object>((Func<object, bool>)(a => a is ComputedAttribute)))).ToList<PropertyInfo>();
            ComputedProperties[type.TypeHandle] = (IEnumerable<PropertyInfo>)list;
            return list;
        }
    }

    internal static class TypeExtensions
    {
        public static string Name(this Type type)
        {
            return type.Name;
        }

        public static bool IsValueType(this Type type)
        {
            return type.IsValueType;
        }

        public static bool IsEnum(this Type type)
        {
            return type.IsEnum;
        }

        public static bool IsGenericType(this Type type)
        {
            return type.IsGenericType;
        }

        public static bool IsInterface(this Type type)
        {
            return type.IsInterface;
        }

        public static TypeCode GetTypeCode(Type type)
        {
            return Type.GetTypeCode(type);
        }

        public static MethodInfo GetPublicInstanceMethod(this Type type, string name, Type[] types)
        {
            return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public, (Binder)null, types, (ParameterModifier[])null);
        }
    }



    /// <summary>
    /// Uses the Name value of the <see cref="ColumnAttribute"/> specified to determine
    /// the association between the name of the column in the query results and the member to
    /// which it will be extracted. If no column mapping is present all members are mapped as
    /// usual.
    /// </summary>
    /// <typeparam name="T">The type of the object that this association between the mapper applies to.</typeparam>

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ColumnAttribute : Attribute
    {
        //public string Name { get; set; }
        public DbType dbType { get; set; }
        public int Size { get; set; }
    }

}
public enum SortAs
{
    Asc,
    Desc
}