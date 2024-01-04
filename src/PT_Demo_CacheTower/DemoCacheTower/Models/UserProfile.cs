using ProtoBuf;

namespace DemoCacheTower.Models;

[ProtoContract]
public class UserProfile
{
    [ProtoMember(1)]
    public int UserId { get; set; }
    [ProtoMember(2)]
    public string UserName { get; set; }
    [ProtoMember(3)]
    public DateTime DateCreatedOrUpdated { get; set; }
}