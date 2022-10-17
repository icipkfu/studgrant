namespace Grant.Services.DomainService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Entities;
    using Core.Enum;

    public interface IGrantEventService :  IDomainService<GrantEvent>
    {
        Task CreateEvent(Grant grant, string title, string subtitle, EventType eventType);

        Task CreateEvent(Grant grant, string title, string subtitle, EventType eventType, GrantQuota[] changes);

        Task CreateEvent(Grant grant, string title, string subtitle, EventType eventType, Grant oldGrant);

        Task CreateEvent(Grant grant, string title, string subtitle, string image, string content, string palette, string classes);

        Task<IQueryable<GrantEvent>> GetGrantEvents(EventFilter filter, List<EventType> eventTypes = null);

        Task<IQueryable<GrantEvent>> GetGrantEvents(long id, EventFilter filter, List<EventType> eventTypes = null);

        Task CreateEvent(Grant grant, string title, string subtitle, EventType eventType, Achievement oldAchievement, Achievement newAchievement);
    }
}
