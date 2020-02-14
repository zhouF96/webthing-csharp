using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.Actions
{
    public abstract class ActionInfo
    {
        protected CancellationTokenSource Source { get; } = new CancellationTokenSource();
        
        internal Guid Id { get; } = Guid.NewGuid();
        internal Thing Thing { get; set; } = default!;
        protected abstract string ActionName { get; }
        
        public string Href { get; internal set; }

        public DateTime TimeRequested { get; } = DateTime.UtcNow;
        public DateTime? TimeCompleted { get; private set; } = null;
        public string Status { get; private set; } = "pending";


        public abstract bool IsValid();
        protected abstract ValueTask InternalExecuteAsync(Thing thing, IServiceProvider provider);
        public async Task ExecuteAsync(Thing thing, IServiceProvider provider)
        {
            var logger = provider.GetRequiredService<ILogger<ActionInfo>>();
            logger.LogInformation("Going to execute {actionName}", ActionName);

            var status = StatusChanged;
            
            Status = "executing";
            
            status?.Invoke(this, EventArgs.Empty);
            
            try
            {
                await InternalExecuteAsync(thing, provider)
                    .ConfigureAwait(false);
                
                logger.LogInformation("{actionName} to executed", ActionName);
            }
            catch (Exception e)
            {
                logger.LogError(e,"Error to execute {actionName}", ActionName);
            }
            
            TimeCompleted = DateTime.UtcNow;
            
            Status = "completed";
            
            status?.Invoke(this, EventArgs.Empty);

        }

        internal string GetActionName() => ActionName;

        public void Cancel()
            => Source.Cancel();

        public event EventHandler? StatusChanged;
    }
}
