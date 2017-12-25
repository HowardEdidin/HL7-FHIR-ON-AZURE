/* 
* 2017 Howard Edidin
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

#region Information

// Solution:  Spark
// Spark.Mongo
// File:  MongoDatabaseFactory.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  09/06/2017 : 11:12 AM

#endregion

namespace FhirOnAzure.Store.Mongo
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Authentication;
    using MongoDB.Driver;

    public static class MongoDatabaseFactory
    {
        private static Dictionary<string, MongoDatabase> _instances;

        public static MongoDatabase GetMongoDatabase(string url)
        {
            if (_instances == null) //instances dictionary is not at all initialized
                _instances = new Dictionary<string, MongoDatabase>();
            if (_instances.Any(i => i.Key == url))
                return _instances.First(i => i.Key == url).Value; //now there must be one.
            var result = CreateMongoDatabase();
            _instances.Add(url, result);
            return _instances.First(i => i.Key == url).Value; //now there must be one.
        }

        private static MongoDatabase CreateMongoDatabase()
        {
            const string connectionString =
                @"mongodb://mongofhir:2sqm6y5HN5e4yoPhCRMSUUKxlANMgJPX91L52vstD6PGxr2zlFYBHanq6bkZHtcTipZ1TRvytSxY5ZMriLTjKQ==@mongofhir.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

            var settings = MongoClientSettings.FromUrl(
                new MongoUrl(connectionString)
            );
            settings.SslSettings =
                new SslSettings {EnabledSslProtocols = SslProtocols.Tls12};
            var mongoClient = new MongoClient(settings);

            return mongoClient.GetServer().GetDatabase("admin");

        }
    }
}
