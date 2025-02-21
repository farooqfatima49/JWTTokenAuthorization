namespace Models
{
    public class AppUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public AppUser()
        {
            Id = Guid.NewGuid().ToString();
            Name = "DummyUser";
            Email = "DummyUser@gmail.com";
            Password = string.Empty;
        }
    }
}
