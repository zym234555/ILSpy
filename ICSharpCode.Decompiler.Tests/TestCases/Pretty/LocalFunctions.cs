// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
#if CS90
using System.Runtime.InteropServices;
#endif

namespace LocalFunctions
{
	internal class LocalFunctions
	{
		[AttributeUsage(AttributeTargets.All)]
		internal class MyAttribute : Attribute
		{

		}

		public class Generic<T1> where T1 : struct, ICloneable, IConvertible
		{
			public int MixedLocalFunction<T2>() where T2 : ICloneable, IConvertible
			{
#pragma warning disable CS0219
				T2 t2 = default(T2);
				object z = this;
				for (int i = 0; i < 10; i++)
				{
					int i2 = 0;
					i2 += NonStaticMethod<object>(0);
#if CS90
					[My]
					[return: My]
					int NonStaticMethod<[My] T3>([My] int unused)
#else
					int NonStaticMethod<T3>(int unused)
#endif
					{
						t2 = default(T2);
						int l = 0;
						return NonStaticMethod3<T1>() + NonStaticMethod3<T2>() + z.GetHashCode();
						int NonStaticMethod3<T4>()
						{
							return i2 + l + NonStaticMethod<T4>(0) + StaticMethod<decimal>();
						}
					}
				}
				return MixedLocalFunction<T1>() + MixedLocalFunction<T2>() + StaticMethod<decimal>() + StaticMethod<int>() + NonStaticMethod2() + StaticMethod4<object>(null) + StaticMethod5<T1>();
				int NonStaticMethod2()
				{
					return GetHashCode();
				}
#if CS80
				static int StaticMethod<T3>() where T3 : struct
#else
				int StaticMethod<T3>() where T3 : struct
#endif
				{
					return typeof(T1).Name.Length + typeof(T2).Name.Length + typeof(T3).Name.Length + StaticMethod<float>() + StaticMethod2<T3, DayOfWeek>() + StaticMethod3<T2, T3, DayOfWeek>();
				}
#if CS80
				static int StaticMethod2<T3, T4>() where T3 : struct where T4 : Enum
#else
				int StaticMethod2<T3, T4>() where T3 : struct where T4 : Enum
#endif
				{
					return typeof(T1).Name.Length + typeof(T2).Name.Length + typeof(T3).Name.Length + typeof(T4).Name.Length + StaticMethod<float>() + StaticMethod2<T3, DayOfWeek>();
				}
#pragma warning disable CS8387
#if CS80
				static int StaticMethod3<T2, T3, T4>() where T2 : IConvertible where T3 : struct where T4 : Enum
#else
				int StaticMethod3<T2, T3, T4>() where T2 : IConvertible where T3 : struct where T4 : Enum
#endif
#pragma warning restore CS8387
				{
					return typeof(T2).Name.Length;
				}
#if CS80
				static int StaticMethod4<T>(T dd)
#else
				int StaticMethod4<T>(T dd)
#endif
				{
					return 0;
				}
#if CS80
				static int StaticMethod5<T3>()
#else
				int StaticMethod5<T3>()
#endif
				{
					int k = 0;
					return k + NonStaticMethod4<T1>();
					int NonStaticMethod4<T4>()
					{
						return k;
					}
				}
#pragma warning restore CS0219
			}

