using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionLinqExtensions
{
    class Program
    {
        static void Main(string[] args)
        {
            //simplified version of Rx based on Action(s) -> relates to observer pattern
            //host action is the functional equivalent of IObservable
            //second action is the function equivalent of IObserver
            Action<Action<int>> observable = new Action<Action<int>>(observer =>
            {
                observer(5);
                observer(6);
                observer(7);
                observer(8);
            });

            Action<Action<string>> stringsObservable = observable.Map(t => "Hello " + t.ToString());
            Action<Action<int>> oddNumbersObservable = observable.Filter(t => t % 2 == 1);
            Action<Action<int>> evenNumbersObservable = observable.Filter(t => t % 2 == 0);

            Console.WriteLine("\r\nEmit strings:");
            stringsObservable(v => Console.WriteLine("String: {0}", v));

            Console.WriteLine("\r\nEmit odd numbers: ");
            oddNumbersObservable(v => Console.WriteLine("Odd: {0}", v));

            Console.WriteLine("\r\nEmit even numbers: ");
            evenNumbersObservable(v => Console.WriteLine("Even: {0}", v));

            Console.WriteLine("\r\nEmit odd and even merged: ");
            (new Action<Action<int>>[2] { oddNumbersObservable, evenNumbersObservable })
                .MergeAll()(v => Console.WriteLine("All: {0}", v));

            Console.WriteLine("\r\nEmit reduced numbers: ");
            observable.Reduce(v => new int[2] { v, v + 1 })(v => Console.WriteLine("Reduced: {0}", v));

            Console.WriteLine("\r\nEmit zipped sum odd and even numbers: ");
            oddNumbersObservable.Zip(evenNumbersObservable, (first, second) => first + second)
                (v => Console.WriteLine("Odd + Even: {0}", v));
        }
    }
}
