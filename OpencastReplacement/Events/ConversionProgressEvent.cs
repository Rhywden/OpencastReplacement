using OpencastReplacement.Models;

namespace OpencastReplacement.Events
{
    public class ConversionProgressEvent
    {
        public async Task Update(ConversionProgressEventArgs args)
        {
            if(Notify is not null)
            {
                await Notify.Invoke(args);
            }
        }

        public event Func<ConversionProgressEventArgs, Task>? Notify;
    }
}
