﻿#region References

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedy.Extensions;
using Speedy.Protocols.Osc;

#pragma warning disable 169

#endregion

namespace Speedy.UnitTests.Extensions
{
	[TestClass]
	public class ReflectionExtensionsTests
	{
		#region Methods

		[TestMethod]
		public void CreateInstance()
		{
			var scenarios = new Dictionary<Type, object>
			{
				{ typeof(byte), (byte) 0 },
				{ typeof(short), (short) 0 },
				{ typeof(ushort), (ushort) 0 },
				{ typeof(int), 0 },
				{ typeof(uint), (uint) 0 },
				{ typeof(long), (long) 0 },
				{ typeof(ulong), (ulong) 0 },
				{ typeof(string), string.Empty },
				{ typeof(DateTime), DateTime.MinValue },
				{ typeof(TimeSpan), TimeSpan.Zero },
				{ typeof(OscTimeTag), OscTimeTag.MinValue }
			};

			foreach (var scenario in scenarios)
			{
				scenario.Key.FullName.Dump();
				Assert.AreEqual(scenario.Value, scenario.Key.CreateInstance(), scenario.Key.FullName);
			}
		}

		[TestMethod]
		public void CreateInstanceForGenerics()
		{
			var scenarios = new ICollection<int>[]
			{
				new Collection<int>(new[] { 1, 2, 3 }),
				new List<int>(new[] { 2, 4, 5 }),
				new HashSet<int>(new[] { 6, 7, 8 })
			};

			foreach (var list in scenarios)
			{
				var type = list.GetType();
				type.FullName.Dump();
				TestHelper.AreEqual(list, type.CreateInstance(list), type.FullName);

				if (!list.IsReadOnly)
				{
					list.Clear();
					TestHelper.AreEqual(list, type.CreateInstance(), type.FullName);
				}
			}
		}

		[TestMethod]
		public void FieldTests()
		{
			var expected = new[] { "PublicField" };
			var type = typeof(ReflectionClassTest);
			var fields = type.GetCachedFields().Select(x => x.Name).ToArray();
			TestHelper.AreEqual(expected, fields);

			expected = new[] { "_privateField", "<PublicProperty>k__BackingField", "<PrivateProperty>k__BackingField" };
			fields = type.GetCachedFields(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic).Select(x => x.Name).ToArray();
			TestHelper.AreEqual(expected, fields);

			var type2 = typeof(ReflectionStructTest);
			expected = new[] { "PublicField2" };
			fields = type2.GetCachedFields().Select(x => x.Name).ToArray();
			TestHelper.AreEqual(expected, fields);

			expected = new[] { "_privateField2", "<PublicProperty2>k__BackingField", "<PrivateProperty2>k__BackingField" };
			fields = type2.GetCachedFields(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic).Select(x => x.Name).ToArray();
			TestHelper.AreEqual(expected, fields);
		}

		[TestMethod]
		public void GetDefaultValue()
		{
			var scenarios = new Dictionary<Type, object>
			{
				{ typeof(byte), (byte) 0 },
				{ typeof(short), (short) 0 },
				{ typeof(ushort), (ushort) 0 },
				{ typeof(int), 0 },
				{ typeof(uint), (uint) 0 },
				{ typeof(long), (long) 0 },
				{ typeof(ulong), (ulong) 0 },
				{ typeof(string), null },
				{ typeof(void), null },
				{ typeof(DateTime), DateTime.MinValue },
				{ typeof(TimeSpan), TimeSpan.Zero },
				{ typeof(OscTimeTag), OscTimeTag.MinValue }
			};

			foreach (var scenario in scenarios)
			{
				scenario.Key.FullName.Dump();
				Assert.AreEqual(scenario.Value, scenario.Key.GetDefaultValue(), scenario.Key.FullName);
			}
		}

		[TestMethod]
		public void MethodTests()
		{
			// Get Method Info by Name
			var type = typeof(ReflectionClassTest);
			var methodInfo = type.GetCachedMethod("MethodOne");
			Assert.AreNotEqual(null, methodInfo);
			Assert.AreEqual("MethodOne", methodInfo.Name);
		}

		[TestMethod]
		public void PropertyTests()
		{
			// By class type
			var expected = new[] { "PublicProperty" };
			var type = typeof(ReflectionClassTest);
			var properties = type.GetCachedProperties().Select(x => x.Name).ToArray();
			TestHelper.AreEqual(expected, properties);
			expected = new[] { "PrivateProperty" };
			properties = type.GetCachedProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic).Select(x => x.Name).ToArray();
			TestHelper.AreEqual(expected, properties);
		}

		#endregion

		#region Classes

		public class ReflectionClassTest
		{
			#region Fields

			public int PublicField;

			private string _privateField;

			#endregion

			#region Properties

			public double PublicProperty { get; set; }

			private double PrivateProperty { get; set; }

			#endregion

			#region Methods

			public void MethodOne()
			{
			}

			#endregion
		}

		#endregion

		#region Structures

		public struct ReflectionStructTest
		{
			#region Fields

			public int PublicField2;

			private string _privateField2;

			#endregion

			#region Properties

			public double PublicProperty2 { get; set; }

			private double PrivateProperty2 { get; set; }

			#endregion
		}

		#endregion
	}
}