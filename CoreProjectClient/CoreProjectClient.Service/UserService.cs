using CoreProjectClient.Data.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoreProjectClient.Service
{
    public class UserService
    {
        string Baseurl = " https://localhost:5001/User/";

        public async Task<ServiceResponse<Customer>> GetUsers()
        {
            ServiceResponse<Customer> users = new ServiceResponse<Customer>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("GetAll");

                if (Res.IsSuccessStatusCode)
                {
                    var customerResponse = Res.Content.ReadAsStringAsync().Result;

                    users = JsonConvert.DeserializeObject<ServiceResponse<Customer>>(customerResponse);

                }
                return users;
            }
        }

        public async Task<Customer> GetUserById(int id)
        {
            ServiceResponse<Customer> user = new ServiceResponse<Customer>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("GetById/"+id);

                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;

                    user = JsonConvert.DeserializeObject<ServiceResponse<Customer>>(Response);

                }
                return user.Entity;
            }
        }

        public async Task<Customer> GetUserConfirm(string email ,string password)
        {
            ServiceResponse<Customer> user = new ServiceResponse<Customer>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("GetUser/?email=" + email+"&password="+password);
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;

                    user = JsonConvert.DeserializeObject<ServiceResponse<Customer>>(Response);

                }
                return user.Entity;
            }
        }

        public async Task<string> AddUser(Customer customer)
        {
            string message = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(customer);           
                var data = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage Res = await client.PostAsync("InsertUser/", data);
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;

                    message = JsonConvert.DeserializeObject<string>(Response);

                }
                return message;
            }
        }


    }
}
