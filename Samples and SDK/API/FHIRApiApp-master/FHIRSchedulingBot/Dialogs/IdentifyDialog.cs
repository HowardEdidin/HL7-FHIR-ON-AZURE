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
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Microsoft.Azure;
using System.Linq;
using model = Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using FHIRSupport;
using RestSharp;
using System.Text;
using System.Collections.Generic;
// Add a using directive at the top of your code file    
using System.Configuration;


namespace FHIRSchedulingBot.Dialogs
{
    [Serializable]
    public class IdentifyDialog : IDialog<string>
    {
        
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Please enter your access PIN Code");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string msg = activity.Text.Trim().ToLower();
            if (msg == "start over" || msg == "exit" || msg == "quit" || msg == "done" || msg == "start again" || msg == "restart" || msg == "leave" || msg == "reset" || msg == "bye" || msg == "goodbye")
            {
                context.PrivateConversationData.Clear();
                await context.PostAsync($"Ending your session...Dont' forget to delete your conversation for privacy!");
                context.Done("");
            }
            else
            {
                model.Patient pat = null;
                // Within the code body set your variable    
                string fhirserver = CloudConfigurationManager.GetSetting("FHIRServerAddress");
                string fhirtenant = CloudConfigurationManager.GetSetting("FHIRAADTenant");
                string fhirclientid = CloudConfigurationManager.GetSetting("FHIRClientId");
                string fhirclientsecret = CloudConfigurationManager.GetSetting("FHIRClientSecret");
                FHIRClient fhirclient = new FHIRClient(fhirserver,fhirtenant,fhirclientid,fhirclientsecret);
                var rslt = (model.Bundle)fhirclient.LoadResource("Patient", "identifier=http://fhirbot.org|" + activity.Text.Trim());
                if (rslt != null && rslt.Entry != null && rslt.Entry.Count > 0)
                {
                    pat = (model.Patient)rslt.Entry.FirstOrDefault().Resource;
                    if (pat != null)
                    {
                        var fbid = pat.Identifier.Single(ident => ident.System == "http://fhirbot.org");
                        if (fbid != null)
                        {
                            var period = fbid.Period;
                            var now = model.FhirDateTime.Now();
                            if (period != null && (now > period.EndElement))
                            {
                                //Use Period Expired remove the token from the patient and update db
                                pat.Identifier.Remove(fbid);
                                fhirclient.SaveResource(pat);
                                pat = null;
                                context.PrivateConversationData.Clear();
                            }
                        }
                    }
                }
                if (pat != null)
                {
                    // Set BotUserData
                    context.PrivateConversationData.SetValue<string>(
                        "id", pat.Id);
                    context.PrivateConversationData.SetValue<string>(
                        "name", pat.Name[0].Text);
                    context.PrivateConversationData.SetValue<DateTime?>(
                        "sessionstart", DateTime.Now);
                    context.PrivateConversationData.SetValue<string>("bearertoken", fhirclient.BearerToken);
                    context.PrivateConversationData.SetValue<string>("fhirserver", fhirserver);
                    context.Done(pat.Name[0].Text);
                    
                } else
                {
                    context.Fail(new InvalidPINException("Not a valid PIN Code"));
                }
            }
        }
    }
}