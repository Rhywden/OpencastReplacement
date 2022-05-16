using OpencastReplacement.Models;

namespace OpencastReplacement.Events
{
    public class VideoAddedEvent
    {
        public async Task Update(Video? video)
        {
            if(Notify is not null)
            {
                await Notify.Invoke(video);
            }
        }

        public event Func<Video?, Task>? Notify;
    }
}
