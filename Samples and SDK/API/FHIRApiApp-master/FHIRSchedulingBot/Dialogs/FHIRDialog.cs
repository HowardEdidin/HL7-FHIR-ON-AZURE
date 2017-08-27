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
using System.Linq;
using Hl7.Fhir.Serialization;
using FHIRSupport;
using RestSharp;
using System.Text;
using System.Collections.Generic;
using Microsoft.Azure;

namespace FHIRSchedulingBot.Dialogs
{
    [Serializable]
    public class FHIRDialog : IDialog<string>
    {
        private bool isSessionTimedOut(IDialogContext context)
        {
            DateTime now = DateTime.Now;
            DateTime? start = null;
            string patid = null;
            context.PrivateConversationData.TryGetValue<string>("id", out patid);
            context.PrivateConversationData.TryGetValue<DateTime?>("sessionstart", out start);
            bool timedout = false;
            if (start != null)
            {
                timedout = ((now - start.Value).TotalSeconds > 600);
            }
            if (patid == null || timedout) return true;
            return false;
        }
        public async Task StartAsync(IDialogContext context)
        {

            string name = null;
            context.PrivateConversationData.TryGetValue<string>("name", out name);
            if (name == null) name = "";
            await context.PostAsync("Hello " + name + ". Please ask or tell me about your health");
            context.Wait(MessageReceivedAsync);
              
        }
       
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string msg = activity.Text.ToLower().Trim();
            if (msg == "start over" || msg == "exit" || msg == "quit" || msg == "done" || msg == "start again" || msg == "restart" || msg == "leave" || msg == "reset" || msg == "bye" || msg == "goodbye")
            {
                context.Done("end");
            } else { 
                if (isSessionTimedOut(context))
                {
                    context.Fail(new SessionTimedOutException("Your session has timedout or is invalid."));
                }
                else
                {
                    string fhirserver = null;
                    context.PrivateConversationData.TryGetValue<string>("fhirserver", out fhirserver);
                    string token = null;
                    context.PrivateConversationData.TryGetValue<string>("bearertoken", out token);
                    string patid = null;
                    context.PrivateConversationData.TryGetValue<string>("id", out patid);
                    //Initialize FHIR Client
                    FHIRClient client = new FHIRClient(fhirserver,token);
                    //Determine Intent/Entities from LUIS
                    var luisresp = await LUISClient.RequestAsync<LUISResponse>(activity.Text);
                    var intent = luisresp.topScoringIntent;

                    //Nothing over 50% confidence bail
                    if (intent == null)
                    {
                        await context.PostAsync($"Sorry, I didn't understand {activity.Text}...");
                       
                    }

                    if (intent.intent == "FindPractitioner")
                    {
                        if (luisresp.entities == null || luisresp.entities.Count() == 0)
                        {
                            await context.PostAsync($"Sorry, I think you want to find a practioner but I can't determine the location you want");
                           
                        }
                        //Handle Geography
                        var geo = luisresp.entities.FirstOrDefault();
                        //Call FHIR Server
                        string parm = geo.entity.ToLower().StartsWith("anywhere") ? "" : $"city={geo.entity}";
                        var bundle = (Hl7.Fhir.Model.Bundle)client.LoadResource("Practitioner", parm);
                        if (bundle.Entry.Count == 0)
                        {
                            await context.PostAsync($"Sorry, I couldn't find providers in {geo.entity}...");
                            
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append($"Got {bundle.Entry.Count()} practioners in {geo.entity}\n\n");
                            foreach (var entry in bundle.Entry)
                            {

                                Hl7.Fhir.Model.Practitioner p = (Hl7.Fhir.Model.Practitioner)entry.Resource;
                                var address = (p.Address.Count > 0 ? p.Address[0] : null);
                                var tel = (p.Telecom.Count > 0 ? p.Telecom[0] : null);
                                sb.Append($"{p.Name[0].Text} is accepting new patients.");
                                if (address != null) sb.Append($"Located at " + address.LineElement[0] + " " + address.City);
                                if (tel != null) sb.Append($" Phone:" + tel.Value);
                                sb.Append("\n\n");
                            }
                            await context.PostAsync(sb.ToString());
                        }

                    }
                    else if (intent.intent == "ChartHeartRate")
                    {
                        var entity = luisresp.FindEntityByType("builtin.number");
                        if (entity != null)
                        {
                            var hr = Convert.ToDecimal(new String(entity.entity.Where(Char.IsDigit).ToArray()));
                            Hl7.Fhir.Model.Observation rv = new Hl7.Fhir.Model.Observation();
                            rv.Status = Hl7.Fhir.Model.ObservationStatus.Final;
                            rv.Category = new List<Hl7.Fhir.Model.CodeableConcept>();
                            Hl7.Fhir.Model.CodeableConcept cc = new Hl7.Fhir.Model.CodeableConcept("http://hl7.org/fhir/observation-category", "vital-signs", "Vital Signs");
                            rv.Category.Add(cc);
                            rv.Code = new Hl7.Fhir.Model.CodeableConcept("http://loinc.org", "8867-4", "Heart rate");
                            rv.Subject = new Hl7.Fhir.Model.ResourceReference("patient/" + patid);
                            rv.Effective = Hl7.Fhir.Model.FhirDateTime.Now();
                            rv.Value = new Hl7.Fhir.Model.Quantity(hr, "beats/minute");
                            var rslt = client.SaveResource(rv);
                            if (rslt)
                            {
                                await context.PostAsync($"Thanks, I recorded your heart rate of {entity.entity} in your medical record");
                            }
                            else
                            {
                                await context.PostAsync($"Sorry I had a problem posting your heartrate...try again later.");
                            }

                        }
                        else
                        {
                            await context.PostAsync($"Sorry, I think you want to record your heartrate vital but I can't understand the value you provided.");
                        }
                    }
                    else if (intent.intent == "ChartWeight")
                    {
                        string entity_weight = null;
                        var entity = luisresp.FindEntityByType("weight");
                        if (entity == null)
                        {
                            entity = luisresp.FindEntityByType("builtin.number");
                            if (entity != null)
                            {
                                entity_weight = entity.entity;
                                var wt = luisresp.FindEntityByType("builtin.dimension");
                                if (wt != null)
                                {
                                    if (wt.entity.ToLower().EndsWith("kilograms") || wt.entity.ToLower().EndsWith("kg"))
                                    {
                                        entity_weight = entity_weight + "kg";
                                    }
                                    else
                                    {
                                        entity_weight = entity_weight + "lbs";
                                    }
                                }
                            }
                        }
                        else
                        {
                            entity_weight = entity.entity;
                        }
                        if (entity_weight != null)
                        {
                            var weight = Convert.ToDecimal(new String(entity_weight.Where(Char.IsDigit).ToArray()));
                            //Determine pounds or kg
                            if (entity_weight.ToLower().EndsWith("kg"))
                            {
                                weight = weight * (decimal)2.20462;
                            }
                            Hl7.Fhir.Model.Observation rv = new Hl7.Fhir.Model.Observation();
                            rv.Status = Hl7.Fhir.Model.ObservationStatus.Final;
                            rv.Category = new List<Hl7.Fhir.Model.CodeableConcept>();
                            Hl7.Fhir.Model.CodeableConcept cc = new Hl7.Fhir.Model.CodeableConcept("http://hl7.org/fhir/observation-category", "vital-signs", "Vital Signs");
                            rv.Category.Add(cc);
                            rv.Code = new Hl7.Fhir.Model.CodeableConcept("http://loinc.org", "29463-7", "Body Weight");
                            rv.Code.Coding.Add(new Hl7.Fhir.Model.Coding("http://snomed.info/sct", "27113001", "Body Weight"));
                            rv.Subject = new Hl7.Fhir.Model.ResourceReference("patient/" + patid);
                            rv.Effective = Hl7.Fhir.Model.FhirDateTime.Now();
                            rv.Value = new Hl7.Fhir.Model.Quantity(weight, "lbs");
                            ((Hl7.Fhir.Model.Quantity)rv.Value).Code = "[lb_av]";
                            var rslt = client.SaveResource(rv);
                            if (rslt)
                            {
                                string weightmsg = "";
                                string parm = $"patient={patid}";
                                var bundle = (Hl7.Fhir.Model.Bundle)client.LoadResource("Observation", parm);
                                List<Hl7.Fhir.Model.Observation> weights = new List<Hl7.Fhir.Model.Observation>();
                                foreach (Hl7.Fhir.Model.Bundle.EntryComponent entry in bundle.Entry)
                                {
                                    Hl7.Fhir.Model.Observation o = (Hl7.Fhir.Model.Observation)entry.Resource;
                                    if (FHIRUtils.isCodeMatch(o.Code.Coding, "http://loinc.org", "29463-7"))
                                    {
                                        weights.Add(o);
                                    }
                                }
                                if (weights.Count > 1)
                                {
                                    Hl7.Fhir.Model.Observation[] sorted = weights.OrderBy(o => Convert.ToDateTime(o.Effective.ToString())).ToArray();
                                    decimal prevw = ((Hl7.Fhir.Model.SimpleQuantity)sorted[sorted.Length - 2].Value).Value.Value;
                                    decimal curw = ((Hl7.Fhir.Model.SimpleQuantity)sorted[sorted.Length - 1].Value).Value.Value;
                                    decimal dif = prevw - curw;
                                    if (dif < 0)
                                    {
                                        weightmsg = "Looks like you gained around " + Math.Round(Math.Abs(dif), 1) + " lbs.";
                                    }
                                    else if (dif > 0)
                                    {
                                        weightmsg = "Looks like you loss around " + Math.Round(Math.Abs(dif), 1) + " lbs. Keep it up!!";
                                    }
                                    else
                                    {
                                        weightmsg = "Your weight is holding steady";
                                    }
                                }
                                await context.PostAsync($"Thanks, I recorded your weight of {weight} lbs in your medical record\n\n{weightmsg}");
                            }
                            else
                            {
                                await context.PostAsync($"Sorry I had a problem posting your weight...try again later.");
                            }

                        }
                    }
                    else if (intent.intent == "ChartGlucose")
                    {
                        var entity = luisresp.FindEntityByType("builtin.number");
                        if (entity != null)
                        {
                            var glucose = Convert.ToDecimal(new String(entity.entity.Where(Char.IsDigit).ToArray()));
                            var unit = "mg/dL";
                            Hl7.Fhir.Model.CodeableConcept loinc = new Hl7.Fhir.Model.CodeableConcept("http://loinc.org", "2345-7", "Glucose [Mass/​volume] in Serum or Plasma");
                            //convert mg/DL if mmol/l
                            if (glucose < 20)
                            {
                                glucose = glucose * 18;
                            }
                            Hl7.Fhir.Model.Observation rv = new Hl7.Fhir.Model.Observation();
                            rv.Status = Hl7.Fhir.Model.ObservationStatus.Final;
                            rv.Category = new List<Hl7.Fhir.Model.CodeableConcept>();
                            Hl7.Fhir.Model.CodeableConcept cc = new Hl7.Fhir.Model.CodeableConcept("http://hl7.org/fhir/observation-category", "laboratory", "Laboratory");
                            rv.Category.Add(cc);
                            rv.Code = loinc;
                            rv.Subject = new Hl7.Fhir.Model.ResourceReference("patient/" + patid);
                            rv.Effective = Hl7.Fhir.Model.FhirDateTime.Now();
                            rv.Issued = DateTimeOffset.UtcNow;
                            rv.ReferenceRange = new List<Hl7.Fhir.Model.Observation.ReferenceRangeComponent>();
                            Hl7.Fhir.Model.Observation.ReferenceRangeComponent rrc = new Hl7.Fhir.Model.Observation.ReferenceRangeComponent();
                            rrc.High = new Hl7.Fhir.Model.SimpleQuantity();
                            rrc.High.Unit = "mg/dL";
                            rrc.High.Code = rrc.High.Unit;
                            rrc.High.System = "http://unitsofmeasure.org";
                            rrc.High.Value = 140;
                            rrc.Low = new Hl7.Fhir.Model.SimpleQuantity();
                            rrc.Low.Unit = "mg/dL";
                            rrc.Low.Code = rrc.Low.Unit;
                            rrc.Low.System = "http://unitsofmeasure.org";
                            rrc.Low.Value = 70;
                            rv.ReferenceRange.Add(rrc);
                            rv.Value = new Hl7.Fhir.Model.Quantity(glucose, unit);
                            ((Hl7.Fhir.Model.Quantity)rv.Value).Code = unit;
                            var rslt = client.SaveResource(rv);
                            if (rslt)
                            {
                                int numglucose = 0;
                                string parm = $"patient={patid}";
                                var bundle = (Hl7.Fhir.Model.Bundle)client.LoadResource("Observation", parm);
                                decimal bga = 0;
                                foreach (Hl7.Fhir.Model.Bundle.EntryComponent entry in bundle.Entry)
                                {
                                    Hl7.Fhir.Model.Observation o = (Hl7.Fhir.Model.Observation)entry.Resource;
                                    if (FHIRUtils.isCodeMatch(o.Code.Coding, "http://loinc.org", "2345-7"))
                                    {
                                        bga = bga + ((Hl7.Fhir.Model.SimpleQuantity)o.Value).Value.Value;
                                        numglucose++;
                                    }

                                }
                                bga = Math.Round(bga / numglucose, 1);
                                decimal a1c = ((Convert.ToDecimal(46.7) + bga) / Convert.ToDecimal(28.7));
                                string bgmsg = $"Your blood glucose is averaging {bga}  mg/dL " + (bga > 140 ? "looks like it's running high" : (bga < 70 ? "looks like it's running low" : ""));
                                bgmsg = bgmsg + "\n\nYour estimated A1C is " + Convert.ToString(Math.Round(a1c, 1));

                                await context.PostAsync($"Thanks, I recorded your blood glucose of {glucose} mg/dL in your medical record\n\n{bgmsg}");
                            }
                            else
                            {
                                await context.PostAsync($"Sorry I had a problem posting your blood glucose...you can try again later.");
                            }

                        }
                        else
                        {
                            await context.PostAsync($"Sorry, I couldnt determine your blood glucose value...try again");
                        }
                    }
                    else if (intent.intent == "DisplayGlucose")
                    {

                        int numglucose = 0;
                        string parm = $"patient={patid}";
                        var bundle = (Hl7.Fhir.Model.Bundle)client.LoadResource("Observation", parm);
                        decimal bga = 0;
                        decimal la1c = 0;
                        decimal ea1c = 0;
                        DateTime? dta1c = null;
                        foreach (Hl7.Fhir.Model.Bundle.EntryComponent entry in bundle.Entry)
                        {
                            Hl7.Fhir.Model.Observation o = (Hl7.Fhir.Model.Observation)entry.Resource;
                            if (FHIRUtils.isCodeMatch(o.Code.Coding, "http://loinc.org", "2345-7"))
                            {
                                bga = bga + ((Hl7.Fhir.Model.SimpleQuantity)o.Value).Value.Value;
                                numglucose++;
                            }
                            else if (FHIRUtils.isCodeMatch(o.Code.Coding, "http://loinc.org", "4548-4"))
                            {
                                la1c = ((Hl7.Fhir.Model.SimpleQuantity)o.Value).Value.Value;
                                dta1c = Convert.ToDateTime(o.Effective.ToString());
                            }


                        }
                        if (numglucose > 0)
                        {
                            bga = Math.Round(bga / numglucose, 1);
                            ea1c = ((Convert.ToDecimal(46.7) + bga) / Convert.ToDecimal(28.7));
                        }
                        string a1cmsg = "I don't see any A1C tests on file for you.";
                        string bgmsg = "I don't see any blood glucose tests on file for you.";
                        if (la1c > 0)
                        {
                            a1cmsg = $"Your last A1C test was on " + String.Format("{0:MM/dd/yyyy}", dta1c.Value) + " the result was " + Convert.ToString(la1c) + "% " + (la1c > 6 ? "thats high" : "");
                        }
                        if (bga > 0)
                        {
                            bgmsg = $"Your blood glucose is averaging {bga}  mg/dL " + (bga > 140 ? "looks like it's running high" : (bga < 70 ? "looks like it's running low" : ""));
                            bgmsg = bgmsg + "\n\nYour current estimated A1C is " + Convert.ToString(Math.Round(ea1c, 1)) + (ea1c < 6 ? " great control!" : (ea1c <= 7 ? " doing good for a diabetic!" : " need to work at lowering!"));
                        }
                        await context.PostAsync($"{a1cmsg}\n\n{bgmsg}");
                    }
                    else if (intent.intent == "DisplayCareplan")
                    {
                        //Call FHIR Server
                        string parm = $"patient={patid}";
                        var bundle = (Hl7.Fhir.Model.Bundle)client.LoadResource("CarePlan", parm);
                        if (bundle.Entry.Count == 0)
                        {
                            await context.PostAsync($"Sorry, I couldn't find a plan of care for you in the database...");
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Your current plan of care addresses the following conditions:\n\n");
                            Hl7.Fhir.Model.CarePlan cp = (Hl7.Fhir.Model.CarePlan)bundle.Entry[0].Resource;
                            int x = 1;
                            foreach (var cond in cp.Addresses)
                            {
                                sb.Append(x++ + ". " + cond.Display + "\n\n");
                            }
                            sb.Append("\n\nYou have the following goals:\n\n");
                            x = 1;
                            foreach (var goal in cp.Goal)
                            {
                                sb.Append(x++ + ". " + goal.Display + "\n\n");
                            }
                            sb.Append("\n\nYou should:\n\n");
                            x = 1;
                            foreach (var act in cp.Activity)
                            {
                                if (act.Detail != null)
                                {
                                    sb.Append(x++ + ". " + act.Detail.Description + "\n\n");
                                }
                                else
                                {
                                    if (act.Reference != null && act.Reference.Reference.StartsWith("MedicationRequest"))
                                    {
                                        sb.Append(x++ + ". Take " + act.Reference.Display + " as directed.\n\n");
                                    }
                                }

                            }
                            await context.PostAsync(sb.ToString());
                        }
                    }
                    else
                    {
                        await context.PostAsync($"Sorry, I didn't understand {activity.Text}...");
                    }
                }
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}