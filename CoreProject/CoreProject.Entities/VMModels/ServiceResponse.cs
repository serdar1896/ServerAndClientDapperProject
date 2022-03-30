using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace CoreProject.Entities.VMModels
{
    public class ServiceResponse<T>
    {
        public ServiceResponse()
        {
            List = new HashSet<T>();
        }
        public bool HasExceptionError { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ExceptionMessage { get; set; }

        public IEnumerable<T> List { get; set; }

        [JsonProperty]
        public T Entity { get; set; }

        public int Count { get { return Entity != null ? 1 : List.Count();  } }

        public bool IsValid => !HasExceptionError && string.IsNullOrEmpty(ExceptionMessage);

        public bool IsSuccessful { get; set; }
    }
}
