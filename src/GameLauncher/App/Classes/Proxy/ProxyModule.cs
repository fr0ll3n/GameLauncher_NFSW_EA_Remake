using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Nancy;
using Nancy.Extensions;

namespace GameLauncher.App.Classes.Proxy;

public class ProxyModule : NancyModule
{
	[CompilerGenerated]
	private static class _003C_003Eo__1
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__8
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, Func<CallSite, object, Response>, CallSite<Func<CallSite, object, Response>>, object, object>> _003C_003Ep__1;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__2;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__9
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__10
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__11
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__12
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__13
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__14
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__15
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__16
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__17
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__18
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__19
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__20
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__21
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__22
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__23
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__24
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__25
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__26
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__27
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__28
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__29
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__30
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__31
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__32
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__33
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__34
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__35
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__36
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__37
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__38
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__39
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__40
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, Response>> _003C_003Ep__1;
	}

	[StructLayout (LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <<-ctor>b__40_0>d : IAsyncStateMachine
	{
		private static class <>o
		{
			public static CallSite<Func<CallSite, object, object>> <>p__0;

			public static CallSite<Func<CallSite, object, object>> <>p__1;

			public static CallSite<Func<CallSite, object, bool>> <>p__2;

			public static CallSite<Func<CallSite, object, object>> <>p__3;
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<object> <>t__builder;

		public ProxyModule <>4__this;

		public object a;

		public CancellationToken ct;

		private object <>u__1;

		private void MoveNext ()
		{
			int num = <>1__state;
			ProxyModule proxyModule = <>4__this;
			object result;
			try {
				dynamic awaiter;
				if (num != 0) {
					awaiter = proxyModule.Forward ("GET", (dynamic)a, ct).GetAwaiter ();
					if (!(bool)awaiter.IsCompleted) {
						num = (<>1__state = 0);
						<>u__1 = awaiter;
						ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
						if (awaiter2 == null) {
							INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
							<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
							awaiter3 = null;
						} else {
							<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
						}
						awaiter2 = null;
						return;
					}
				} else {
					awaiter = <>u__1;
					<>u__1 = null;
					num = (<>1__state = -1);
				}
				result = awaiter.GetResult ();
			} catch (Exception exception) {
				<>1__state = -2;
				<>t__builder.SetException (exception);
				return;
			}
			<>1__state = -2;
			<>t__builder.SetResult (result);
		}

		void IAsyncStateMachine.MoveNext ()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext ();
		}

		[DebuggerHidden]
		private void SetStateMachine (IAsyncStateMachine stateMachine)
		{
			<>t__builder.SetStateMachine (stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine (stateMachine);
		}
	}

	[StructLayout (LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <<-ctor>b__40_1>d : IAsyncStateMachine
	{
		private static class <>o
		{
			public static CallSite<Func<CallSite, object, object>> <>p__0;

			public static CallSite<Func<CallSite, object, object>> <>p__1;

			public static CallSite<Func<CallSite, object, bool>> <>p__2;

			public static CallSite<Func<CallSite, object, object>> <>p__3;
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<object> <>t__builder;

		public ProxyModule <>4__this;

		public object a;

		public CancellationToken ct;

		private object <>u__1;

		private void MoveNext ()
		{
			int num = <>1__state;
			ProxyModule proxyModule = <>4__this;
			object result;
			try {
				dynamic awaiter;
				if (num != 0) {
					awaiter = proxyModule.Forward ("POST", (dynamic)a, ct).GetAwaiter ();
					if (!(bool)awaiter.IsCompleted) {
						num = (<>1__state = 0);
						<>u__1 = awaiter;
						ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
						if (awaiter2 == null) {
							INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
							<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
							awaiter3 = null;
						} else {
							<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
						}
						awaiter2 = null;
						return;
					}
				} else {
					awaiter = <>u__1;
					<>u__1 = null;
					num = (<>1__state = -1);
				}
				result = awaiter.GetResult ();
			} catch (Exception exception) {
				<>1__state = -2;
				<>t__builder.SetException (exception);
				return;
			}
			<>1__state = -2;
			<>t__builder.SetResult (result);
		}

		void IAsyncStateMachine.MoveNext ()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext ();
		}

		[DebuggerHidden]
		private void SetStateMachine (IAsyncStateMachine stateMachine)
		{
			<>t__builder.SetStateMachine (stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine (stateMachine);
		}
	}

	[StructLayout (LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <<-ctor>b__40_2>d : IAsyncStateMachine
	{
		private static class <>o
		{
			public static CallSite<Func<CallSite, object, object>> <>p__0;

			public static CallSite<Func<CallSite, object, object>> <>p__1;

			public static CallSite<Func<CallSite, object, bool>> <>p__2;

			public static CallSite<Func<CallSite, object, object>> <>p__3;
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<object> <>t__builder;

		public ProxyModule <>4__this;

		public object a;

		public CancellationToken ct;

		private object <>u__1;

		private void MoveNext ()
		{
			int num = <>1__state;
			ProxyModule proxyModule = <>4__this;
			object result;
			try {
				dynamic awaiter;
				if (num != 0) {
					awaiter = proxyModule.Forward ("PUT", (dynamic)a, ct).GetAwaiter ();
					if (!(bool)awaiter.IsCompleted) {
						num = (<>1__state = 0);
						<>u__1 = awaiter;
						ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
						if (awaiter2 == null) {
							INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
							<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
							awaiter3 = null;
						} else {
							<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
						}
						awaiter2 = null;
						return;
					}
				} else {
					awaiter = <>u__1;
					<>u__1 = null;
					num = (<>1__state = -1);
				}
				result = awaiter.GetResult ();
			} catch (Exception exception) {
				<>1__state = -2;
				<>t__builder.SetException (exception);
				return;
			}
			<>1__state = -2;
			<>t__builder.SetResult (result);
		}

		void IAsyncStateMachine.MoveNext ()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext ();
		}

		[DebuggerHidden]
		private void SetStateMachine (IAsyncStateMachine stateMachine)
		{
			<>t__builder.SetStateMachine (stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine (stateMachine);
		}
	}

	[StructLayout (LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <<-ctor>b__40_3>d : IAsyncStateMachine
	{
		private static class <>o
		{
			public static CallSite<Func<CallSite, object, object>> <>p__0;

			public static CallSite<Func<CallSite, object, object>> <>p__1;

			public static CallSite<Func<CallSite, object, bool>> <>p__2;

			public static CallSite<Func<CallSite, object, object>> <>p__3;
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<object> <>t__builder;

		public ProxyModule <>4__this;

		public object a;

		public CancellationToken ct;

		private object <>u__1;

		private void MoveNext ()
		{
			int num = <>1__state;
			ProxyModule proxyModule = <>4__this;
			object result;
			try {
				dynamic awaiter;
				if (num != 0) {
					awaiter = proxyModule.Forward ("DELETE", (dynamic)a, ct).GetAwaiter ();
					if (!(bool)awaiter.IsCompleted) {
						num = (<>1__state = 0);
						<>u__1 = awaiter;
						ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
						if (awaiter2 == null) {
							INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
							<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
							awaiter3 = null;
						} else {
							<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
						}
						awaiter2 = null;
						return;
					}
				} else {
					awaiter = <>u__1;
					<>u__1 = null;
					num = (<>1__state = -1);
				}
				result = awaiter.GetResult ();
			} catch (Exception exception) {
				<>1__state = -2;
				<>t__builder.SetException (exception);
				return;
			}
			<>1__state = -2;
			<>t__builder.SetResult (result);
		}

		void IAsyncStateMachine.MoveNext ()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext ();
		}

		[DebuggerHidden]
		private void SetStateMachine (IAsyncStateMachine stateMachine)
		{
			<>t__builder.SetStateMachine (stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine (stateMachine);
		}
	}

	[StructLayout (LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <<-ctor>b__40_4>d : IAsyncStateMachine
	{
		private static class <>o
		{
			public static CallSite<Func<CallSite, object, object>> <>p__0;

			public static CallSite<Func<CallSite, object, object>> <>p__1;

			public static CallSite<Func<CallSite, object, bool>> <>p__2;

			public static CallSite<Func<CallSite, object, object>> <>p__3;
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<object> <>t__builder;

		public ProxyModule <>4__this;

		public object a;

		public CancellationToken ct;

		private object <>u__1;

		private void MoveNext ()
		{
			int num = <>1__state;
			ProxyModule proxyModule = <>4__this;
			object result;
			try {
				dynamic awaiter;
				if (num != 0) {
					awaiter = proxyModule.Forward ("GET", (dynamic)a, ct).GetAwaiter ();
					if (!(bool)awaiter.IsCompleted) {
						num = (<>1__state = 0);
						<>u__1 = awaiter;
						ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
						if (awaiter2 == null) {
							INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
							<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
							awaiter3 = null;
						} else {
							<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
						}
						awaiter2 = null;
						return;
					}
				} else {
					awaiter = <>u__1;
					<>u__1 = null;
					num = (<>1__state = -1);
				}
				result = awaiter.GetResult ();
			} catch (Exception exception) {
				<>1__state = -2;
				<>t__builder.SetException (exception);
				return;
			}
			<>1__state = -2;
			<>t__builder.SetResult (result);
		}

		void IAsyncStateMachine.MoveNext ()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext ();
		}

		[DebuggerHidden]
		private void SetStateMachine (IAsyncStateMachine stateMachine)
		{
			<>t__builder.SetStateMachine (stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine (stateMachine);
		}
	}

	[StructLayout (LayoutKind.Auto)]
	[CompilerGenerated]
	private struct <<-ctor>b__40_5>d : IAsyncStateMachine
	{
		private static class <>o
		{
			public static CallSite<Func<CallSite, object, object>> <>p__0;

			public static CallSite<Func<CallSite, object, object>> <>p__1;

			public static CallSite<Func<CallSite, object, bool>> <>p__2;

			public static CallSite<Func<CallSite, object, object>> <>p__3;
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<object> <>t__builder;

		public ProxyModule <>4__this;

		public object a;

		public CancellationToken ct;

		private object <>u__1;

		private void MoveNext ()
		{
			int num = <>1__state;
			ProxyModule proxyModule = <>4__this;
			object result;
			try {
				dynamic awaiter;
				if (num != 0) {
					awaiter = proxyModule.Forward ("POST", (dynamic)a, ct).GetAwaiter ();
					if (!(bool)awaiter.IsCompleted) {
						num = (<>1__state = 0);
						<>u__1 = awaiter;
						ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
						if (awaiter2 == null) {
							INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
							<>t__builder.AwaitOnCompleted (ref awaiter3, ref this);
							awaiter3 = null;
						} else {
							<>t__builder.AwaitUnsafeOnCompleted (ref awaiter2, ref this);
						}
						awaiter2 = null;
						return;
					}
				} else {
					awaiter = <>u__1;
					<>u__1 = null;
					num = (<>1__state = -1);
				}
				result = awaiter.GetResult ();
			} catch (Exception exception) {
				<>1__state = -2;
				<>t__builder.SetException (exception);
				return;
			}
			<>1__state = -2;
			<>t__builder.SetResult (result);
		}

		void IAsyncStateMachine.MoveNext ()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext ();
		}

		[DebuggerHidden]
		private void SetStateMachine (IAsyncStateMachine stateMachine)
		{
			<>t__builder.SetStateMachine (stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine (IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine (stateMachine);
		}
	}

	[CompilerGenerated]
	private static class <>o__41
	{
		public static CallSite<Func<CallSite, ProxyModule, string, object, object>> <>p__0;

		public static CallSite<Func<CallSite, object, Response>> <>p__1;
	}

	public unsafe ProxyModule ()
	{
		Get ("/nfsw/Engine.svc/{path*}", async delegate(dynamic a, CancellationToken ct) {
			dynamic awaiter16 = Forward ("GET", a, ct).GetAwaiter ();
			if (!(bool)awaiter16.IsCompleted) {
				ICriticalNotifyCompletion awaiter17 = awaiter16 as ICriticalNotifyCompletion;
				AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder6 = default(AsyncTaskMethodBuilder<object>);
				if (awaiter17 == null) {
					INotifyCompletion awaiter18 = (INotifyCompletion)(object)awaiter16;
					asyncTaskMethodBuilder6.AwaitOnCompleted (ref awaiter18, ref *(<<-ctor>b__40_0>d*)/*Error near IL_0194: stateMachine*/);
				} else {
					asyncTaskMethodBuilder6.AwaitUnsafeOnCompleted (ref awaiter17, ref *(<<-ctor>b__40_0>d*)/*Error near IL_01a7: stateMachine*/);
				}
				/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
			}
			return awaiter16.GetResult ();
		});
		Post ("/nfsw/Engine.svc/{path*}", async delegate(dynamic a, CancellationToken ct) {
			dynamic awaiter13 = Forward ("POST", a, ct).GetAwaiter ();
			if (!(bool)awaiter13.IsCompleted) {
				ICriticalNotifyCompletion awaiter14 = awaiter13 as ICriticalNotifyCompletion;
				AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder5 = default(AsyncTaskMethodBuilder<object>);
				if (awaiter14 == null) {
					INotifyCompletion awaiter15 = (INotifyCompletion)(object)awaiter13;
					asyncTaskMethodBuilder5.AwaitOnCompleted (ref awaiter15, ref *(<<-ctor>b__40_1>d*)/*Error near IL_0194: stateMachine*/);
				} else {
					asyncTaskMethodBuilder5.AwaitUnsafeOnCompleted (ref awaiter14, ref *(<<-ctor>b__40_1>d*)/*Error near IL_01a7: stateMachine*/);
				}
				/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
			}
			return awaiter13.GetResult ();
		});
		Put ("/nfsw/Engine.svc/{path*}", async delegate(dynamic a, CancellationToken ct) {
			dynamic awaiter10 = Forward ("PUT", a, ct).GetAwaiter ();
			if (!(bool)awaiter10.IsCompleted) {
				ICriticalNotifyCompletion awaiter11 = awaiter10 as ICriticalNotifyCompletion;
				AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder4 = default(AsyncTaskMethodBuilder<object>);
				if (awaiter11 == null) {
					INotifyCompletion awaiter12 = (INotifyCompletion)(object)awaiter10;
					asyncTaskMethodBuilder4.AwaitOnCompleted (ref awaiter12, ref *(<<-ctor>b__40_2>d*)/*Error near IL_0194: stateMachine*/);
				} else {
					asyncTaskMethodBuilder4.AwaitUnsafeOnCompleted (ref awaiter11, ref *(<<-ctor>b__40_2>d*)/*Error near IL_01a7: stateMachine*/);
				}
				/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
			}
			return awaiter10.GetResult ();
		});
		Delete ("/nfsw/Engine.svc/{path*}", async delegate(dynamic a, CancellationToken ct) {
			dynamic awaiter7 = Forward ("DELETE", a, ct).GetAwaiter ();
			if (!(bool)awaiter7.IsCompleted) {
				ICriticalNotifyCompletion awaiter8 = awaiter7 as ICriticalNotifyCompletion;
				AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder3 = default(AsyncTaskMethodBuilder<object>);
				if (awaiter8 == null) {
					INotifyCompletion awaiter9 = (INotifyCompletion)(object)awaiter7;
					asyncTaskMethodBuilder3.AwaitOnCompleted (ref awaiter9, ref *(<<-ctor>b__40_3>d*)/*Error near IL_0194: stateMachine*/);
				} else {
					asyncTaskMethodBuilder3.AwaitUnsafeOnCompleted (ref awaiter8, ref *(<<-ctor>b__40_3>d*)/*Error near IL_01a7: stateMachine*/);
				}
				/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
			}
			return awaiter7.GetResult ();
		});
		Get ("/nfsw/Engine.svc", async delegate(dynamic a, CancellationToken ct) {
			dynamic awaiter4 = Forward ("GET", a, ct).GetAwaiter ();
			if (!(bool)awaiter4.IsCompleted) {
				ICriticalNotifyCompletion awaiter5 = awaiter4 as ICriticalNotifyCompletion;
				AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder2 = default(AsyncTaskMethodBuilder<object>);
				if (awaiter5 == null) {
					INotifyCompletion awaiter6 = (INotifyCompletion)(object)awaiter4;
					asyncTaskMethodBuilder2.AwaitOnCompleted (ref awaiter6, ref *(<<-ctor>b__40_4>d*)/*Error near IL_0194: stateMachine*/);
				} else {
					asyncTaskMethodBuilder2.AwaitUnsafeOnCompleted (ref awaiter5, ref *(<<-ctor>b__40_4>d*)/*Error near IL_01a7: stateMachine*/);
				}
				/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
			}
			return awaiter4.GetResult ();
		});
		Post ("/nfsw/Engine.svc", async delegate(dynamic a, CancellationToken ct) {
			dynamic awaiter = Forward ("POST", a, ct).GetAwaiter ();
			if (!(bool)awaiter.IsCompleted) {
				ICriticalNotifyCompletion awaiter2 = awaiter as ICriticalNotifyCompletion;
				AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder = default(AsyncTaskMethodBuilder<object>);
				if (awaiter2 == null) {
					INotifyCompletion awaiter3 = (INotifyCompletion)(object)awaiter;
					asyncTaskMethodBuilder.AwaitOnCompleted (ref awaiter3, ref *(<<-ctor>b__40_5>d*)/*Error near IL_0194: stateMachine*/);
				} else {
					asyncTaskMethodBuilder.AwaitUnsafeOnCompleted (ref awaiter2, ref *(<<-ctor>b__40_5>d*)/*Error near IL_01a7: stateMachine*/);
				}
				/*Error near IL_01b0: leave MoveNext - await not detected correctly*/;
			}
			return awaiter.GetResult ();
		});
	}

	private async Task<Response> Forward (string method, dynamic args, CancellationToken ct)
	{
		if (<>o__41.<>p__1 == null) {
			<>o__41.<>p__1 = CallSite<Func<CallSite, object, Nancy.Response>>.Create (Binder.Convert (CSharpBinderFlags.None, typeof(Response), typeof(ProxyModule)));
		}
		Func<CallSite, object, Response> target = <>o__41.<>p__1.Target;
		CallSite<Func<CallSite, object, Response>> <>p__ = <>o__41.<>p__1;
		return target (<>p__, await Task.Run (() => ForwardSync (method, args), ct));
	}

	private Response ForwardSync (string method, dynamic args)
	{
		try {
			string text = "";
			try {
				text = (string)args.path;
			} catch {
			}
			if (text == null) {
				text = "";
			}
			string text2 = SbrwProxy.ServerUrl.TrimEnd (new char[1] { '/' });
			if (!string.IsNullOrEmpty (text)) {
				if (!text.StartsWith ("/")) {
					text = "/" + text;
				}
				text2 += text;
			}
			string query = Request.Url.Query;
			if (!string.IsNullOrEmpty (query)) {
				text2 += (query.StartsWith ("?") ? query : ("?" + query));
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create (text2);
			httpWebRequest.Method = method;
			httpWebRequest.Timeout = 180000;
			httpWebRequest.ReadWriteTimeout = 180000;
			httpWebRequest.AutomaticDecompression = DecompressionMethods.None;
			httpWebRequest.KeepAlive = false;
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.ProtocolVersion = HttpVersion.Version11;
			string host = new Uri (SbrwProxy.ServerUrl).Host;
			try {
				httpWebRequest.Host = host;
			} catch {
			}
			foreach (KeyValuePair<string, IEnumerable<string>> header in Request.Headers) {
				string key = header.Key;
				if (string.IsNullOrEmpty (key)) {
					continue;
				}
				string text3 = key.ToLowerInvariant ();
				switch (text3) {
				case "host":
				case "connection":
				case "content-length":
				case "transfer-encoding":
					continue;
				}
				string text4 = ((header.Value != null && Enumerable.Any<string> (header.Value)) ? Enumerable.First<string> (header.Value) : "");
				switch (text3) {
				case "content-type":
					httpWebRequest.ContentType = text4;
					continue;
				case "user-agent":
					httpWebRequest.UserAgent = text4;
					continue;
				case "accept-encoding":
					httpWebRequest.Headers ["Accept-Encoding"] = "gzip, deflate";
					continue;
				}
				try {
					httpWebRequest.Headers [key] = text4;
				} catch {
				}
			}
			if (string.IsNullOrEmpty (httpWebRequest.UserAgent)) {
				httpWebRequest.UserAgent = "Blackbox";
			}
			if (httpWebRequest.Headers ["Accept-Encoding"] == null) {
				httpWebRequest.Headers ["Accept-Encoding"] = "gzip, deflate";
			}
			if (method != "GET" && method != "HEAD") {
				byte[] array = null;
				try {
					using MemoryStream memoryStream = new MemoryStream ();
					Request.Body.CopyTo (memoryStream);
					array = memoryStream.ToArray ();
				} catch {
					array = Encoding.UTF8.GetBytes (Request.Body.AsString (Encoding.UTF8) ?? "");
				}
				if (array != null && array.Length != 0) {
					httpWebRequest.ContentLength = array.Length;
					using Stream stream4 = ((WebRequest)httpWebRequest).GetRequestStream ();
					stream4.Write (array, 0, array.Length);
				} else {
					httpWebRequest.ContentLength = 0L;
				}
			}
			SbrwProxy.Log (method + " /" + text + " -> " + text2);
			HttpWebResponse httpWebResponse = null;
			try {
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
			} catch (WebException ex6) {
				WebException ex7 = ex6;
				WebException wex = ex7;
				if (wex.Response == null) {
					SbrwProxy.Log ("ERR no response: " + ((Exception)(object)wex).Message);
					return new Response {
						StatusCode = Nancy.HttpStatusCode.BadGateway,
						ContentType = "text/plain",
						Contents = delegate(Stream s) {
							byte[] bytes3 = Encoding.UTF8.GetBytes ("proxy error: " + ((Exception)(object)wex).Message);
							s.Write (bytes3, 0, bytes3.Length);
						}
					};
				}
				httpWebResponse = (HttpWebResponse)wex.Response;
			}
			HttpWebResponse httpWebResponse2 = httpWebResponse;
			try {
				int statusCode = (int)httpWebResponse.StatusCode;
				string contentType = httpWebResponse.ContentType ?? "application/xml;charset=UTF-8";
				string text5 = httpWebResponse.Headers ["Content-Encoding"] ?? "";
				byte[] bytes;
				using (Stream stream5 = ((WebResponse)httpWebResponse).GetResponseStream ()) {
					using MemoryStream memoryStream2 = new MemoryStream ();
					stream5?.CopyTo (memoryStream2);
					bytes = memoryStream2.ToArray ();
				}
				SbrwProxy.Log ("OK " + statusCode + " /" + text + " len=" + bytes.Length + " enc=" + text5);
				Response response = new Response {
					StatusCode = (Nancy.HttpStatusCode)statusCode,
					ContentType = contentType,
					Contents = delegate(Stream stream3) {
						if (bytes != null && bytes.Length != 0) {
							stream3.Write (bytes, 0, bytes.Length);
						}
					}
				};
				response.Headers ["Content-Length"] = bytes.Length.ToString ();
				if (!string.IsNullOrEmpty (text5)) {
					response.Headers ["Content-Encoding"] = text5;
				}
				try {
					response.Headers ["Connection"] = "close";
				} catch {
				}
				return response;
			} finally {
				((IDisposable)httpWebResponse2)?.Dispose ();
			}
		} catch (Exception ex8) {
			Exception ex9 = ex8;
			Exception ex5 = ex9;
			SbrwProxy.Log ("ERR: " + ex5.Message);
			return new Response {
				StatusCode = Nancy.HttpStatusCode.BadGateway,
				ContentType = "text/plain",
				Contents = delegate(Stream s) {
					byte[] bytes2 = Encoding.UTF8.GetBytes ("proxy error: " + ex5.Message);
					s.Write (bytes2, 0, bytes2.Length);
				}
			};
		}
	}
}
