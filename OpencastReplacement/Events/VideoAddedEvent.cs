namespace OpencastReplacement.Events
{
    public class VideoAddedEvent
    {
        public async Task Update(bool added)
        {
            if(Notify is not null)
            {
                await Notify.Invoke(added);
            }
        }

        public event Func<bool, Task>? Notify;
    }
}
