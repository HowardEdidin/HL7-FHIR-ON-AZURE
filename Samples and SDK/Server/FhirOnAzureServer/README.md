Configuration
============= 


Step 1
------

In the Azure Portal, create a new Cosmos DB Mongo Api as shown below


![](https://i.imgur.com/DBSeA9V.jpg)

Copy the  example code. You will use it in Step 2.

![](https://i.imgur.com/j1ogXTj.jpg)


Next Create a new database named `admin`



Step 2
------


In Spark.Mongo open MongoDatabaseFactory.cs in the Infrastructure folder

Replace `connectionString = @"mongodb://mongofhir:xxxxxxxxxxxx==@xxxxx.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";`

with your Cosmos DB Mongo Api connection string

        private static MongoDatabase CreateMongoDatabase()
        {
            const string connectionString =
                @"mongodb://mongofhir:xxxxxxxxxxxx==@xxxxx.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

            var settings = MongoClientSettings.FromUrl(
                new MongoUrl(connectionString)
            );
            settings.SslSettings =
                new SslSettings {EnabledSslProtocols = SslProtocols.Tls12};
            var mongoClient = new MongoClient(settings);


		#pragma warning disable 618
            return mongoClient.GetServer().GetDatabase("admin");
		#pragma warning restore 618
        }
        
Step 3
------

Open the FhirOnAzure `web.config`

 	<!--Use your Application name -->
    <add key="FHIR_ENDPOINT" value="http://XXXXXXXXX.azurewebsites.net/fhir" />
    <!--Use your Application name -->

Step 4
------
When you build you will get several warning. You can ignore them.