			public int MixedLocalFunction2Delegate<T2>() where T2 : ICloneable, IConvertible
			{
				T2 t2 = default(T2);
				object z = this;
				for (int i = 0; i < 10; i++)
				{
					int i2 = 0;
					i2 += StaticInvokeAsFunc(NonStaticMethod<object>);
					int NonStaticMethod<T3>()
					{
						t2 = default(T2);
						int l = 0;
						return StaticInvokeAsFunc(NonStaticMethod3<T1>) + StaticInvokeAsFunc(NonStaticMethod3<T2>) + z.GetHashCode();
						int NonStaticMethod3<T4>()
						{
							return i2 + l + StaticInvokeAsFunc(NonStaticMethod<T4>) + StaticInvokeAsFunc(StaticMethod<decimal>);
						}
					}
				}
				Console.WriteLine(t2);
				return StaticInvokeAsFunc(MixedLocalFunction2Delegate<T1>) + StaticInvokeAsFunc(MixedLocalFunction2Delegate<T2>) + StaticInvokeAsFunc(StaticMethod<decimal>) + StaticInvokeAsFunc(StaticMethod<int>) + StaticInvokeAsFunc(NonStaticMethod2) + StaticInvokeAsFunc(StaticMethod4<T1>) + new Func<object, int>(StaticMethod5<object>)(null) + StaticInvokeAsFunc2<object>(StaticMethod5<object>) + new Func<Func<object, int>, int>(StaticInvokeAsFunc2<object>)(StaticMethod5<object>);
				int NonStaticMethod2()
				{
					return GetHashCode();
				}
#if CS80
				static int StaticInvokeAsFunc(Func<int> func)
#else
				int StaticInvokeAsFunc(Func<int> func)
#endif
				{
					return func();
				}
#if CS80
				static int StaticInvokeAsFunc2<T>(Func<T, int> func)
#else
				int StaticInvokeAsFunc2<T>(Func<T, int> func)
#endif
				{
					return func(default(T));
				}
#if CS80
				static int StaticMethod<T3>() where T3 : struct
#else
				int StaticMethod<T3>() where T3 : struct
#endif
				{
					return typeof(T1).Name.Length + typeof(T2).Name.Length + typeof(T3).Name.Length + StaticInvokeAsFunc(StaticMethod<float>) + StaticInvokeAsFunc(StaticMethod2<T3, DayOfWeek>) + StaticInvokeAsFunc(StaticMethod3<T2, T3, DayOfWeek>);
				}
#if CS80
				static int StaticMethod2<T3, T4>() where T3 : struct where T4 : Enum
#else
				int StaticMethod2<T3, T4>() where T3 : struct where T4 : Enum
#endif
				{
					return typeof(T1).Name.Length + typeof(T2).Name.Length + typeof(T3).Name.Length + typeof(T4).Name.Length + StaticInvokeAsFunc(StaticMethod<float>) + StaticInvokeAsFunc(StaticMethod2<T3, DayOfWeek>);
				}
#pragma warning disable CS8387
#if CS80
				static int StaticMethod3<T2, T3, T4>() where T2 : IConvertible where T3 : struct where T4 : Enum
#else
				int StaticMethod3<T2, T3, T4>() where T2 : IConvertible where T3 : struct where T4 : Enum
#endif
#pragma warning restore CS8387
				{
					return typeof(T2).Name.Length;
				}
#if CS80
				static int StaticMethod4<T3>()
#else
				int StaticMethod4<T3>()
#endif
				{
					int k = 0;
					return k + StaticInvokeAsFunc(NonStaticMethod4<T1>);
					int NonStaticMethod4<T4>()
					{
						return k;
					}
				}
#if CS80
				static int StaticMethod5<T>(T dd)
#else
				int StaticMethod5<T>(T dd)
#endif
				{
					return 0;
				}
			}

			public static void Test_CaptureT<T2>()
			{
#pragma warning disable CS0219
				T2 t2 = default(T2);
				Method<int>();
				void Method<T3>()
				{
					t2 = default(T2);
					T2 t2x = t2;
					T3 t3 = default(T3);
					Method2();
					void Method2()
					{
						t2 = default(T2);
						t2x = t2;
						t3 = default(T3);
					}
				}
#pragma warning restore CS0219
			}

			public void TestGenericArgs<T2>() where T2 : List<T2>
			{
				ZZ<T2>(null);
				ZZ3<object>(null);
#if CS80
				static void Nop<T>(T data)
#else
				void Nop<T>(T data)
#endif
				{
				}
#if CS80
				static void ZZ<T3>(T3 t3) where T3 : T2
#else
				void ZZ<T3>(T3 t3) where T3 : T2
#endif
				{
					Nop<List<T2>>(t3);
					ZZ2<T3>(t3);
					ZZ4();
					void ZZ4()
					{
						Nop<List<T2>>(t3);
					}
				}
#if CS80
				static void ZZ2<T3>(T3 t3)
#else
				void ZZ2<T3>(T3 t3)
#endif
				{
					Nop<List<T2>>((List<T2>)(object)t3);
				}
#if CS80
				static void ZZ3<T3>(T3 t3)
#else
				void ZZ3<T3>(T3 t3)
#endif
				{
					Nop<List<T2>>((List<T2>)(object)t3);
				}
			}

#if false
			public void GenericArgsWithAnonymousType()
			{
				Method<int>();
#if CS80
				static void Method<T2>()
#else
				void Method<T2>()
#endif
				{
					int i = 0;
					var obj2 = new {
						A = 1
					};
					Method2(obj2);
					Method3(obj2);
					void Method2<T3>(T3 obj1)
					{
						//keep nested
						i = 0;
					}
#if CS80
					static void Method3<T3>(T3 obj1)
#else
					void Method3<T3>(T3 obj1)
#endif
					{
					}
				}
			}
#if CS80
			public void NameConflict()
			{
				int i = 0;
				Method<int>();
				void Method<T2>()
				{
					Method();
					void Method()
					{
						Method<T2>();
						i = 0;
						void Method<T2>()
						{
							i = 0;
							Method();
							static void Method()
							{
							}
						}
					}
				}
			}
#endif
#endif
		}

