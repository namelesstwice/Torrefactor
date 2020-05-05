using AspNetCore.Identity.Mongo.Model;

namespace Torrefactor.Entities.Auth
{
    public class ApplicationUser : MongoUser
    {
        public string? DisplayName { get; set; }
    }
}