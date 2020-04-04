using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Torrefactor.Models.Auth;
using Torrefactor.Tests.Integration.Clients;

namespace Torrefactor.Tests.Integration
{
    [TestFixture]
    public class AuthTest : BaseIntegrationTest
    {
        [Test]
        public async Task Should_return_false_if_credentials_are_invalid()
        {
            var client = CreateServer().CreateAuthClient();
            
            var response = await client.SignIn("foo@bar.com", "123");
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
                Assert.AreEqual(false, response.Json.Value<bool>("success"));
            });
        }

        [Test]
        public async Task Should_return_null_if_current_user_is_not_authenticated()
        {
            var client = CreateServer().CreateAuthClient();
            
            var response = await client.GetCurrentUser();

            Assert.AreEqual("", response.Content);
        }

        [Test]
        public async Task Should_not_allow_to_sign_in_if_user_is_not_approved()
        {
            var client = CreateServer().CreateAuthClient();

            var regResult = await client.Register("blah@blah.com", "123", "John Doe");
            var signInResponse = await client.SignIn("blah@blah.com", "123");
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(regResult.StatusCode, HttpStatusCode.OK);
                Assert.AreEqual(false, signInResponse.Json.Value<bool>("success"));
            });
        }

        [Test]
        public async Task Should_allow_admin_to_sign_in_without_approval()
        {
            var client = CreateServer().CreateAuthClient();

            await client.Register("admin@blah.com", "123", "The Admin");
            var signInResponse = await client.SignIn("admin@blah.com", "123");
            
            Assert.AreEqual(true, signInResponse.Json.Value<bool>("success"));
        }

        [Test]
        public async Task Should_not_allow_regular_user_to_confirm_own_account()
        {
            var client = CreateServer().CreateAuthClient();

            await client.Register("blah@blah.com", "123", "The Admin");
            await client.SignIn("blah@blah.com", "123");
            var confirmResponse = await client.ConfirmUser("123");
            
            Assert.AreEqual(HttpStatusCode.Unauthorized, confirmResponse.StatusCode);
        }

        [Test]
        public async Task Should_allow_user_to_login_after_admin_approval()
        {
            var client = CreateServer().CreateAuthClient();
            
            // Step 1: user inits registration
            await client.Register("blah@blah.com", "123", "John Doe");
            
            // Step 2: admin confirms his account
            await client.Register("admin@blah.com", "123", "The Admin");
            await client.SignIn("admin@blah.com", "123");
            var users = (await client.GetNotConfirmedUsers()).Model<UserModel[]>();
            var userId = users.Single().Id;
            await client.ConfirmUser(userId);

            // Step 3: user tries to sign in
            var signInResult = await client.SignIn("blah@blah.com", "123");
            
            Assert.AreEqual(true, signInResult.Json.Value<bool>("success"));
        }
    }
}