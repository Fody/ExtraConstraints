using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraConstraints.Fody
{
	public static class Extensions
	{
		public static IEnumerable<T> RecurseTree<T>(this T @this, Func<T, IEnumerable<T>> childSelector, bool includeRoot = true)
		{
			Func<T, IEnumerable<T>> cs = arg => arg == null ? Enumerable.Empty<T>() : (childSelector(arg) ?? Enumerable.Empty<T>());

			var enumerators = new Lazy<Stack<IEnumerator<T>>>();
			var enumerator = cs(@this).GetEnumerator();

			do
			{
				if (enumerator.MoveNext())
				{
					enumerators.Value.Push(enumerator);
					enumerator = cs(enumerator.Current).GetEnumerator();

					continue;
				}

				enumerator.Dispose();
				if (enumerators.Value.Count == 0) break;

				enumerator = enumerators.Value.Pop();
				yield return enumerator.Current;
			} while (true);

			if (includeRoot)
			{
				yield return @this;
			}
		}
	}
}
