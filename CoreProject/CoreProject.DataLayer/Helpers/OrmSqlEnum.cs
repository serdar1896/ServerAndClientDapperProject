namespace CoreProject.DataLayer.Helpers
{
    public class OrmSqlEnum
    {
        public string GetAllSQL<T>()
        {
            return $"SELECT * FROM [{typeof(T).Name}](nolock)";
        }
        public string GetByIdSQL<T>()
        {
            return $"SELECT * FROM {typeof(T).Name}(nolock) WHERE Id=@Id";
        }
        public string DeleteSQL<T>()
        {
            return $"DELETE FROM {typeof(T).Name} WHERE Id=@Id";
        }

    }
}
