using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionLinqExtensions
{
    static class ActionLinqExtensions
    {
        public static Action<Action<R>> Map<T, R>(this Action<Action<T>> observable, Func<T, R> mapper)
        {
            return new Action<Action<R>>(action =>
            {
                observable(t => action(mapper(t)));
            });
        }

        public static Action<Action<T>> Filter<T>(this Action<Action<T>> observable, Func<T, bool> predicate)
        {
            return new Action<Action<T>>(action =>
            {
                observable(t => { if (predicate(t)) { action(t); } });
            });
        }

        public static Action<Action<T>> MergeAll<T>(this IEnumerable<Action<Action<T>>> observables)
        {
            return new Action<Action<T>>(action =>
            {
                foreach (var observable in observables)
                {
                    observable(t => action(t));
                }
            });
        }

        public static Action<Action<T>> Reduce<T>(this Action<Action<T>> observable, Func<T, IEnumerable<T>> many)
        {
            return new Action<Action<T>>(action =>
            {
                observable(t => { foreach (var item in many(t)) { action(item); } });
            });
        }

        public static Action<Action<R>> Zip<T, R>(this Action<Action<T>> firstObservable, Action<Action<T>> secondObservable, Func<T, T, R> mapper)
        {
            return new Action<Action<R>>(action =>
            {
                var firstQueue = new Queue<T>();
                var secondQueue = new Queue<T>();

                Action emitIfBoth = () =>
                {
                    if (firstQueue.Any() && secondQueue.Any()) { action(mapper(firstQueue.Dequeue(), secondQueue.Dequeue())); }
                };

                firstObservable(t =>
                {
                    firstQueue.Enqueue(t);
                    emitIfBoth();
                });

                secondObservable(t =>
                {
                    secondQueue.Enqueue(t);
                    emitIfBoth();
                });
            });
        }
    }
}
