using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace RxExcelLikeApp
{
    public class SumModel
    {
        private readonly IObservable<string> left;
        private readonly IObservable<string> right;

        public SumModel(IObservable<string> left, IObservable<string> right)
        {
            this.left = left;
            this.right = right;
        }

        public IObservable<int> Compute()
        {
            var leftValues = left
                    .Where(leftValue => leftValue.Count() > 0 && leftValue.All(c => char.IsDigit(c)))
                    .Select(leftValue => int.Parse(leftValue));

            var rightValues = right
                    .Where(rightValue => rightValue.Count() > 0 && rightValue.All(c => char.IsDigit(c)))
                    .Select(rightValue => int.Parse(rightValue));
            return 
                leftValues.CombineLatest(rightValues, (leftValue, rightValue) => leftValue + rightValue);
        }
    }
}
