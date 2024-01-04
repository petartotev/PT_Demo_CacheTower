namespace DemoCacheTower.Models;

public class UserContext
{
    public async Task<UserProfile> GetUserForIdAsync(int id)
    {
        return new UserProfile
        {
            UserId = id,
            UserName = $"username-{id}",
            DateCreatedOrUpdated = DateTime.Now
        };
    }
}