		private int field;

		private Lazy<object> nonCapturinglocalFunctionInLambda = new Lazy<object>(delegate {
			return CreateValue();

#if CS80
			static object CreateValue()
#else
			object CreateValue()
#endif
			{
				return null;
			}
		});

		private Lazy<object> capturinglocalFunctionInLambda = new Lazy<object>(delegate {
			int x = 42;
			return Do();

			object Do()
			{
				return CreateValue();

				int CreateValue()
				{
					return x;
				}
			}
		});

		private static void Test(int x)
		{
		}

		private static int GetInt(string a)
		{
			return a.Length;
		}

		private static string GetString(int a)
		{
			return a.ToString();
		}

		public static void StaticContextNoCapture(int length)
		{
			for (int i = 0; i < length; i++)
			{
				LocalWrite("Hello " + i);
			}

#if CS80
			static void LocalWrite(string s)
#else
			void LocalWrite(string s)
#endif
			{
				Console.WriteLine(s);
			}
		}

		public static void StaticContextSimpleCapture(int length)
		{
			for (int i = 0; i < length; i++)
			{
				LocalWrite();
			}

			void LocalWrite()
			{
				Console.WriteLine("Hello " + length);
			}
		}

		public static void StaticContextCaptureForLoopVariable(int length)
		{
			int i;
			for (i = 0; i < length; i++)
			{
				LocalWrite();
			}
			void LocalWrite()
			{
				Console.WriteLine("Hello " + i + "/" + length);
			}
		}

		public void ContextNoCapture()
		{
			for (int i = 0; i < field; i++)
			{
				LocalWrite("Hello " + i);
			}

#if CS80
			static void LocalWrite(string s)
#else
			void LocalWrite(string s)
#endif
			{
				Console.WriteLine(s);
			}
		}

		public void ContextSimpleCapture()
		{
			for (int i = 0; i < field; i++)
			{
				LocalWrite();
			}

			void LocalWrite()
			{
				Console.WriteLine("Hello " + field);
			}
		}

		public void ContextCaptureForLoopVariable()
		{
			int i;
			for (i = 0; i < field; i++)
			{
				LocalWrite();
			}
			void LocalWrite()
			{
				Console.WriteLine("Hello " + i + "/" + field);
			}
		}

		public void CapturedOutsideLoop()
		{
			int i = 0;
			while (i < field)
			{
				i = GetInt("asdf");
				LocalWrite();
			}

			void LocalWrite()
			{
				Console.WriteLine("Hello " + i + "/" + field);
			}
		}

		public void CapturedInForeachLoop(IEnumerable<string> args)
		{
			foreach (string arg2 in args)
			{
				string arg = arg2;
				LocalWrite();
				void LocalWrite()
				{
					Console.WriteLine("Hello " + arg);
				}
			}
		}

		public void Overloading()
		{
			Test(5);
			LocalFunctions.Test(2);

#if CS80
			static void Test(int x)
#else
			void Test(int x)
#endif
			{
				Console.WriteLine("x: {0}", x);
			}
		}

		private void Name()
		{

		}

		private void LocalFunctionHidingMethod()
		{
			Action action = this.Name;
			Name();
			action();

#if CS80
			static void Name()
#else
			void Name()
#endif
			{

			}
		}

		public void NamedArgument()
		{
			Use(Get(1), Get(2), Get(3));
			Use(Get(1), c: Get(2), b: Get(3));

#if CS80
			static int Get(int i)
#else
			int Get(int i)
#endif
			{
				return i;
			}

#if CS80
			static void Use(int a, int b, int c)
#else
			void Use(int a, int b, int c)
#endif
			{
				Console.WriteLine(a + b + c);
			}
		}

		public static Func<int> LambdaInLocalFunction()
		{
			int x = (int)Math.Pow(2.0, 10.0);
			return Create();

			Func<int> Create()
			{
				return () => x;
			}
		}

		public static Func<int> MethodRef()
		{
			int x = (int)Math.Pow(2.0, 10.0);
			Enumerable.Range(1, 100).Select(LocalFunction);
			return null;

			int LocalFunction(int y)
			{
				return x * y;
			}
		}

