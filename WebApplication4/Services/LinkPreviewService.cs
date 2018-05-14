using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebApplication4.Models;

namespace WebApplication4.Services

{
    public class LinkPreviewService : BaseService
    {
        private const string ApiKey = "5ad095aa9ed741cf1d1f0eb3017f316666faebd7ee902";
        private  readonly HttpClient Client = new HttpClient();


        public LinkPreviewService()
        {
            
        }
        public  void AddLinkPreviewInDb(LinkPreview linkPreview)
        {
            var context = new ValidationContext(linkPreview, null, null);
            var results = new List<ValidationResult>();
            linkPreview.Id = Guid.NewGuid();
            if (Validator.TryValidateObject(linkPreview, context, results, true))
            {
                Context.LinkPreview.Add(linkPreview);
                Context.SaveChanges();
                var errors = Context.GetValidationErrors();
                if (errors.Any())
                {
                    foreach(var error in errors)
                    {
                        System.Diagnostics.Debug.WriteLine(error.ToString());
                    }
                }
            }
        }
           
        public  LinkPreview FindLinkPreview(string url)
        {
            return Context.LinkPreview.FirstOrDefault(x => x.Url == url);
        }
        public  LinkPreview FindLinkPreviewById(Guid id)
        {
            return Context.LinkPreview.FirstOrDefault(x => x.Id == id);
        }

        public  async Task<LinkPreview>  GetUrlPreview(string url)
        {
            var values = new Dictionary<string, string>
            {
                { "key", ApiKey },
                { "q", url }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await Client.PostAsync("https://api.linkpreview.net", content);
            var result = JsonConvert.DeserializeObject<LinkPreview>(await response.Content.ReadAsStringAsync()); 
            return  result;
        }

       
    }
}