﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace ReactNative.Tests
{
    static class DispatcherHelpers
    {
        public static async Task RunOnDispatcherAsync(Action action)
        {
            Dispatcher dispatcher = Application.Current != null
                ? Application.Current.Dispatcher
                : Dispatcher.CurrentDispatcher;
            if (Thread.CurrentThread == Dispatcher.CurrentDispatcher.Thread)
            {
                dispatcher.Invoke(action);
            }
            else
            {
                await dispatcher.InvokeAsync(action).Task.ConfigureAwait(false);
            }
        }

        public static async Task<T> CallOnDispatcherAsync<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();

            await RunOnDispatcherAsync(() =>
            {
                var result = func();

                Task.Run(() => tcs.SetResult(result));
            }).ConfigureAwait(false);

            return await tcs.Task.ConfigureAwait(false);
        }

        public static async Task CallOnDispatcherAsync(Func<Task> asyncFunc)
        {
            var tcs = new TaskCompletionSource<bool>();

            await RunOnDispatcherAsync(async () =>
            {
                await asyncFunc();
                await Task.Run(() => tcs.SetResult(true));
            }).ConfigureAwait(false);

            await tcs.Task.ConfigureAwait(false);
        }
    }
}
