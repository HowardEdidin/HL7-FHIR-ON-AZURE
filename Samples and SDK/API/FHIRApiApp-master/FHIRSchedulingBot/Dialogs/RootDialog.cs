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

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace FHIRSchedulingBot.Dialogs
{   [Serializable]
    public class RootDialog : IDialog<object>
    {
        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome to the FHIR bot!");
            context.Call(new IdentifyDialog(), this.IdentityDialogComplete);
        }
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
            return Task.CompletedTask;
        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            /* When MessageReceivedAsync is called, it's passed an IAwaitable<IMessageActivity>. To get the message,
             *  await the result. */
            var message = await result;

            await this.SendWelcomeMessageAsync(context);
        }
        private async Task IdentityDialogComplete(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var message = await result;
                if (String.IsNullOrEmpty(message))
                {
                    await this.SendWelcomeMessageAsync(context);
                }
                else
                {
                    context.Call(new FHIRDialog(), FHIRDialogComplete);
                }
            }
            catch (InvalidPINException)
            {
                await context.PostAsync("I'm sorry, you need to provide a valid PIN code to continue");
                await this.SendWelcomeMessageAsync(context);
            }


        }
        private async Task FHIRDialogComplete(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var message = await result;
                await context.PostAsync($"Ending your session...Dont' forget to delete your conversation for privacy!");
            }
            catch (SessionTimedOutException)
            {
                await context.PostAsync("Your session has timed out!");
            }
            finally
            {
                context.PrivateConversationData.Clear();
                await this.SendWelcomeMessageAsync(context);
            }
        }

    }
}