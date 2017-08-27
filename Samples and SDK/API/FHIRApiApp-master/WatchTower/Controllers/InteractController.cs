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
using Microsoft.Azure;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using WatchTower;
using model=Hl7.Fhir.Model;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using FHIRSupport;
using System.Configuration;
namespace WatchTower.Controllers
{
    
    [RoutePrefix("api")]
    public class InteractController : ApiController
    {
        static RegistryManager registryManager;
        static string connectionString = "HostName=fhiriothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=5vdnDln+xOet3I84vBy+24QIqhdSJy6WZ+mp0nipxsQ=";
        public FHIRClient _client = null;
        public InteractController()
        {
            string fhirserver = CloudConfigurationManager.GetSetting("FHIRServerAddress");
            string fhirtenant = CloudConfigurationManager.GetSetting("FHIRAADTenant");
            string fhirclientid = CloudConfigurationManager.GetSetting("FHIRClientId");
            string fhirclientsecret = CloudConfigurationManager.GetSetting("FHIRClientSecret");
            _client = new FHIRClient(fhirserver, fhirtenant, fhirclientid, fhirclientsecret);
        }
        [HttpGet]
        [Route("getoauthtoken")]
        public HttpResponseMessage GetToken()
        {
            HttpResponseMessage response = null;
            response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(_client.BearerToken, Encoding.UTF8, "application/json");
            return response;
        }
        [HttpGet]
        [Route("gentoken/{id}")]
        public HttpResponseMessage Get(string id)
        {
            HttpResponseMessage response = null;
            string token = "";
    
            //load Patient and generated a new PIN Code valid for 48 hours
            model.Patient pat = (model.Patient) _client.LoadResource("Patient/" + id);
            if  (pat != null)
            {
                var fbid = pat.Identifier.FirstOrDefault(ident => ident.System == "http://fhirbot.org");
                if (fbid != null)
                {
                    pat.Identifier.Remove(fbid);
                }
                Random r = new Random();
                int randNum = r.Next(1000000);
                token = randNum.ToString("D6");
                model.Identifier newid = new model.Identifier("http://fhirbot.org", token);
                newid.Period = new model.Period(model.FhirDateTime.Now(), new model.FhirDateTime(DateTimeOffset.Now.AddDays(2)));
                pat.Identifier.Add(newid);
                _client.SaveResource(pat);
                string respval = "{\"token\":\"" + token + "\"}";
                response = this.Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(respval, Encoding.UTF8, "application/json");
            } else
            {
                response = this.Request.CreateResponse(HttpStatusCode.NotFound);
                response.Content = new StringContent("Can't find patient with id " + id, Encoding.UTF8, "text/html");
            }
                    
           
            return response;
        }
        [HttpPut]
        [Route("regdevice")]
        public async Task<HttpResponseMessage> Post()
        {

            HttpResponseMessage response = null;
            string raw = await Request.Content.ReadAsStringAsync();
            JObject obj = JObject.Parse(raw);
          
            Hl7.Fhir.Model.Device dev = new Hl7.Fhir.Model.Device();
            dev.Id = Guid.NewGuid().ToString();
            //stub in IOT Hub
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            Device device = await AddDeviceAsync(dev.Id);
            if (device != null)
            {
                dev.Type = new model.CodeableConcept("http://snomed.info/sct", "448703006", "Pulse oximeter (physical object)");
                dev.ManufactureDate = "2016-12-11";
                dev.ExpirationDate = "2021-12-11";
                dev.Identifier = new List<model.Identifier>();
                dev.Identifier.Add(new model.Identifier("http://fhiriothub/key", device.Authentication.SymmetricKey.PrimaryKey));
                dev.Patient = new model.ResourceReference("Patient/" + obj.GetValue("id"));
                dev.Manufacturer = "Acme Medical Devices, Inc.";
                dev.LotNumber = "29328-992";
                dev.Model = "SuperDeluxePulseOx";
                if (_client.SaveResource(dev))
                {


                    string respval = "{\"devid\":\"" + dev.Id + "\"}";
                    response = this.Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(respval, Encoding.UTF8, "application/json");
                }
                else
                {
                    response = this.Request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.Content = new StringContent("{\"message\":\"Error creating device\"", Encoding.UTF8, "application/json");
                }
            } else
            {
                response = this.Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("{\"message\":\"Error registering device with IOT hub\"", Encoding.UTF8, "application/json");
            }


            return response;
        }
        private static async Task<Device> AddDeviceAsync(string deviceId)
        {

            Device device = null;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
                
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);

            }
            catch (Exception)
            {
                device = null;
            }
            return device;
            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }
    }
}
