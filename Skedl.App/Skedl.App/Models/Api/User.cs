using CommunityToolkit.Mvvm.ComponentModel;

namespace Skedl.App.Models.Api
{
    public partial class User : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? GroupId { get; set; }

        [ObservableProperty]
        private Group group;

        public string AvatarName { get; set; }
        public byte[] Avatar { get; set; }
        public string University { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
