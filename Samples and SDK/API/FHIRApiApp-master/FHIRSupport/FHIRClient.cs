 /* 
 * 2017 Microsoft Corp
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS “AS IS”
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Specialized;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hl7.Fhir.Rest;
using System.Configuration;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Model;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace FHIRSupport
{
    public class FHIRClient
    {
        private IRestClient _client = null;
        private FhirJsonParser _parser = null;
        public FHIRClient(string baseurl,string bearerToken)
        {
            init(baseurl, bearerToken);
        }
        public FHIRClient(string baseurl,string tenent=null,string clientid=null,string secret=null)
        {
            string tokenresp = null;
            if (tenent != null && clientid != null && secret != null)
            {
                tokenresp = GetOAUTH2BearerToken(tenent, baseurl, clientid, secret);
            }
            init(baseurl, tokenresp);
        }
        public string BearerToken { get; set; }
        private void init(string baseurl, string bearerToken)
        {
            _client = new RestClient(baseurl);
            _parser = new FhirJsonParser();
            _parser.Settings.AcceptUnknownMembers = true;
            _parser.Settings.AllowUnrecognizedEnums = true;
            BearerToken = bearerToken;
        }
        
        private string GetOAUTH2BearerToken(string tenent,string baseurl,string clientid,string secret)
        {
            using (WebClient client = new WebClient())
            {
               byte[] response =
                client.UploadValues("https://login.microsoftonline.com/" + tenent + "/oauth2/token", new NameValueCollection()
                {
                    {"grant_type","client_credentials"},
                    {"client_id",clientid},
                    { "client_secret", secret },
                    { "resource", baseurl }
                });


                string result = System.Text.Encoding.UTF8.GetString(response);
                JObject obj = JObject.Parse(result);
                return (string) obj["access_token"];
            }
        }
        public Resource LoadResource(string resource,string parmstring=null)
        {
            var request = new RestRequest(resource + (parmstring != null ? "?" + parmstring :""), Method.GET);
            request.AddHeader("Accept", "application/json");
            if (BearerToken != null)
            {
                request.AddHeader("Authorization", "Bearer " + BearerToken);
            }
            IRestResponse response2 = _client.Execute(request);
            var reader = FhirJsonParser.CreateFhirReader(response2.Content);
            return _parser.Parse<Resource>(reader);
        }
        public bool SaveResource(Resource r)
        {
            string rt = Enum.GetName(typeof(Hl7.Fhir.Model.ResourceType), r.ResourceType);
            var request = new RestRequest(rt, Method.POST);
            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/json");
            if (BearerToken != null)
            {
                request.AddHeader("Authorization", "Bearer " + BearerToken);
            }
            string srv = FhirSerializer.SerializeResourceToJson(r);
            request.AddParameter("application/json; charset=utf-8", srv, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            IRestResponse response2 = _client.Execute(request);
            return (response2.ResponseStatus == ResponseStatus.Completed);
        }

    }
}