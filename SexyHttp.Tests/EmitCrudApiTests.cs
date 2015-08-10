using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SexyHttp.HttpHandlers;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class EmitCrudApiTests
    {
        [Test]
        public async void Get()
        {
            using (MockHttpServer.ReturnJson(request => Task.FromResult(JToken.FromObject(new { Id = 1, FirstName = "John", LastName = "Doe" }))))
            {
                var client = HttpApiClient<ICrudApi<User>>.Create("http://localhost:8844", new HttpClientHandler());
                var user = await client.Get(1);
                Assert.AreEqual(1, user.Id);
                Assert.AreEqual("John", user.FirstName);
                Assert.AreEqual("Doe", user.LastName);
            }
        }

        [Test]
        public async void GetAll()
        {
            using (MockHttpServer.ReturnJson(request => Task.FromResult(JToken.FromObject(new[] { new { Id = 1, FirstName = "John", LastName = "Doe" }}))))
            {
                var client = HttpApiClient<ICrudApi<User>>.Create("http://localhost:8844", new HttpClientHandler());
                var users = await client.GetAll();
                Assert.AreEqual(1, users[0].Id);
                Assert.AreEqual("John", users[0].FirstName);
                Assert.AreEqual("Doe", users[0].LastName);
            }
        }

        [Test]
        public async void Post()
        {
            User user = null;
            using (MockHttpServer.Json(request =>
            {
                user = new User { Id = 1, FirstName = "John", LastName = "Doe" };
            }))
            {
                var client = HttpApiClient<ICrudApi<User>>.Create("http://localhost:8844", new HttpClientHandler());
                await client.Post(new User { Id = 1, FirstName = "John", LastName = "Doe" });
                Assert.AreEqual(1, user.Id);
                Assert.AreEqual("John", user.FirstName);
                Assert.AreEqual("Doe", user.LastName);
            }
        }

        [Test]
        public async void Put()
        {
            User user = null;
            using (MockHttpServer.Json(request =>
            {
                user = new User { Id = 1, FirstName = "John", LastName = "Doe" };
            }))
            {
                var client = HttpApiClient<ICrudApi<User>>.Create("http://localhost:8844", new HttpClientHandler());
                await client.Put(1, new User { Id = 1, FirstName = "John", LastName = "Doe" });
                Assert.AreEqual(1, user.Id);
                Assert.AreEqual("John", user.FirstName);
                Assert.AreEqual("Doe", user.LastName);
            }
        }

        [Test]
        public async void Delete()
        {
            int deletedEntityId = 0;
            using (MockHttpServer.Null(request =>
            {
                deletedEntityId = int.Parse(request.Url.ToString().Split('/').Last());
            }))
            {
                var client = HttpApiClient<ICrudApi<User>>.Create("http://localhost:8844", new HttpClientHandler());
                await client.Delete(1);
                Assert.AreEqual(1, deletedEntityId);
            }
        }

        public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public interface ICrudApi<T>
        {
            [Get("{id}")]
            Task<T> Get(int id);

            [Get]
            Task<T[]> GetAll();

            [Post]
            Task Post(T entity);

            [Put("{id}")]
            Task Put(int id, T entity);

            [Delete("{id}")]
            Task Delete(int id);
        }
         
    }
}