using CoreProjectClient.Data.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CoreProjectClient.Service
{
    public class ProductService
    {
        string Baseurl = " https://localhost:5001/";


        public async Task<ServiceResponse<Product>> GetProduct()
        {
            ServiceResponse<Product> productList = new ServiceResponse<Product>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("Product/GetAll");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var ProductResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    productList = JsonConvert.DeserializeObject<ServiceResponse<Product>>(ProductResponse);

                }
                //returning the employee list to view  
                return productList;
            }
        }


    }
}
