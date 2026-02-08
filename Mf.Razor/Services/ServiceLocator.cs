using Mf.Razor.Events;
using Mf.Razor.Interfaces;

namespace Mf.Razor.Services
{
    internal abstract class ServiceLocator : IServiceLocator
    {
        internal IServiceProvider _serviceProvider;
        protected ServiceLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private event ServiceLocatorDelegate? Update;
        public int EventNumbers { get; private set; } = 0;
        public virtual Task InvokeUpdate()
        {
            Update?.Invoke();
            return Task.CompletedTask;
        }

        public virtual void AddEvent(ServiceLocatorDelegate input)
        {
            try
            {
                Update += input;
                EventNumbers++;
            }
            catch (Exception ex)
            {

            }

        }

        public virtual void RemoveEvent(ServiceLocatorDelegate input)
        {
            try
            {
                Update -= input;
                EventNumbers--;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
