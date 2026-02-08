
using Mf.Razor.Entity.Interfaces;
using Mf.Razor.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mf.Razor.Services
{
    public class RequestService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1); // دو درخواست همزمان

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //ServiceLocator.Log($"Error Limiter : {ex.Message}");
                return await action();
            }
            finally
            {
                //ServiceLocator.Log($"Relase : {DateTime.Now.ToString()}");
                _semaphore.Release();
            }

        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            await _semaphore.WaitAsync();
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //ServiceLocator.Log($"Error Limiter : {ex.Message}");
                await action();
            }
            finally
            {
                //ServiceLocator.Log($"Relase : {DateTime.Now.ToString()}");
                _semaphore.Release();
            }
        }
    }
}
