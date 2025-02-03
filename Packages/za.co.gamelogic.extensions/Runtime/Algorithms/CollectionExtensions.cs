using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Gamelogic.Extensions.Internal;
using JetBrains.Annotations;
using UnityEngine;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// This class provides useful extension methods for collections, mostly IEnumerable.
	/// </summary>
	[Version(1, 0, 0)]
	public static class CollectionExtensions
	{
		#region Types
		private class CountableEnumerable<T> : IReadOnlyCollection<T>
		{
			private readonly IEnumerable<T> source;
			
			public int Count => source.Count();
			
			public CountableEnumerable(IEnumerable<T> source)
			{
				this.source = source;
			}

			[Support.MustDisposeResource] // ReSharper disable once NotDisposedResourceIsReturned
			public IEnumerator<T> GetEnumerator() => source.GetEnumerator();
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
		}
		#endregion
		
		#region Static Methods

		/// <summary>
		/// Returns all elements of the source which are of FilterType.
		/// </summary>
		public static IEnumerable<TFilter> FilterByType<T, TFilter>(this IEnumerable<T> source)
			where T : class
			where TFilter : class, T
		{
			return source.Where(item => item is TFilter).Cast<TFilter>();
		}

		/// <summary>
		/// Removes all the elements in the list that does not satisfy the predicate.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="source">The list to remove elements from.</param>
		/// <param name="predicate">The predicate used to filter elements. 
		/// All elements that don't satisfy the predicate will be matched.</param>
		public static void RemoveAllBut<T>(this List<T> source, Predicate<T> predicate)
		{
			source.RemoveAll(Inverse);
			return;

			bool Inverse(T item) => !predicate(item);
		}

		/// <summary>
		/// Returns whether this source is empty.
		/// </summary>
		public static bool IsEmpty<T>(this ICollection<T> collection) => collection.Count == 0;

		/// <summary>
		/// Add all elements of other to the given source.
		/// </summary>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> other)
		{
			if (other == null)//nothing to add
			{
				return;
			}

			foreach (var obj in other)
			{
				collection.Add(obj);
			}
		}

		/// <summary>
		/// Returns a pretty string representation of the given list. The resulting string looks something like
		/// <c>[a, b, c]</c>.
		/// </summary>
		public static string ListToString<T>(this IEnumerable<T> source)
		{
			if (source == null)
			{
				return "null";
			}
			
			var countableSource = source.AsCountable();
			
			if (!countableSource.Any())
			{
				return "[]";
			}

			if (countableSource.Count == 1)
			{
				return "[" + countableSource.First() + "]";
			}

			var s = "";

			s += countableSource.ButFirst().Aggregate(s, (res, x) => res + ", " + x.ListToString());
			s = "[" + countableSource.First().ListToString() + s + "]";

			return s;
		}


		[Version(1, 4, 0)]
		// TODO: Implement this method correctly
		private static string ListToString(this object obj) => obj as string;
		
		/// <summary>
		/// Returns an enumerable of all elements of the given list	but the first,
		/// keeping them in order.
		/// </summary>
		public static IEnumerable<T> ButFirst<T>(this IEnumerable<T> source) => source.Skip(1);
		
		/// <summary>
		/// Returns an enumerable of all elements in the given 
		/// list but the last, keeping them in order.
		/// </summary>
		public static IEnumerable<T> ButLast<T>(this IEnumerable<T> source)
		{
			var lastX = default(T);
			var first = true;

			foreach (var x in source)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					yield return lastX;
				}

				lastX = x;
			}
		}

		/// <summary>
		/// Finds the minimum element in the source as scored by its projection.
		/// </summary>
		public static TSource MinBy<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> selector)
		{
			return source.MinBy(selector, Comparer<TKey>.Default);
		}

		/// <summary>
		/// Finds the minimum element in the source as scored by the given function applied to a projection on the elements.
		/// </summary>
		public static TSource MinBy<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> selector, 
			IComparer<TKey> comparer)
		{
			source.ThrowIfNull("source");
			selector.ThrowIfNull("selector");
			comparer.ThrowIfNull("comparer");
			
			using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
			{
				if (!sourceIterator.MoveNext())
				{
					throw new InvalidOperationException("Sequence was empty");
				}

				var min = sourceIterator.Current;
				var minKey = selector(min);
				
				while (sourceIterator.MoveNext())
				{
					var candidate = sourceIterator.Current;
					var candidateProjected = selector(candidate);

					if (comparer.Compare(candidateProjected, minKey) < 0)
					{
						min = candidate;
						minKey = candidateProjected;
					}
				}
				return min;
			}
		}

		/// <summary>
		/// Finds the minimum element in the source as scored by its projection.
		/// </summary>
		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
		{
			return source.MaxBy(selector, Comparer<TKey>.Default);
		}

		/// <summary>
		/// Finds the minimum element in the source as scored by the given function applied to a projection on the elements.
		/// </summary>

		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			source.ThrowIfNull("source");
			selector.ThrowIfNull("selector");
			comparer.ThrowIfNull("comparer");

			using (var sourceIterator = source.GetEnumerator())
			{
				if (!sourceIterator.MoveNext())
				{
					throw new InvalidOperationException("Sequence was empty");
				}

				var max = sourceIterator.Current;
				var maxKey = selector(max);

				while (sourceIterator.MoveNext())
				{
					var candidate = sourceIterator.Current;
					var candidateProjected = selector(candidate);
					
					if (comparer.Compare(candidateProjected, maxKey) > 0)
					{
						max = candidate;
						maxKey = candidateProjected;
					}
				}

				return max;
			}
		}

		/// <summary>
		/// Returns an enumerable with elements in order, but the first element is moved to the end.
		/// </summary>
		public static IEnumerable<T> RotateLeft<T>(this IEnumerable<T> source)
		{
			var enumeratedList = source as IList<T> ?? source.ToList();
			return enumeratedList.ButFirst().Concat(enumeratedList.Take(1));
		}

		/// <summary>
		/// Returns an enumerable with elements in order, but the last element is moved to the front.
		/// </summary>
		public static IEnumerable<T> RotateRight<T>(this IEnumerable<T> source)
		{
			var enumeratedList = source as IList<T> ?? source.ToList();
			yield return enumeratedList.Last();

			foreach (var item in enumeratedList.ButLast())
			{
				yield return item;
			}
		}

		/// <summary>
		/// Returns a random element from a source.
		/// </summary>
		/// <typeparam name="T">The type of items generated from the source.</typeparam>
		/// <param name="source">The list.</param>
		/// <returns>An item randomly selected from the source.</returns>
		public static T RandomItem<T>(this IEnumerable<T> source)
		{
			return RandomItem(source, GLRandom.GlobalRandom);
		}

		/// <summary>
		/// Returns a random element from a source.
		/// </summary>
		/// <typeparam name="T">The type of items generated from the source.</typeparam>
		/// <param name="source">The list.</param>
		/// <param name="random">The random generator to use.</param>
		/// <returns>An item randomly selected from the source.</returns>
		public static T RandomItem<T>(this IEnumerable<T> source, IRandom random)
		{
			return source.SampleRandom(1, random).First();
		}

		/// <summary>
		/// Returns a random sample from a source.
		/// </summary>
		/// <typeparam name="T">The type of elements of the source.</typeparam>
		/// <param name="source">The source from which to sample.</param>
		/// <param name="sampleCount">The number of samples to return.</param>
		/// <returns>Generates a ransom subset from a given source.</returns>
		public static IEnumerable<T> SampleRandom<T>(this IEnumerable<T> source, int sampleCount)
		{
			return SampleRandom(source, sampleCount, GLRandom.GlobalRandom);
		}

		/// <summary>
		/// Returns a random sample from a source.
		/// </summary>
		/// <typeparam name="T">The type of elements of the source.</typeparam>
		/// <param name="source">The source from which to sample.</param>
		/// <param name="sampleCount">The number of samples to return.</param>
		/// <param name="random">The random generator to use.</param>
		/// <returns>Generates a ransom subset from a given source.</returns>
		public static IEnumerable<T> SampleRandom<T>(
			this IEnumerable<T> source, 
			int sampleCount, 
			IRandom random)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			if (sampleCount < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(sampleCount));
			}

			/* Reservoir sampling. */
			var samples = new List<T>();

			//Must be 1, otherwise we have to use Range(0, i + 1)
			var i = 1;

			foreach (var item in source)
			{
				if (i <= sampleCount)
				{
					samples.Add(item);
				}
				else
				{
					// Randomly replace elements in the reservoir with a decreasing probability.
					var r = random.Next(i);

					if (r < sampleCount)
					{
						samples[r] = item;
					}
				}

				i++;
			}

			return samples;
		}

		/// <summary>
		/// Shuffles a list.
		/// </summary>
		/// <typeparam name="T">The type of items in the list.</typeparam>
		/// <param name="list">The list to shuffle.</param>
		public static void Shuffle<T>(this IList<T> list)
		{
			list.Shuffle(GLRandom.GlobalRandom);
		}

		/// <summary>
		/// Shuffles a list.
		/// </summary>
		/// <typeparam name="T">The type of items in the list.</typeparam>
		/// <param name="list">The list to shuffle.</param>
		/// <param name="random">The random generator to use.</param>
		public static void Shuffle<T>(this IList<T> list, IRandom random)  
		{  
			var n = list.Count;  
			
			while (n > 1) 
			{  
				n--;
				int k = random.Next(0, n + 1);  
				(list[k], list[n]) = (list[n], list[k]);
			}  
		}
		
		/// <summary>
		/// Gives sliding window of the given size over the source.
		/// </summary>
		/// <param name="source">The source to slide over.</param>
		/// <param name="windowSize">The size of the window. Must be positive.</param>
		/// <typeparam name="T">The type of elements in the source.</typeparam>
		/// <returns>An <see cref="IEnumerable{T}"/> of buffers of the given size. Each item represents a window at a
		/// certain position. Only full windows are returned. Suppose the source is the sequence 1, 2, 3, 4, 5. If we run
		/// the sliding window width a size of three, we get the sequences (1, 2, 3), (2, 3, 4), (3, 4, 5).</returns>
		/// <exception cref="ArgumentOutOfRangeException">The <paramref name="windowSize"/> is not positive.</exception>
		public static IEnumerable<IReadOnlyBuffer<T>> SlidingWindow<T>(this IEnumerable<T> source, int windowSize)
		{
			windowSize.ThrowIfNotPositive(nameof(windowSize));
			var buffer = windowSize == 2 ? new Capacity2Buffer<T>() : (IBuffer<T>)new RingBuffer<T>(windowSize);
			
			foreach (var item in source)
			{
				buffer.Insert(item);
				
				if(buffer.IsFull)
				{
					yield return buffer;
				}
			}
		}
		
		/// <summary>
		/// Fills the list with the given value. 
		/// </summary>
		/// <param name="list">The list to fill.</param>
		/// <param name="value">The value to fill the list with.</param>
		/// <typeparam name="T">The type of items in the list.</typeparam>
		// TODO: Should we provide ovcerloads that take a Func, or indices?
		[Version(4, 0, 0)]
		public static void Fill<T>(this IList<T> list, T value)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = value;
			}
		}

		/// <summary>
		/// Fills the list with the default value of the type.
		/// </summary>
		/// <param name="list">The list to fill.</param>
		/// <typeparam name="T">The type of items in the list.</typeparam>
		[Version(4, 0, 0)]
		public static void FillWithDefault<T>(this IList<T> list) => Fill(list, default);
		
		/// <summary>
		/// Returns the first half of elements from a source.
		/// </summary>
		[Version(1, 2, 0)]
		public static IEnumerable<T> TakeHalf<T>(this IEnumerable<T> source)
		{
			source.ThrowIfNull("source");
			
			var countableSource = source.AsCountable();
			int count = countableSource.Count;
			return countableSource.Take(count/2);
		}

		/// <summary>
		/// Returns the last n elements from a source.
		/// </summary>
		[Version(1, 2, 0)]
		public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
		{
			source.ThrowIfNull("source");
			var countableSource = source.AsCountable();
			int count = countableSource.Count;

			return count <= n ? countableSource : countableSource.Skip(count - n);
		}

		/// <summary>
		/// Find an element in a collection by binary searching. 
		/// This requires the collection to be sorted on the values returned by getSubElement
		/// This will compare some derived property of the elements in the collection, rather than the elements
		/// themselves.
		/// </summary>
		public static int BinarySearch<TCollection, TElement>(
			this ICollection<TCollection> source, 
			TElement value, Func<TCollection, TElement> getSubElement) 
			=> BinarySearch(source, value, getSubElement, 0, source.Count, null);

		/// <summary>
		/// Find an element in a collection by binary searching. 
		/// This requires the collection to be sorted on the values returned by getSubElement
		/// This will compare some derived property of the elements in the collection, rather than the elements
		/// themselves.
		/// </summary>
		public static int BinarySearch<TCollection, TElement>(
			this ICollection<TCollection> source, 
			TElement value, 
			Func<TCollection, TElement> getSubElement, 
			IComparer<TElement> comparer)
			=> BinarySearch(source, value, getSubElement, 0, source.Count, comparer);

		/// <summary>
		/// Find an element in a collection by binary searching. 
		/// This requires the collection to be sorted on the values returned by getSubElement
		/// This will compare some derived property of the elements in the collection, rather than the elements
		/// themselves.
		/// </summary>
		public static int BinarySearch<TCollection, TElement>(
			this ICollection<TCollection> source, 
			TElement value, 
			Func<TCollection, TElement> getSubElement,
			int index, 
			int length)
			=> BinarySearch(source, value, getSubElement, index, length, null);

		/// <summary>
		/// Find an element in a collection by binary searching. 
		/// This requires the collection to be sorted on the values returned by getSubElement
		/// This will compare some derived property of the elements in the collection, rather than the elements
		/// themselves.
		/// </summary>
		public static int BinarySearch<TCollection, TElement>(
			this ICollection<TCollection> source, 
			TElement value, 
			Func<TCollection, TElement> getSubElement,
			int index, 
			int length,
			IComparer<TElement> comparer)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index), 
					"index is less than the lower bound of array.");
			}

			if (length < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(length), 
					"Value has to be >= 0.");
			}

			// re-ordered to avoid possible integer overflow
			if (index > source.Count - length)
			{
				throw new ArgumentException(
					"index and length do not specify a valid range in array.");
			}
			
			// ReSharper disable once ConvertIfStatementToNullCoalescingAssignment Thanks Unity
			if (comparer == null)
			{
				comparer = Comparer<TElement>.Default;
			}

			int min = index;
			int max = index + length - 1;

			while (min <= max)
			{
				int mid = (min + ((max - min) >> 1));
				int cmp = comparer.Compare(getSubElement(source.ElementAt(mid)), value);

				if (cmp == 0)
				{
					return mid;
				}

				if (cmp > 0)
				{
					max = mid - 1;
				}
				else
				{
					min = mid + 1;
				}
			}

			return ~min;
		}

		/// <summary>
		/// Checks whether the sequences are equal.
		/// </summary>
		/// <returns><c>true</c> if the number of elements in the sequences are equal, 
		/// and all the elements compared item by item are equal (using the CompareTo method), 
		/// <c>false</c> otherwise.</returns>
		[Version(1, 3, 0)]
		public static bool AreSequencesEqual<T>(IEnumerable<T> s1, IEnumerable<T> s2)
			where T:IComparable
		{
			if (s1 == null)
			{
				throw new NullReferenceException("s1");
			}

			if (s2 == null)
			{
				throw new NullReferenceException("s2");
			}

			var list1 = s1.ToList();
			var list2 = s2.ToList();

			if (list1.Count != list2.Count)
			{
				return false;
			}

			return !list1.Where((t, i) => t.CompareTo(list2[i]) != 0).Any();
		}
		
		/// <summary>
		/// Try to get the first element of a sequence, if it exists.
		/// </summary>
		/// <param name="source">The source to get the first element from.</param>
		/// <param name="first">The first element of the source, if it exists.</param>
		/// <typeparam name="T">The type of elements in the source.</typeparam>
		/// <returns><see langword="true"/> if the source is not empty, <see langword="false"/> otherwise.</returns>
		[Version(3, 0, 0)]
		public static bool TryFirst<T>(this IEnumerable<T> source, out T first)
		{
			source.ThrowIfNull("source");
			var countableSource = source.AsCountable();
			first = countableSource.FirstOrDefault();
			return countableSource.Any();
		}
		
		/// <summary>
		/// Try to get the last element of a sequence, if it exists.
		/// </summary>
		/// <param name="source">The source to get the last element from.</param>
		/// <param name="last">The last element of the source, if it exists.</param>
		/// <typeparam name="T">The type of elements in the source.</typeparam>
		/// <returns><see langword="true"/> if the source is not empty, <see langword="false"/> otherwise.</returns>
		[Version(3, 0, 0)]
		public static bool TryLast<T>(this IEnumerable<T> source, out T last)
		{
			source.ThrowIfNull("source");
			var countableSource = source.AsCountable();
			last = countableSource.LastOrDefault();
			return countableSource.Any();
		}
		
		/// <summary>
		/// Try to get the first element of a sequence that satisfies a predicate, if it exists.
		/// </summary>
		/// <param name="source">The source to get the first element from.</param>
		/// <param name="predicate">The predicate to test the elements against.</param>
		/// <param name="first">The first element of the source that satisfies the predicate, if it exists.</param>
		/// <typeparam name="T">The type of elements in the source.</typeparam>
		/// <returns><see langword="true"/> if the source is not empty, <see langword="false"/> otherwise.</returns>
		[Version(3, 0, 0)]
		public static bool TryFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T first)
		{
			source.ThrowIfNull("source");
			var countableSource = source.AsCountable();
			first = countableSource.FirstOrDefault(predicate);
			return countableSource.Any(predicate);
		}
		
		/// <summary>
		/// Try to get the last element of a sequence that satisfies a predicate, if it exists.
		/// </summary>
		/// <param name="source">The source to get the last element from.</param>
		/// <param name="predicate">The predicate to test the elements against.</param>
		/// <param name="last">The last element of the source that satisfies the predicate, if it exists.</param>
		/// <typeparam name="T">The type of elements in the source.</typeparam>
		/// <returns><see langword="true"/> if the source is not empty, <see langword="false"/> otherwise.</returns>
		[Version(3, 0, 0)]
		public static bool TryLast<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T last)
		{
			source.ThrowIfNull("source");
			var countableSource = source.AsCountable();
			last = countableSource.LastOrDefault(predicate);
			return countableSource.Any();
		}
		
		/// <summary>
		/// Converts the source to an <see cref="IReadOnlyCollection{T}"/>.
		/// </summary>
		/// <param name="source">The source to convert.</param>
		/// <typeparam name="T">The type of elements in the source.</typeparam>
		/// <returns>The source as an <see cref="IReadOnlyCollection{T}"/>.</returns>
		/// <remarks>Enumerate <see cref="IEnumerable{T}"/> using this to get a better solution for when you need only
		/// a count in addition to enumeration. If the source is already an <see cref="IReadOnlyCollection{T}"/>, it is
		/// casted, otherwise it is converted to an <see cref="IReadOnlyCollection{T}"/>. The resulting collection is
		/// enumerates multiple times, so if you want to avoid this e numerate the collection to something else.
		/// </remarks>
		[Version(3, 0, 0)]
		// TODO: Should be as collection
		public static IReadOnlyCollection<T> AsCountable<T>(this IEnumerable<T> source)
			=> source as IReadOnlyCollection<T> ?? new CountableEnumerable<T>(source);
		
		/// <summary>
		/// Converts the source to an <see cref="IReadOnlyList{T}"/>.
		/// </summary>
		/// <param name="source">The source to convert.</param>
		/// <typeparam name="T">The type of elements in the source.</typeparam>
		/// <returns>The source as an <see cref="IReadOnlyList{T}"/>.</returns>
		/// <remarks> If the source is already an <see cref="IList{T}"/>, it is
		/// casted, otherwise it is converted to an <see cref="List{T}"/> using <see cref="Enumerable.ToList{TSource}"/>.
		/// </remarks>
		[Version(3, 0, 0)]
		public static IReadOnlyList<T> AsList<T>(this IEnumerable<T> source)
			=> source as IReadOnlyList<T> ?? source.ToList();
		
		/// <summary>
		/// Checks if two lists have the same elements, regardless of their order.
		/// </summary>
		/// <typeparam name="T">The type of elements in the lists.</typeparam>
		/// <param name="first">The first collection to compare.</param>
		/// <param name="second">The second collection to compare.</param>
		/// <returns>True if both collections contain the same elements, false otherwise.</returns>
		[Version(3, 0, 0)]
		public static bool HasSameElementsAs<T>(this IEnumerable<T> first, IEnumerable<T> second)
		{
			var set = new HashSet<T>(first);
			return set.SetEquals(second);
		}
		
		/// <summary>
		/// Aggregates the source collection using two different aggregation functions.
		/// </summary>
		/// <typeparam name="T">The type of elements in the source collection.</typeparam>
		/// <param name="source">The source collection to aggregate.</param>
		/// <param name="aggregator1">The first aggregation function.</param>
		/// <param name="aggregator2">The second aggregation function.</param>
		/// <returns>A tuple containing the results of the two aggregation functions.</returns>
		// TODO: Implement a more efficient version of this
		[Version(3, 0, 0)]
		public static (T, T) Aggregate<T>(
			this IEnumerable<T> source, 
			Func<T, T, T> aggregator1, 
			Func<T, T, T> aggregator2)
		{
			return source.Select( item => (item, item)).Aggregate(Aggregator);

			(T, T) Aggregator((T, T) x, (T, T) y) => (aggregator1(x.Item1, y.Item1), aggregator2(x.Item2, y.Item2));
		}

		/// <summary>
		/// Finds the minimum and maximum values in a collection of floats.
		/// </summary>
		/// <param name="source">The source collection to find the minimum and maximum values in.</param>
		/// <returns>A tuple containing the minimum and maximum values.</returns>
		[Version(3, 0, 0)]
		public static (float, float) MinMax(this IEnumerable<float> source) 
			=> source.Aggregate(Mathf.Min, Mathf.Max);

		/// <summary>
		/// Finds the minimum and maximum values in a collection of integers.
		/// </summary>
		/// <param name="source">The source collection to find the minimum and maximum values in.</param>
		/// <returns>A tuple containing the minimum and maximum values.</returns>
		[Version(3, 0, 0)]
		public static (int, int) MinMax(this IEnumerable<int> source)
			=> source.Aggregate(Mathf.Min, Mathf.Max);

		/// <summary>
		/// Iterates over a collection, yielding each element along with its index.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="source">The source collection to iterate over.</param>
		/// <returns>An enumerable of tuples containing the elements and their indices.</returns>
		[Version(3, 2, 0)]
		public static IEnumerable<(T item, int index)> WithIndices<T>(this IEnumerable<T> source)
		{
			int index = 0;
		
			foreach (var item in source)
			{
				yield return (item, index);
				index++;
			}
		}
		
		/// <summary>
		/// Determines whether the source collection contains exactly one element.
		/// </summary>
		/// <typeparam name="T">The type of elements in the source collection.</typeparam>
		/// <param name="source">The source collection to check.</param>
		/// <returns>True if the source collection contains exactly one element, false otherwise.</returns>
		[Version(3, 2, 0)]
		public static bool HasSingle<T>(this IEnumerable<T> source) => source.Count() == 1;

		/// <summary>
		/// Tries to retrieve the single element from the source collection.
		/// </summary>
		/// <typeparam name="T">The type of elements in the source collection.</typeparam>
		/// <param name="source">The source collection to retrieve the element from.</param>
		/// <param name="result">The single element from the source collection, if it exists.</param>
		/// <returns>True if the source collection contains exactly one element, false otherwise. If true, the single element is assigned to the out parameter 'result'.</returns>
		[Version(3, 2, 0)]
		public static bool TrySingle<T>(this IEnumerable<T> source, out T result)
		{
			result = default;

			using (var enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					return false;
				}

				result = enumerator.Current;

				if (!enumerator.MoveNext())
				{
					return true;
				}

				result = default;
				return false;
			}
		}

		/// <summary>
		/// Returns the maximum elements in the source collection based on a selector function.
		/// This overload uses the default comparer for the type TComparable.
		/// </summary>
		/// <typeparam name="T">The type of elements in the source collection.</typeparam>
		/// <typeparam name="TComparable">The type of the value returned by the selector function.</typeparam>
		/// <param name="source">The source collection.</param>
		/// <param name="selector">A function to extract a comparable value from each element.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> that contains the maximum elements in the source collection.</returns>
		[Version(3, 2, 0)]
		public static IEnumerable<T> MaxItemsBy<T, TComparable>(
			[NotNull]this IEnumerable<T> source,
			[NotNull] Func<T, TComparable> selector)
			where TComparable : IComparable<TComparable>
		{
			source.ThrowIfNull(nameof(source));
			
			return source.MaxItemsBy(selector, Comparer<TComparable>.Default);
		}

		/// <summary>
		/// Returns the maximum elements in the source collection based on a selector function and a specified comparer.
		/// </summary>
		/// <typeparam name="T">The type of elements in the source collection.</typeparam>
		/// <typeparam name="TComparable">The type of the value returned by the selector function.</typeparam>
		/// <param name="source">The source collection.</param>
		/// <param name="selector">A function to extract a comparable value from each element.</param>
		/// <param name="comparer">The comparer to use when comparing elements.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> that contains the maximum elements in the source collection.</returns>
		[Version(3, 2, 0)]
		public static IEnumerable<T> MaxItemsBy<T, TComparable>(
			[NotNull] this IEnumerable<T> source, 
			[NotNull] Func<T, TComparable> selector,
			[NotNull] Comparer<TComparable> comparer)
			where TComparable : IComparable<TComparable>
		{
			source.ThrowIfNull(nameof(source));
			selector.ThrowIfNull(nameof(selector));
			comparer.ThrowIfNull(nameof(comparer));
			
			using (var enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					return Enumerable.Empty<T>();
				}

				var max = selector(enumerator.Current);
				var maxList = new List<T> { enumerator.Current };

				while (enumerator.MoveNext())
				{
					var value = selector(enumerator.Current);

					if (comparer.Compare(value, max) > 0)
					{
						max = value;
						maxList.Clear();
						maxList.Add(enumerator.Current);
					}
					else if (comparer.Compare(value, max) == 0)
					{
						maxList.Add(enumerator.Current);
					}
				}

				return maxList;
			}
		}
		
		/// <summary>
			/// Returns the minimum elements in the source collection based on a selector function.
			/// This overload uses the default comparer for the type TComparable.
			/// </summary>
			/// <typeparam name="T">The type of elements in the source collection.</typeparam>
			/// <typeparam name="TComparable">The type of the value returned by the selector function.</typeparam>
			/// <param name="source">The source collection.</param>
			/// <param name="selector">A function to extract a comparable value from each element.</param>
			/// <returns>An <see cref="IEnumerable{T}"/> that contains the minimum elements in the source collection.</returns>
			[Version(3, 2, 0)]
		public static IEnumerable<T> MinItemsBy<T, TComparable>(
				[NotNull] this IEnumerable<T> source,
				[NotNull] Func<T, TComparable> selector)
				where TComparable : IComparable<TComparable>
			{
				source.ThrowIfNull(nameof(source));
				selector.ThrowIfNull(nameof(selector));

				return source.MinItemsBy(selector, Comparer<TComparable>.Default);
			}
			
			/// <summary>
			/// Returns the minimum elements in the source collection based on a selector function and a specified comparer.
			/// </summary>
			/// <typeparam name="T">The type of elements in the source collection.</typeparam>
			/// <typeparam name="TComparable">The type of the value returned by the selector function.</typeparam>
			/// <param name="source">The source collection.</param>
			/// <param name="selector">A function to extract a comparable value from each element.</param>
			/// <param name="comparer">The comparer to use when comparing elements.</param>
			/// <returns>An <see cref="IEnumerable{T}"/> that contains the minimum elements in the source collection.</returns>
			[Version(3, 2, 0)]
			public static IEnumerable<T> MinItemsBy<T, TComparable>(
				[NotNull] this IEnumerable<T> source, 
				[NotNull] Func<T, TComparable> selector,
				[NotNull] Comparer<TComparable> comparer)
				where TComparable : IComparable<TComparable>
			{
				source.ThrowIfNull(nameof(source));
				selector.ThrowIfNull(nameof(selector));
				comparer.ThrowIfNull(nameof(comparer));

				using (var enumerator = source.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return Enumerable.Empty<T>();
					}

					var min = selector(enumerator.Current);
					var minList = new List<T> { enumerator.Current };

					while (enumerator.MoveNext())
					{
						var value = selector(enumerator.Current);

						if (comparer.Compare(value, min) < 0)
						{
							min = value;
							minList.Clear();
							minList.Add(enumerator.Current);
						}
						else if (comparer.Compare(value, min) == 0)
						{
							minList.Add(enumerator.Current);
						}
					}

					return minList;
				}
			}
		
		#endregion
		
		#region IndexOperations
		// TODO Do we really need this null check here? 
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Version(4, 0, 0)]
		internal static bool Less<T>(T v, T w) 
			where T : IComparable<T> 
			=> v.CompareTo(w) < 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Version(4, 0, 0)]
		internal static bool LessAt<T>(T[] list, int i, int j) 
			where T : IComparable<T>
			=> Less(list[i], list[j]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Version(4, 0, 0)]
		internal static bool LessAt<T>(IList<T> list, int i, int j) 
			where T : IComparable<T>
			=> Less(list[i], list[j]);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Version(4, 0, 0)]
		internal static bool LessAt<T>(this T[] list, int i, int j, IComparer<T> comparer) 
			=> comparer.Less(list[i], list[j]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Version(4, 0, 0)]
		internal static bool LessAt<T>(IComparer<T> comparer, IList<T> list, int i, int j) 
			=> comparer.Less(list[i], list[j]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Version(4, 0, 0)]
		internal static bool LessOrEqual<T>(T v, T w) 
			where T : IComparable<T>
			=> v.CompareTo(w) <= 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Version(4, 0, 0)]
		internal static bool LessOrEqualAt<T>(this IList<T> list, int i, int j) 
			where T : IComparable<T> 
			=> LessOrEqual(list[i], list[j]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Version(4, 0, 0)]
		internal static void MoveAt<T>(this IList<T> list, int sourceIndex, int destinationIndex)
		{
			list[destinationIndex] = list[sourceIndex];
			list[sourceIndex] = default;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Version(4, 0, 0)]
		internal static void MoveAt<T>(this T[] list, int sourceIndex, int destinationIndex)
		{
			list[destinationIndex] = list[sourceIndex];
			list[sourceIndex] = default;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Version(4, 0, 0)]
		internal static void SwapAt<T>(this IList<T> list, int i, int j) => (list[i], list[j]) = (list[j], list[i]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Version(4, 0, 0)]
		internal static void SwapAt<T>(this T[] list, int i, int j) => (list[i], list[j]) = (list[j], list[i]);

		#endregion
	}
}