		public static int Fib(int i)
		{
			return FibHelper(i);

#if CS80
			static int FibHelper(int n)
#else
			int FibHelper(int n)
#endif
			{
				if (n <= 0)
				{
					return 0;
				}

				return FibHelper(n - 1) + FibHelper(n - 2);
			}
		}
		public int MutuallyRecursiveLocalFunctions()
		{
			return B(4) + C(3);

#if CS80
			static int A(int i)
#else
			int A(int i)
#endif
			{
				if (i > 0)
				{
					return A(i - 1) + 2 * B(i - 1) + 3 * C(i - 1);
				}
				return 1;
			}

#if CS80
			static int B(int i)
#else
			int B(int i)
#endif
			{
				if (i > 0)
				{
					return 3 * A(i - 1) + B(i - 1);
				}
				return 1;
			}

#if CS80
			static int C(int i)
#else
			int C(int i)
#endif
			{
				if (i > 0)
				{
					return 2 * A(i - 1) + C(i - 1);
				}
				return 1;
			}
		}

		public static int NestedLocalFunctions(int i)
		{
			return A();

			int A()
			{
				double x = Math.Pow(10.0, 2.0);
				return B();

				int B()
				{
					return i + (int)x;
				}
			}
		}

		public static int LocalFunctionInLambda(IEnumerable<int> xs)
		{
			return xs.First(delegate (int x) {
				return Do();

				bool Do()
				{
					return x == 3;
				}
			});
		}

		public static IEnumerable<int> YieldReturn(int n)
		{
			return GetNumbers();

			IEnumerable<int> GetNumbers()
			{
				for (int i = 0; i < n; i++)
				{
					yield return i;
				}
			}
		}

		public void WriteCapturedParameter(int i)
		{
			ParamWrite();
			Console.WriteLine(i);

			void ParamWrite()
			{
				i++;
			}
		}

		//public static void LocalFunctionInUsing()
		//{
		//	using (MemoryStream memoryStream = new MemoryStream()) {
		//		Do();

		//		void Do()
		//		{
		//			memoryStream.WriteByte(42);
		//		}
		//	}
		//}

		public void NestedCapture1()
		{
			Method(null);

#if CS80
			static Action<object> Method(Action<object> action)
#else
			Action<object> Method(Action<object> action)
#endif
			{
				return Method2;

				void Method2(object containerBuilder)
				{
					Method3(containerBuilder);
				}

				void Method3(object containerBuilder)
				{
					action(containerBuilder);
				}
			}
		}

		public int NestedCapture2()
		{
			return Method();
#if CS80
			static int Method()
#else
			int Method()
#endif
			{
				int t0 = 0;
				return ZZZ();
				int ZZZ()
				{
					t0 = 0;
					int t2 = t0;
					return new Func<int>(ZZZ3)();
					int ZZZ3()
					{
						t0 = 0;
						t2 = 0;
						return ZZZ2();
					}
				}
				int ZZZ2()
				{
					t0 = 0;
					int t3 = t0;
					return new Func<int>(ZZZ4)();
					int ZZZ4()
					{
						t0 = 0;
						t3 = 0;
						return 0;
					}
				}
			}
		}

		public int Issue1798_NestedCapture2()
		{
			return Method();
#if CS80
			static int Method()
#else
			int Method()
#endif
			{
				int t0 = 0;
				return ZZZ();
				int ZZZ()
				{
					t0 = 0;
					int t2 = t0;
					return ((Func<int>)delegate {
						t0 = 0;
						t2 = 0;
						return ZZZ2();
					})();
				}
				int ZZZ2()
				{
					t0 = 0;
					int t3 = t0;
#if !OPT
					Func<int> func = delegate {
#else
					return ((Func<int>)delegate {
#endif
						t0 = 0;
						t3 = 0;
						return 0;
#if !OPT
					};
					return func();
#else
					})();
#endif
				}
			}
		}

		public int Issue1798_NestedCapture2b()
		{
			return Method();
#if CS80
			static int Method()
#else
			int Method()
#endif
			{
				int t0 = 0;
				return ZZZ() + ZZZ2();
				int ZZZ()
				{
					t0 = 0;
					int t2 = t0;
					return ((Func<int>)delegate {
						t0 = 0;
						t2 = 0;
						return ZZZ2();
					})();
				}
				int ZZZ2()
				{
					t0 = 0;
					int t3 = t0;
#if !OPT
					Func<int> func = delegate {
#else
					return ((Func<int>)delegate {
#endif
						t0 = 0;
						t3 = 0;
						return 0;
#if !OPT
					};
					return func();
#else
					})();
#endif
				}
			}
		}

#if CS90
		public void Issue2196()
		{
			EnumWindows(0L, 0L);

			[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "EnumWindows")]
			static extern int EnumWindows(long hWnd, long lParam);
		}
#endif
	}
}
