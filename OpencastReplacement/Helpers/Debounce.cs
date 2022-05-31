namespace OpencastReplacement.Helpers
{
    public static class DebounceHelper
    {
        public static Action<T> Debounce<T>(this Action<T> func, int milliSeconds = 500)
        {
            CancellationTokenSource? cancelTokenSource = null;
            return arg =>
            {
                cancelTokenSource?.Cancel();
                cancelTokenSource = new CancellationTokenSource();

                Task.Delay(milliSeconds, cancelTokenSource.Token)
                    .ContinueWith(t =>
                    {
                        if(t.IsCompletedSuccessfully)
                        {
                            func(arg);
                        }
                    }, TaskScheduler.Default);
            };
        }
    }
}
