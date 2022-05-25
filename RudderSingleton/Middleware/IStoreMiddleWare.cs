namespace RudderSingleton.Middleware
{
    /// <summary>
    /// Provides a custom logic to execute when an action is being processed
    /// </summary>
    public interface IStoreMiddleware
    {
        /// <summary>
        /// Logic to execute when an action is being processed
        /// </summary>
        /// <typeparam name="T">Action type</typeparam>
        /// <param name="action">Action instance</param>
        Task Run<T>(T action);
    }
}
