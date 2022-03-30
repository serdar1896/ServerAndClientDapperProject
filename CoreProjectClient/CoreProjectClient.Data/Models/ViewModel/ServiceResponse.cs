using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreProjectClient.Data.Models.ViewModel
{
    public class ServiceResponse<T>
    {
        public ServiceResponse()
        {
            List = new HashSet<T>();
        }
        public bool HasExceptionError { get; set; }
    
        public string ExceptionMessage { get; set; }

        public IEnumerable<T> List { get; set; }

        [JsonProperty]
        public T Entity { get; set; }

        public int Count { get; set; }

        public bool IsValid { get; set; }

        public bool IsSuccessful { get; set; }
    }
}
